<p align="center">
  <img src="assets/jvfi-banner.png" alt="JVFI-VAAPI" width="900">
</p>

# JVFI-VAAPI (форк JVFI)

[English](README_EN.md) | [繁體中文](README.md) | **Русский**

**JVFI-VAAPI** — неофициальный форк [SkillGodAk/JVFI](https://github.com/SkillGodAk/JVFI) с **исходным кодом** и исправлением **VAAPI на Jellyfin 12** (AMD / Intel: ошибка FFmpeg 218, «фатальная ошибка плеера» при `h264_vaapi`).

База: JVFI **0.7.6.0**, сборка под **Jellyfin 12.1** (.NET 10). Оригинальный репозиторий публикует только ZIP без исходников; здесь — патч `-filter_hw_device vk` → `va` перед `hwupload` + `h264_vaapi`.

Клиентам (Web, Media Player, Android / TV) отдельно ничего ставить не нужно.

## Основные возможности

Как у JVFI: целевой FPS **23.976–240** (по умолчанию **60**), серверная интерполяция, VAAPI / QSV / NVENC и fallback на CPU, HUD, выбор пользователей и библиотек.

Дополнительно в этом форке:

- Исправлен пайплайн **VAAPI encode** на Jellyfin 12, когда в команде ffmpeg стоит `-filter_hw_device vk`
- Рекомендуется **Output encoder: Vaapi** (не только Software) на AMD с `/dev/dri/renderD128`

## Установка

### Через каталог плагинов Jellyfin

1. Открой **Панель управления** → **Плагины** → **Репозитории** → **Добавить**.
2. Укажи:

| Поле | Значение |
|------|----------|
| Имя | `JVFI-VAAPI` |
| URL репозитория | `https://shipa-2.github.io/JVFI-VAAPI/manifest.json` |

3. Сохрани, зайди в **Каталог** плагинов, найди **JVFI** (или **JVFI-VAAPI**), установи.
4. **Полностью перезапусти** Jellyfin (на TrueNAS — stop/start приложения, не только «Restart» в UI).

> Каталог работает после включения **GitHub Pages** в репозитории (ветка / Actions). Если manifest ещё недоступен — используй ручную установку ниже.

### Ручная установка

1. Скачай ZIP с [GitHub Releases](https://github.com/shipa-2/JVFI-VAAPI/releases) (файл `jellyfin-video-frame-interpolation_0.7.6.1.zip`). Если релиза ещё нет — собери локально: `./build-release.sh` в корне репозитория, затем используй полученный ZIP.
2. Распакуй содержимое в каталог плагинов Jellyfin:

```text
config/plugins/Jellyfin Video Frame Interpolation/
```

**TrueNAS SCALE (приложение Jellyfin)** — путь к `config` зависит от dataset; типично что-то вроде:

```text
/mnt/<pool>/ix-applications/releases/jellyfin/<version>/config/plugins/Jellyfin Video Frame Interpolation/
```

или bind-mount, который ты указал при установке app. Положи туда **все файлы из ZIP** (DLL, deps.json, Harmony и т.д.).

**Docker (пример):**

```text
/volume1/docker/jellyfin/config/plugins/Jellyfin Video Frame Interpolation/
```

3. Удали старую папку **`JVFI_0.7.6.0`**, если ставил официальный плагин из другого ZIP (чтобы не было двух версий).
4. **Полностью перезапусти** Jellyfin.

### Сборка из исходников (если нет готового Release)

На машине с [.NET 10 SDK](https://dotnet.microsoft.com/download):

```bash
git clone https://github.com/shipa-2/JVFI-VAAPI.git
cd JVFI-VAAPI/src
dotnet publish -c Release -o ../dist
```

Скопируй всё из **`dist/`** в `config/plugins/Jellyfin Video Frame Interpolation/`, перезапусти сервер.

## Настройка Jellyfin и JVFI

1. **Воспроизведение** → **Транскoding**: аппаратное ускорение **VAAPI**, устройство **`/dev/dri/renderD128`**, кодирование включено.
2. **TrueNAS**: GPU должна быть назначена приложению Jellyfin.
3. **Плагины** → **JVFI**:
   - включи интерполяцию;
   - **Output encoder mode**: **Vaapi** (для R9 380X и аналогов);
   - **VAAPI output upscale**: Off / 720p / 1080p / 1440p / 2160p; масштабирование выполняется после интерполяции и только для меньшего исходника (обычный VAAPI scaler, не AI/VSR);
   - отметь пользователя и библиотеку;
   - целевой FPS (для теста можно **48** или **30**);
   - HUD можно **Off**, если мешает.
4. При воспроизведении **нужен транскод** (не Direct Play): в клиенте выбери конвертацию или ограничь bitrate.

## Совместимость

| Пункт | Статус |
|--------|--------|
| Jellyfin **12.1.x** | Целевая версия форка |
| Jellyfin 10.11.x | Оригинальный JVFI; этот форк собран под 12.x |
| Linux + AMD VAAPI | Исправление рассчитано на этот случай |
| CPU-only fallback | Как у JVFI, но 60 FPS на CPU часто не успевает в realtime |

## Отличие от официального JVFI

| | Официальный JVFI | JVFI-VAAPI |
|---|------------------|------------|
| Репозиторий | [SkillGodAk/JVFI](https://github.com/SkillGodAk/JVFI) | [shipa-2/JVFI-VAAPI](https://github.com/shipa-2/JVFI-VAAPI) |
| Исходники | Нет (только ZIP) | Есть в `src/` |
| VAAPI + Jellyfin 12 | Может падать с кодом 218 | Патч `filter_hw_device` |

Плагин **SkillGodAK** — сторонний продукт; форк **неофициальный**.
