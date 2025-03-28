using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Server;
using System.IO.Pipelines;
using System.Text.Json;
using Tizzani.AzureDevOps.MCP.Tools.Git;

namespace Tizzani.AzureDevOps.MCP.SnapshotTests;

public sealed class TestToolSchema
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    
    private readonly Pipe _clientToServerPipe = new();
    private readonly Pipe _serverToClientPipe = new();
    private readonly IMcpServer _server;
    
    public TestToolSchema()
    {
        ServiceCollection sc = new();
        sc.AddSingleton<IServerTransport>(new StdioServerTransport("TestServer", _clientToServerPipe.Reader.AsStream(), _serverToClientPipe.Writer.AsStream()));
        sc.AddSingleton(new HttpClient());
        sc.AddMcpServer().WithToolsFromAssembly(typeof(AdoPullRequestsTool).Assembly);
        
        _server = sc.BuildServiceProvider().GetRequiredService<IMcpServer>();
    }
    
    [Fact]
    public async Task VerifyTestToolSchema()
    {
        var client = await CreateMcpClientForServer();
        var tools = await client.ListToolsAsync(TestContext.Current.CancellationToken);
        await Verify(JsonSerializer.Serialize(tools.Select(x => x.JsonSchema), JsonOptions));
    }

    private async Task<IMcpClient> CreateMcpClientForServer()
    {
        await _server.StartAsync(TestContext.Current.CancellationToken);

        var serverStdinWriter = new StreamWriter(_clientToServerPipe.Writer.AsStream());
        var serverStdoutReader = new StreamReader(_serverToClientPipe.Reader.AsStream());

        var serverConfig = new McpServerConfig
        {
            Id = "TestServer",
            Name = "TestServer",
            TransportType = "ignored",
        };

        return await McpClientFactory.CreateAsync(
            serverConfig,
            createTransportFunc: (_, _) => new StreamClientTransport(serverStdinWriter, serverStdoutReader),
            cancellationToken: TestContext.Current.CancellationToken);
    }
}