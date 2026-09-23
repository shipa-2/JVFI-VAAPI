#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")" && pwd)"
VERSION="${1:-0.7.6.2}"
ZIP="$ROOT/jellyfin-video-frame-interpolation_${VERSION}.zip"
OUT="$ROOT/src/bin/Release/net10.0"

dotnet build "$ROOT/src/Jellyfin Video Frame Interpolation.csproj" -c Release

rm -rf "$ROOT/dist"
mkdir -p "$ROOT/dist"
cp "$OUT/Jellyfin Video Frame Interpolation.dll" "$OUT/Jellyfin Video Frame Interpolation.deps.json" "$ROOT/dist/"
cp "$ROOT/packaging/meta.json" "$ROOT/dist/meta.json"

if [[ -f "$ROOT/dist/0Harmony.dll" ]]; then
  echo "ERROR: 0Harmony.dll must be embedded (Costura), not shipped separately." >&2
  exit 1
fi

(
  cd "$ROOT/dist"
  zip -r "$ZIP" . -x '*.pdb'
)

MD5=$(md5sum "$ZIP" | awk '{print toupper($1)}')
echo "ZIP: $ZIP"
echo "MD5 (for manifest.json checksum): $MD5"
echo "Upload to GitHub Release tag v${VERSION} as: $(basename "$ZIP")"
