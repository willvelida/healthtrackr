@description('The name of the SQL logical server.')
param serverName string

@description('The name of the SQL Database.')
param sqlDBName string

@description('Location for all resources.')
param location string = resourceGroup().location

@description('The administrator username of the SQL logical server.')
param administratorLogin string

@description('The administrator password of the SQL logical server.')
@secure()
param administratorLoginPassword string

@description('The Key Vault to store the SQL secrets in')
param keyVaultName string

@description('The tags applied to this SQL Server')
param tags object

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

var sqlConnectionSecretName = 'SqlConnectionString'

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: serverName
  location: location
  tags: tags
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  name: sqlDBName
  location: location
  parent: sqlServer
  tags: tags
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}

resource sqlDBConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: sqlConnectionSecretName
  parent: keyVault
  properties: {
    value: 'Server=tcp:${sqlServer.name}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${sqlDBName};Persist Security Info=False;User ID=${administratorLogin};Password=${administratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
}

resource allowAzureFirewallRule 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  name: 'AllowAllWindowsAzureIps'
 parent: sqlServer
 properties: {
  startIpAddress: '0.0.0.0'
  endIpAddress: '0.0.0.0'
 } 
}
