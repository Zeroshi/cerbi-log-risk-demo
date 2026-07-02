#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

export PATH="$PATH:$HOME/.dotnet/tools"
scanner_package="${CERBI_SCANNER_PACKAGE:-Cerbi.Scanner}"

mkdir -p scan-results

if ! grep -Fq '$HOME/.dotnet/tools' "$HOME/.bashrc" 2>/dev/null; then
  echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> "$HOME/.bashrc"
fi

echo "Recording .NET SDK details..."
dotnet --info > /tmp/dotnet-info.txt

echo "Restoring Cerbi demo projects..."
dotnet restore src/dotnet/UnsafeApi/UnsafeApi.csproj
dotnet restore src/dotnet/SafeApi/SafeApi.csproj

echo "Building Cerbi demo projects..."
dotnet build src/dotnet/UnsafeApi/UnsafeApi.csproj --no-restore
dotnet build src/dotnet/SafeApi/SafeApi.csproj --no-restore

echo "Installing or updating Cerbi Scanner package: ${scanner_package}..."
if dotnet tool list --global | awk 'NR > 2 { print $1 }' | grep -Fxq "$(printf '%s' "$scanner_package" | tr '[:upper:]' '[:lower:]')"; then
  dotnet tool update --global "$scanner_package"
else
  dotnet tool install --global "$scanner_package"
fi

cerbi-scanner --help >/tmp/cerbi-scanner-help.txt

echo "Codespaces setup complete. Run the scanner with the command documented in README.md and docs/codespaces.md."
