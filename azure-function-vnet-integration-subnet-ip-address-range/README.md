# Integrate an Azure Function into a VNET

This example shows you how you can integrate an Azure Function into a VNET.
To have a simple demo, an Azure Function is implemented with a Http Trigger that invokes a Kubernetes pod running inside a VNET.
The pod API echos the source IP address of the calling function to show that the Azure Function VNET integration uses a private Ip address within the integration subnet's Ip addresss range.

## Architecture Overview

Todo

## Setup 

The following steps guide you through the setup process of the sample.

### Create a ResourceGroup

```Shell
az group create -n azfunc-vnet -l westeurope
```

### Create a VNET

```Shell
az network vnet create --name azfunc-vnet --resource-group azfunc-vnet-rg --address-prefixes 10.0.0.0/16 --subnet-name cluster-subnet --subnet-prefix 10.0.0.0/24 --location westeurope
```

And add a subnet for the Azure Function's integration

```Shell
az network vnet subnet create --address-prefixes 10.0.1.0/28 --name azfunc-subnet --resource-group azfunc-vnet-rg --vnet-name azfunc-vnet
```

### Create the Azure Function

First create a hosting plan:

```Shell
az appservice plan create -g azfunc-vnet-rg -n azfunc-vnet-plan --sku S1 -l westeurope
```

Create a storage account:

```Shell
az storage account create -n azfuncvnetstorage -l westeurope -g azfunc-vnet-rg -l westeurope --sku Standard_LRS
```

Create a functinapp:

``` Shell
az functionapp create -n myazfuncvnet -g azfunc-vnet-rg  --storage-account azfuncvnetstorage --plan azfunc-vnet-plan --os-type Windows --runtime dotnet
```

Integrate the function into the VNET:

```Shell
az functionapp vnet-integration add -g azfunc-vnet-rg -n myazfuncvnet --vnet azfunc-vnet --subnet azfunc-subnet
```

### Create an AKS Cluster

The cluster uses advanced networking:

```Shell
az aks create --resource-group azfunc-vnet-rg --name aksazfunc --network-plugin azure --vnet-subnet-id "/subscriptions/c309f4ef-c319-482b-8889-9076684e38f2/resourceGroups/azfunc-vnet-rg/providers/Microsoft.Network/virtualNetworks/azfunc-vnet/subnets/cluster-subnet" --service-cidr 10.2.0.0/24 --dns-service-ip 10.2.0.10 --node-count 1
```

Get aks credentials:

```
az aks get-credentials -n aksazfunc -g azfunc-vnet-rg
```

### Deploy the sample application

To deploy the Azure Function you need to install __Azure Functions Core Tools v3__. 
I you haven't already installed the tools you can find the installation guidelines [here](https://github.com/Azure/azure-functions-core-tools#azure-functions-core-tools)

Now open a command line and switch to the function directory __MyHttpFunction__ and deploye to function to Azure with the following command:

```Shell
func azure functionapp publish myazfuncvnet --force
```

Note down the Invoke url of the function:

```
...
Functions in myazfuncvnet:
    EchoIpAddressFunction - [httpTrigger]
        Invoke url: https://myazfuncvnet.azurewebsites.net/api/echoipaddressfunction?code=<code>
```

After that, navigate to the __EchoServer__ folder and run the following __kubectl__ command to deploy the EchoServer:

```Shell
kubectl apply -f ./deploy
```

### Invoke the function

Open a either a browser or use curl to invoke the function:

```Shell
curl <function url with code>
```

Now you see that the Azure FGunction uses a privat IP Address within the IP address range of the Azure Function integration VNET:

```
::ffff:10.0.1.13
```
