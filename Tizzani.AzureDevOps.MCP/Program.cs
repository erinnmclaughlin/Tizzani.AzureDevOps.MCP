using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;
using ModelContextProtocol.Protocol.Types;
using System.Net.Http.Headers;
using Tizzani.AzureDevOps.MCP;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Configuration.AddCommandLine(args);
builder.Configuration.AddEnvironmentVariables();

var adoToken = builder.Configuration.GetRequiredConfigurationValue("ado_token");
var adoOrg = builder.Configuration.GetRequiredConfigurationValue("ado_organization");
var adoProject = builder.Configuration.GetRequiredConfigurationValue("ado_project");

Console.WriteLine(adoToken);
Console.WriteLine(adoOrg);
Console.WriteLine(adoProject);

builder.Services.AddScoped(_ =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri($"https://dev.azure.com/{adoOrg}/{adoProject}/_apis/wit/workItems/") };
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", $":{adoToken}".ToBase64EncodedString());
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    return httpClient;
});

builder.Services.AddMcpServer(o =>
{
    o.ServerInfo = new Implementation { Name = "Tizzani.AzureDevOps.MCP", Version = "1.0.0" };
    o.Capabilities = new ServerCapabilities
    {
        Tools = new ToolsCapability
        {
            CallToolHandler = McpServices.ConfigureCallToolHandler,
            ListToolsHandler = McpServices.ConfigureListToolsHandler
        }
    };
}).WithStdioServerTransport();

await builder.Build().RunAsync();
