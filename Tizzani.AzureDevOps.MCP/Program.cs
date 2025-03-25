using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Protocol.Types;
using System.Net.Http.Headers;
using Tizzani.AzureDevOps.MCP;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();

builder.Services.AddScoped(sp =>
{
    var c = sp.GetRequiredService<IConfiguration>();
    var adoToken = c.GetRequiredConfigurationValue("ado_token");
    var adoOrg = c.GetRequiredConfigurationValue("ado_organization");
    var adoProject = c.GetRequiredConfigurationValue("ado_project");
    
    var httpClient = new HttpClient { BaseAddress = new Uri($"https://dev.azure.com/{adoOrg}/{adoProject}/") };
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", $":{adoToken}".ToBase64EncodedString());
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    return httpClient;
});

builder.Services.AddMcpServer(o =>
{
    o.ServerInfo = new Implementation { Name = "tizzani-adomcp", Version = "1.0.0" };
    o.Capabilities = new ServerCapabilities
    {
        Tools = CustomMcpServerBuilder.AddMcpTools()
    };
}).WithStdioServerTransport();

await builder.Build().RunAsync();