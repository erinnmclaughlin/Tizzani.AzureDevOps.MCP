using System.Text;
using System.Text.Json.Serialization;

namespace Tizzani.AzureDevOps.MCP.Tools.Git;

[McpServerToolType]
public static class AdoDiffsTool
{
    private const string ApiVersion = "7.2-preview.1";
    
    [McpServerTool("getDiffs")]
    [Description("Gets a list of diffs between two branches.")]
    public static async Task<JsonElement> GetBranchDiffs(
        HttpClient httpClient,
        [Description("The ID or name of the repository.")] string repositoryId,
        [Description("Version string identifier (name of tag/branch, SHA1 of commit)")] string baseVersion,
        [Description("Version type (branch, tag, or commit). Determines how Id is interpreted")] GitVersionType baseVersionType,
        [Description("Version string identifier (name of tag/branch, SHA1 of commit)")] string targetVersion,
        [Description("Version type (branch, tag, or commit). Determines how Id is interpreted")] GitVersionType targetVersionType,
        CancellationToken ct = default)
    {
        var requestUri = new StringBuilder($"_apis/git/repositories/{repositoryId}/diffs/commits?api-version={ApiVersion}")
            .Append($"&baseVersion={baseVersion}")
            .Append($"&baseVersionType={baseVersionType}")
            .Append($"&targetVersion={targetVersion}")
            .Append($"&targetVersionType={targetVersionType}")
            .ToString();
        
        var response = await httpClient.GetAsync(requestUri, ct);
        return await response.Content.ReadFromJsonAsync<JsonElement>(ct);
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GitVersionType
    {
        [Description("Interpret the version as a branch name")]
        [JsonStringEnumMemberName("branch")]
        Branch,
        [Description("Interpret the version as a commit ID (SHA1)")]
        [JsonStringEnumMemberName("commit")]
        Commit,
        [Description("Interpret the version as a tag name")]
        [JsonStringEnumMemberName("tag")]
        Tag
    }
}