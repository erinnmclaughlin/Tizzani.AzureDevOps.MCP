namespace Tizzani.AzureDevOps.MCP.Tools.Git;

[McpToolType]
public static class AdoPullRequestThreadsTool
{
    private const string ApiVersion = "7.2-preview.1";
    
    [McpTool("getPullRequestThreads")]
    [Description("Retrieve all threads for a specified pull request.")]
    public static async Task<JsonElement> GetPullRequestThreads(
        HttpClient httpClient,
        [Description("The ID of the repository to retrieve threads for.")]
        string repositoryId,
        [Description("The ID of the pull request to retrieve threads for.")]
        int pullRequestId,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/threads?api-version={ApiVersion}", ct);
    }
    
    [McpTool("createPullRequestThread")]
    [Description("Creates a new thread on the pull request.")]
    public static async Task<JsonElement> CreatePullRequestThread(
        HttpClient httpClient,
        [Description("The ID or name of the repository to create a thread on.")]
        string repositoryId,
        [Description("The ID of the pull request to create a thread on.")]
        int pullRequestId,
        [Description("The comment content.")]
        string content,
        CancellationToken ct = default)
    {
        var requestUri = $"_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/threads?api-version={ApiVersion}";

        var requestBody = new
        {
            comments = new object[] { new { content } },
            status = "active"
        };
        
        var response = await httpClient.PostAsJsonAsync(requestUri, requestBody, ct);
        return await response.Content.ReadFromJsonAsync<JsonElement>(ct);
    }
    
    [McpTool("createPullRequestThreadForFile")]
    [Description("Creates a new comment on the pull request for a specific file.")]
    public static async Task<JsonElement> CreatePullRequestThreadForFile(
        HttpClient httpClient,
        [Description("The ID or name of the repository to create a thread on.")]
        string repositoryId,
        [Description("The ID of the pull request to create a thread on.")]
        int pullRequestId,
        [Description("The comment content.")]
        string content,
        [Description("The path of the file to comment on.")]
        string filePath,
        [Description("Position of last character of the thread's span in left file.")]
        int? leftFileEndLine = null,
        [Description("Position of last character of the thread's span in left file.")]
        int? leftFileEndOffset = null,
        [Description("Position of first character of the thread's span in left file.")]
        int? leftFileStartLine = null,
        [Description("Position of first character of the thread's span in left file.")]
        int? leftFileStartOffset = null,
        [Description("Position of last character of the thread's span in right file.")]
        int? rightFileEndLine = null,
        [Description("Position of last character of the thread's span in right file.")]
        int? rightFileEndOffset = null,
        [Description("Position of first character of the thread's span in right file.")]
        int? rightFileStartLine = null,
        [Description("Position of first character of the thread's span in right file.")]
        int? rightFileStartOffset = null,
        CancellationToken ct = default)
    {
        var requestUri = $"_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/threads?api-version={ApiVersion}";

        var requestBody = new
        {
            comments = new object[] { new { content } },
            status = "active",
            threadContext = new
            {
                filePath,
                leftFileEnd = !(leftFileEndLine.HasValue && leftFileEndOffset.HasValue) ? null : new
                {
                    line = leftFileEndLine.Value,
                    offset = leftFileEndOffset.Value
                },
                leftFileStart = !(leftFileStartLine.HasValue && leftFileStartOffset.HasValue) ? null : new
                {
                    line = leftFileStartLine.Value,
                    offset = leftFileStartOffset.Value
                },
                rightFileEnd = !(rightFileEndLine.HasValue && rightFileEndOffset.HasValue) ? null : new
                {
                    line = rightFileEndLine.Value,
                    offset = rightFileEndOffset.Value
                },
                rightFileStart = !(rightFileStartLine.HasValue && rightFileStartOffset.HasValue) ? null : new
                {
                    line = rightFileStartLine.Value,
                    offset = rightFileStartOffset.Value
                }
            }
        };
        
        var response = await httpClient.PostAsJsonAsync(requestUri, requestBody, ct);
        return await response.Content.ReadFromJsonAsync<JsonElement>(ct);
    }
    
}