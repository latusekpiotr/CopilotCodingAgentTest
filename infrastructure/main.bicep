@description('The name of the environment (e.g., dev, staging, prod)')
param environmentName string = 'dev'

@description('The location where resources will be deployed')
param location string = resourceGroup().location

@description('The name prefix for all resources')
param appName string = 'todolist'

// Variables
var uniqueSuffix = substring(uniqueString(resourceGroup().id), 0, 6)
var appServicePlanName = '${appName}-plan-${environmentName}-${uniqueSuffix}'
var appServiceName = '${appName}-app-${environmentName}-${uniqueSuffix}'

// App Service Plan (Basic tier for cost efficiency)
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true // Required for Linux App Service Plan
  }
  tags: {
    Environment: environmentName
    Project: 'TodoList'
  }
}

// App Service
resource appService 'Microsoft.Web/sites@2023-01-01' = {
  name: appServiceName
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    reserved: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: false // Set to false for Basic tier to save costs
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
    }
    httpsOnly: true
    clientAffinityEnabled: false
  }
  tags: {
    Environment: environmentName
    Project: 'TodoList'
  }
}

// Outputs
output appServiceUrl string = 'https://${appService.properties.defaultHostName}'
output appServiceName string = appService.name
output resourceGroupName string = resourceGroup().name