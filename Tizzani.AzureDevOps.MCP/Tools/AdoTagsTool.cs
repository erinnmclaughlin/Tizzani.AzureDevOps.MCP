using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tizzani.AzureDevOps.MCP.Tools;

[McpToolType]
public class AdoTagsTool
{
    private const string ApiVersion = "7.2-preview.1";
    
    [McpTool("getTags")]
    [Description("Get all the tags for the project.")]
    public static async Task<JsonElement> GetWorkItemTags(HttpClient httpClient, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/wit/tags?api-version={ApiVersion}", ct);
    }
}