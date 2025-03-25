namespace Tizzani.AzureDevOps.MCP.Tools.WorkItemTracking;

[McpToolType]
public static class AdoCommentsTool
{
    private const string ApiBaseAddress = "_apis/wit/workItems";
    private const string ApiVersion = "7.2-preview.4";
    
    [McpTool("getComments")]
    [Description("Gets a list of comments for a specific work item.")]
    public static async Task<JsonElement> GetWorkItemComments(
        HttpClient httpClient,
        [Description("The work item ID to get comments for.")] int workItemId, 
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"{ApiBaseAddress}/{workItemId}/comments?api-version={ApiVersion}", ct);
    }
    
    [McpTool("addComment")]
    [Description("Adds a comment to a specific work item.")]
    public static async Task<JsonElement> AddWorkItemComment(
        HttpClient httpClient,
        [Description("The ID of the work item to add a comment to.")] int workItemId, 
        [Description("The text of the comment to add.")] string text, 
        CancellationToken ct = default)
    {
        var requestUri = $"{ApiBaseAddress}/{workItemId}/comments?api-version={ApiVersion}";
        
        var response = await httpClient.PostAsJsonAsync(requestUri, new { text }, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<JsonElement>(ct);
    }
}