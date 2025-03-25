using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;
using System.Text.Json;

var configuration = new ConfigurationBuilder()
    .AddCommandLine(args)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var accessToken = configuration["AzureDevOps:AccessToken"];
Console.WriteLine(accessToken);

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

var comments = await client.CallToolAsync("getComments", new Dictionary<string, object>
{
    ["workItemId"] = 21
});

Console.WriteLine(JsonSerializer.SerializeToElement(comments.Content.FirstOrDefault()?.Text ?? "{}"));
