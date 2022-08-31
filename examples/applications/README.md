# Examples
## Prerequisites
- Upp and running k8s cluster with deployed Argo CD.
- Installed `sops` via init-containers into `argocd-repo-server`. See more in [argocd examples](https://github.com/mmalyska/argocd-secret-replacer/tree/main/examples/argocd).
- Installed `replacer` as `custom-tool` and configured as plugin.
- Generated `age` keys for encryption/decryption.
- Added `age` key file via mounting secret or environment variable.
