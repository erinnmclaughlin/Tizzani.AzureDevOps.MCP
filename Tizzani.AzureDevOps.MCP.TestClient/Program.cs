using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;
using System.Text.Json;
using System.Text.Json.Nodes;

// This is a scratch pad for testing the MCP client. Will likely be removed soon.

var configuration = new ConfigurationBuilder()
    .AddCommandLine(args)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var accessToken = configuration["AzureDevOps:AccessToken"];
//Console.WriteLine(accessToken);

Console.WriteLine("Starting server...");

var client = await McpClientFactory.CreateAsync(
    new McpServerConfig
    {
        Id = "Tizzani.AzureDevOps.MCP",
        Name = "Tizzani.AzureDevOps.MCP",
        TransportType = TransportTypes.StdIo,
        //Location = "http://localhost:5000/sse",
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
    //loggerFactory: LoggerFactory.Create(x => x.AddConsole())
);

Console.WriteLine("Server started.");

// Print the list of tools available from the server.
//await foreach (var tool in client.ListToolsAsync())
//{
//    Console.WriteLine($"{tool.Name} ({tool.Description})");
//    Console.WriteLine(tool.InputSchema);
//    Console.WriteLine();
//}

//var result = await client.CallToolAsync("getTags", new Dictionary<string, object>());

//var result = await client.CallToolAsync("queryByWiql", new Dictionary<string, object>
//{
//    ["query"] = "Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = 'Task' AND [State] <> 'Closed' AND [State] <> 'Removed' order by [Microsoft.VSTS.Common.Priority] asc, [System.CreatedDate] desc"
//});

//var result = await client.CallToolAsync("getWorkItem", new Dictionary<string, object>
//{
//    ["workItemId"] = 21,
    //["fields"] = new[] { "System.Id", "System.Title", "System.State" }
//});

//var result = await client.CallToolAsync("getPullRequests", new Dictionary<string, object>
//{
//    ["pullRequestStatus"] = "all",
//    ["top"] = 5
//});

//var result = await client.CallToolAsync("getPullRequestById", new Dictionary<string, object>
//{
//    ["pullRequestId"] = 22
//});

//var result = await client.CallToolAsync("createPullRequestThread", new Dictionary<string, object>
//{
//    ["repositoryId"] = "Beacon",
//    ["pullRequestId"] = 26,
//    ["content"] = "This is a test comment!"
//});

//var result = await client.CallToolAsync("createPullRequestThreadForFile", new Dictionary<string, object>
//{
//    ["repositoryId"] = "Beacon",
//    ["pullRequestId"] = 26,
//    ["content"] = "I'm attempting to comment on the DisableScroll property in Modal.razor",
//    ["filePath"] = "/src/Client/BeaconUI.Core/Common/Modals/Modal.razor",
//    ["rightFileStartLine"] = 20,
//    ["rightFileStartOffset"] = 1,
//    ["rightFileEndLine"] = 20,
//    ["rightFileEndOffset"] = 43 // I manually checked the length of the line
//});

//var result = await client.CallToolAsync("getPullRequestThreads", new Dictionary<string, object>
//{
//    ["repositoryId"] = "Beacon",
//    ["pullRequestId"] = 26
//});

//var result = await client.CallToolAsync("getPullRequestCommits", new Dictionary<string, object>
//{
//    ["repositoryId"] = "Beacon",
//    ["pullRequestId"] = 26
//});

//var result = await client.CallToolAsync("getCommitChanges", new Dictionary<string, object>
//{
//    ["repositoryId"] = "Beacon",
//    ["commitId"] = "92b1c27905ee045dd6b0b3beced6a34a68357ecf"
//});

//var result = await client.CallToolAsync("getPullRequestIteration", new Dictionary<string, object>
//{
//    ["repositoryId"] = "Beacon",
//    ["pullRequestId"] = 26,
//    ["iterationId"] = 3
//});

/*
var result = await client.CallToolAsync("getDiffs", new Dictionary<string, object>
{
    ["repositoryId"] = "Beacon",
    ["baseVersion"] = "5612f800254686b7db5d244976e31aaf70ca3dc0",
    ["baseVersionType"] = "commit",
    ["targetVersion"] = "92b1c27905ee045dd6b0b3beced6a34a68357ecf",
    ["targetVersionType"] = "commit"
});
*/

// objectId: "e71b47ab1e381926ff738f9cf60b31a21c92a0e0"
// originalObjectId: "52cf9aeed6dcca525176028d68a80eefbb3c4633"

var result = await client.CallToolAsync("getBlob", new Dictionary<string, object>
{
    ["repositoryId"] = "Beacon",
    ["sha1"] = "e71b47ab1e381926ff738f9cf60b31a21c92a0e0"
});

var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    WriteIndented = true
};

//var jsonObject = JsonSerializer.Deserialize<JsonObject>(result.Content.FirstOrDefault()?.Text ?? "{}");
var jsonElement = JsonSerializer.SerializeToElement(result);
var jsonObject = JsonSerializer.Deserialize<JsonObject>(jsonElement);
var jsonString = jsonObject?.ToJsonString(jsonOptions);
Console.WriteLine(jsonString ?? "No result.");
