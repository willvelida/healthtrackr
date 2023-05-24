@description('The location that our App Service Plan will be deployed to')
param location string

@description('Name of the App Service Plan')
param appServicePlanName string

@description('The tags that will be applied to the App Service Plan')
param tags object

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  kind: 'linux'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {
    reserved: true
  }
}
