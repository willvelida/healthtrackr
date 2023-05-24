@description('The name to give to the Budget')
param budgetName string

@description('The limit of the budget')
param amount int = 100

@description('The email of the owner of this budget')
param ownerEmailAddress string

@description('The start date of this budget in YYYY-MM-DD format')
param startDate string

@description('The name of the resource group to monitor')
param resourceGroupName string = resourceGroup().name

var firstThreshold = 50
var secondThreshold = 90

resource budget 'Microsoft.Consumption/budgets@2021-10-01' = {
  name: budgetName
  properties: {
    amount: amount
    category: 'Cost'
    timeGrain: 'BillingMonth'
    timePeriod: {
      startDate: startDate
    }
    notifications: {
      NotificationForExceededBudget1: {
        contactEmails: [
          ownerEmailAddress
        ]
        enabled: true
        operator: 'GreaterThanOrEqualTo'
        threshold: firstThreshold
      }
      NotificationForExceededBudget2: {
        contactEmails: [
          ownerEmailAddress
        ]
        enabled: true
        operator: 'GreaterThanOrEqualTo'
        threshold: secondThreshold
      }
    }
    filter: {
      dimensions: {
        name: 'ResourceGroupName'
        operator: 'In'
        values: [
          resourceGroupName
        ]
      }
    }
  }  
}
