@description('Deployment location')
param location string = resourceGroup().location
@description('Application name prefix')
param appName string = 'zavastorefront'
@description('Container image to deploy (registry/image:tag)')
param imageName string

var uniqueSuffix = uniqueString(resourceGroup().id)
var acrName = 'junpark${uniqueSuffix}'
var laWorkspaceName = 'zavastorefront-la-${uniqueSuffix}'

// Modules
module acr 'modules/acr.bicep' = {
  name: 'acrModule'
  params: {
    acrName: acrName
    location: location
  }
}

module log 'modules/logAnalytics.bicep' = {
  name: 'logModule'
  params: {
    location: location
    workspaceName: laWorkspaceName
  }
}

module app 'modules/appService.bicep' = {
  name: 'appModule'
  params: {
    appName: appName
    location: location
    imageName: imageName
    acrLoginServer: acr.outputs.loginServer
  }
}

output webAppName string = app.outputs.webAppName
output acrLoginServer string = acr.outputs.loginServer
output logAnalyticsWorkspaceId string = log.outputs.workspaceId
