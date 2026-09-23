# JVFI-VAAPI 0.7.6.1

## Changes

- Fix VAAPI hardware encode on Jellyfin 12 when the transcode command uses `-filter_hw_device vk`: rewrite to `-filter_hw_device va` before JVFI `hwupload` + `h264_vaapi`.
- Based on upstream JVFI **0.7.6.0**; build target **Jellyfin 12.1** (.NET 10).

## Install

See [README.ru.md](README.ru.md) (Russian) or [README_EN.md](README_EN.md).

---

# Русский

- Исправлен пайплайн VAAPI на Jellyfin 12 (ошибка 218 / падение плеера).
- Установка: Releases ZIP или `manifest.json` → `https://shipa-2.github.io/JVFI-VAAPI/manifest.json`
