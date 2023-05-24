@description('Specifies the name of the Log Analytics workspace resource')
param logAnalyticsWorkspaceName string

@description('The location that our resources will be deployed to')
param location string

@description('The tags that will be applied to the Log Analytics workspace')
param tags object

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

output logAnalyticsId string = logAnalytics.id
