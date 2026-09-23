<p align="center">
  <img src="assets/jvfi-banner.png" alt="JVFI-VAAPI" width="900">
</p>

# JVFI-VAAPI（Jellyfin 实时补帧 · VAAPI 修正版）

[繁體中文](README.md) | **简体中文** | [English](README_EN.md) | [Русский](README.ru.md)

本仓库为 [SkillGodAk/JVFI](https://github.com/SkillGodAk/JVFI) 的 **VAAPI 修正 fork**（基于 JVFI **0.7.6.0**，面向 **Jellyfin 12.1** 构建）。修复 Jellyfin 12 在 VAAPI 路径上 `-filter_hw_device vk` 与 `h264_vaapi` + `hwupload` 不兼容、导致 FFmpeg **218** 与播放器致命错误的问题。

## 从 Jellyfin 插件目录安装

在 `控制台` → `插件` → `存储库` 中新增：

- 名称：`JVFI-VAAPI`
- 存储库网址：

```text
https://shipa-2.github.io/JVFI-VAAPI/manifest.json
```

安装后**完整重启** Jellyfin。

## 手动安装

从 [GitHub Releases](https://github.com/shipa-2/JVFI-VAAPI/releases) 下载 ZIP，解压到：

```text
jellyfin/config/plugins/Jellyfin Video Frame Interpolation/
```

Docker 示例：

```text
/volume1/docker/jellyfin/config/plugins/Jellyfin Video Frame Interpolation/
```

删除旧版 `JVFI_0.7.6.0` 目录（如有），**完整重启** Jellyfin。

## 从源码构建

```bash
git clone https://github.com/shipa-2/JVFI-VAAPI.git
cd JVFI-VAAPI/src
dotnet publish -c Release -o ../dist
```

将 `dist/` 复制到插件目录后重启。

## 使用

启用 JVFI、选择用户与库、**Output encoder** 建议 **Vaapi**；播放需**转码**而非 Direct Play。
