# argocd-secret-replacer
An [Argo CD](https://github.com/argoproj/argo-cd/) plugin to replace placeholders in Kubernetes manifests with secrets stored in:
- [sops](https://github.com/mozilla/sops)

More stores may come in future releases.

The application will scan manifests from stdin looking for the string `<secret:key|modifier>` to be replaced from selected store and outputted to stdout.

## Why should I use it?
- Allows you to store secrets securely when using Git Ops approach
  - Configuration is in Git
  - Secrets are stored in selected secret store
- Works well with helm and kustomize generated manifests
- Allows you to securely replace values in any kind of documents

## Installing in Argo CD as an plugin
You can find [Kustomization example](https://github.com/mmalyska/argocd-secret-replacer/tree/main/examples/argocd/kustomize) how to install this application as a plugin.

If you want to install plugin in Argo CD, you can build your own Argo CD image with the plugin already inside, or make use of an Init Container to pull the binary. 
More about plugins you can find in Argo CD [documentation](https://argo-cd.readthedocs.io/en/stable/operator-manual/custom_tools/).

## Environment Variables
| Environment Variable Name | Purpose                                                                                        | Example                         | Required? |
|---------------------------|------------------------------------------------------------------------------------------------|---------------------------------|-----------|
| ARGOCD_ENV_SOPS_EXE       | Allows to change sops executable that is used in sops flow                                     | /custom-tools/sops              | no        |
| SOPS_*                    | Configuring sops via environment variables as in documentation https://github.com/mozilla/sops | SOPS_AGE_KEY_FILE=/sops-age/key |           |

Application uses `ARGOCD_ENV_` prefix from version of Argo CD 2.4 or higher. Previous versions without prefix are not supported.

## Plugin usage
After installing plugin into /custom-tools/ directory, you need to add it inside Argo CD config map under `configManagementPlugins`.

```yaml
- name: replacer-helm
  init:
    command: ["/bin/sh", "-c"]
    args: ["helm dependency build"]
  generate:
    command: [sh, -c]
    args: ["helm template --release-name $ARGOCD_APP_NAME --namespace $ARGOCD_APP_NAMESPACE . | argocd-secret-replacer sops -f $ARGOCD_ENV_SOPS_FILE"]
- name: replacer-kustomize
  generate:
    command: ["sh", "-c"]
    args: ["kustomize build . | argocd-secret-replacer sops -f $ARGOCD_ENV_SOPS_FILE"]
```
**Using plugins won't allow you to use the Argo CD build-in application helm/kustomize options.**

More about using plugins you can find in Argo CD [documentation](https://argoproj.github.io/argo-cd/user-guide/config-management-plugins/).

## Testing 
In this example we will use sops as our provider.

Assuming that you have kustomize files that has secret configured and in `secret.sec.yaml` file you have it encrypted we will create example app as below.
```YAML
apiVersion: argoproj.io/v1alpha1
kind: Application
metadata:
  name: sops-replacer-test
spec:
  destination:
    server: 'https://kubernetes.default.svc'
    namespace: sops-replacer-test
  syncPolicy:
    automated:
      selfHeal: true
    syncOptions:
      - CreateNamespace=true
  source:
    repoURL: '<your git repo>'
    path: sops-replacer-test
    plugin:
      name: replacer-kustomize
      env:
          - name: SOPS_FILE
            value: secret.sec.yaml
    targetRevision: HEAD
```
For more detailed examples, please go to [examples directory](https://github.com/mmalyska/argocd-secret-replacer/tree/main/examples/applications).

## Modifiers
You can modify the resulting output with the following modifiers:

* base64: Will base64 encode the secret.
