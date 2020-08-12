# Gerd
Gerd (قرد) is Arabic for monkey. Gerd by Onyx is a light-weight chaos monkey implementation for k8s (kubernetes). If you want to adopt chaos engineering, then setup a Gerd in your cluster!

## Chaos Engineering
Chaos engineering is the discipline of experimenting on a software system in production in order to build confidence in the system's capability to withstand turbulent and unexpected conditions. In software development, a given software system's ability to tolerate failures while still ensuring adequate quality of service—often generalized as resiliency—is typically specified as a requirement. However, development teams often fail to meet this requirement due to factors such as short deadlines or lack of knowledge of the field. Chaos engineering is a technique to meet the resilience requirement.
Source [Wikipedia](https://en.wikipedia.org/wiki/Chaos_engineering)

## How does Gerd work
 Gerd wakes up and starts wreaking havoc and causing chaos across your k8s cluster at random times. Many implementations of Chaos engineering operate during "working hours"; business days between 9-5, to ensure that someone is available to clean up after the chaos. We feel that this violates what we want to achieve in adopting chaos engineering, which is: systems must be reselient, and must be designed and built to handle failures from day one.
 
 We belive that you should setup chaos in your dev/test/prod clusters/namespaces. **Don't do that in prod only - please!**

## Setting up Gerd in your k8s cluster
The k8s deployment objects for Gerd k8s deployment are defind in the [deployemnt.yml](deployemnt.yml) file. Which does the following:

### Create a Service Account
Create a service account to be used by Gerd for calling your cluster's API server
```
apiVersion: v1
kind: ServiceAccount
metadata:
  name: gerd
```
### Create a Cluster Role
Create a cluster role that defines the k8s API permissions needed by Gerd to operate. The permissions needed are: `namespaces - list and get`; `pods - list, get and delete`.
```
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRole
metadata:
  name: gerd-chaos-worker
rules:
- apiGroups: [""]
  resources: ["namespaces"]
  verbs: ["get", "list"]
- apiGroups: [""]
  resources: ["pods"]
  verbs: ["get", "list", "delete"]
```
### Create the Service Account to the Cluster Role
Bind the service acccount created to the cluster role.
```
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: gerd-chaos-role-binding
subjects:
- kind: ServiceAccount
  name: gerd
  namespace: default
roleRef:
  kind: ClusterRole
  name: gerd-chaos-worker
  apiGroup: rbac.authorization.k8s.io
```
### Create the Pod
Create a Gerd pod in your cluster and use the service account created above to run the worker.
```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: onyx-gerd
  labels:
    app: onyx-gerd
spec:
  replicas: 1
  selector:
    matchLabels:
      app: onyx-gerd
  template:
    metadata:
      labels:
        app: onyx-gerd
    spec:
      serviceAccountName: gerd
      containers:
      - name: onyx-gerd
        image: onyxgerd20200809104133.azurecr.io/onyxgerd:latest
        ports:
        - containerPort: 80
```
### Embrace chaos
By default; Gerd requires opt-in to chaos. This can be done by updating a pod's deployment definition to include the 'onyx.gerd.enabled' label as part of the deployment spec as in the below example.
```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: httpbin-deployment
  labels:
    app: httpbin
spec:
  replicas: 3
  selector:
    matchLabels:
      app: httpbin
  template:
    metadata:
      labels:
        app: httpbin
        onyx.gerd.enabled: 'true'
    spec:
      containers:
      - name: httpbin
        image: kennethreitz/httpbin:latest
        ports:
        - containerPort: 80
```
