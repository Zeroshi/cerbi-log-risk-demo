# Find unsafe logs before they reach production

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/Zeroshi/cerbi-log-risk-demo?quickstart=1)

This public demo shows the Scanner-first Cerbi adoption path for platform engineering, security, and DevSecOps teams:

1. Unsafe logging exists in normal application code.
2. Cerbi Scanner identifies sensitive, risky, and costly logging patterns before production.
3. CI/CD can report findings first and later fail based on policy.
4. Developers can fix the logging issue with safer structured logging patterns.
5. Runtime governance can optionally add defense-in-depth after the static-analysis path is understood.

The sample is intentionally small. It runs without databases, message brokers, or production dependencies.

## Current compatibility

- Cerbi Scanner: **1.1.0**
- Scanner runtime: **.NET 10 LTS**
- Demo applications: **.NET 10**
- Scanner package: `Cerbi.Scanner`
- Scanner command: `cerbi-scanner scan`

## What is included

- `src/dotnet/UnsafeApi` — intentionally unsafe ASP.NET Core logging examples.
- `src/dotnet/SafeApi` — corrected structured logging patterns.
- `policies/cerbi-policy.yml` — required/disallowed fields, high-cardinality warnings, and CI threshold.
- `.github/workflows/cerbi-scan.yml` — GitHub Actions CLI integration.
- `.azure-pipelines/cerbi-scan.yml` — Azure DevOps CLI integration.
- `examples/` — checked-in Scanner 1.1.0 JSON, SARIF, HTML, and Markdown sample output.

## Run locally

Prerequisites:

- .NET 10 LTS SDK
- Internet access to install the public `Cerbi.Scanner` .NET global tool if it is not already installed

Install or update Scanner:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool update --global Cerbi.Scanner --version 1.1.0 || \
  dotnet tool install --global Cerbi.Scanner --version 1.1.0
```

Build the demo applications:

```bash
dotnet build src/dotnet/UnsafeApi/UnsafeApi.csproj
dotnet build src/dotnet/SafeApi/SafeApi.csproj
```

Run Scanner in **report-only mode** first:

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

Review the generated reports:

```bash
code scan-results/findings.json scan-results/findings.sarif scan-results/build-summary.md
```

When you want to demonstrate a CI gate, rerun with:

```bash
cerbi-scanner scan \
  --path . \
  --policy policies/cerbi-policy.yml \
  --fail-on error \
  --format json --output scan-results/findings.json \
  --sarif scan-results/findings.sarif \
  --summary scan-results/build-summary.md
```

`--fail-on error` maps to the Scanner high-severity threshold and returns a non-zero exit code when matching demo findings exist.

## Run in GitHub Codespaces

Use this when you want to try the demo without installing .NET locally.

1. Click **Open in GitHub Codespaces** above.
2. Wait for the `.NET 10` dev container to finish its post-create setup.
3. From the repository root, verify the environment:

```bash
dotnet --version
cerbi-scanner --version
cerbi-scanner scan --help
```

4. Run the report-only command from the previous section.
5. Open the generated files under `scan-results/`.

The Codespace contains no production credentials or external application dependencies. The demo Scanner run is local to the Codespace.

For a fuller walkthrough, see [`docs/codespaces.md`](docs/codespaces.md).

## Expected findings

The checked-in Scanner 1.1.0 example report currently contains **12 findings** across the intentionally unsafe sample, including:

- sensitive data (`CERBI001`)
- credential/raw-payload risk (`CERBI002`)
- disallowed structured fields (`CERBI003`)
- high-cardinality fields (`CERBI004`)
- risky object-state/destructuring exposure (`CERBI006`)

For example, the checked-in report identifies `password` and `email` at `src/dotnet/UnsafeApi/Program.cs:20` and a high-cardinality `sessionId` at line 24.

The corrected examples in `src/dotnet/SafeApi` show the intended safer pattern.

## CI/CD examples

### GitHub Actions

`.github/workflows/cerbi-scan.yml`:

- sets up .NET 10 LTS
- installs Cerbi Scanner 1.1.0
- builds the demo applications
- runs `cerbi-scanner scan`
- writes JSON, SARIF, and Markdown output
- uploads SARIF to GitHub code scanning when available

### Azure DevOps

`.azure-pipelines/cerbi-scan.yml` uses the same CLI engine with .NET 10 LTS. Organizations using the free Azure DevOps extension can replace the CLI invocation with `CerbiScan@1`.

## Data handling

Cerbi Scanner performs static analysis against the checked-out repository. It does not modify source files and does not upload scan results unless upload is explicitly enabled.

The demo writes generated output to ignored `scan-results/`. The stable sample reports under `examples/` are checked in so you can inspect real Scanner output without running the tool.

## Troubleshooting

If `cerbi-scanner` is missing or the installed version is stale:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool update -g Cerbi.Scanner --version 1.1.0 || \
  dotnet tool install -g Cerbi.Scanner --version 1.1.0
```

If you opened a Codespace before the .NET 10 devcontainer update, use **Codespaces: Rebuild Container** from the VS Code Command Palette or create a fresh Codespace from `main` after the update is merged.

If restore fails, run:

```bash
dotnet restore src/dotnet/UnsafeApi/UnsafeApi.csproj
dotnet restore src/dotnet/SafeApi/SafeApi.csproj
```

## Optional runtime governance story

Scanner-first adoption addresses risky logging before production. Runtime governance is a separate, optional step for workloads that need deterministic enforcement after deployment. The Scanner demo does not require CerbiShield or route application logs through a Cerbi service.
