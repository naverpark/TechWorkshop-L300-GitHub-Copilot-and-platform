@description('App service name')
param appName string
@description('Location')
param location string = resourceGroup().location
@description('Container image name (registry/image:tag)')
param imageName string
@description('ACR login server (optional)')
param acrLoginServer string = ''

var planName = '${appName}-plan'
var siteName = appName

resource plan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: planName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource site 'Microsoft.Web/sites@2021-02-01' = {
  name: siteName
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: plan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|${imageName}'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
        {
          name: 'ACR_LOGIN_SERVER'
          value: acrLoginServer
        }
      ]
    }
  }
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = if (false) {
  name: 'unused'
  location: location
}

output webAppName string = site.name
