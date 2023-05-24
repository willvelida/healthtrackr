@description('The name of the Service Bus Namespace that will be deployed')
param serviceBusNamespaceName string

@description('The location that our resources will be deployed to')
param location string

@description('The tags that will be applied to the Service Bus Namespace')
param tags object

resource serviceBus 'Microsoft.ServiceBus/namespaces@2022-01-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  tags: tags
  sku: {
    name: 'Basic'
  }
  identity: {
    type: 'SystemAssigned'
  }
}
