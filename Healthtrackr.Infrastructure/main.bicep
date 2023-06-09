@description('The location that resources will be deployed to. Default is the location of the resource group')
param location string = resourceGroup().location

@description('Specifies the name of the App Service Plan')
param appServicePlanName string

@description('Specifies the name of the Application Insights resource')
param appInsightsName string

@description('Specifies the name of the Container App Environment')
param containerAppEnvName string

@description('Specifies the name of the Container Registry')
param containerRegistryName string

@description('Specifies the name of the Cosmos DB account')
param cosmosDbAccountName string

@description('Specifies the name of the Cosmos DB database')
param cosmosDbDatabaseName string

@description('Specifies the name of the Key Vault resource')
param keyVaultName string

@description('Specifies the name of the App Configuration resource')
param appConfigName string

@description('Specifies the name of the Service Bus Namespace')
param serviceBusNamespaceName string

@description('Specifies the name of the Log Analytics workspace')
param logAnalyticsWorkspaceName string

@description('The timestamp that this template was last deployed')
param lastDeployed string = utcNow()

@description('The name of the SQL Server that will be deployed')
param sqlServerName string

@description('The administrator username of the SQL logical server')
param sqlAdminLogin string

@description('The administrator password of the SQL logical server')
param sqlAdminPassword string

@description('The name of the database in this Cosmos DB account')
param databaseName string

@description('Name of the Budget that will be created')
param budgetName string

@description('The email address to use for the budget')
param emailAddress string

@description('Flag to indicate if this is a new Key Vault, and therefore should have no access policies configured. Default is false')
param isNewKeyVault bool = false

var tags = {
  ApplicationName: 'Healthtrackr'
  Component: 'Infrastructure'
  Environment: 'Production'
  LastDeployed: lastDeployed
}
var budgetStartDate = '2023-01-01'
var accessPolicies = isNewKeyVault ? [] : reference(resourceId('Microsoft.KeyVault/vaults', keyVaultName), '2022-07-01').accessPolicies

module appServicePlan 'modules/app-service-plan.bicep' = {
  name: 'asp'
  params: {
    appServicePlanName: appServicePlanName
    location: location
    tags: tags
  }
}

module appInsights 'modules/app-insights.bicep' = {
  name: 'appins'
  params: {
    appInsightsName: appInsightsName 
    tags: tags
    location: location
    logAnalyticsId: logAnalytics.outputs.logAnalyticsId
    keyVaultName: keyVault.outputs.keyVaultName
  }
}

module appConfig 'modules/app-config.bicep' = {
  name: 'appconfig'
  params: {
    appConfigurationName: appConfigName
    location: location
    tags: tags
  }
}

module budget 'modules/budget.bicep' = {
  name: 'budget'
  params: {
    budgetName: budgetName
    ownerEmailAddress: emailAddress
    startDate: budgetStartDate
  }
}

module containerAppEnv 'modules/container-app-environment.bicep' = {
  name: 'ca-env'
  params: {
    appInsightsName: appInsights.outputs.appInsightsName
    envName: containerAppEnvName
    location:location
    logAnalyticsWorkspaceName: logAnalytics.outputs.logAnalyticsName
    tags: tags
  }
}

module containerRegistry 'modules/container-registry.bicep' = {
  name: 'acr'
  params: {
    containerRegistryName: containerRegistryName
    location: location
    tags: tags
  }
}

module cosmosDb 'modules/cosmos-db.bicep' = {
  name: 'cosmos-db'
  params: {
    appConfigName: appConfig.outputs.appConfigName
    cosmosDBAccountName: cosmosDbAccountName
    databaseName: cosmosDbDatabaseName
    keyVaultName: keyVault.outputs.keyVaultName
    location: location
    tags: tags
  }
}

module keyVault 'modules/key-vault.bicep' = {
  name: 'kv'
  params: {
    keyVaultName: keyVaultName
    tags: tags
    location: location
    accessPolicies: accessPolicies
  }
}

module logAnalytics 'modules/log-analytics.bicep' = {
  name: 'law'
  params: {
    location: location 
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    tags: tags
    keyVaultName: keyVault.outputs.keyVaultName
  }
}

module serviceBus 'modules/service-bus.bicep' = {
  name: 'service-bus'
  params: {
    serviceBusNamespaceName: serviceBusNamespaceName 
    tags: tags
    location: location
  }
}

module sql 'modules/sql-server.bicep' = {
  name: 'sql'
  params: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    serverName: sqlServerName
    sqlDBName: databaseName
    location: location
    tags: tags
    keyVaultName: keyVault.outputs.keyVaultName
  }
}
