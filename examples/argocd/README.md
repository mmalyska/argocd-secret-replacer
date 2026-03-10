## Installing Argo CD with plugin
This example will install Argo CD using kustomize.

Four CMP sidecar variants are configured:

| Plugin name | Verb | Tool | Discovery trigger |
|---|---|---|---|
| `sops-replacer-plugin-kustomize` | `sops` | kustomize | `ARGOCD_ENV_SOPS_SECRET_FILE` set |
| `sops-replacer-plugin-helm` | `sops` | helm | `ARGOCD_ENV_SOPS_SECRET_FILE` set |
| `secret-replacer-plugin-kustomize` | `secret` | kustomize | `ARGOCD_ENV_SECRET_PROVIDER` set |
| `secret-replacer-plugin-helm` | `secret` | helm | `ARGOCD_ENV_SECRET_PROVIDER` set |

### sops variants
Generate and fill the age key file in `resources/sops-age-secret.yaml` before applying.

### secret variants
Create a Kubernetes Secret named `cluster-secrets` in the `argocd` namespace containing the secret
keys your applications reference. This can be managed manually or via an ExternalSecret.

```bash
kubectl create secret generic cluster-secrets --namespace argocd \
  --from-literal=my-key=my-value
```

If you want to check applications deployed with using this Argo CD kustomize deployment check this [examples directory](https://github.com/mmalyska/argocd-secret-replacer/tree/main/examples/applications).
