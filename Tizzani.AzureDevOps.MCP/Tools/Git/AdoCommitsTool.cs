namespace Tizzani.AzureDevOps.MCP.Tools.Git;

[McpToolType]
public static class AdoCommitsTool
{
    private const string ApiVersion = "7.2-preview.1";
    
    [McpTool("getCommit")]
    [Description("Gets a specific commit.")]
    public static async Task<JsonElement> GetCommit(
        HttpClient httpClient,
        [Description("The ID or name of the repository.")] string repositoryId,
        [Description("The commit ID to get.")] string commitId,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/git/repositories/{repositoryId}/commits/{commitId}?api-version={ApiVersion}", ct);
    }
    
    [McpTool("getCommitChanges")]
    [Description("Retrieve changes for a particular commit.")]
    public static async Task<JsonElement> GetCommitChanges(
        HttpClient httpClient,
        [Description("The ID or name of the repository.")] string repositoryId,
        [Description("The commit ID to get.")] string commitId,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/git/repositories/{repositoryId}/commits/{commitId}/changes?api-version={ApiVersion}", ct);
    }
}