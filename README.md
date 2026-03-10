# argocd-secret-replacer

An [Argo CD](https://github.com/argoproj/argo-cd/) Config Management Plugin (CMP) that replaces secret placeholders in Kubernetes manifests with values from a secrets store.

Supported secret stores:
- [sops](https://github.com/mozilla/sops) (YAML and JSON)
- Kubernetes Secret volume mounts (plain-text files on disk)

The tool reads manifests from **stdin**, scans them for `<secret:key|modifier>` placeholders, replaces them with values from the selected store, and writes the result to **stdout**.

## Why use it?

- Store secrets securely alongside GitOps configuration
- Works transparently with helm and kustomize generated manifests
- Supports any text-based Kubernetes manifest format
- Automatically handles base64-encoded values (e.g. `Secret.data` fields)

## Installation

### As an Argo CD Config Management Plugin (recommended)

The recommended installation method uses the Argo CD [Config Management Plugin v2 (sidecar) pattern](https://argo-cd.readthedocs.io/en/stable/operator-manual/config-management-plugins/).

A ready-to-use [Kustomization example](https://github.com/mmalyska/argocd-secret-replacer/tree/main/examples/argocd/kustomize) is provided that:

- Deploys the plugin as sidecar containers in `argocd-repo-server`
- Configures both kustomize and helm plugin variants
- Mounts a sops age key from a Kubernetes Secret

```bash
# Apply with your own age key secret already created
kubectl apply -k examples/argocd/kustomize
```

The plugin image is published to `ghcr.io/mmalyska/argocd-secret-replacer`.

### Manual binary installation

Self-contained binaries are published on every [GitHub release](https://github.com/mmalyska/argocd-secret-replacer/releases) for:

| Platform | Target |
|---|---|
| Linux x64 | `linux-x64` |
| Linux x64 musl (Alpine) | `linux-musl-x64` |
| Linux ARM64 | `linux-arm64` |
| Linux ARM64 musl | `linux-musl-arm64` |
| Windows x64 | `win-x64` |
| macOS x64 | `osx-x64` |
| macOS ARM64 (Apple Silicon) | `osx-arm64` |

Download and place the binary in `/custom-tools/` (or any directory in your PATH inside the Argo CD repo-server container).

## Environment Variables

| Variable | Purpose | Example | Required |
|---|---|---|---|
| `ARGOCD_ENV_SOPS_EXE` | Override the sops executable path | `/custom-tools/sops` | No (sops verb only) |
| `ARGOCD_ENV_SOPS_SECRET_FILE` | Path to the sops-encrypted secrets file (used by the CMP plugin discovery) | `secret.sec.yaml` | No (sops verb only) |
| `SOPS_*` | Any sops configuration variable (see [sops docs](https://github.com/mozilla/sops)) | `SOPS_AGE_KEY_FILE=/sops-age/key` | Depends on sops config |

> **Note:** The `ARGOCD_ENV_` prefix is required for Argo CD 2.4+. Older versions without this prefix are not supported.

## Plugin usage (CMP v2 sidecar)

The plugin is configured as a `ConfigManagementPlugin` resource mounted into the repo-server sidecar. Two secret provider verbs are available:

| Verb | Flag | Description |
|---|---|---|
| `sops` | `-f <file>` | Decrypt a sops-encrypted YAML/JSON file |
| `secret` | `--mount <dir>` | Read from a Kubernetes Secret mounted as a directory |

The example below shows both kustomize and helm variants using each provider.

### Kustomize

```yaml
apiVersion: argoproj.io/v1alpha1
kind: ConfigManagementPlugin
metadata:
  name: sops-replacer-plugin-kustomize
spec:
  version: v1.0
  allowConcurrency: true
  discover:
    find:
      command:
        - sh
        - "-c"
        - "[[ ! -z $ARGOCD_ENV_SOPS_SECRET_FILE ]] && find . -name 'kustomization.yaml' && find . -name '$ARGOCD_ENV_SOPS_SECRET_FILE'"
  generate:
    command:
      - bash
      - "-c"
      - |-
        kustomize build --enable-alpha-plugins . | argocd-secret-replacer sops -f "$ARGOCD_ENV_SOPS_SECRET_FILE"
  lockRepo: false
```

### Helm

```yaml
apiVersion: argoproj.io/v1alpha1
kind: ConfigManagementPlugin
metadata:
  name: sops-replacer-plugin-helm
spec:
  version: v1.0
  allowConcurrency: true
  discover:
    find:
      command:
        - sh
        - "-c"
        - "[[ ! -z $ARGOCD_ENV_SOPS_SECRET_FILE ]] && find . -name 'Chart.yaml' && find . -name '$ARGOCD_ENV_SOPS_SECRET_FILE'"
  init:
    command:
      - bash
      - "-c"
      - helm dependency update
  generate:
    command:
      - bash
      - "-c"
      - |-
        helm template --include-crds --release-name "$ARGOCD_APP_NAME" --namespace "$ARGOCD_APP_NAMESPACE" --kube-version $KUBE_VERSION --api-versions $KUBE_API_VERSIONS . | argocd-secret-replacer sops -f "$ARGOCD_ENV_SOPS_SECRET_FILE"
  lockRepo: false
```

### Argo CD Application

To use the plugin in an application, set the `plugin` source:

```yaml
apiVersion: argoproj.io/v1alpha1
kind: Application
metadata:
  name: my-app
spec:
  source:
    repoURL: 'https://github.com/my-org/my-repo'
    path: my-app
    plugin:
      name: sops-replacer-plugin-kustomize
      env:
        - name: SOPS_SECRET_FILE
          value: secret.sec.yaml
  destination:
    server: 'https://kubernetes.default.svc'
    namespace: my-app
```

### Kubernetes Secret volume mount (secret verb)

To use the `secret` verb instead, mount a Kubernetes Secret as a volume into the sidecar and point `--mount` at it:

```yaml
generate:
  command:
    - bash
    - "-c"
    - |-
      kustomize build --enable-alpha-plugins . | argocd-secret-replacer secret --mount /cluster-secrets
```

Each key in the Kubernetes Secret becomes a file in the mounted directory, and its value is the file content.

For complete working examples see the [examples directory](https://github.com/mmalyska/argocd-secret-replacer/tree/main/examples/applications).

## Placeholder syntax

The tool scans input for the following pattern:

```
<secret:key|modifier1|modifier2>
```

- `key` — for `sops`: key path within the decrypted file's `data:` section; for `secret`: filename in the mounted directory
- `modifier1`, `modifier2` — optional output modifiers (pipe-separated)

The aliases `<sops:...>` and `<secret:...>` are both supported.

### Automatic base64 handling

The replacer also detects base64-encoded strings (≥10 characters) and checks whether their decoded content contains placeholders. If a match is found, the replacement is performed and the result is re-encoded as base64. This means Kubernetes `Secret` `data:` fields are handled automatically — no manual `|base64` modifier is needed.

## Modifiers

| Modifier | Description |
|---|---|
| `base64` | Base64-encodes the secret value |

**Example:**

```yaml
# Input
apiVersion: v1
kind: Secret
stringData:
  password: <secret:db.password>
data:
  token: <secret:api.token|base64>
```

## Building from source

Requirements: [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)

```bash
dotnet restore
dotnet build --configuration Release
dotnet test
```

To publish a self-contained binary:

```bash
dotnet publish src/Replacer/Replacer.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  -o ./publish
```

## Development

A [Dev Container](.devcontainer/devcontainer.json) is provided using `.NET 9` with `sops` pre-installed. Open the repository in VS Code and select **Reopen in Container**.

> E2E tests for the `sops` verb require a valid age key configured. Unit tests and `secret` verb E2E tests have no external dependencies.
