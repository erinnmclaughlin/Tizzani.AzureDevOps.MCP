using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Types;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace Tizzani.AzureDevOps.MCP;

public static class CustomMcpServerBuilder
{
    public static IMcpServerBuilder AddMcpServerWithTools(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddMcpServer(o =>
        {
            o.ServerInfo = new Implementation { Name = "tizzani-adomcp", Version = "1.0.0" };
            o.Capabilities = new ServerCapabilities
            {
                Tools = AddMcpTools()
            };
        });
    }

    // temporary until this is built into the MCP library:
    public static Tool BuildTool(MethodInfo m, IServiceProvider sp)
    {
        var properties = new Dictionary<string, object?>();
        var required = new HashSet<string>();
        
        var parameters = m.GetParameters()
            .Where(p => p.ParameterType != typeof(CancellationToken) && sp.GetService(p.ParameterType) == null);

        foreach (var p in parameters)
        {
            if (string.IsNullOrWhiteSpace(p.Name))
                throw new Exception("All MCP tool method parameters must have a name.");
            
            var propName = p.Name.ToCamelCase();
            var description = p.GetCustomAttribute<DescriptionAttribute>()?.Description;
                
            properties[propName] = JsonSerializerOptions.Web.GetJsonSchemaAsNode(p.ParameterType, new JsonSchemaExporterOptions
            {
                TreatNullObliviousAsNonNullable = true,
                TransformSchemaNode = (context, schema) => TransformSchemaNode(context, schema, description)
            });

            if (!p.HasDefaultValue)
            {
                required.Add(propName);
            }
        }
        
        return new Tool
        {
            Name = m.GetCustomAttribute<McpToolAttribute>()?.Name ?? m.Name,
            Description = m.GetCustomAttribute<DescriptionAttribute>()?.Description,
            InputSchema = JsonSerializer.SerializeToElement(new
            {
                type = "object",
                // ugly but very temporary:
                properties,
                required
            })
        };
    }

    public static ToolsCapability AddMcpTools()
    {
        var methods = typeof(CustomMcpServerBuilder).Assembly
            .GetTypes()
            .Where(x => x.GetCustomAttribute<McpToolTypeAttribute>() != null)
            .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static));

        return new ToolsCapability
        {
            ListToolsHandler = (request, _) =>
            {
                using var scope = request.Server.ServiceProvider!.CreateScope();
                var sp = scope.ServiceProvider;

                return Task.FromResult(new ListToolsResult
                {
                    Tools = methods.Select(m => BuildTool(m, sp)).ToList()
                });
            },
            CallToolHandler = async (request, ct) =>
            {
                var toolName = request.Params?.Name!;

                var method = methods.First(x => x.Name == toolName || x.GetCustomAttribute<McpToolAttribute>()?.Name == toolName);
                var parameters = new List<object?>();

                using var scope = request.Server.ServiceProvider!.CreateScope();

                foreach (var parameterInfo in method.GetParameters())
                {
                    if (parameterInfo.ParameterType == typeof(CancellationToken))
                    {
                        parameters.Add(ct);
                        continue;
                    }

                    var parameter = scope.ServiceProvider.GetService(parameterInfo.ParameterType);

                    if (parameter == null)
                    {
                        var argument = request.Params?.Arguments?.GetValueOrDefault(parameterInfo.Name!);
                        var argumentAsJson = JsonSerializer.SerializeToElement(argument);
                        parameter = argumentAsJson.Deserialize(parameterInfo.ParameterType)!;
                    }

                    if (parameter == null)
                    {
                        if (parameterInfo.HasDefaultValue)
                        {
                            parameters.Add(parameterInfo.DefaultValue);
                            continue;
                        }
                        
                        throw new Exception($"Unable to resolve argument '{parameterInfo.Name}' of type {parameterInfo.ParameterType.Name}");
                    }

                    parameters.Add(parameter);
                }

                dynamic task = method.Invoke(null, parameters.ToArray())!;
                var result = await task;

                return new CallToolResponse { Content = [new Content { Text = result?.ToString(), Type = "text" }] };
            }
        };
    }

    private static JsonNode TransformSchemaNode(JsonSchemaExporterContext _, JsonNode schema, string? description)
    {
        if (schema is not JsonObject jObj)
        {
            // Handle the case where the schema is a Boolean.
            var valueKind = schema.GetValueKind();
            Debug.Assert(valueKind is JsonValueKind.True or JsonValueKind.False);
            schema = jObj = new JsonObject();
            if (valueKind is JsonValueKind.False)
            {
                jObj.Add("not", true);
            }
        }

        if (description != null)
        {
            foreach (var (_, childNode) in jObj)
            {
                if (childNode is null || childNode.GetValueKind() != JsonValueKind.Object)
                    continue;
                                    
                var childObj = childNode.AsObject();

                if (childObj.TryGetPropertyValue("description", out var d))
                {
                    if (d?.GetValue<string>() == description)
                    {
                        childObj.Remove("description");
                    }
                }
            }
                                
            jObj.Add("description", description);
        }

        return schema;
    }
}
