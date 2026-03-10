# argocd-secret-replacer — Project Memory

## What it is
C# (.NET 9) CLI tool — Argo CD Config Management Plugin (CMP).
Reads Kubernetes manifests from stdin, replaces `<secret:key|modifier>` placeholders with values from a secrets store, writes to stdout.
Two providers: `sops` verb (sops-encrypted file) and `secret` verb (K8s Secret volume mount directory).

## Build / Test commands
- `dotnet restore` — restore NuGet packages
- `dotnet build --configuration Release` — build
- `dotnet test` — run all tests (E2E tests require `sops` binary in PATH)
- `dotnet publish src/Replacer/Replacer.csproj --configuration Release --runtime linux-x64 --self-contained true -o publish`

## Key files
- [src/Replacer/Program.cs](src/Replacer/Program.cs) — entry point
- [src/Replacer/Options.cs](src/Replacer/Options.cs) — sops verb options (-f flag)
- [src/Replacer/MountedSecretOptions.cs](src/Replacer/MountedSecretOptions.cs) — secret verb options (--mount flag)
- [src/Replacer/SecretsProvider/ISecretsProviderFactory.cs](src/Replacer/SecretsProvider/ISecretsProviderFactory.cs) — provider factory (switch on options type)
- [src/Replacer/SecretsProvider/Sops/SopsSecretProvider.cs](src/Replacer/SecretsProvider/Sops/SopsSecretProvider.cs) — runs sops -d
- [src/Replacer/SecretsProvider/MountedSecret/MountedSecretProvider.cs](src/Replacer/SecretsProvider/MountedSecret/MountedSecretProvider.cs) — reads files from mounted K8s Secret dir
- [src/Replacer/Substitution/ISecretReplacer.cs](src/Replacer/Substitution/ISecretReplacer.cs) — regex replacement logic
- [src/Replacer/Modifiers/](src/Replacer/Modifiers/) — modifier plugins (auto-discovered by reflection)
- [Directory.Build.props](Directory.Build.props) — shared MSBuild settings (net9.0, TreatWarningsAsErrors=true, Nullable=enable)
- [.editorconfig](.editorconfig) — code style (4-space C#, LF, var everywhere, file-scoped namespaces)

## Tech stack
- .NET 9, C# (latestmajor), xUnit, Moq, YamlDotNet, CommandLineParser, System.Text.Json
- CI: GitHub Actions — build-validation (ubuntu/windows/macOS), release (7 targets), CodeQL, SonarCloud

## Code conventions
- `var` everywhere (enforced)
- File-scoped namespaces
- Expression-bodied members preferred
- Interfaces: `I` prefix; type params: `T` prefix
- Private fields: camelCase (no underscore)
- All warnings treated as errors

## Important notes
- `sops` is installed in the devcontainer via `ghcr.io/devcontainers-extra/features/sops`
- E2E tests for sops verb still need a valid age key; `secret` verb E2E tests have no external deps
- Modifiers are discovered via reflection — no factory registration needed for new ones
- Sops secrets must have a `data:` key in the decrypted YAML/JSON
- `ARGOCD_ENV_SOPS_EXE` overrides the sops binary path
- MountedSecretProvider trims trailing whitespace from file values (K8s sometimes adds newlines)
