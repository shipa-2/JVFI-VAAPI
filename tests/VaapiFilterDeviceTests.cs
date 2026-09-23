using System;
using Jellyfin.Plugin.JVFI.Runtime;
using Xunit;

namespace JvfiFork.Tests;

public sealed class VaapiFilterDeviceTests
{
    [Fact]
    public void Transform_uses_cpu_decode_and_named_vaapi_upload_for_tonga_hybrid()
    {
        const string commandLine =
            "-init_hw_device drm=dr:/dev/dri/renderD128 -init_hw_device vaapi=va@dr " +
            "-init_hw_device vulkan=vk@dr -filter_hw_device vk -hwaccel vaapi " +
            "-hwaccel_output_format vaapi -i file:\"/media/test.mp4\" " +
            "-codec:v:0 h264_vaapi -vf \"scale_vaapi=format=nv12\"";

        var request = new FfmpegTransformRequest(
            TargetFps: 60,
            MaxWidth: 0,
            HudTextFile: string.Empty,
            ShowHud: false,
            HudPosition: "TopLeft",
            EncoderMode: OutputEncoderMode.Vaapi,
            JellyfinHardwarePipeline: HardwarePipeline.Vaapi,
            Backend: InterpolationBackend.OfficialFramerate,
            SourceWidth: 960,
            SourceHeight: 540);

        var result = FfmpegCommandTransformer.Transform(commandLine, request);

        Assert.True(result.Applied);
        Assert.Contains("-filter_hw_device va", result.CommandLine, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("-filter_hw_device vk", result.CommandLine, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("-hwaccel vaapi", result.CommandLine, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("-init_hw_device vaapi=va@dr", result.CommandLine, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("scale_vaapi=format=nv12,hwdownload", result.CommandLine, StringComparison.Ordinal);
        Assert.Contains("framerate=fps=60/1,format=nv12,hwupload", result.CommandLine, StringComparison.Ordinal);
        Assert.DoesNotContain("hwupload_vaapi", result.CommandLine, StringComparison.Ordinal);
    }

    [Fact]
    public void Transform_upscales_after_upload_when_vaapi_target_is_larger()
    {
        const string commandLine =
            "-init_hw_device drm=dr:/dev/dri/renderD128 -init_hw_device vaapi=va@dr " +
            "-init_hw_device vulkan=vk@dr -filter_hw_device vk -hwaccel vaapi " +
            "-hwaccel_output_format vaapi -i file:\"/media/test.mp4\" " +
            "-codec:v:0 h264_vaapi -vf \"scale_vaapi=format=nv12\"";

        var request = new FfmpegTransformRequest(
            TargetFps: 60,
            MaxWidth: 0,
            HudTextFile: string.Empty,
            ShowHud: false,
            HudPosition: "TopLeft",
            EncoderMode: OutputEncoderMode.Vaapi,
            JellyfinHardwarePipeline: HardwarePipeline.Vaapi,
            Backend: InterpolationBackend.OfficialFramerate,
            SourceWidth: 960,
            SourceHeight: 540,
            OutputScaleHeight: 1080);

        var result = FfmpegCommandTransformer.Transform(commandLine, request);

        Assert.True(result.Applied);
        Assert.Contains("framerate=fps=60/1,format=nv12,hwupload,scale_vaapi=w=-2:h=1080:format=nv12", result.CommandLine, StringComparison.Ordinal);
    }

    [Fact]
    public void Transform_strips_vaapi_for_software_encoder_to_avoid_gpu_hybrid()
    {
        const string commandLine =
            "-init_hw_device drm=dr:/dev/dri/renderD128 -init_hw_device vaapi=va@dr " +
            "-init_hw_device vulkan=vk@dr -filter_hw_device vk -hwaccel vaapi " +
            "-hwaccel_output_format vaapi -i file:\"/media/test.mp4\" " +
            "-codec:v:0 h264_vaapi -vf \"setparams=color_primaries=bt709,scale_vaapi=format=nv12\"";

        var request = new FfmpegTransformRequest(
            TargetFps: 60,
            MaxWidth: 0,
            HudTextFile: string.Empty,
            ShowHud: false,
            HudPosition: "TopLeft",
            EncoderMode: OutputEncoderMode.Software,
            JellyfinHardwarePipeline: HardwarePipeline.Vaapi,
            Backend: InterpolationBackend.OfficialFramerate,
            SourceWidth: 960,
            SourceHeight: 540);

        var result = FfmpegCommandTransformer.Transform(commandLine, request);

        Assert.True(result.Applied);
        Assert.Contains("libx264", result.CommandLine, StringComparison.Ordinal);
        Assert.DoesNotContain("-hwaccel vaapi", result.CommandLine, StringComparison.Ordinal);
        Assert.DoesNotContain("scale_vaapi", result.CommandLine, StringComparison.Ordinal);
        Assert.DoesNotContain("init_hw_device", result.CommandLine, StringComparison.Ordinal);
        Assert.Contains("framerate=fps=60/1", result.CommandLine, StringComparison.Ordinal);
    }
}
