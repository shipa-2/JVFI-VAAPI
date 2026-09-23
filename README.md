# JVFI-VAAPI

Fork of [SkillGodAk/JVFI](https://github.com/SkillGodAk/JVFI) with **plugin source** and a fix for **VAAPI + Jellyfin 12** on AMD/Intel (Vulkan `filter_hw_device` vs `h264_vaapi`).

Upstream publishes release ZIPs only; this repository adds decompiled/patched C# (based on JVFI **0.7.6.0**) and builds against **Jellyfin 12.1** (`net10.0`).

## Problem

Jellyfin often passes `-filter_hw_device vk`. JVFI runs CPU `framerate` then `hwupload` before `h264_vaapi`, so frames stay on **Vulkan** while the encoder expects **VAAPI** → FFmpeg exit **218**, player fatal error.

## Fix

In `FfmpegCommandTransformer.EnsureFilterHwDeviceForOutputPipeline`, when the output pipeline is VAAPI:

`-filter_hw_device vk` → `-filter_hw_device va` (matches `-init_hw_device vaapi=va@dr`).

## Build

```bash
cd src
dotnet publish -c Release -o ../dist
```

Install: copy `dist/*` into Jellyfin `config/plugins/Jellyfin Video Frame Interpolation/`, full restart. Use **Output encoder: Vaapi** in JVFI settings.

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download) (same as Jellyfin 12.1).

## Tests

```bash
cd tests
dotnet test -c Release
```

(Requires ASP.NET Core 10 runtime for the test host.)

## Catalog (optional)

Original `manifest.json` and `assets/` from upstream can still be used for GitHub Pages; point releases to this repo’s build artifacts when you publish them.

## Credits

Original **JVFI** by SkillGodAK. This fork is unofficial; consider opening an upstream PR.
