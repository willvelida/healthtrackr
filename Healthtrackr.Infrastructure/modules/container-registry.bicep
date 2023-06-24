@description('Specifies the name of the Container Registry')
param containerRegistryName string

@description('Specifies the location to deploy the Container Registry')
param location string

@description('Specifies the tags to apply to this Container Registry')
param tags object

resource acr 'Microsoft.ContainerRegistry/registries@2023-01-01-preview' = {
  name: containerRegistryName
  location: location
  tags: tags
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}
