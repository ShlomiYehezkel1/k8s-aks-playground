# Bash Commands and Verifications

## Command
```
az login
```

### Verify
```
az account show
```

### Expected Result
Shows current Azure subscription details


## Command
```
az acr list --resource-group devops-rg --output table
```

### Expected Result
Lists available Azure Container Registries (ACRs) in the resource group


## Command
```
az acr login --name devopsacr123
```

### Expected Result
Logs in to the ACR so you can push images


## Command
```
docker tag service-a devopsacr123.azurecr.io/service-a:v1
```

### Expected Result
Prepares the image for ACR by tagging it with the registry


## Command
```
docker push devopsacr123.azurecr.io/service-a:v1
```

### Verify
```
az acr repository list --name devopsacr123 --output table
```

### Expected Result
Image is uploaded and appears in the ACR repository list


## Command
```
az aks create `
  --resource-group devops-rg `
  --name devops-aks `
  --node-count 1 `
  --node-vm-size Standard_B2s `
  --enable-managed-identity `
  --generate-ssh-keys
```

### Verify
```
az aks list --output table
```

### Expected Result
Cluster is created and listed


## Command
```
az aks get-credentials --resource-group devops-rg --name devops-aks
```

### Verify
```
kubectl config get-contexts
```

### Expected Result
Context for devops-aks is now active


## Command
```
kubectl get nodes
```

### Expected Result
Node is in Ready state


## Command
```
az aks update `
  --name devops-aks `
  --resource-group devops-rg `
  --attach-acr devopsacr123
```

### Expected Result
Enables AKS to pull images from ACR


## Command
```
kubectl apply -f service-a.yaml
kubectl apply -f service-b.yaml
kubectl apply -f ingress.yaml
kubectl apply -f network-policy.yaml
```

### Verify
```
kubectl get pods
kubectl get svc
kubectl get ingress
```

### Expected Result
Pods are Running, Services and Ingress are created


## Command
```
kubectl get svc -n ingress-basic
```

### Expected Result
Shows LoadBalancer external IP for Ingress


## Command
```
curl http://<EXTERNAL-IP>/service-a
```

### Expected Result
Returns 'Welcome to Service A!'


## Command
```
curl http://<EXTERNAL-IP>/service-b
```

### Expected Result
Returns 'Welcome to Service B!'


## Command
```
curl http://<EXTERNAL-IP>/service-a/health
```

### Expected Result
Returns health check message for Service A


## Command
```
curl http://<EXTERNAL-IP>/service-b/health
```

### Expected Result
Returns health check message for Service B

