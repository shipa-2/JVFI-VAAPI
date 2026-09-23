using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using Jellyfin.Plugin.JVFI.Runtime;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JVFI.Services;

/// <summary>Owns per-session metrics and FFmpeg progress files.</summary>
public sealed class MetricsFileStore
{
	private readonly string _directory;

	private readonly ILogger<MetricsFileStore> _logger;

	private readonly ConcurrentDictionary<string, byte> _disabledSessions = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);

	public MetricsFileStore(ILogger<MetricsFileStore> logger)
	{
		_logger = logger;
		_directory = RuntimeDataPath.GetSubdirectory("metrics");
		TryEnsureDirectory();
	}

	public string Initialize(string sessionId)
	{
		if (_disabledSessions.ContainsKey(sessionId))
		{
			return string.Empty;
		}
		try
		{
			EnsureDirectory();
			string path = GetPath(sessionId);
			File.WriteAllText(path, string.Empty, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
			return path;
		}
		catch (Exception ex) when (IsTelemetryIoFailure(ex))
		{
			Disable(sessionId, ex, "初始化 metrics");
			return string.Empty;
		}
	}

	public JVFIMetrics? Read(string sessionId)
	{
		if (_disabledSessions.ContainsKey(sessionId))
		{
			return null;
		}
		try
		{
			string path = GetPath(sessionId);
			return File.Exists(path) ? JVFIMetricsParser.Parse(File.ReadAllText(path)) : null;
		}
		catch (Exception ex) when (IsTelemetryIoFailure(ex))
		{
			Disable(sessionId, ex, "讀取 metrics");
			return null;
		}
	}

	public string InitializeProgress(string sessionId)
	{
		if (_disabledSessions.ContainsKey(sessionId))
		{
			return string.Empty;
		}
		try
		{
			EnsureDirectory();
			string progressPath = GetProgressPath(sessionId);
			File.WriteAllText(progressPath, string.Empty, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
			return progressPath;
		}
		catch (Exception ex) when (IsTelemetryIoFailure(ex))
		{
			Disable(sessionId, ex, "初始化 FFmpeg progress");
			return string.Empty;
		}
	}

	public FfmpegProgressSnapshot? ReadProgress(string sessionId)
	{
		if (_disabledSessions.ContainsKey(sessionId))
		{
			return null;
		}
		try
		{
			string progressPath = GetProgressPath(sessionId);
			if (!File.Exists(progressPath))
			{
				return null;
			}
			string[] array = File.ReadAllLines(progressPath);
			long? num = null;
			double? fps = null;
			string progress = string.Empty;
			string[] array2 = array;
			foreach (string text in array2)
			{
				int num2 = text.IndexOf('=');
				if (num2 > 0)
				{
					string text2 = text.Substring(0, num2);
					string text3 = text;
					int num3 = num2 + 1;
					string text4 = text3.Substring(num3, text3.Length - num3);
					double result2;
					if (text2 == "frame" && long.TryParse(text4, out var result))
					{
						num = result;
					}
					else if (text2 == "fps" && double.TryParse(text4, NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
					{
						fps = result2;
					}
					else if (text2 == "progress")
					{
						progress = text4;
					}
				}
			}
			return num.HasValue ? new FfmpegProgressSnapshot(num.Value, fps, progress) : null;
		}
		catch (Exception ex) when (IsTelemetryIoFailure(ex))
		{
			Disable(sessionId, ex, "讀取 FFmpeg progress");
			return null;
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
			File.Delete(GetProgressPath(sessionId));
		}
		catch (Exception ex) when (IsTelemetryIoFailure(ex))
		{
			LoggerExtensions.LogDebug((ILogger)(object)_logger, ex, "JVFI：清理 telemetry 檔案失敗，session={SessionId}；忽略此非關鍵錯誤。", new object[1] { sessionId });
		}
	}

	private string GetProgressPath(string sessionId)
	{
		return GetPath(sessionId) + ".ffprogress";
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

	private void TryEnsureDirectory()
	{
		try
		{
			EnsureDirectory();
		}
		catch (Exception ex) when (IsTelemetryIoFailure(ex))
		{
			LoggerExtensions.LogWarning((ILogger)(object)_logger, ex, "JVFI：metrics runtime 目錄建立失敗，telemetry 將 fail-soft；directory={Directory}", new object[1] { _directory });
		}
	}

	private void Disable(string sessionId, Exception ex, string operation)
	{
		if (_disabledSessions.TryAdd(sessionId, 0))
		{
			LoggerExtensions.LogWarning((ILogger)(object)_logger, ex, "JVFI：{Operation}失敗，本次 session 已停用 metrics/progress telemetry，但補幀仍會繼續。session={SessionId} directory={Directory}", new object[3] { operation, sessionId, _directory });
		}
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
