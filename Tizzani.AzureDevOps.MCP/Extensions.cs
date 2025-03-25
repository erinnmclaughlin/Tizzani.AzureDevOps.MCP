using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Tizzani.AzureDevOps.MCP;

internal static class Extensions
{
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input) || char.IsLower(input[0]))
            return input;

        var span = input.AsSpan();
        
        return string.Create(span.Length, span, (chars, state) =>
        {
            chars[0] = char.ToLowerInvariant(state[0]);
            state[1..].CopyTo(chars[1..]);
        });
    }
    
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

    public static StringBuilder AppendIfNotNull<T>(this StringBuilder builder, [StringSyntax("CompositeFormat")] string format, T? value)
    {
        // hack: treat empty strings as null for this use case
        if (value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
            return builder;
        
        return value is not null ? builder.Append(string.Format(format, value)) : builder;
    }
}