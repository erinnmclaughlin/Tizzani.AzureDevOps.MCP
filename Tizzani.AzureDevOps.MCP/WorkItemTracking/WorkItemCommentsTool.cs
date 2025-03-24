using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tizzani.AzureDevOps.MCP.WorkItemTracking;

[McpToolType]
public sealed class WorkItemCommentsTool
{
    private const string DefaultApiVersion = "7.2-preview.4";
    
    [McpTool("getComments")]
    [Description("Gets a list of comments for a specific work item.")]
    public static async Task<JsonElement> GetWorkItemComments(
        HttpClient httpClient,
        [Description("The work item ID to get comments for.")] int workItemId, 
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"{workItemId}/comments?api-version={DefaultApiVersion}", ct);
    }
    
    [McpTool("addComment")]
    [Description("Adds a comment to a specific work item.")]
    public static async Task<JsonElement> AddWorkItemComment(
        HttpClient httpClient,
        [Description("The ID of the work item to add a comment to.")] int workItemId, 
        [Description("The text of the comment to add.")] string text, 
        CancellationToken ct = default)
    {
        var requestUri = $"{workItemId}/comments?api-version={DefaultApiVersion}";
        
        var response = await httpClient.PostAsJsonAsync(requestUri, new { text }, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<JsonElement>(ct);
    }
}