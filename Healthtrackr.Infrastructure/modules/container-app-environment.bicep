@description('The name given to this container app environment')
param envName string

@description('The location to deploy this container app environment')
param location string

@description('The name of the Log Analytics workspace that this environment will send logs to')
param logAnalyticsWorkspaceName string

@description('The name of the Application Insights workspace this environment will sent Dapr logs to')
param appInsightsName string

@description('The tags to apply to this Container App environment')
param tags object

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: logAnalyticsWorkspaceName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource containerAppEnv 'Microsoft.App/managedEnvironments@2023-04-01-preview' = {
  name: envName
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
    daprAIConnectionString: appInsights.properties.ConnectionString
  }
}

output containerAppEnvId string = containerAppEnv.id
