@description('The name given to this container app environment')
param envName string

@description('The location to deploy this container app environment')
param location string

@description('The Id of the Log Analytics workspace that this environment will send logs to')
param logAnalyticsId string

@description('The shared key of the Log Analytics workspace that this environment will send logs to')
@secure()
param logAnalyticsSharedKey string

@description('The Connection String that this container app environment will use to send Dapr Service-to-Service communication telemtry to')
@secure()
param aiConnectionString string

@description('The tags to apply to this Container App environment')
param tags object

resource containerAppEnv 'Microsoft.App/managedEnvironments@2023-04-01-preview' = {
  name: envName
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsId
        sharedKey: logAnalyticsSharedKey
      }
    }
    daprAIConnectionString: aiConnectionString
  }
}

output containerAppEnvId string = containerAppEnv.id
