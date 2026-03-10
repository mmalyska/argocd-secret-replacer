# Secret Mount Example

This example demonstrates using the `secret` verb, which reads secrets from a Kubernetes Secret
mounted as a volume directory — no sops encryption or age keys required.

## Prerequisites

- A running Kubernetes cluster with Argo CD installed
- The `secret-replacer-plugin-kustomize` CMP sidecar deployed (see [argocd/kustomize](../../argocd/kustomize/))
- A Kubernetes Secret named `cluster-secrets` in the `argocd` namespace containing the keys
  referenced in your manifests

## Create the cluster-secrets Secret

```bash
kubectl create secret generic cluster-secrets \
  --namespace argocd \
  --from-literal=sample-key=my-secret-value
```

Or manage it via an ExternalSecret (e.g. with External Secrets Operator pulling from a vault).

## Deploy the example application

```yaml
apiVersion: argoproj.io/v1alpha1
kind: Application
metadata:
  name: secret-mount-example
  namespace: argocd
spec:
  project: default
  source:
    repoURL: 'https://github.com/mmalyska/argocd-secret-replacer'
    path: examples/applications/secret-mount
    plugin:
      name: secret-replacer-plugin-kustomize
      env:
        - name: SECRET_PROVIDER
          value: cluster-secrets
  destination:
    server: 'https://kubernetes.default.svc'
    namespace: default
  syncPolicy:
    automated: {}
```

## How it works

The `<secret:sample-key>` placeholder in `resources/secret.yaml` is replaced with the content of
the file `/cluster-secrets/sample-key` inside the sidecar container, which is the value of the
`sample-key` key from the `cluster-secrets` Kubernetes Secret.
