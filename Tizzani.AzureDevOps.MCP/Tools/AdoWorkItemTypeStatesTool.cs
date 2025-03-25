using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tizzani.AzureDevOps.MCP.Tools;

[McpToolType]
public class AdoWorkItemTypeStatesTool
{
    private const string ApiVersion = "7.2-preview.1";
    
    [McpTool("getWorkItemTypeStates")]
    [Description("Gets work item type states for a given work item type")]
    public static async Task<JsonElement> GetWorkItemTypeStates(
        HttpClient httpClient,
        [Description("The work item type name.")] string workItemType,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/wit/workitemtypes/{workItemType}/states?api-version={ApiVersion}", ct);
    }
}