@description('Name of the App Insights instance that will be deployed')
param appInsightsName string

@description('The location that our App Insights instance will be deployed to')
param location string = resourceGroup().location

@description('The Log Analytics Id that this App Insights will use to stream logs to')
param logAnalyticsId string

@description('The name of the Key Vault that this App Insights workspace will use to store secrets in')
param keyVaultName string

@description('The tags that will be applied to the App Insights instance')
param tags object

var aiConnectionStringSecretName = 'AppInsConnectionString'

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    WorkspaceResourceId: logAnalyticsId
  }
}

resource appInsightsConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  name: aiConnectionStringSecretName
  parent: keyVault
  properties: {
    value: appInsights.properties.ConnectionString
  }
}
