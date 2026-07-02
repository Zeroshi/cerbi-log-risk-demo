#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

mkdir -p scan-results

echo "Recording .NET SDK details..."
dotnet --info > /tmp/dotnet-info.txt

echo "Restoring Cerbi demo projects..."
dotnet restore src/dotnet/UnsafeApi/UnsafeApi.csproj
dotnet restore src/dotnet/SafeApi/SafeApi.csproj

echo "Building Cerbi demo projects..."
dotnet build src/dotnet/UnsafeApi/UnsafeApi.csproj --no-restore
dotnet build src/dotnet/SafeApi/SafeApi.csproj --no-restore

if command -v cerbi-scanner >/dev/null 2>&1; then
  echo "Cerbi Scanner is already available on PATH."
elif [ -n "${CERBI_SCANNER_PACKAGE:-}" ]; then
  echo "Installing Cerbi Scanner from CERBI_SCANNER_PACKAGE=${CERBI_SCANNER_PACKAGE}..."
  dotnet tool install --global "$CERBI_SCANNER_PACKAGE"
  echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> "$HOME/.bashrc"
else
  cat <<'MESSAGE'
Cerbi Scanner was not installed because CERBI_SCANNER_PACKAGE is not set.
Codespaces is ready for the public demo projects. If your scanner is distributed
as a .NET tool, install it manually from the repo root with:

  dotnet tool install --global <YOUR_CERBI_SCANNER_PACKAGE_ID>

No secrets are required by this dev container.
MESSAGE
fi
