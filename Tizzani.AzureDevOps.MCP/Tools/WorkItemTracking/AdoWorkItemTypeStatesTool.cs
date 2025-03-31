namespace Tizzani.AzureDevOps.MCP.Tools.WorkItemTracking;

[McpServerToolType]
public static class AdoWorkItemTypeStatesTool
{
    private const string ApiVersion = "7.2-preview.1";
    
    [McpServerTool(Name = "getWorkItemTypeStates")]
    [Description("Gets work item type states for a given work item type")]
    public static async Task<JsonElement> GetWorkItemTypeStates(
        HttpClient httpClient,
        [Description("The work item type name.")] string workItemType,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/wit/workitemtypes/{workItemType}/states?api-version={ApiVersion}", ct);
    }
}