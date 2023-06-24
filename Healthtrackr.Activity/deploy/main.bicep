@description('The location that we deploy our resources to. Default value is the location of the resource group')
param location string = resourceGroup().location

@description('Name of the storage account provisioned for use by the Function')
param storageAccountName string

@description('The SKU for the storage account. Default is Standard_LRS')
param storageSku string = 'Standard_LRS'

@description('The name of the app service plan that we will deploy our Function to')
param appServicePlanName string

@description('The name of the Function App that we will deploy.')
param functionAppName string

@description('The name of the App Insights instance that this function will send logs to')
param appInsightsName string

@description('The name of the key vault that we will create Access Policies for')
param keyVaultName string

@description('The name of the Cosmos DB account that this Function will use')
param cosmosDbAccountName string

@description('The name of the database in Cosmos DB that the created container will use')
param cosmosDatabaseName string

@description('The name of the Service Bus Namespace that this Function will use')
param serviceBusNamespace string

@description('The name of the App Config instance that this function will use')
param appConfigName string

@description('The time that the resource was last deployed')
param lastDeployed string = utcNow()

var functionRuntime = 'dotnet-isolated'
var tags = {
  ApplicationName: 'Healthtrackr'
  Component: 'Activity'
  Environment: 'Production'
  LastDeployed: lastDeployed
}

var activityQueueName = 'activityqueue'
var activityTableName = 'Activity'
var serviceBusOwnerRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '090c5cfd-751d-490a-894a-3ce6f1109419')
var serviceBusDataReceiverRole = subscriptionResourceId('Microsoft.Authorization/roleDefinitions','4f6d3b9b-027b-4f4c-9142-0e5a2a2247e0')
var serviceBusDataSenderRole = subscriptionResourceId('Microsoft.Authorization/roleDefinitions','69a216fc-b8fb-44d8-bc22-1f3c2cd27a39')
var appConfigDataReaderRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '516239f1-63e1-4d78-a4de-a74fb236a071')

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: appConfigName
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-03-01' existing = {
  name: appServicePlanName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource keyVault 'Microsoft.KeyVault/vaults@2021-11-01-preview' existing = {
  name: keyVaultName
}

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2022-02-15-preview' existing = {
  name: cosmosDbAccountName
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2021-11-01' existing = {
  name: serviceBusNamespace
}

resource activityQueue 'Microsoft.ServiceBus/namespaces/queues@2021-11-01' = {
  name: activityQueueName
  parent: serviceBus
}

resource activityQueueSetting 'Microsoft.AppConfiguration/configurationStores/keyValues@2022-05-01' = {
  parent: appConfig
  name: 'Healthtrackr:ActivityQueueName'
  properties: {
    value: activityQueue.name
    tags: tags
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: storageSku
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
  }
}

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppName
  location: location
  tags: tags
  kind: 'functionapp'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: 'InstrumentationKey=${appInsights.properties.InstrumentationKey}'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: functionRuntime
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'CosmosDbEndpoint'
          value: cosmosDb.properties.documentEndpoint
        }
        {
          name: 'CosmosDbConnection__accountEndpoint'
          value: cosmosDb.properties.documentEndpoint
        }
        {
          name: 'CosmosDbConnection__credential'
          value: 'managedIdentity'
        }
        {
          name: 'ServiceBusConnection__fullyQualifiedNamespace'
          value: '${serviceBus.name}.servicebus.windows.net'
        }
        {
          name: 'ServiceBusEndpoint'
          value: '${serviceBus.name}.servicebus.windows.net'
        }
        {
          name: 'KeyVaultUri'
          value: keyVault.properties.vaultUri
        }
        {
          name: 'AzureAppConfigEndpoint'
          value: appConfig.properties.endpoint
        }
        {
          name: 'WEBSITE_TIME_ZONE'
          value: 'New Zealand Standard Time'
        }
      ]
    }
    httpsOnly: true
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource accessPolicies 'Microsoft.KeyVault/vaults/accessPolicies@2022-07-01' = {
  name: 'add'
  parent: keyVault
  properties: {
    accessPolicies: [
      {
        objectId: functionApp.identity.principalId
        tenantId: functionApp.identity.tenantId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ] 
  }
}

resource appConfigDataReaderRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(appConfig.id, functionApp.id, appConfigDataReaderRoleId)
  scope: appConfig
  properties: {
    principalId: functionApp.identity.principalId
    roleDefinitionId: appConfigDataReaderRoleId
    principalType: 'ServicePrincipal'
  }
}

resource serviceBusOwnerRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(serviceBus.id, functionApp.id, serviceBusOwnerRoleId)
  properties: {
    principalId: functionApp.identity.principalId
    roleDefinitionId: serviceBusOwnerRoleId
    principalType: 'ServicePrincipal'
  }
}

resource serviceBusReceiverRole 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid(serviceBus.id, functionApp.id, serviceBusDataReceiverRole)
  scope: serviceBus
  properties: {
    principalId: functionApp.identity.principalId
    roleDefinitionId: serviceBusDataReceiverRole
    principalType: 'ServicePrincipal'
  }
}

resource serviceBusSenderRole 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid(serviceBus.id, functionApp.id, serviceBusDataSenderRole)
  scope: serviceBus
  properties: {
    principalId: functionApp.identity.principalId
    roleDefinitionId: serviceBusDataSenderRole
    principalType: 'ServicePrincipal'
  }
}

module activityContainer 'modules/cosmos-container.bicep' = {
  name: 'activity-container'
  params: {
    appConfigName: appConfig.name
    containerName: activityTableName
    cosmosDbAccountName: cosmosDbAccountName
    databaseName: cosmosDatabaseName
  }
}

module sqlRoleAssignment 'modules/sql-role-assignment.bicep' = {
  name: 'sqlRoleAssignment'
  params: {
    cosmosDbAccountName: cosmosDb.name
    functionAppPrincipalId: functionApp.identity.principalId
  }
}
