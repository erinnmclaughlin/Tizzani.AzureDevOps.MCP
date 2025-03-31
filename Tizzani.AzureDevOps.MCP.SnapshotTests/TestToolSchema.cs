using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using System.Text.Json;

namespace Tizzani.AzureDevOps.MCP.SnapshotTests;

public sealed class TestToolSchema
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    
    private readonly McpServerConfig _testServerConfig;
    
    public TestToolSchema()
    {
        _testServerConfig = new McpServerConfig
        {
            Id = "test_server",
            Name = "TestServer",
            TransportType = TransportTypes.StdIo,
            TransportOptions = new Dictionary<string, string>
            {
                ["command"] = OperatingSystem.IsWindows() ? "Tizzani.AzureDevOps.MCP.exe" : "dotnet",
                ["arguments"] = ""
            }
        };

        if (!OperatingSystem.IsWindows())
        {
            _testServerConfig.TransportOptions["arguments"] = "Tizzani.AzureDevOps.MCP.dll";
        }

        _testServerConfig.TransportOptions["arguments"] += " --ado_token=nope --ado_organization=nope --ado_project=nope";
    }
    
    [Fact]
    public async Task VerifyTestToolSchema()
    {
        var client = await McpClientFactory.CreateAsync(_testServerConfig, cancellationToken: TestContext.Current.CancellationToken);
        var tools = await client.ListToolsAsync(TestContext.Current.CancellationToken);
        await Verify(JsonSerializer.Serialize(tools.Select(x => x.JsonSchema), JsonOptions));
    }
}
