# Cerbi Scanner GitHub Marketplace Action release plan

This branch is a public end-to-end validation harness for the future dedicated `Zeroshi/cerbi-scanner-action` repository. Do not merge the root `action.yml` into the demo repository as the permanent Marketplace distribution model.

## Target repository

Create a dedicated public repository:

`Zeroshi/cerbi-scanner-action`

The repository should contain only the Action wrapper and supporting files required to document, test, release, and maintain it. The private `Zeroshi/Cerbi.Scanner` repository remains private; the Action installs the published `Cerbi.Scanner` NuGet tool at runtime.

## Files to promote from this validation branch

- `action.yml` -> repository root
- `scripts/run-cerbi-scanner.ps1` -> same path
- `ACTION_README.md` -> `README.md`
- `LICENSE` -> reuse the existing MIT wrapper license or replace it with the approved Cerbi wrapper license before publication
- CI workflow -> add a dedicated self-test workflow that checks out the Action repository and scans a fixture/demo checkout

## Marketplace release gates

1. Dedicated repository is public.
2. `action.yml` is the single Action metadata file at repository root.
3. Action name is unique in GitHub Marketplace.
4. Action installs and runs the current supported `Cerbi.Scanner` package without access to the private Scanner source repository.
5. Default scan is local/report-only (`fail-on: none`, `upload-sarif: false`).
6. `fail-on` correctly replays Scanner exit code `1` only after reports are generated.
7. Scanner/configuration exit code `2` fails the Action.
8. JSON, SARIF, and Markdown outputs are generated and exposed as Action outputs.
9. Markdown is appended to `$GITHUB_STEP_SUMMARY`.
10. `upload-sarif: true` works when the calling workflow grants `security-events: write`.
11. SARIF produced by Scanner 1.1.0 is normalized for GitHub's rejection of empty `fixes[].artifactChanges` entries.
12. No source or findings are uploaded to Cerbi by the wrapper.
13. Test Linux, Windows, and macOS GitHub-hosted runners before tagging `v1`.
14. Pin the first Marketplace release to a tested Scanner package version rather than `latest`.
15. Accept the GitHub Marketplace Developer Agreement and publish a tagged release with the Action Marketplace checkbox enabled.
16. After release, move/update the floating `v1` tag only after the corresponding immutable release has passed CI.

## Initial Marketplace positioning

**Name:** Cerbi Logging Governance Scanner

**Description:** Find sensitive data, unsafe logging patterns, and logging policy violations before production.

Recommended categories: security and continuous integration, subject to the categories GitHub presents during release.

## Initial usage

```yaml
permissions:
  contents: read

steps:
  - uses: actions/checkout@v7

  - name: Cerbi logging governance scan
    uses: Zeroshi/cerbi-scanner-action@v1
```

For GitHub code scanning:

```yaml
permissions:
  contents: read
  security-events: write

steps:
  - uses: actions/checkout@v7

  - name: Cerbi logging governance scan
    uses: Zeroshi/cerbi-scanner-action@v1
    with:
      policy: cerbi-policy.yml
      fail-on: high
      upload-sarif: 'true'
```

## Distribution boundary

The public Action is intentionally thin. Detection logic, language analyzers, rule behavior, and scanner packaging remain in `Cerbi.Scanner`. The Action owns only GitHub-specific setup, invocation, report paths, optional SARIF publication, job summary rendering, and CI exit-code propagation.
