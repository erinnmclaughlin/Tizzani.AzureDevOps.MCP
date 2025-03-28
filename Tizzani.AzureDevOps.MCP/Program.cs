using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol.Types;
using System.Net.Http.Headers;
using Tizzani.AzureDevOps.MCP;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();

var adoToken = builder.Configuration.GetRequiredConfigurationValue("ado_token");
var adoOrg = builder.Configuration.GetRequiredConfigurationValue("ado_organization");
var adoProject = builder.Configuration.GetRequiredConfigurationValue("ado_project");

builder.Services.AddSingleton(_ =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri($"https://dev.azure.com/{adoOrg}/{adoProject}/") };
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", $":{adoToken}".ToBase64EncodedString());
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    return httpClient;
});

builder.Services.AddMcpServer(o =>
{
    o.ServerInfo = new Implementation { Name = "Tizzani.AzureDevOps.MCP", Version = "1.0.0" };
})
.WithStdioServerTransport()
.WithToolsFromAssembly();

await builder.Build().RunAsync();