# argocd-secret-replacer — Project Memory

## What it is
C# (.NET 9) CLI tool — Argo CD Config Management Plugin (CMP).
Reads Kubernetes manifests from stdin, replaces `<secret:key|modifier>` placeholders with sops-decrypted values, writes to stdout.

## Build / Test commands
- `dotnet restore` — restore NuGet packages
- `dotnet build --configuration Release` — build
- `dotnet test` — run all tests (E2E tests require `sops` binary in PATH)
- `dotnet publish src/Replacer/Replacer.csproj --configuration Release --runtime linux-x64 --self-contained true -o publish`

## Key files
- [src/Replacer/Program.cs](src/Replacer/Program.cs) — entry point
- [src/Replacer/Options.cs](src/Replacer/Options.cs) — CLI verb/options (sops verb, -f flag)
- [src/Replacer/Substitution/ISecretReplacer.cs](src/Replacer/Substitution/ISecretReplacer.cs) — regex replacement logic
- [src/Replacer/SecretsProvider/Sops/SopsSecretProvider.cs](src/Replacer/SecretsProvider/Sops/SopsSecretProvider.cs) — runs sops -d
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
- E2E tests call sops subprocess — need sops installed (not in devcontainer by default)
- Modifiers are discovered via reflection — no factory registration needed for new ones
- Sops secrets must have a `data:` key in the decrypted YAML/JSON
- `ARGOCD_ENV_SOPS_EXE` overrides the sops binary path
