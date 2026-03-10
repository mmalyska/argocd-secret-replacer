# Examples

| Example | Provider | Description |
|---|---|---|
| [kustomize](kustomize/) | `sops` | Kustomize app using a sops-encrypted secrets file |
| [helm](helm/) | `sops` | Helm chart using a sops-encrypted secrets file |
| [secret-mount](secret-mount/) | `secret` | Kustomize app using a Kubernetes Secret volume mount |

## Prerequisites (sops examples)
- Running k8s cluster with Argo CD deployed.
- `sops-replacer-plugin-kustomize` or `sops-replacer-plugin-helm` CMP sidecar installed. See [argocd examples](https://github.com/mmalyska/argocd-secret-replacer/tree/main/examples/argocd).
- Generated `age` keys and the age key mounted into the sidecar.

## Prerequisites (secret-mount example)
- Running k8s cluster with Argo CD deployed.
- `secret-replacer-plugin-kustomize` CMP sidecar installed. See [argocd examples](https://github.com/mmalyska/argocd-secret-replacer/tree/main/examples/argocd).
- A `cluster-secrets` Kubernetes Secret in the `argocd` namespace.
