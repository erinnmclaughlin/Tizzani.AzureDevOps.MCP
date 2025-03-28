namespace Tizzani.AzureDevOps.MCP.Tools.Git;

[McpServerToolType]
public static class AdoRepositoriesTool
{
    private const string ApiVersion = "7.2-preview.1";
    
    [McpServerTool("listRepositories")]
    [Description("Retrieve all the git repositories in the project.")]
    public static async Task<JsonElement> ListRepositories(HttpClient httpClient, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<JsonElement>($"_apis/git/repositories?api-version={ApiVersion}", ct);
    }
}