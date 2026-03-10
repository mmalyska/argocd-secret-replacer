# CLAUDE.md — argocd-secret-replacer

## Security

**Never store secrets, credentials, tokens, API keys, or other sensitive data in:**
- This file (`CLAUDE.md`)
- Memory files (`.claude/MEMORY.md`)
- Any committed file in this repository
- GitHub Actions workflow files (use GitHub Secrets instead)

## Project Overview

`argocd-secret-replacer` is a C# (.NET 9) CLI tool that acts as an [Argo CD](https://github.com/argoproj/argo-cd/) Config Management Plugin (CMP). It reads Kubernetes manifests from **stdin**, replaces `<secret:key|modifier>` placeholders with values from a secrets store (currently sops), and writes the result to **stdout**.

## Repository Structure

```
argocd-secret-replacer/
├── src/Replacer/               # Main CLI application
│   ├── Program.cs              # Entry point — parses args, runs replacer
│   ├── Options.cs              # SopsOptions (verb "sops", -f/--file flag)
│   ├── LoggerMessages.cs       # Compile-time log message definitions
│   ├── Modifiers/              # Output modifiers (e.g. base64)
│   ├── SecretsProvider/        # Secret store abstraction + Sops impl
│   └── Substitution/           # SecretReplacer — regex-based replacement
├── test/
│   ├── ReplacerUnitTests/      # xUnit unit tests (with Moq)
│   └── ReplacerE2ETests/       # xUnit E2E tests that invoke the real binary
├── examples/
│   ├── argocd/kustomize/       # Kustomization to install plugin into ArgoCD
│   └── applications/           # Example ArgoCD application manifests
├── Directory.Build.props        # Shared MSBuild props (target: net9.0)
├── .editorconfig                # Code style rules (enforced as warnings)
└── argocd-secret-replacer.sln  # Visual Studio solution
```

## Build & Test Commands

```bash
# Restore dependencies
dotnet restore

# Build (Release)
dotnet build --configuration Release --no-restore

# Run all tests
dotnet test --no-restore --verbosity normal

# Run unit tests only
dotnet test test/ReplacerUnitTests --no-restore

# Run E2E tests only (requires sops binary in PATH)
dotnet test test/ReplacerE2ETests --no-restore

# Publish self-contained binary (linux-x64 example)
dotnet publish src/Replacer/Replacer.csproj --configuration Release --runtime linux-x64 --self-contained true -o publish
```

The E2E tests require a `sops` binary to be installed and accessible. The binary is set up by `jkroepke/setup-sops` in CI.

## Key Architecture Decisions

### Placeholder Syntax
The tool looks for `<secret:key.path|modifier1|modifier2>` (or `<sops:...>`) in the input stream. The regex in [ISecretReplacer.cs](src/Replacer/Substitution/ISecretReplacer.cs) is source-generated (`[GeneratedRegex]`).

### Auto base64 Detection
The replacer also scans every base64-encoded segment (≥10 chars) for placeholders inside decoded content. If a substitution is found, the result is re-encoded as base64. This handles Kubernetes `Secret` `data` fields transparently.

### Sops Provider
- Calls `sops -d <file>` as a subprocess.
- The sops executable can be overridden with `ARGOCD_ENV_SOPS_EXE`.
- Supports YAML (`.yaml`/`.yml`) and JSON (`.json`) sops files.
- Secrets are read from the `data:` key in the decrypted file.

### MountedSecret Provider
- Reads secrets from a directory of plain-text files (Kubernetes Secret mounted as a volume).
- Each filename is the secret key; the file content is the value (trailing whitespace trimmed).
- CLI verb: `secret --mount <dir>` — options in `src/Replacer/MountedSecretOptions.cs`.
- Implementation in `src/Replacer/SecretsProvider/MountedSecret/MountedSecretProvider.cs`.
- No subprocess or external dependencies.

### Modifiers
Modifiers implement `IModifier` and are discovered at runtime via reflection. Currently only `base64` is implemented. New modifiers are auto-discovered without factory registration.

## Code Style (enforced via .editorconfig)

- **Indentation**: 4 spaces for C#, 2 spaces for YAML/JSON/XML.
- **Line endings**: LF (CRLF for `.cmd`/`.bat`).
- **C# conventions**:
  - `var` everywhere (even for built-in types).
  - File-scoped namespaces (`namespace Foo;`).
  - Expression-bodied members preferred.
  - Interfaces prefixed with `I`, type parameters with `T`.
  - Private fields: `camelCase` (no underscore prefix).
  - `TreatWarningsAsErrors = true` — all analyzer warnings are errors.
  - Nullable reference types enabled.
  - Implicit usings enabled.

## CI / Release

- **Build validation** ([build-validation.yml](.github/workflows/build-validation.yml)): runs on push to `main` and PRs. Builds and tests on ubuntu, windows, macOS with .NET 9.
- **Release** ([release.yml](.github/workflows/release.yml)): triggered on GitHub release publish. Publishes self-contained binaries for 7 targets: `linux-x64`, `win-x64`, `osx-x64`, `osx-arm64`, `linux-musl-x64`, `linux-arm64`, `linux-musl-arm64`.
- Also runs CodeQL and SonarCloud analysis.

## Development Environment

A devcontainer ([.devcontainer/devcontainer.json](.devcontainer/devcontainer.json)) is provided using the `mcr.microsoft.com/devcontainers/dotnet:9.0` image. It installs the C# Dev Kit, Claude Code VSCode extensions, and `sops` (via `ghcr.io/devcontainers-extra/features/sops`).

Note: E2E tests for the `sops` verb still require a valid age key to be configured. Unit tests and `secret` verb E2E tests have no external dependencies.

## Adding a New Secrets Provider

1. Create a new options class decorated with `[Verb("myverb", ...)]` in `src/Replacer/`.
2. Implement `ISecretsProvider`.
3. Register in `SecretsProviderFactory.GetProvider()`.
4. Add the verb type to `ParseArguments(args, typeof(SopsOptions), typeof(MyOptions))` in `Program.cs`.

## Adding a New Modifier

1. Create a class implementing `IModifier` in `src/Replacer/Modifiers/`.
2. Set `Key` to the modifier name used in `<secret:path|mymodifier>`.
3. No registration needed — it is discovered via reflection.
