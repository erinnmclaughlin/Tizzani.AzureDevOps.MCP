using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tizzani.AzureDevOps.MCP;

// this is only necessary because [McpTool] can only be used on static methods (for now)

public static class McpServices
{
    private const string DefaultApiVersion = "7.2-preview.4";
    
    public static async Task<CallToolResponse> ConfigureCallToolHandler(RequestContext<CallToolRequestParams> request, CancellationToken ct)
    {
        return request.Params?.Name switch
        {
            null => throw new McpServerException("Missing required parameter 'name'"),
            "addComment" => await CallAddCommentHandler(request, ct),
            "getComments" => await CallGetCommentsTool(request, ct),
            _ => throw new McpServerException($"Unknown tool: {request.Params.Name}")
        };
    }
    
    public static Task<ListToolsResult> ConfigureListToolsHandler(RequestContext<ListToolsRequestParams> _, CancellationToken ct)
    {
        return Task.FromResult(new ListToolsResult
        {
            Tools =
            [
                AddCommentTool,
                GetCommentsTool
            ]
        });
    }
    
    public static Tool AddCommentTool => new()
    {
        Name = "addComment",
        Description = "Adds a comment to a specific work item.",
        InputSchema = JsonSerializer.SerializeToElement(new
        {
            type = "object",
            properties = new {
                workItemId = new
                {
                    type = "integer",
                    description = "The ID of the work item to add a comment to."
                },
                text = new
                {
                    type = "string",
                    description = "The text of the comment to add."
                }
            },
            required = new[] { "workItemId", "text" }
        })
    };
    
    public static Tool GetCommentsTool => new()
    {
        Name = "getComments",
        Description = "Gets a list of comments for a specific work item.",
        InputSchema = JsonSerializer.SerializeToElement(new
        {
            type = "object",
            properties = new
            {
                workItemId = new
                {
                    type = "integer",
                    description = "The ID of the work item to get comments for."
                }
            },
            required = new[] { "workItemId" }
        })
    };
    
    public static async Task<CallToolResponse> CallAddCommentHandler(RequestContext<CallToolRequestParams> request, CancellationToken ct)
    {
        if (request.Params?.Arguments is null || !request.Params.Arguments.TryGetValue("workItemId", out var maybeWorkItemId))
        {
            throw new McpServerException("Missing required argument 'workItemId'");
        }
        
        if (!int.TryParse(maybeWorkItemId.ToString(), out var workItemId))
        {
            throw new McpServerException("Argument 'workItemId' must be an integer.");
        }
        
        if (!request.Params.Arguments.TryGetValue("text", out var maybeComment))
        {
            throw new McpServerException("Missing required argument 'text'");
        }

        if (maybeComment.ToString() is not { Length: > 0 } text)
        {
            throw new McpServerException("Argument 'text' cannot be empty.");
        }
        
        using var scope = request.Server.ServiceProvider!.CreateScope();
        var commentClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
        
        var requestUri = $"{workItemId}/comments?api-version={DefaultApiVersion}";
        
        var response = await commentClient.PostAsJsonAsync(requestUri, new { text }, ct);
        response.EnsureSuccessStatusCode();

        var content = new Content
        {
            Text = await response.Content.ReadAsStringAsync(ct),
            Type = "text"
        };
        
        return new CallToolResponse
        {
            Content = [content]
        };
    }
    
    public static async Task<CallToolResponse> CallGetCommentsTool(RequestContext<CallToolRequestParams> request, CancellationToken ct)
    {
        if (request.Params?.Arguments is null || !request.Params.Arguments.TryGetValue("workItemId", out var maybeWorkItemId))
        {
            throw new McpServerException("Missing required argument 'workItemId'");
        }

        if (!int.TryParse(maybeWorkItemId.ToString(), out var workItemId))
        {
            throw new McpServerException("Argument 'workItemId' must be an integer.");
        }
        
        using var scope = request.Server.ServiceProvider!.CreateScope();
        var commentClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
        
        var content = new Content
        {
            Text = await commentClient.GetStringAsync($"{workItemId}/comments?api-version={DefaultApiVersion}", ct),
            Type = "text"
        };
        
        return new CallToolResponse
        {
            Content = [content]
        };
    }
}
