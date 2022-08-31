## Kustomize example
Remember to encrypt `secret.sec.yaml` with sops using command
`sops -e -i secret.sec.yaml`.

## Usage
Deploy new application using manifest below.
```yaml
apiVersion: argoproj.io/v1alpha1
kind: Application
metadata:
    name: sops-replacer-kustomize-test
spec:
    source:
      repoURL: '<your git repository>'
      path: sops-replacer-kustomize-test
      targetRevision: main
      plugin:
        name: replacer-kustomize
        env:
          - name: SOPS_FILE
            value: secret.sec.yaml
    destination:
      server: 'https://kubernetes.default.svc'
      namespace: sops-replacer-kustomize-test
    syncPolicy:
      automated:
        selfHeal: true
      syncOptions:
        - CreateNamespace=true
```
Created resources should have values replaced from store.
