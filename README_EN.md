<p align="center">
  <img src="assets/jvfi-banner.png" alt="JVFI-VAAPI" width="900">
</p>

# JVFI-VAAPI (Jellyfin frame interpolation · VAAPI fix)

[繁體中文](README.md) | [简体中文](README.zh-CN.md) | **English** | [Русский](README.ru.md)

Unofficial fork of [SkillGodAk/JVFI](https://github.com/SkillGodAk/JVFI) with **source code** and a fix for **Jellyfin 12 + VAAPI** (`-filter_hw_device vk` vs `h264_vaapi` / FFmpeg exit **218**). Based on JVFI **0.7.6.0**, built for **Jellyfin 12.1** (.NET 10).

## Install from the Jellyfin catalog

`Dashboard` → `Plugins` → `Repositories` → add:

- Name: `JVFI-VAAPI`
- Repository URL:

```text
https://shipa-2.github.io/JVFI-VAAPI/manifest.json
```

Install **JVFI** from the catalog, then **fully restart** Jellyfin.

(GitHub Pages must be enabled on this repo. If the manifest URL fails, use manual install.)

## Manual installation

Download the ZIP from [GitHub Releases](https://github.com/shipa-2/JVFI-VAAPI/releases) and extract to:

```text
jellyfin/config/plugins/Jellyfin Video Frame Interpolation/
```

Docker example:

```text
/volume1/docker/jellyfin/config/plugins/Jellyfin Video Frame Interpolation/
```

Remove a previous official `JVFI_0.7.6.0` plugin folder if present. **Fully restart** Jellyfin.

## Build from source

```bash
git clone https://github.com/shipa-2/JVFI-VAAPI.git
cd JVFI-VAAPI/src
dotnet publish -c Release -o ../dist
```

Copy everything under `dist/` into the plugin folder above, then restart.

## Usage

Enable VAAPI in Jellyfin playback settings; in **JVFI** set **Output encoder** to **Vaapi**, enable interpolation, select users/libraries. Playback must **transcode** (not Direct Play).

## Supported environments

| Item | Status |
|---|---|
| Jellyfin **12.1.x** | Primary target for this fork |
| Linux AMD / Intel VAAPI | Fix targets this pipeline |

Not affiliated with the Jellyfin project. Original JVFI by SkillGodAK.
