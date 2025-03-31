namespace Tizzani.AzureDevOps.MCP.Tools.Git;

[McpServerToolType]
public static class AdoBlobsTool
{
    private const string ApiVersion = "7.2-preview.1";
    
    [McpServerTool(Name = "getBlob")]
    [Description("Gets the file contents of a specific blob.")]
    public static async Task<string> GetBlob(
        HttpClient httpClient,
        [Description("The ID or name of the repository.")] string repositoryId,
        [Description("SHA1 hash of the file. You can get the SHA1 of a file using the \"Git/Items/Get Item\" endpoint.")] string sha1,
        CancellationToken ct = default)
    {
        await using var stream = await httpClient.GetStreamAsync($"_apis/git/repositories/{repositoryId}/blobs/{sha1}?api-version={ApiVersion}&$format=octetstream", ct);
        var sr = new StreamReader(stream);
        return await sr.ReadToEndAsync(ct);
    }
}