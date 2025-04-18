namespace Tizzani.AzureDevOps.MCP.Tools.WorkItemTracking;

[McpServerToolType]
public static class AdoWorkItemTypeCategoriesTool
{
    private const string ApiBaseAddress = "_apis/wit/workitemtypecategories";
    private const string ApiVersion = "7.2-preview.2";
    
    [McpServerTool(Name = "getWorkItemTypeCategory")]
    [Description("Gets a specific work item type category by name.")]
    public static async Task<JsonElement> GetWorkItemTypeCategory(
        HttpClient httpClient, 
        [Description("The category name.")] string categoryName,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"{ApiBaseAddress}/{categoryName}?api-version={ApiVersion}", ct);
    }
    
    [McpServerTool(Name = "getWorkItemTypeCategories")]
    [Description("Get all the work item type categories for the project.")]
    public static async Task<JsonElement> GetWorkItemTypeCategories(HttpClient httpClient, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"{ApiBaseAddress}?api-version={ApiVersion}", ct);
    }
}