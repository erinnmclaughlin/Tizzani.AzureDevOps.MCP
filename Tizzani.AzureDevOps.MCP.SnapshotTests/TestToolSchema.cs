using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Tizzani.AzureDevOps.MCP.Tools.Git;

namespace Tizzani.AzureDevOps.MCP.SnapshotTests;

public class TestToolSchema
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    [Fact]
    public void TestSchemaWithEnum()
    {
        var toolType = typeof(AdoDiffsTool);
        var toolMethod = toolType.GetMethod("GetBranchDiffs");
        
        var sp = new ServiceCollection().BuildServiceProvider();

        var tool = CustomMcpServerBuilder.BuildTool(toolMethod!, sp);

        var jsonObj = JsonObject.Create(tool.InputSchema);
        Assert.NotNull(jsonObj);
        
        jsonObj.TryGetPropertyValue("properties", out var properties);
        Assert.NotNull(properties);
        
        properties.AsObject().TryGetPropertyValue("baseVersionType", out var baseVersionType);
        Assert.NotNull(baseVersionType);

        baseVersionType.AsObject().TryGetPropertyValue("type", out var type);
        Assert.Equal("string", type?.GetValue<string>());
    }
    
    [Fact]
    public async Task VerifyTestToolSchema()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => new HttpClient());
        services.AddMcpServerWithTools();
        
        var serviceProvider = services.BuildServiceProvider();

        var result = new List<Tool>();
        
        var methods = typeof(CustomMcpServerBuilder).Assembly
            .GetTypes()
            .Where(x => x.GetCustomAttribute<McpToolTypeAttribute>() != null)
            .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static));

        foreach (var method in methods)
        {
            var tool = CustomMcpServerBuilder.BuildTool(method, serviceProvider);
            result.Add(tool);
        }

        await Verify(result.Select(r => JsonObject.Create(JsonSerializer.SerializeToElement(r, JsonOptions))?.ToJsonString(JsonOptions)));
    }
}