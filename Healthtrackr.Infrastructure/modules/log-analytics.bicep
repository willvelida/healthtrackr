@description('Specifies the name of the Log Analytics workspace resource')
param logAnalyticsWorkspaceName string

@description('The location that our resources will be deployed to')
param location string

@description('The tags that will be applied to the Log Analytics workspace')
param tags object

@description('The name of the Key Vault that this Log Analytics workspace will use to store secrets in')
param keyVaultName string

var sharedKeySecretName = 'LogAnalyticsSharedKey'

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: logAnalyticsWorkspaceName
  location: location
  tags: tags
  properties: {
    retentionInDays: 30
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource lawSharedKeySecret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  name: sharedKeySecretName
  parent: keyVault
  properties: {
    value: logAnalytics.listKeys().primarySharedKey
  }
}

output logAnalyticsId string = logAnalytics.id
output logAnalyticsName string = logAnalytics.name
