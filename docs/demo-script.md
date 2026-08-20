# Five-minute Cerbi demo script

## 0:00 - Set the context

"This demo shows how Cerbi helps teams find unsafe logs before they reach production. We start with normal application code, scan it in CI, review findings against policy, and then show the corrected pattern. Runtime governance is optional defense-in-depth after scanner adoption."

## 0:45 - Open the unsafe file

Open `src/dotnet/UnsafeApi/Program.cs`.

Point out the unsafe examples:

- Raw request body logging through `rawBody`.
- Email and password fields in a structured log statement.
- Authorization header and JWT-like token logging.
- SSN-like and credit-card-like values.
- Risky object destructuring with `{@payload}`.
- High-cardinality identifiers such as `userId`, `sessionId`, `customerId`, and `requestId`.

## 1:45 - Run or show the scanner

The Codespaces rebuild uses .NET 10 LTS and installs Cerbi Scanner 1.1.0. If the Codespace was created before the runtime update, rebuild the container or create a fresh Codespace.

For a first-look demo, run Scanner in report-only mode:

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

Open the generated report and point out that the checked-in Scanner 1.1.0 example contains 12 findings. Use the actual examples: `password` and `email` at `src/dotnet/UnsafeApi/Program.cs:20`, and high-cardinality `sessionId` at line 24.

Generated scan output belongs under ignored `scan-results/`. Stable sample output is checked in under `examples/` so the walkthrough still has inspectable evidence when you do not want to run the scanner live.

## 2:30 - Show the policy and gate behavior

Open `policies/cerbi-policy.yml`.

Explain:

- `--fail-on none` reports findings without failing the command.
- `--fail-on error` maps to Scanner's high-severity threshold and returns a non-zero exit code when a high-or-higher finding exists.
- Required fields keep logs traceable: `service`, `environment`, `correlationId`, `eventName`.
- Disallowed fields prevent credential and sensitive-field patterns from being treated as acceptable logging.
- High-cardinality warnings help identify downstream logging cost and index pressure.

Then show the gate command or pipeline configuration with `--fail-on error`.

Open `.github/workflows/cerbi-scan.yml` or `.azure-pipelines/cerbi-scan.yml` and explain that both run the same Scanner CLI engine and can publish JSON, SARIF, and Markdown artifacts.

## 3:30 - Open the safe file

Open `src/dotnet/SafeApi/Program.cs`.

Point out the corrected pattern:

- Stable event name: `checkout.accepted`.
- Required governance fields: `service`, `environment`, `correlationId`, `eventName`.
- No password, token, authorization header, raw body, SSN, or card number.
- Uses a lower-cardinality amount bucket instead of dumping the full checkout object.

## 4:30 - Connect scanner-first to runtime governance

"Scanner is the low-friction discovery and CI path. If selected workloads also need runtime enforcement, CerbiStream or Cerbi Gateway can apply governance at a runtime boundary while CerbiShield manages policy and evidence. Scanner itself does not require that runtime deployment."

Close by asking which repository or service they would want to scan first.
