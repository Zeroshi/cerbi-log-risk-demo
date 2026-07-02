# Cerbi Scanner demo summary

> Sample output for buyer demos. Regenerate with your Cerbi Scanner build before using in production documentation.

Policy: `policies/cerbi-policy.yml`  
Fail threshold: `error`  
Result: **failed**

| Rule | Severity | File | Line | Finding | Recommendation | Confidence |
| --- | --- | --- | ---: | --- | --- | --- |
| CERBI003 | error | `src/dotnet/UnsafeApi/Program.cs` | 19 | Raw request body logged. | Log approved metadata instead of `rawBody`. | high |
| CERBI001 | error | `src/dotnet/UnsafeApi/Program.cs` | 20 | Email and password appear in a log statement. | Omit password and avoid directly identifying email values. | high |
| CERBI002 | error | `src/dotnet/UnsafeApi/Program.cs` | 21 | Authorization header or token logged. | Never log credentials, tokens, or authorization headers. | high |
| CERBI001 | error | `src/dotnet/UnsafeApi/Program.cs` | 22 | SSN-like and credit-card-like values logged. | Block, tokenize, or mask regulated identifiers. | high |
| CERBI004 | warning | `src/dotnet/UnsafeApi/Program.cs` | 23 | Object destructuring may leak nested fields. | Log explicit approved fields. | medium |
| CERBI006 | warning | `src/dotnet/UnsafeApi/Program.cs` | 24 | High-cardinality identifiers used as dimensions. | Use stable dimensions and correlation IDs. | medium |

The safe sample in `src/dotnet/SafeApi/Program.cs` uses a stable `eventName`, includes required governance fields, and avoids raw payload or credential logging.
