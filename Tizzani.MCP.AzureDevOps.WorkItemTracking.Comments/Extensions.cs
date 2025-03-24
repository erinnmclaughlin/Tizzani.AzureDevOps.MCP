using Microsoft.Extensions.Configuration;
using System.Text;

namespace Tizzani.MCP.AzureDevOps.WorkItemTracking.Comments;

internal static class Extensions
{
    public static string ToBase64EncodedString(this string text)
    {
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(text));
    }

    public static string GetRequiredConfigurationValue(this IConfiguration config, string key)
    {
        if (config[key] is not { Length: > 0 } value) 
            throw new ArgumentException($"Missing required parameter '{key}'.");

        return value;
    }
}