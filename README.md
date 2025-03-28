# Azure DevOps MCP Server
An MCP Server for integrating with Azure DevOps REST API.

[![Nuget version](https://img.shields.io/nuget/v/tizzani.azuredevops.mcp)](https://www.nuget.org/packages/Tizzani.AzureDevOps.MCP)

## Installation
Download the tool from [NuGet](https://www.nuget.org/packages/Tizzani.AzureDevOps.MCP).
```sh
dotnet tool install --global Tizzani.AzureDevOps.MCP --prerelease
```

## IDE Integration
```json
"mcpServers": {
  "mcp-azuredevops": {
    "command": "tizzani.adomcp",
    "args": [
      "--ado_token=Your-Token",
      "--ado_organization=Your-Organization",
      "--ado_project=Your-Project"
    ]
  }
}
```
