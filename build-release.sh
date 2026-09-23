#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")" && pwd)"
VERSION="${1:-0.7.6.1}"
ZIP="$ROOT/jellyfin-video-frame-interpolation_${VERSION}.zip"

dotnet publish "$ROOT/src/Jellyfin Video Frame Interpolation.csproj" -c Release -o "$ROOT/dist"
cp "$ROOT/packaging/meta.json" "$ROOT/dist/meta.json"
(
  cd "$ROOT/dist"
  zip -r "$ZIP" . -x '*.pdb'
)

MD5=$(md5sum "$ZIP" | awk '{print toupper($1)}')
echo "ZIP: $ZIP"
echo "MD5 (for manifest.json checksum): $MD5"
echo "Upload to GitHub Release tag v${VERSION} as: $(basename "$ZIP")"
