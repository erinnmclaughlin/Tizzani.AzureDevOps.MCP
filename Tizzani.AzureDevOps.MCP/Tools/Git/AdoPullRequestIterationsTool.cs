namespace Tizzani.AzureDevOps.MCP.Tools.Git;

[McpToolType]
public static class AdoPullRequestIterationsTool
{
    private const string ApiVersion = "7.2-preview.2";
    
    [McpTool("getPullRequestIterations")]
    [Description("Gets a list of iterations for a specific pull request.")]
    public static async Task<JsonElement> GetPullRequestIterations(
        HttpClient httpClient,
        [Description("The ID or name of the repository.")] string repositoryId,
        [Description("The pull request ID to get iterations for.")] int pullRequestId,
        CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/iterations?api-version={ApiVersion}", ct);
        return await response.Content.ReadFromJsonAsync<JsonElement>(ct);
    }
    
    [McpTool("getPullRequestIteration")]
    [Description("Gets a specific iteration for a specific pull request.")]
    public static async Task<JsonElement> GetPullRequestIteration(
        HttpClient httpClient,
        [Description("The ID or name of the repository.")] string repositoryId,
        [Description("The pull request ID to get iterations for.")] int pullRequestId,
        [Description("The iteration ID to get.")] int iterationId,
        CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/iterations/{iterationId}?api-version={ApiVersion}", ct);
        return await response.Content.ReadFromJsonAsync<JsonElement>(ct);
    }
}