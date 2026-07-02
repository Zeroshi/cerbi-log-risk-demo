#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

dotnet_tools_path="$HOME/.dotnet/tools"
export PATH="$PATH:$dotnet_tools_path"
scanner_package="${CERBI_SCANNER_PACKAGE:-Cerbi.Scanner}"
scanner_version="${CERBI_SCANNER_VERSION:-1.1.0}"

mkdir -p scan-results

ensure_dotnet_tools_path() {
  local profile_file="$1"

  touch "$profile_file"
  if ! grep -Fq '$HOME/.dotnet/tools' "$profile_file"; then
    echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> "$profile_file"
  fi
}

ensure_dotnet_tools_path "$HOME/.bashrc"
ensure_dotnet_tools_path "$HOME/.profile"

echo "Verifying .NET SDK version..."
dotnet --version

echo "Verifying installed .NET runtimes..."
dotnet --list-runtimes

echo "Restoring Cerbi demo projects..."
dotnet restore src/dotnet/UnsafeApi/UnsafeApi.csproj
dotnet restore src/dotnet/SafeApi/SafeApi.csproj

echo "Building Cerbi demo projects..."
dotnet build src/dotnet/UnsafeApi/UnsafeApi.csproj --no-restore
dotnet build src/dotnet/SafeApi/SafeApi.csproj --no-restore

echo "Installing or updating Cerbi Scanner package: ${scanner_package} >= ${scanner_version}..."
installed_tool_ids="$(dotnet tool list --global | awk 'NR > 2 { print tolower($1) }')"
scanner_package_id="$(printf '%s' "$scanner_package" | tr '[:upper:]' '[:lower:]')"

if printf '%s\n' "$installed_tool_ids" | grep -Fxq "$scanner_package_id"; then
  dotnet tool update --global "$scanner_package" --version "$scanner_version"
else
  dotnet tool install --global "$scanner_package" --version "$scanner_version"
fi

echo "Verifying Cerbi Scanner command..."
cerbi-scanner --version
cerbi-scanner scan --help

echo "Codespaces setup complete. Run the scanner with the command documented in README.md and docs/codespaces.md."
