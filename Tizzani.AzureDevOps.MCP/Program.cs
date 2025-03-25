using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;
using System.Net.Http.Headers;
using Tizzani.AzureDevOps.MCP;

var configuration = new ConfigurationBuilder()
    .AddCommandLine(args)
    .AddEnvironmentVariables()
    .Build();

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<IConfiguration>(configuration);

var adoToken = configuration.GetRequiredConfigurationValue("ado_token");
var adoOrg = configuration.GetRequiredConfigurationValue("ado_organization");
var adoProject = configuration.GetRequiredConfigurationValue("ado_project");

serviceCollection.AddScoped(_ =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri($"https://dev.azure.com/{adoOrg}/{adoProject}/") };
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", $":{adoToken}".ToBase64EncodedString());
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    return httpClient;
});

var serverOptions = new McpServerOptions
{
    ServerInfo = new Implementation { Name = "AzureDevOpsMCP", Version = "1.0.0" },
    Capabilities = new ServerCapabilities
    {
        Tools = CustomMcpServerBuilder.AddMcpTools()
    }
};

var mcpServer = McpServerFactory.Create(new StdioServerTransport("AzureDevOpsMCP"), serverOptions, serviceProvider: serviceCollection.BuildServiceProvider());

await mcpServer.StartAsync();

while (true)
{
    await Task.Delay(2000);
}