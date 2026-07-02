# What developers miss in logs

## Sensitive data in structured fields

Structured logging makes fields searchable, but it also makes mistakes easier to index and retain. Field names such as `email`, `ssn`, `cardNumber`, `creditCard`, and `password` should be reviewed by policy before production.

## Secrets and tokens

Bearer tokens, JWTs, API keys, refresh tokens, and authorization headers are credential material. Logging them creates replay risk and expands the blast radius of log access.

## Raw body logging

Raw request or response bodies frequently include fields the developer did not intend to log. They also drift as APIs evolve. Prefer explicit allow-listed metadata and stable event names.

## Object dumps

Destructuring an object with patterns such as `{@payload}` can expose nested sensitive fields added later by another developer. Log explicit approved fields instead.

## Schema drift

Logs are operational contracts. When teams add new fields without governance, dashboards, alerts, retention rules, and downstream security controls drift. Required fields such as `service`, `environment`, `correlationId`, and `eventName` keep events consistent.

## High-cardinality cost risk

Fields such as `userId`, `sessionId`, `customerId`, and `requestId` can explode index cardinality when used as dimensions. Keep them out of default indexed dimensions unless there is a clear operational need. Use `correlationId` for traceability and stable categories for dashboards.
