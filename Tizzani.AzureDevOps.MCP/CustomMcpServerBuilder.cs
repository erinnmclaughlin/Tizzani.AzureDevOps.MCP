using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Types;
using System.Reflection;
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
        return new Tool
        {
            Name = m.GetCustomAttribute<McpToolAttribute>()?.Name ?? m.Name,
            Description = m.GetCustomAttribute<DescriptionAttribute>()?.Description,
            InputSchema = JsonSerializer.SerializeToElement(new
            {
                type = "object",
                // ugly but very temporary:
                properties = m.GetParameters()
                    .Where(p => p.ParameterType != typeof(CancellationToken) && sp.GetService(p.ParameterType) == null)
                    .ToDictionary(p => p.Name!, p =>
                    {
                        var node = JsonSerializerOptions.Default.GetJsonSchemaAsNode(p.ParameterType);
                        string? type;

                        if (node.GetValueKind() == JsonValueKind.String)
                        {
                            type = node.GetValue<string>();
                        }
                        else if (node.GetValueKind() == JsonValueKind.Object)
                        {
                            var typeObj = node.AsObject().TryGetPropertyValue("type", out var t) ? t : null;

                            type = typeObj?.GetValueKind() == JsonValueKind.Array
                                ? typeObj.AsArray().First()?.GetValue<string>()
                                : typeObj?.GetValue<string>();
                        }
                        else
                        {
                            type = node.GetValue<string>();
                        }
                        
                        return new
                        {
                            type,
                            description = p.GetCustomAttribute<DescriptionAttribute>()?.Description
                        };
                    }),
                required = m.GetParameters()
                    .Where(p => p.ParameterType != typeof(CancellationToken) && sp.GetService(p.ParameterType) == null)
                    .Where(p => !p.HasDefaultValue)
                    .Select(p => p.Name)
                    .ToArray()
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
}
