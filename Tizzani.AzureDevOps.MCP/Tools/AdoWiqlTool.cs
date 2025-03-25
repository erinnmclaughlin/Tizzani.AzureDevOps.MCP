using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tizzani.AzureDevOps.MCP.Tools;

[McpToolType]
public sealed class AdoWiqlTool
{
    private const string ApiVersion = "7.2-preview.2";

    [McpTool("queryByWiql")]
    [Description("Executes a WIQL query against the Azure DevOps API.")]
    public static async Task<JsonElement> ExecuteWiqlQuery(
        HttpClient httpClient,
        [Description("The text of the WIQL query to execute.")] string query,
        CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync($"_apis/wit/wiql?api-version={ApiVersion}", new { query }, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<JsonElement>(ct);
    }
}