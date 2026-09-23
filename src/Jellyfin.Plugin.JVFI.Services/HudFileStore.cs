using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JVFI.Services;

/// <summary>Owns UTF-8 text files reloaded by FFmpeg drawtext filters.</summary>
public sealed class HudFileStore
{
	private readonly string _directory;

	private readonly ILogger<HudFileStore> _logger;

	private readonly ConcurrentDictionary<string, byte> _disabledSessions = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);

	public HudFileStore(ILogger<HudFileStore> logger)
	{
		_logger = logger;
		_directory = RuntimeDataPath.GetSubdirectory("hud");
		TryEnsureDirectory(null);
	}

	/// <summary>
	/// Initializes a HUD file. HUD is optional telemetry: failures disable HUD for this
	/// session and must never abort the FFmpeg command transformation.
	/// </summary>
	public string Initialize(string sessionId, string text)
	{
		if (_disabledSessions.ContainsKey(sessionId))
		{
			return string.Empty;
		}
		try
		{
			EnsureDirectory();
			string path = GetPath(sessionId);
			Write(path, text);
			return path;
		}
		catch (Exception ex) when (IsTelemetryIoFailure(ex))
		{
			Disable(sessionId, ex, "初始化 HUD");
			return string.Empty;
		}
	}

	public void Update(string sessionId, string text)
	{
		if (_disabledSessions.ContainsKey(sessionId))
		{
			return;
		}
		try
		{
			EnsureDirectory();
			Write(GetPath(sessionId), text);
		}
		catch (Exception ex) when (IsTelemetryIoFailure(ex))
		{
			Disable(sessionId, ex, "更新 HUD");
		}
	}

	public void Remove(string sessionId)
	{
		_disabledSessions.TryRemove(sessionId, out var _);
		try
		{
			string path = GetPath(sessionId);
			File.Delete(path);
			File.Delete(path + ".tmp");
		}
		catch (Exception ex) when (IsTelemetryIoFailure(ex))
		{
			LoggerExtensions.LogDebug((ILogger)(object)_logger, ex, "JVFI：清理 HUD 檔案失敗，session={SessionId}；忽略此非關鍵錯誤。", new object[1] { sessionId });
		}
	}

	private string GetPath(string sessionId)
	{
		string text = string.Concat(sessionId.Select((char character) => (!char.IsLetterOrDigit(character)) ? '_' : character));
		return Path.Combine(_directory, text + ".txt");
	}

	private void EnsureDirectory()
	{
		Directory.CreateDirectory(_directory);
	}

	private void TryEnsureDirectory(string? sessionId)
	{
		try
		{
			EnsureDirectory();
		}
		catch (Exception ex) when (IsTelemetryIoFailure(ex))
		{
			LoggerExtensions.LogWarning((ILogger)(object)_logger, ex, "JVFI：HUD runtime 目錄建立失敗，HUD 將 fail-soft；directory={Directory} session={SessionId}", new object[2]
			{
				_directory,
				sessionId ?? "startup"
			});
		}
	}

	private void Disable(string sessionId, Exception ex, string operation)
	{
		if (_disabledSessions.TryAdd(sessionId, 0))
		{
			LoggerExtensions.LogWarning((ILogger)(object)_logger, ex, "JVFI：{Operation}失敗，本次 session 已停用 HUD，但補幀仍會繼續。session={SessionId} directory={Directory}", new object[3] { operation, sessionId, _directory });
		}
	}

	private static void Write(string path, string text)
	{
		string text2 = path + ".tmp";
		File.WriteAllText(text2, text ?? string.Empty, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
		File.Move(text2, path, overwrite: true);
	}

	private static bool IsTelemetryIoFailure(Exception ex)
	{
		if (ex is IOException || ex is UnauthorizedAccessException || ex is NotSupportedException || ex is SecurityException)
		{
			return true;
		}
		return false;
	}
}
