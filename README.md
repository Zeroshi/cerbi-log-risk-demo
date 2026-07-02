# Find unsafe logs before they reach production.

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](REPLACE_WITH_CODESPACES_DEEP_LINK)

This public demo shows the scanner-first Cerbi adoption path for platform engineering, security, and DevSecOps teams:

1. Unsafe logging exists in normal application code.
2. Cerbi Scanner identifies sensitive, risky, and costly logging patterns before production.
3. CI/CD can fail based on policy.
4. Developers can fix the logging issue with safe structured logging patterns.
5. Runtime governance can optionally demonstrate how CerbiShield redacts or blocks risky fields before logs reach downstream sinks.

The sample is intentionally small. It is designed to run in GitHub Codespaces without databases, message brokers, or heavyweight services.

> **Scanner availability note**
> This repository is a demo harness. The pipeline and Codespaces setup expect a Cerbi Scanner CLI to be available as either `cerbi-scanner` on `PATH` or as a configured .NET tool package. If your Cerbi Scanner distribution uses different command names, package IDs, output switches, or supported language rules, update the commands in this repo to match your licensed scanner build. The checked-in files under `examples/` are sample outputs for demos, not proof that the scanner has run.

## What is included

- `src/dotnet/UnsafeApi` contains realistic unsafe ASP.NET Core logging examples.
- `src/dotnet/SafeApi` shows corrected structured logging patterns.
- `policies/cerbi-policy.yml` defines required fields, disallowed fields, high-cardinality warnings, and the fail threshold.
- `.github/workflows/cerbi-scan.yml` shows GitHub Actions integration.
- `.azure-pipelines/cerbi-scan.yml` shows Azure DevOps integration patterns.
- `examples/` contains sample JSON, SARIF, and Markdown scanner outputs for a buyer-facing walkthrough.

## Path A: run the scanner locally

Prerequisites:

- .NET 9 SDK for the sample applications.
- A Cerbi Scanner CLI installation that matches your Cerbi distribution.

Build the samples:

```bash
dotnet build src/dotnet/UnsafeApi/UnsafeApi.csproj
dotnet build src/dotnet/SafeApi/SafeApi.csproj
```

Run the scanner when the CLI is available:

```bash
cerbi-scanner scan \
  --path . \
  --policy policies/cerbi-policy.yml \
  --fail-on error \
  --format json --output examples/findings.local.json \
  --sarif examples/findings.local.sarif \
  --summary examples/build-summary.local.md
```

Expected demo result: unsafe findings are reported in `src/dotnet/UnsafeApi`, while `src/dotnet/SafeApi` illustrates how developers should fix the patterns.

## Path B: run the scanner in Codespaces

1. Open the repository in GitHub Codespaces using the badge above after replacing the placeholder link.
2. Review `docs/demo-script.md` and `docs/what-to-look-for.md`.
3. Build the .NET samples.
4. Run the scanner command from Path A if your scanner CLI is available in the Codespace.

The dev container attempts a non-blocking scanner install only when scanner package configuration is provided. It will not fail Codespaces startup if the scanner package is private or unavailable.

## Path C: see CI/CD examples

- GitHub Actions: `.github/workflows/cerbi-scan.yml`
- Azure DevOps: `.azure-pipelines/cerbi-scan.yml`

Both pipeline examples are intentionally copy-friendly. They scan the repository with `policies/cerbi-policy.yml`, publish JSON/SARIF/Markdown outputs, and fail only when the configured policy threshold is met.

## Optional runtime governance story

Scanner-first adoption prevents risky logging before production. For runtime defense-in-depth, CerbiShield can be introduced in application logging paths to tag, redact, or block fields locally before events reach log sinks. Runtime governance should use cached rules and local deterministic decisions; application logs should not be routed through a public control-plane service.
