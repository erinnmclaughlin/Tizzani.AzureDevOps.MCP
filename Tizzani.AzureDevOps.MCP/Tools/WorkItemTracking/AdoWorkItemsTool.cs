namespace Tizzani.AzureDevOps.MCP.Tools.WorkItemTracking;

[McpServerToolType]
public static class AdoWorkItemsTool
{
    private const string ApiBaseAddress = "_apis/wit/workitems";
    private const string ApiVersion = "7.2-preview.3";
    
    [McpServerTool("getWorkItems")]
    [Description("Returns a list of work items. (Maximum 200)")]
    public static async Task<JsonElement> GetWorkItems(
        HttpClient httpClient,
        [Description("The work item IDs.")] int[] workItemIds,
        [Description("The fields to return in the results. If not included, all fields will be returned.")] string[]? fields = null,
        CancellationToken ct = default)
    {
        var ids = string.Join(",", workItemIds);
        var requestUri = $"{ApiBaseAddress}?ids={ids}&api-version={ApiVersion}";
        
        if (fields is { Length: > 0 })
            requestUri += $"&fields={string.Join(",", fields)}";
        
        return await httpClient.GetFromJsonAsync<JsonElement>(requestUri, ct);
    }
    
    [McpServerTool("getWorkItem")]
    [Description("Returns a single work item.")]
    public static async Task<JsonElement> GetWorkItem(
        HttpClient httpClient,
        [Description("The work item ID.")] int workItemId,
        [Description("The fields to return in the results. If not included, all fields will be returned.")] string[]? fields = null,
        CancellationToken ct = default)
    {
        var requestUri = $"{ApiBaseAddress}/{workItemId}?api-version={ApiVersion}";
        
        if (fields is { Length: > 0 })
            requestUri += $"&fields={string.Join(",", fields)}";
        
        return await httpClient.GetFromJsonAsync<JsonElement>(requestUri, ct);
    }
}