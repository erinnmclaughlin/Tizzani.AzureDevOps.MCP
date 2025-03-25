<div align="center">
    <h3><b>Azure DevOps MCP Server</b></h3>
    <p>An MCP Server for querying with Azure DevOps REST API.</p>
    <div>
        <img alt="Nuget version" src="https://img.shields.io/nuget/v/tizzani.azuredevops.mcp">
        <img alt="GitHub last commit" src="https://img.shields.io/github/last-commit/erinnmclaughlin/tizzani.azuredevops.mcp/main">
        <!--<img alt="GitHub Workflow Status (with event)" src="https://img.shields.io/github/actions/workflow/status/erinnmclaughlin/Tizzani.QueryStringSerializer/dotnet.yml">-->
    </div>
</div>

<hr />

## Installation
Download the tool from [NuGet](https://www.nuget.org/packages/Tizzani.AzureDevOps.MCP).
```sh
dotnet tool install --global Tizzani.AzureDevOps.MCP --version 1.0.0-preview.20250324.2
```

## IDE Integration
For Cursor or Claude, add the `tizzani.adomcp` command with the required arguments.
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
