using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JVFI.Runtime;
using MediaBrowser.Common.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JVFI.Services;

/// <summary>Probes the official Jellyfin FFmpeg and optional accelerator engines without changing playback behavior.</summary>
public sealed class HardwareCapabilityService : IHostedService
{
	private static readonly string[] FfmpegCandidates = ((!OperatingSystem.IsWindows()) ? new string[3] { "/usr/lib/jellyfin-ffmpeg/ffmpeg", "/usr/local/bin/ffmpeg", "ffmpeg" } : new string[3] { "C:\\Program Files\\Jellyfin\\Server\\ffmpeg.exe", "C:\\Program Files\\Jellyfin\\Server\\ffmpeg\\ffmpeg.exe", "ffmpeg.exe" });

	private readonly ILogger<HardwareCapabilityService> _logger;

	private readonly IConfigurationManager _configurationManager;

	private readonly SemaphoreSlim _probeLock = new SemaphoreSlim(1, 1);

	private HardwareCapabilityReport _current = HardwareCapabilityParser.Failed("尚未執行探測");

	public HardwareCapabilityReport Current => Volatile.Read(in _current);

	public HardwareCapabilityService(ILogger<HardwareCapabilityService> logger, IConfigurationManager configurationManager)
	{
		_logger = logger;
		_configurationManager = configurationManager;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await RefreshAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public async Task<HardwareCapabilityReport> RefreshAsync(CancellationToken cancellationToken)
	{
		await _probeLock.WaitAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		try
		{
			string ffmpegPath = FindFfmpeg();
			if (ffmpegPath == null)
			{
				return StoreFailure("找不到 Jellyfin FFmpeg");
			}
			string version = await RunAsync(ffmpegPath, "-version", cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			string filters = await RunAsync(ffmpegPath, "-hide_banner -filters", cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			bool flag = filters.Contains("framerate", StringComparison.OrdinalIgnoreCase);
			if (flag)
			{
				flag = await TryRunAsync(ffmpegPath, HardwareSelfTestCommandBuilder.BuildFramerate(), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			bool framerateSelfTestPassed = flag;
			HardwareCapabilityReport hardwareCapabilityReport = HardwareCapabilityParser.Parse(ffmpegPath, version, filters, framerateSelfTestPassed, minterpolateSelfTestPassed: false, qsvFrcSelfTestPassed: false, GetJellyfinHardwarePipeline());
			Volatile.Write(ref _current, hardwareCapabilityReport);
			LoggerExtensions.LogInformation((ILogger)(object)_logger, "JVFI：官方 FFmpeg 能力探測完成，解碼={Decode}，補幀={Interpolation}，編碼={Encode}，原因={Reason}", new object[4] { hardwareCapabilityReport.DecodeBackend, hardwareCapabilityReport.InterpolationBackend, hardwareCapabilityReport.EncodeBackend, hardwareCapabilityReport.Reason });
			return hardwareCapabilityReport;
		}
		catch (Exception ex) when (!(ex is OperationCanceledException))
		{
			LoggerExtensions.LogWarning((ILogger)(object)_logger, ex, "JVFI：官方 FFmpeg 能力探測失敗，已停用補幀並保留 Jellyfin 原始播放路徑", Array.Empty<object>());
			return StoreFailure(ex.Message);
		}
		finally
		{
			_probeLock.Release();
		}
	}

	private HardwarePipeline GetJellyfinHardwarePipeline()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return OutputEncoderSelector.FromJellyfinHardware(((object)EncodingConfigurationExtensions.GetEncodingOptions(_configurationManager).HardwareAccelerationType/*cast due to constrained. prefix*/).ToString());
		}
		catch (Exception ex)
		{
			LoggerExtensions.LogWarning((ILogger)(object)_logger, ex, "JVFI：無法讀取 Jellyfin 硬體加速設定，能力頁將退回 FFmpeg 可用能力判斷。", Array.Empty<object>());
			return HardwarePipeline.Unknown;
		}
	}

	private HardwareCapabilityReport StoreFailure(string reason)
	{
		HardwareCapabilityReport hardwareCapabilityReport = HardwareCapabilityParser.Failed(reason);
		Volatile.Write(ref _current, hardwareCapabilityReport);
		LoggerExtensions.LogWarning((ILogger)(object)_logger, "JVFI：{Reason}", new object[1] { hardwareCapabilityReport.Reason });
		return hardwareCapabilityReport;
	}

	private static string? FindFfmpeg()
	{
		string[] ffmpegCandidates = FfmpegCandidates;
		foreach (string text in ffmpegCandidates)
		{
			if (Path.IsPathRooted(text))
			{
				if (File.Exists(text))
				{
					return text;
				}
				continue;
			}
			return text;
		}
		return null;
	}

	private static async Task<string> RunAsync(string executable, string arguments, CancellationToken cancellationToken)
	{
		using Process process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = executable,
				Arguments = arguments,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			}
		};
		process.Start();
		Task<string> stdout = process.StandardOutput.ReadToEndAsync(cancellationToken);
		Task<string> stderr = process.StandardError.ReadToEndAsync(cancellationToken);
		await process.WaitForExitAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		string result = await stdout.ConfigureAwait(continueOnCapturedContext: false) + "\n" + await stderr.ConfigureAwait(continueOnCapturedContext: false);
		if (process.ExitCode != 0)
		{
			throw new InvalidOperationException($"FFmpeg 探測命令失敗，結束碼 {process.ExitCode}");
		}
		return result;
	}

	private static async Task<bool> TryRunAsync(string executable, string arguments, CancellationToken cancellationToken)
	{
		try
		{
			await RunAsync(executable, arguments, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return true;
		}
		catch (InvalidOperationException)
		{
			return false;
		}
	}
}
