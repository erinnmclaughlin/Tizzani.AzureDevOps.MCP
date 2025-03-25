using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

builder.Services.AddMcpServerWithTools().WithStdioServerTransport();

await builder.Build().RunAsync();