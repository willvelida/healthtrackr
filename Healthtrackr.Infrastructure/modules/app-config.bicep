@description('The name of the App Configuration resource that will be deployed')
param appConfigurationName string

@description('The location that our App Configuration resource will be deployed to.')
param location string

@description('The tags that will be applied to the App Configuration resource')
param tags object

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' = {
  name: appConfigurationName
  location: location
  tags: tags
  sku: {
    name: 'free'
  }
  identity: {
    type: 'SystemAssigned'
  }
}

output appConfigName string = appConfig.name
