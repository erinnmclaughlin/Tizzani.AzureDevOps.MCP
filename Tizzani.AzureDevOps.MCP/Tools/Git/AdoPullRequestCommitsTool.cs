namespace Tizzani.AzureDevOps.MCP.Tools.Git;

[McpToolType]
public static class AdoPullRequestCommitsTool
{
    private const string ApiVersion = "7.2-preview.1";
    
    [McpTool("getPullRequestCommits")]
    [Description("Gets a list of commits for a specific pull request.")]
    public static async Task<JsonElement> GetPullRequestCommits(
        HttpClient httpClient,
        [Description("The ID or name of the repository.")] string repositoryId,
        [Description("The pull request ID to get commits for.")] int pullRequestId,
        CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/commits?api-version={ApiVersion}", ct);
        return await response.Content.ReadFromJsonAsync<JsonElement>(ct);
    }
    
    [McpTool("getPullRequestCommit")]
    [Description("Gets a list of commits for a specific pull request.")]
    public static async Task<JsonElement> GetPullRequestCommit(
        HttpClient httpClient,
        [Description("The ID or name of the repository.")] string repositoryId,
        [Description("The pull request ID to get commits for.")] int pullRequestId,
        CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/commits?api-version={ApiVersion}", ct);
        return await response.Content.ReadFromJsonAsync<JsonElement>(ct);
    }
}