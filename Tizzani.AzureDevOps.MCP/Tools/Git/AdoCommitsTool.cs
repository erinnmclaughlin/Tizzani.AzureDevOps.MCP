namespace Tizzani.AzureDevOps.MCP.Tools.Git;

[McpServerToolType]
public static class AdoCommitsTool
{
    private const string ApiVersion = "7.2-preview.1";
    
    [McpServerTool("getCommit")]
    [Description("Gets a specific commit.")]
    public static async Task<JsonElement> GetCommit(
        HttpClient httpClient,
        [Description("The ID or name of the repository.")] string repositoryId,
        [Description("The commit ID to get.")] string commitId,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/git/repositories/{repositoryId}/commits/{commitId}?api-version={ApiVersion}", ct);
    }
    
    [McpServerTool("getCommitChanges")]
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