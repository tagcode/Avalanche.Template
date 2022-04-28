// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Extension methods for <see cref="ITemplateBreakdown"/>.</summary>
public static class TemplateArgumentExtractExtensions
{
    /// <summary>Create regular expression pattern of <paramref name="templateBreakdown"/>.</summary>
    /// <param name="templateBreakdown"></param>
    /// <returns></returns>
    public static (Regex, IDictionary<string, string>) CreatePattern(ITemplateBreakdown templateBreakdown)
    {
        // Create parameterName to groupName mapping
        IDictionary<string, string> parameterNameToGroupNameMap = new Dictionary<string, string>();
        // Create pattern string
        string patternString = "^"+CreatePatternString(templateBreakdown, parameterNameToGroupNameMap) +"$";
        // Compile pattern
        Regex pattern = new Regex(patternString, RegexOptions.Compiled|RegexOptions.Singleline);
        // Return pattern
        return (pattern, parameterNameToGroupNameMap);
    }

    /// <summary>Convert <paramref name="parameterName"/> into group name.</summary>
    /// <param name="parameterName"></param>
    /// <param name="parameterNameToGroupNameMap">Table of already used group names</param>
    /// <returns></returns>
    public static string ConvertParameterNameToGroupName(string parameterName, IDictionary<string, string> parameterNameToGroupNameMap)
    {
        //
        StructList20<char> outputChars = new();
        ReadOnlySpan<char> inputChars = parameterName.AsSpan();
        //
        for (int i = 0; i < inputChars.Length; i++)
        {
            char ch = inputChars[i];
            if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z')) outputChars.Add(ch);
        }
        //
        if (outputChars.Count == 1 && outputChars[0] == '0') outputChars.Insert(0, '_');
        //
        string str = null!;
        // 
        while (parameterNameToGroupNameMap.Values.Contains(str = new string(outputChars.ToArray()))) outputChars.Add('_');
        //
        parameterNameToGroupNameMap[parameterName] = str;
        //
        return str;
    }

    /// <summary>Create regular expression pattern of <paramref name="templateBreakdown"/>.</summary>
    /// <param name="templateBreakdown"></param>
    /// <returns></returns>
    public static string CreatePatternString(ITemplateBreakdown templateBreakdown, IDictionary<string, string>? parameterNameToGroupNameMap = null, IDictionary<string, object?>? args = null)
    {
        // 
        StringBuilder sb = new();
        if (parameterNameToGroupNameMap == null) parameterNameToGroupNameMap = new Dictionary<string, string>();
        //
        foreach (ITemplatePart part in templateBreakdown.Parts)
        {
            // Convert placeholder into capture group
            if (part is ITemplatePlaceholderPart placeholderPart)
            {
                // Add value
                if (args != null && args.TryGetValue(placeholderPart.Parameter.Unescaped.AsString(), out object? value) && value != null)
                {
                    sb.Append(value);
                }
                // Add pattern
                else
                {
                    // Get parameter name
                    string parameterName = placeholderPart.Parameter.Unescaped.AsString();
                    // Convert to group name
                    if (!parameterNameToGroupNameMap.TryGetValue(parameterName, out string? groupName)) parameterNameToGroupNameMap[parameterName] = groupName = ConvertParameterNameToGroupName(parameterName, parameterNameToGroupNameMap);
                    // Open capture group
                    sb.Append("(?<");
                    sb.Append(groupName);
                    sb.Append(">.*)");
                }
            }
            // Other part
            else
            {
                // Append part
                sb.Append(Regex.Escape(part.Unescaped.AsString()));
            }
        }
        // 
        string pattern = sb.ToString();
        // Return 
        return pattern;
    }

    /// <summary>Pattern provider</summary>
    static readonly IProvider<ITemplateBreakdown, (Regex, IDictionary<string, string>)> patternProvider = Providers.Func<ITemplateBreakdown, (Regex, IDictionary<string, string>)>(CreatePattern);
    /// <summary>Pattern provider</summary>
    static readonly IProvider<ITemplateBreakdown, (Regex, IDictionary<string, string>)> patternProviderCached = patternProvider.Cached<ITemplateBreakdown, (Regex, IDictionary<string, string>)>();
    /// <summary>Pattern provider</summary>
    public static IProvider<ITemplateBreakdown, (Regex, IDictionary<string, string>)> PatternProvider => patternProvider;
    /// <summary>Pattern provider with weak keyed cache</summary>
    public static IProvider<ITemplateBreakdown, (Regex, IDictionary<string, string>)> PatternProviderCached => patternProviderCached;

    /// <summary>Try extract arguments from printed <paramref name="input"/> string.</summary>
    /// <exception cref="InvalidOperationException">If arguments could not be extracted.</exception>
    public static ReadOnlyMemory<char>[] ExtractArguments(this ITemplateBreakdown templateBreakdown, string input)
    {
        // Extract arguments
        if (TryExtractArguments(templateBreakdown, input, out ReadOnlyMemory<char>[] arguments)) return arguments;
        // Throw
        throw new InvalidOperationException($"Could not extract arguments of '{input}'.");
    }
    /// <summary>Try extract arguments from printed <paramref name="input"/> string.</summary>
    /// <exception cref="InvalidOperationException">If arguments could not be extracted.</exception>
    public static ReadOnlyMemory<char>[] ExtractArguments(this ITemplateBreakdown templateBreakdown, ReadOnlyMemory<char> input)
    {
        // Extract arguments
        if (TryExtractArguments(templateBreakdown, input, out ReadOnlyMemory<char>[] arguments)) return arguments;
        // Throw
        throw new InvalidOperationException($"Could not extract arguments of '{input}'.");
    }

    /// <summary>Try extract arguments from printed <paramref name="input"/> string.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static bool TryExtractArguments(this ITemplateBreakdown templateBreakdown, string input, out ReadOnlyMemory<char>[] arguments) => TryExtractArguments(templateBreakdown, input.AsMemory(), out arguments);
    /// <summary>Try extract arguments from printed <paramref name="input"/> string.</summary>
    public static bool TryExtractArguments(this ITemplateBreakdown templateBreakdown, ReadOnlyMemory<char> input, out ReadOnlyMemory<char>[] arguments)
    {
        // Create pattern
        (Regex pattern, IDictionary<string, string> parameterNameToGroupNameMap) = PatternProviderCached[templateBreakdown];
        //
        (string text, int start, int length) = input.GetString();
        // Match
        Match match = pattern.Match(text, start, length);
        // No match
        if (!match.Success) { arguments = null!; return false; }
        // Argument list
        StructList8<ReadOnlyMemory<char>> argumentList = new();
        // Iterate groups (each correspond to placeholder)
        for (int i=0; i<templateBreakdown.Placeholders.Length; i++)
        {
            // Get parameter part
            ITemplateParameterPart? parameterPart = templateBreakdown.Placeholders[i]?.Parameter;
            // No parameter part, index unknown
            if (parameterPart == null) continue;
            // Get parameter name
            string parameterName = parameterPart.Unescaped.AsString();
            // Get group name
            if (!parameterNameToGroupNameMap.TryGetValue(parameterName, out string? groupName)) continue;
            // Get group
            Group group = match.Groups[groupName];
            // Get argument
            ReadOnlyMemory<char> argument = input.Slice(group.Index, group.Length);
            // Get index
            int parameterIndex = parameterPart.ParameterIndex;
            // Error with index
            if (parameterIndex < 0 || parameterIndex > templateBreakdown.ParameterNames.Length+8) { arguments = null!; return false; }
            // Pad list
            for (int j = 0; j < parameterIndex; j++) argumentList.Add(null!);
            // Add to list
            if (argumentList.Count == parameterIndex) argumentList.Add(argument); else argumentList[parameterIndex] = argument;
        }
        // Return
        arguments = argumentList.ToArray();
        return true;
    }

    /// <summary>Try extract arguments from printed <paramref name="input"/> string.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static bool TryExtractArgumentsTo(this ITemplateBreakdown templateBreakdown, string input, Memory<ReadOnlyMemory<char>> arguments)  => TryExtractArgumentsTo(templateBreakdown, input.AsMemory(), arguments);
    /// <summary>Try extract arguments from <paramref name="input"/> into caller allocated <paramref name="arguments"/>.</summary>
    public static bool TryExtractArgumentsTo(this ITemplateBreakdown templateBreakdown, ReadOnlyMemory<char> input, Memory<ReadOnlyMemory<char>> arguments)
    {
        // Create pattern
        (Regex pattern, IDictionary<string, string> parameterNameToGroupNameMap) = PatternProviderCached[templateBreakdown];
        //
        (string text, int start, int length) = input.GetString();
        // Match
        Match match = pattern.Match(text, start, length);
        // No match
        if (!match.Success) return false;
        // Argument span
        Span<ReadOnlyMemory<char>> argumentSpan = arguments.Span;
        // Argument list
        for (int i = 0; i < templateBreakdown.Placeholders.Length; i++) argumentSpan[i] = default;
        // Iterate groups (each correspond to placeholder)
        for (int i=0; i<templateBreakdown.Placeholders.Length; i++)
        {
            // Get parameter part
            ITemplateParameterPart? parameterPart = templateBreakdown.Placeholders[i]?.Parameter;
            // No parameter part, index unknown
            if (parameterPart == null) continue;
            // Get parameter name
            string parameterName = parameterPart.Unescaped.AsString();
            // Get group name
            if (!parameterNameToGroupNameMap.TryGetValue(parameterName, out string? groupName)) continue;
            // Get group
            Group group = match.Groups[groupName];
            // Get argument
            ReadOnlyMemory<char> argument = input.Slice(group.Index, group.Length);
            // Get index
            int parameterIndex = parameterPart.ParameterIndex;
            // Error with index
            if (parameterIndex < 0 || parameterIndex > arguments.Length) return false;
            // Add to list
            argumentSpan[parameterIndex] = argument;
        }
        // Return
        return true;
    }
}
