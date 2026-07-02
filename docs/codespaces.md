# Run the Cerbi demo in GitHub Codespaces

GitHub Codespaces gives each user their own isolated development environment for this repository. A prospect can open the demo, build the .NET sample applications, and run the Cerbi Scanner without changing their local machine.

## What Codespaces creates

- Each user creates their own Codespace from the repository or fork they selected.
- The container uses the Microsoft .NET 10 devcontainer image configured in `.devcontainer/devcontainer.json`.
- Container startup restores and builds the unsafe and safe sample applications targeting .NET 10.
- `README.md` and `docs/demo-script.md` open automatically in VS Code for the walkthrough.
- The setup script installs or updates the default `Cerbi.Scanner` .NET tool package so `cerbi-scanner` is available on `PATH`.
- No secrets are required for the sample application projects.

If your Cerbi Scanner package is private, authenticate to that package source in your own Codespace before installing it. The public demo repository does not include private package credentials.

## Rebuild after devcontainer changes

Devcontainer image and setup-script changes are applied when the Codespace container is created or rebuilt. If you opened this repository before a devcontainer fix landed, do one of the following:

- Rebuild the existing Codespace from VS Code with **Command Palette** > **Codespaces: Rebuild Container**.
- Or delete the broken Codespace in GitHub, then create a new Codespace from the updated `main` branch.

## Data handling and safety

This demo is intentionally safe sample code. It contains illustrative unsafe logging patterns so the scanner can show findings, but it is not connected to production services, databases, queues, or live customer systems.

No application code is uploaded to Cerbi by the sample projects or the dev container. The scanner command runs locally inside the Codespace against the checked-out repository. Generated outputs are written to local files under ignored `scan-results/`; keep committed demo examples under `examples/`.

## Expected commands

Run commands from the repository root. If you are unsure where you are, reset to the root first:

```bash
cd "$(git rev-parse --show-toplevel)"
```

Build the sample applications:

```bash
dotnet --version
dotnet --list-runtimes
cerbi-scanner --help
dotnet build src/dotnet/UnsafeApi/UnsafeApi.csproj
dotnet build src/dotnet/SafeApi/SafeApi.csproj
```

Run the scanner. This is the exact Codespaces command users should run from the repository root:

```bash
mkdir -p scan-results
cerbi-scanner scan \
  --path . \
  --policy policies/cerbi-policy.yml \
  --fail-on error \
  --format json --output scan-results/findings.json \
  --sarif scan-results/findings.sarif \
  --summary scan-results/build-summary.md
```

Review the generated outputs:

```bash
cat scan-results/build-summary.md
code scan-results/findings.json scan-results/findings.sarif
```

## Expected output

The scanner should report risky logging examples in `src/dotnet/UnsafeApi`, including raw payloads, credential-like fields, authorization headers, regulated identifiers, and high-cardinality values.

The corrected examples in `src/dotnet/SafeApi` show the intended pattern: stable event names, required governance fields, and no sensitive data in structured log properties.

If you do not have a licensed scanner available in the Codespace, use the checked-in sample outputs instead:

```bash
code examples/build-summary.md examples/findings.json examples/findings.sarif
```

## Troubleshooting

### Scanner command is not found

If your licensed scanner package ID differs from the public default, install that package and then rerun the scan:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool update --global <YOUR_CERBI_SCANNER_PACKAGE_ID> || dotnet tool install --global <YOUR_CERBI_SCANNER_PACKAGE_ID>
```

### Restore fails

Rerun restore explicitly from the repository root:

```bash
dotnet restore src/dotnet/UnsafeApi/UnsafeApi.csproj
dotnet restore src/dotnet/SafeApi/SafeApi.csproj
```

### Commands fail in a fork or copied workspace

Confirm the repository root and rerun from there:

```bash
pwd
git rev-parse --show-toplevel
cd "$(git rev-parse --show-toplevel)"
```

### Codespace was created before a devcontainer fix

Rebuild the existing Codespace from VS Code with **Command Palette** > **Codespaces: Rebuild Container**, or delete the broken Codespace and create a new Codespace from the updated branch.

## Codespaces deep link placeholder

The README intentionally uses this placeholder badge target:

```markdown
[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](REPLACE_WITH_CODESPACES_DEEP_LINK)
```

Do not replace it with a guessed URL. After the repository is published in GitHub, generate the final Codespaces link from the GitHub UI and paste it into the badge target in `README.md`.

## Fresh rebuild expectation

After a full Codespaces rebuild, or after deleting and recreating the Codespace from the updated branch, the demo should work without runtime mismatch errors: the container uses .NET 10, both sample APIs target `net10.0`, startup restores and builds both projects, and the `Cerbi.Scanner` tool is installed or updated before the walkthrough.
