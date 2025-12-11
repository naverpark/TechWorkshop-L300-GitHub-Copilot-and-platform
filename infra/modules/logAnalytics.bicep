@description('Location for resources')
param location string = resourceGroup().location
@description('Log Analytics workspace name')
param workspaceName string

resource workspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: workspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

output workspaceId string = workspace.id
