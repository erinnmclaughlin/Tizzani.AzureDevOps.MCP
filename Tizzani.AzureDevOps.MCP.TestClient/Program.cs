using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;
using System.Text.Json;

// This is a scratch pad for testing the MCP client. Will likely be removed soon.

var configuration = new ConfigurationBuilder()
    .AddCommandLine(args)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var accessToken = configuration["AzureDevOps:AccessToken"];

Console.WriteLine("Starting server...");

var client = await McpClientFactory.CreateAsync(
    new McpServerConfig
    {
        Id = "Tizzani.AzureDevOps.MCP",
        Name = "Tizzani.AzureDevOps.MCP",
        TransportType = TransportTypes.StdIo,
        TransportOptions = new Dictionary<string, string>
        {
            ["command"] = "dotnet",
            ["arguments"] = $"run --project ../Tizzani.AzureDevOps.MCP/Tizzani.AzureDevOps.MCP.csproj --ado_project=Beacon --ado_organization=BeaconLMS --ado_token={accessToken}"
        }
    },
    new McpClientOptions 
    { 
        ClientInfo = new Implementation { Name = "TestClient", Version = "1.0.0" } 
    }
);

Console.WriteLine("Server started.");

// Print the list of tools available from the server.
await foreach (var tool in client.ListToolsAsync())
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
    Console.WriteLine(tool.InputSchema);
    Console.WriteLine();
}

//var result = await client.CallToolAsync("getTags", new Dictionary<string, object>());

//var result = await client.CallToolAsync("queryByWiql", new Dictionary<string, object>
//{
//    ["query"] = "Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = 'Task' AND [State] <> 'Closed' AND [State] <> 'Removed' order by [Microsoft.VSTS.Common.Priority] asc, [System.CreatedDate] desc"
//});

var result = await client.CallToolAsync("getWorkItem", new Dictionary<string, object>
{
    ["workItemId"] = 21,
    ["fields"] = new[] { "System.Id", "System.Title", "System.State" }
});

Console.WriteLine(JsonSerializer.SerializeToElement(result.Content.FirstOrDefault()?.Text ?? "{}"));
