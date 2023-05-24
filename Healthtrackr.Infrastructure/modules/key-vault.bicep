@description('The name of the Key Vault that will be deployed')
param keyVaultName string

@description('The location that our resources will be deployed to.')
param location string = resourceGroup().location

@description('The access policies to apply to the Key Vault')
param accessPolicies array

@description('The tags that will be applied to the Key Vault')
param tags object

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enabledForTemplateDeployment: true
    accessPolicies: accessPolicies
  }
}

output keyVaultName string = keyVault.name
