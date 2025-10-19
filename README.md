## 🙌 Author

Shlomi Yehezkel  
DevOps + .NET Engineer

# AKS Deployment: Two .NET Services with Ingress and Network Policy

This project demonstrates how to deploy two .NET minimal API services (Service A and Service B) to Azure Kubernetes Service (AKS), expose them using NGINX Ingress, enforce network policies, and secure the cluster via Azure Container Registry (ACR) integration.

## 🔧 Tech Stack

- Azure Kubernetes Service (AKS)
- Azure Container Registry (ACR)
- NGINX Ingress Controller
- Kubernetes Network Policies
- .NET 9 Minimal API (Service A, Service B)
- Docker

---

## 🚀 Setup Instructions

### 1. Create Resource Group and ACR

```bash
az group create --name devops-rg --location westeurope
az acr create --resource-group devops-rg --name devopsacr123 --sku Basic --admin-enabled true
az acr login --name devopsacr123
```

✅ **Verify:** `az acr list --resource-group devops-rg --output table`

---

### 2. Build and Push Docker Images

```bash
docker build -t devopsacr123.azurecr.io/service-a:v1 ../ServiceA
docker build -t devopsacr123.azurecr.io/service-b:v1 ../ServiceB

docker push devopsacr123.azurecr.io/service-a:v1
docker push devopsacr123.azurecr.io/service-b:v1
```

✅ **Verify:** `az acr repository list --name devopsacr123 --output table`

---

### 3. Create AKS Cluster and Attach ACR

```bash
az aks create `
  --resource-group devops-rg `
  --name devops-aks `
  --node-count 1 `
  --node-vm-size Standard_B2s `
  --enable-managed-identity `
  --generate-ssh-keys

az aks update `
  --name devops-aks `
  --resource-group devops-rg `
  --attach-acr devopsacr123
```

✅ **Verify:** `az aks list -o table` and `az aks get-credentials --name devops-aks --resource-group devops-rg`

---

### 4. Deploy Services to AKS

```bash
kubectl apply -f service-a.yaml
kubectl apply -f deployment-service-a.yaml
kubectl apply -f service-b.yaml
kubectl apply -f deployment-service-b.yaml
```

✅ **Verify:** `kubectl get pods`, `kubectl get svc`

---

### 5. Apply Network Policy

```bash
kubectl apply -f network-policy-deny-service-a-to-b.yaml
```

✅ **Verify:** `kubectl get networkpolicy`

---

### 6. Install NGINX Ingress

```bash
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

helm install nginx-ingress ingress-nginx/ingress-nginx `
  --namespace ingress-basic `
  --create-namespace `
  --set controller.replicaCount=1 `
  --set controller.nodeSelector."kubernetes\.io/os"=linux `
  --set controller.admissionWebhooks.patch.nodeSelector."kubernetes\.io/os"=linux `
  --set defaultBackend.nodeSelector."kubernetes\.io/os"=linux
```

✅ **Verify:**

```bash
kubectl get pods -n ingress-basic
kubectl get svc -n ingress-basic
```

---

### 7. Apply Ingress Rules

```bash
kubectl apply -f ingress.yaml
```

✅ **Verify:**

```bash
kubectl get ingress
kubectl describe ingress services-ingress
```

---

### 8. Test Services

```bash
kubectl run test-shell --rm -i -t --image=busybox -- /bin/sh
wget -qO- http://<EXTERNAL-IP>/service-a
wget -qO- http://<EXTERNAL-IP>/service-b
```

✅ **Output:**
- "Welcome to Service A!"
- "Welcome to Service B!"
- Health checks return successful responses

---

## 🔐 NSG Considerations

Ensure that the NSG (Network Security Group) associated with your AKS node pool allows inbound TCP traffic on ports `80` and `443` to the public IP used by the Ingress controller.

---

## 📁 Files

- `service-a.yaml` – ClusterIP service for Service A
- `deployment-service-a.yaml` – Deployment for Service A
- `service-b.yaml` – ClusterIP service for Service B
- `deployment-service-b.yaml` – Deployment for Service B
- `network-policy-deny-service-a-to-b.yaml` – Network policy to block traffic from A to B
- `ingress.yaml` – NGINX Ingress routes
- `k8s-deployment-commands.md`
---

## 🧩 Architecture Diagram

```plaintext
+-------------+         +-----------------+         +----------------+
|             |         |                 |         |                |
|   Browser   +-------> |   Ingress NGINX +-------> |   Service A    |
|             |         |                 |         |   (API)        |
+-------------+         |                 |         +----------------+
                        |                 |               *  ↑
                        |                 |               ↓  |
                        |                 |         +----------------+
                        |                 +-------> |                |            
                        +-----------------+         |   Service B    |
                                                    |   (API)        |
                                                    +----------------+
```


## 🚀 Project Badges

![Azure Kubernetes Service](https://img.shields.io/badge/AKS-Deployed-blue?logo=azure-kubernetes-service)
![Docker](https://img.shields.io/badge/Docker-Containerized-blue?logo=docker)
![.NET](https://img.shields.io/badge/.NET-9.0-success?logo=dotnet)
![CI/CD](https://img.shields.io/badge/CI/CD-GitHub%20Actions-green?logo=githubactions)
