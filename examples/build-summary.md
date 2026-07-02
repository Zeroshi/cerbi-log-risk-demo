# Cerbi Logging Governance Scan

## Verdict: Fail

* Scan path: `/workspaces/cerbi-log-risk-demo`
* Scan time: 2026-07-02T20:20:42.5388857+00:00
* Duration: 0.18s
* Files scanned: 2
* Logging calls found: 7
* Active findings: 12
* Suppressed findings: 0

> Source snippets are omitted unless `--snippets` is explicitly enabled. Persisted reports are still redacted before writing.

## Severity breakdown

| Severity | Count |
| --- | ---: |
| Critical | 0 |
| High | 8 |
| Medium | 0 |
| Low | 0 |
| Warning | 4 |
| Info | 0 |

## Top rules

| Rule | Name | Count |
| --- | --- | ---: |
| CERBI003 | Structured Log Field Is Not Allowed | 4 |
| CERBI004 | High Cardinality Logging Field | 3 |
| CERBI001 | Sensitive Data In Log | 2 |
| CERBI002 | Secret-like Token or Credential In Logging Path | 2 |
| CERBI006 | Exception Logging May Expose Sensitive Object State | 1 |

## Top findings

| Rule | Severity | Location | Message | Remediation |
| --- | --- | --- | --- | --- |
| CERBI002 | High | `src/dotnet/UnsafeApi/Program.cs:19:5` | Template field 'rawBody' suggests raw body or payload data is being logged. Raw request/response bodies frequently contain credentials, PII, or secrets. | Replace '{{ rawBody }}' with individually named safe fields. If body inspection is required for debugging, apply a content-filtering or redaction pipeline before logging. |
| CERBI003 | High | `src/dotnet/UnsafeApi/Program.cs:20:5` | Structured field 'password' is not allowed by the active logging policy. Profile: 'default'. | Remove the 'password' field from the log call. If this data is needed for debugging, use a Cerbi redaction adapter. |
| CERBI001 | High | `src/dotnet/UnsafeApi/Program.cs:20:5` | Field 'email' is considered sensitive (profile: 'default'). Logging sensitive fields may expose PII or confidential data in log sinks. | Review whether 'email' needs to be logged. Consider masking, hashing, or omitting this field. Use a Cerbi redaction adapter if the field is required for diagnostics. |
| CERBI003 | High | `src/dotnet/UnsafeApi/Program.cs:21:5` | Structured field 'authorization' is not allowed by the active logging policy. Profile: 'default'. | Remove the 'authorization' field from the log call. If this data is needed for debugging, use a Cerbi redaction adapter. |
| CERBI003 | High | `src/dotnet/UnsafeApi/Program.cs:22:5` | Structured field 'ssn' is not allowed by the active logging policy. Profile: 'default'. | Remove the 'ssn' field from the log call. If this data is needed for debugging, use a Cerbi redaction adapter. |
| CERBI002 | High | `src/dotnet/UnsafeApi/Program.cs:23:5` | Template field 'payload' suggests raw body or payload data is being logged. Raw request/response bodies frequently contain credentials, PII, or secrets. | Replace '{{ payload }}' with individually named safe fields. If body inspection is required for debugging, apply a content-filtering or redaction pipeline before logging. |
| CERBI003 | High | `src/dotnet/UnsafeApi/Program.cs:24:5` | Structured field 'sessionId' is not allowed by the active logging policy. Profile: 'default'. | Remove the 'sessionId' field from the log call. If this data is needed for debugging, use a Cerbi redaction adapter. |
| CERBI001 | High | `src/dotnet/UnsafeApi/Program.cs:24:5` | Field 'userId' is considered sensitive (profile: 'default'). Logging sensitive fields may expose PII or confidential data in log sinks. | Review whether 'userId' needs to be logged. Consider masking, hashing, or omitting this field. Use a Cerbi redaction adapter if the field is required for diagnostics. |
| CERBI004 | Warning | `src/dotnet/UnsafeApi/Program.cs:20:5` | Structured field 'email' looks high-cardinality and may increase log index/storage cost. | Keep 'email' out of indexed dimensions unless it is explicitly needed for correlation; prefer sampling, hashing, or non-indexed attributes. |
| CERBI006 | Warning | `src/dotnet/UnsafeApi/Program.cs:23:5` | Destructuring operator used on 'payload' (potentially-sensitive object type). Object destructuring serialises the full object graph into the log, which may expose sensitive fields that are not individually named. | Replace '{@payload}' with individually named, safe fields. If the full object is required for diagnostics, apply a Cerbi destructuring policy or use a masking destructuring policy. |
| CERBI004 | Warning | `src/dotnet/UnsafeApi/Program.cs:24:5` | Structured field 'sessionId' looks high-cardinality and may increase log index/storage cost. | Keep 'sessionId' out of indexed dimensions unless it is explicitly needed for correlation; prefer sampling, hashing, or non-indexed attributes. |
| CERBI004 | Warning | `src/dotnet/UnsafeApi/Program.cs:24:5` | Structured field 'requestId' looks high-cardinality and may increase log index/storage cost. | Keep 'requestId' out of indexed dimensions unless it is explicitly needed for correlation; prefer sampling, hashing, or non-indexed attributes. |

## Remediation guidance

* Remove sensitive fields from log templates and structured properties before they leave the application.
* Prefer safe identifiers, correlation IDs, and coarse-grained operational metadata over payload, token, credential, or PII values.
* Update the Cerbi policy only after security review; use suppressions for time-bound exceptions with clear ownership.
