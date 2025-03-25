using System.Text;
using System.Text.Json.Serialization;

namespace Tizzani.AzureDevOps.MCP.Tools.Git;

[McpToolType]
public static class AdoPullRequestsTool
{
    private const string ApiVersion = "7.2-preview.2";
    
    [McpTool("getPullRequests")]
    [Description("Retrieve all pull requests matching a specified criteria. Please note that description field will be truncated up to 400 symbols in the result.")]
    public static async Task<JsonElement> GetPullRequests(
        HttpClient httpClient,
        [Description("(Optional) The number of pull requests to ignore. For example, to retrieve results 101-150, set top to 50 and skip to 100.")] int? skip = null,
        [Description("(Optional) The number of pull requests to retrieve.")] int? top = null,
        [Description("(Optional) If set, search for pull requests that were created by this identity.")] string? creatorId = null,
        [Description("(Optional) Whether to include the _links field on the shallow references.")] bool? includeLinks = null,
        [Description("(Optional) If set, search for pull requests whose target branch is in this repository.")] string? repositoryId = null,
        [Description("(Optional) If set, search for pull requests that are in this state. Defaults to Active if unset.")] PullRequestStatus? pullRequestStatus = null,
        [Description("(Optional) If set, filters pull requests that contain the specified text in the title.")] string? title = null,
        CancellationToken ct = default)
    {
        var requestUri = new StringBuilder($"_apis/git/pullrequests?api-version={ApiVersion}")
            .AppendIfNotNull("&$skip={0}", skip)
            .AppendIfNotNull("&$top={0}", top)
            .AppendIfNotNull("&searchCriteria.creatorId={0}", creatorId)
            .AppendIfNotNull("&searchCriteria.includeLinks={0}", includeLinks)
            .AppendIfNotNull("&searchCriteria.repositoryId={0}", repositoryId)
            .AppendIfNotNull("&searchCriteria.status={0}", pullRequestStatus)
            .AppendIfNotNull("&searchCriteria.title={0}", title)
            .ToString();
        
        return await httpClient.GetFromJsonAsync<JsonElement>(requestUri, ct);
    }

    [McpTool("getPullRequestById")]
    public static async Task<JsonElement> GetPullRequestById(
        HttpClient httpClient,
        [Description("The ID of the pull request to retrieve.")]
        int pullRequestId,
        CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/git/pullrequests/{pullRequestId}?api-version={ApiVersion}", ct);
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PullRequestStatus
    {
        [Description("Pull request is abandoned.")]
        [JsonStringEnumMemberName("abandoned")]
        Abandoned,
        [Description("Pull request is active.")]
        [JsonStringEnumMemberName("active")]
        Active,
        [Description("Used in pull request search criteria to include all statuses.")]
        [JsonStringEnumMemberName("all")]
        All,
        [Description("Pull request is completed.")]
        [JsonStringEnumMemberName("completed")]
        Completed
    }
}