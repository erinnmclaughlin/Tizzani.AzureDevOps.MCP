using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tizzani.AzureDevOps.MCP.Tools;

[McpToolType]
public class AdoWorkItemTypeFieldsTool
{
    private const string ApiVersion = "7.2-preview.3";
    
    [McpTool("getWorkItemTypeFields")]
    [Description("Gets work item type fields for a given work item type")]
    public static async Task<JsonElement> GetWorkItemTypeFields(
        HttpClient httpClient,
        [Description("The work item type name.")] string workItemType,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/wit/workitemtypes/{workItemType}/fields?api-version={ApiVersion}", ct);
    }
    
    [McpTool("getWorkItemTypeField")]
    [Description("Gets a specific work item type field by name.")]
    public static async Task<JsonElement> GetWorkItemTypeField(
        HttpClient httpClient,
        [Description("The work item type name.")] string workItemType,
        [Description("The field name.")] string field,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/wit/workitemtypes/{workItemType}/fields/{field}?api-version={ApiVersion}", ct);
    }
}