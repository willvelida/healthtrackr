@description('Name of the App Insights instance that will be deployed')
param appInsightsName string

@description('The location that our App Insights instance will be deployed to')
param location string = resourceGroup().location

@description('The Log Analytics Id that this App Insights will use to stream logs to')
param logAnalyticsId string

@description('The tags that will be applied to the App Insights instance')
param tags object

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
