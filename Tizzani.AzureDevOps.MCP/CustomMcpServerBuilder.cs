using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Schema;

namespace Tizzani.AzureDevOps.MCP;

// temporary until this is built into the MCP library

public static class CustomMcpServerBuilder
{
    public static IMcpServerBuilder AddMcpServerWithTools(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddMcpServer(o =>
        {
            o.ServerInfo = new Implementation { Name = "AzureDevOpsMCP", Version = "1.0.0" };
            o.Capabilities = new ServerCapabilities
            {
                Tools = AddMcpTools()
            };
        });
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
                
                var options = JsonSerializerOptions.Default;

                return Task.FromResult(new ListToolsResult
                {
                    Tools = methods.Select(m => new Tool
                    {
                        Name = m.GetCustomAttribute<McpToolAttribute>()?.Name ?? m.Name,
                        Description = m.GetCustomAttribute<DescriptionAttribute>()?.Description,
                        InputSchema = JsonSerializer.SerializeToElement(new
                        {
                            type = "object",
                            properties = m.GetParameters()
                                .Where(p => p.ParameterType != typeof(CancellationToken) && sp.GetService(p.ParameterType) == null)
                                .ToDictionary(p => p.Name!, p => new
                                {
                                    type = options.GetJsonSchemaAsNode(p.ParameterType),
                                    description = p.GetCustomAttribute<DescriptionAttribute>()?.Description
                                }),
                            required = m.GetParameters()
                                .Where(p => p.ParameterType != typeof(CancellationToken) && sp.GetService(p.ParameterType) == null)
                                .Where(p => !p.HasDefaultValue)
                                .Select(p => p.Name)
                                .ToArray()
                        })
                    })
                    .ToList()
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
