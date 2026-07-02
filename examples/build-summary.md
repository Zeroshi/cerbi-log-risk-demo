# Cerbi Scanner Build Summary

**Command:** `cerbi-scanner scan --path . --policy policies/cerbi-policy.yml --fail-on error --format json --output scan-results/findings.json --sarif scan-results/findings.sarif --summary scan-results/build-summary.md`

**Result:** failed

| Severity | Count |
| --- | ---: |
| Error | 4 |
| Warning | 3 |
| Total | 7 |

## Findings

| Rule | Severity | Location | Message |
| --- | --- | --- | --- |
| CERBI003 | error | `src/dotnet/UnsafeApi/Program.cs:19` | Raw request body is logged as a structured field. |
| CERBI001 | error | `src/dotnet/UnsafeApi/Program.cs:20` | Email and password appear in a log statement. |
| CERBI002 | error | `src/dotnet/UnsafeApi/Program.cs:21` | Authorization header or bearer token is logged. |
| CERBI001 | error | `src/dotnet/UnsafeApi/Program.cs:22` | SSN-like and credit-card-like values are logged. |
| CERBI004 | warning | `src/dotnet/UnsafeApi/Program.cs:23` | Object destructuring may expose nested sensitive fields. |
| CERBI006 | warning | `src/dotnet/UnsafeApi/Program.cs:24` | High-cardinality identifiers are used as log dimensions. |
| CERBI005 | warning | `src/dotnet/UnsafeApi/Program.cs:19` | Required governance fields are missing from log events. |
