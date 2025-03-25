namespace Tizzani.AzureDevOps.MCP.Tools.WorkItemTracking;

[McpToolType]
public static class AdoWorkItemTypesTool
{
    private const string ApiVersion = "7.2-preview.2";
    
    [McpTool("getWorkItemTypes")]
    [Description("Get all the work item types for the project.")]
    public static async Task<JsonElement> GetWorkItemTypes(HttpClient httpClient, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/wit/workitemtypes?api-version={ApiVersion}", ct);
    }
    
    [McpTool("getWorkItemType")]
    [Description("Get a specific work item type by name.")]
    public static async Task<JsonElement> GetWorkItemType(
        HttpClient httpClient, 
        [Description("The work item type name.")] string workItemType,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/wit/workitemtypes/{workItemType}?api-version={ApiVersion}", ct);
    }
}