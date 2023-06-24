@description('The name of the container to create for the Cosmos DB account')
param containerName string

@description('The name of the Cosmos DB account to deploy the container to')
param cosmosDbAccountName string

@description('The name of the database that will hold the container')
param databaseName string

@description('The App Configuration instance that this container will use to store the container name as a setting')
param appConfigName string

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: appConfigName
}

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2022-08-15' existing = {
  name: cosmosDbAccountName
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2022-08-15' existing = {
  name: databaseName
  parent: cosmosDb
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2022-08-15' = {
  name: containerName
  parent: database
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: [
          '/Date'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        includedPaths: [
          {
            path: '/*'
          }
        ]
      }
    }
  }
}

resource cosmosContainerSetting 'Microsoft.AppConfiguration/configurationStores/keyValues@2022-05-01' = {
  name: 'HealthCheckr:${containerName}ContainerName'
  parent: appConfig
  properties: {
    value: container.name
  }
}
