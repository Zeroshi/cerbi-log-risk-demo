# Five-minute Cerbi demo script

## 0:00 - Set the context

"This demo shows how Cerbi helps teams find unsafe logs before they reach production. We start with normal application code, scan it in CI, fail based on policy, and then show the corrected pattern. Runtime governance is optional defense-in-depth after scanner adoption."

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

The Codespaces rebuild installs .NET 10 and installs or updates the default `Cerbi.Scanner` .NET tool. From the repository root, run the documented scanner command:

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

Generated scan output belongs under ignored `scan-results/`. Keep checked-in sample outputs under `examples/` as stable fallback demo artifacts. If the scanner is not installed in the demo environment, open `examples/build-summary.md` and explain that it is sample output representing the expected scanner result.

## 2:30 - Show the policy and pipeline failure behavior

Open `policies/cerbi-policy.yml`.

Explain:

- `failOn: error` means error-level findings fail CI.
- Required fields keep logs traceable: `service`, `environment`, `correlationId`, `eventName`.
- Disallowed fields prevent credentials, regulated identifiers, and raw payloads from leaving the app.
- High-cardinality warnings help reduce downstream logging cost and index pressure.

Open `.github/workflows/cerbi-scan.yml` or `.azure-pipelines/cerbi-scan.yml`.

Explain that teams copy the pipeline, point it at their policy, and publish JSON/SARIF/Markdown artifacts for developers and security review.

## 3:30 - Open the safe file

Open `src/dotnet/SafeApi/Program.cs`.

Point out the corrected pattern:

- Stable event name: `checkout.accepted`.
- Required governance fields: `service`, `environment`, `correlationId`, `eventName`.
- No password, token, authorization header, raw body, SSN, or card number.
- Uses a low-cardinality amount bucket instead of dumping the full checkout object.

## 4:30 - Connect scanner-first to runtime governance

"The scanner catches risky logs before merge. For services that need defense-in-depth, CerbiShield can enforce the same governance schema at runtime. The runtime path should be local, deterministic, and lightweight: cached rules, redaction or tagging before logs reach sinks, and no dependency on a public control-plane service in the hot path."

Close by asking which repository or service they want to scan first.
