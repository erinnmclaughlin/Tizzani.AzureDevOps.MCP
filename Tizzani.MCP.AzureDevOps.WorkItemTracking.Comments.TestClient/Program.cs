using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;
using System.Text.Json;
using System.Text.Json.Nodes;

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
        Id = "Tizzani.MCP.AzureDevOps.WorkItemTracking.Comments",
        Name = "Tizzani.MCP.AzureDevOps.WorkItemTracking.Comments",
        TransportType = TransportTypes.StdIo,
        TransportOptions = new Dictionary<string, string>
        {
            ["command"] = "dotnet",
            ["arguments"] = $"run --project ../Tizzani.MCP.AzureDevOps.WorkItemTracking.Comments/Tizzani.MCP.AzureDevOps.WorkItemTracking.Comments.csproj --ado_project=Beacon --ado_organization=BeaconLMS --ado_token={accessToken}"
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
}

var comments = await client.CallToolAsync("getComments", new Dictionary<string, object>
{
    ["workItemId"] = 21
});

var json = JsonSerializer.Deserialize<JsonObject>(comments.Content.FirstOrDefault()?.Text ?? "{}");
var jsonString = json?.ToJsonString(new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true });

Console.WriteLine(jsonString);
