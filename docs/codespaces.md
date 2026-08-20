# Run the Cerbi demo in GitHub Codespaces

GitHub Codespaces gives each user an isolated development environment for this repository. A prospect can open the demo, build the sample applications, and run Cerbi Scanner without changing their local machine.

## What Codespaces creates

- A Codespace scoped to this repository.
- The Microsoft .NET 10 devcontainer image configured in `.devcontainer/devcontainer.json`.
- Two small .NET 10 sample applications: one intentionally unsafe, one corrected.
- Cerbi Scanner 1.1.0 installed as the `Cerbi.Scanner` .NET global tool.
- `README.md` and `docs/demo-script.md` opened automatically for the walkthrough.
- No database, message broker, production service, or Cerbi account requirement.

## Open the demo

Use the public Codespaces link:

https://codespaces.new/Zeroshi/cerbi-log-risk-demo?quickstart=1

If you already had a Codespace open before the .NET 10 devcontainer update, rebuild it from the VS Code Command Palette with **Codespaces: Rebuild Container**, or create a fresh Codespace after the change is merged.

## Verify the environment

From the repository root:

```bash
dotnet --version
dotnet --list-runtimes
cerbi-scanner --version
cerbi-scanner scan --help
```

The SDK should report a `10.0.x` version and Scanner should report 1.1.0.

Build the sample applications:

```bash
dotnet build src/dotnet/UnsafeApi/UnsafeApi.csproj
dotnet build src/dotnet/SafeApi/SafeApi.csproj
```

## Run report-only first

The first scan should report findings without failing the command:

```bash
mkdir -p scan-results
cerbi-scanner scan \
  --path . \
  --policy policies/cerbi-policy.yml \
  --fail-on none \
  --format json --output scan-results/findings.json \
  --sarif scan-results/findings.sarif \
  --summary scan-results/build-summary.md
```

Review the generated outputs:

```bash
code scan-results/findings.json scan-results/findings.sarif scan-results/build-summary.md
```

## Demonstrate a CI gate

After reviewing report-only output, rerun with a failure threshold:

```bash
cerbi-scanner scan \
  --path . \
  --policy policies/cerbi-policy.yml \
  --fail-on error \
  --format json --output scan-results/findings.json \
  --sarif scan-results/findings.sarif \
  --summary scan-results/build-summary.md
```

The unsafe demo intentionally contains high-severity findings, so `--fail-on error` should return a non-zero exit code.

## Expected output

The checked-in Scanner 1.1.0 report under `examples/findings.json` contains 12 findings. It demonstrates sensitive data, credential/raw-payload risk, disallowed structured fields, high-cardinality data, and risky object-state exposure.

Examples from the checked-in report include:

- `CERBI003` — `password` at `src/dotnet/UnsafeApi/Program.cs:20`
- `CERBI001` — `email` at `src/dotnet/UnsafeApi/Program.cs:20`
- `CERBI004` — `sessionId` at `src/dotnet/UnsafeApi/Program.cs:24`

The corrected application in `src/dotnet/SafeApi` shows the intended safer logging pattern.

If you only want to inspect stable output, open the checked-in reports:

```bash
code examples/findings.json examples/findings.sarif examples/build-summary.md
```

## Data handling

The sample repository contains intentionally unsafe **demo** logging patterns only. It is not connected to live customer systems or production data.

Scanner runs locally inside the Codespace against the checked-out files. It does not modify the application source and does not upload scan results unless upload is explicitly enabled. Generated files are written under ignored `scan-results/`.

## Troubleshooting

### Scanner command is missing

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool update -g Cerbi.Scanner --version 1.1.0 || \
  dotnet tool install -g Cerbi.Scanner --version 1.1.0
```

### The Codespace still reports .NET 9

The Codespace was created from the previous devcontainer definition. Rebuild the container or create a new Codespace from the updated branch/main after merge.

### Restore fails

```bash
dotnet restore src/dotnet/UnsafeApi/UnsafeApi.csproj
dotnet restore src/dotnet/SafeApi/SafeApi.csproj
```

### Commands are running from the wrong directory

```bash
cd "$(git rev-parse --show-toplevel)"
```

## CI examples

- GitHub Actions: `.github/workflows/cerbi-scan.yml`
- Azure DevOps: `.azure-pipelines/cerbi-scan.yml`

Both examples now provision .NET 10 LTS and run the same Scanner 1.1.0 CLI used in Codespaces.
