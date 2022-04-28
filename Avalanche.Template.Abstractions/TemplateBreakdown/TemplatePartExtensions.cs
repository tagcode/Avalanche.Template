// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="ITemplatePart"/>.</summary>
public static class TemplatePartExtensions
{
    /// <summary>Start index in <see cref="ITemplateText.Text"/>, including opening enclosement '{'.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int Index(this ITemplatePart part)
    {
        // Get index
        if (MemoryMarshal.TryGetString(part.Escaped, out string? text, out int index, out int length) && text != null) return index;
        // No string
        return 0;
    }
    /// <summary>Length of the escaped character sequence.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int Length(this ITemplatePart part) => part.Escaped.Length;

    /// <summary>Filter and cast malformed parts</summary>
    public static IEnumerable<ITemplateMalformedPart> MalformedParts(this IEnumerable<ITemplatePart> parts)
    {
        // Iterate each
        foreach(var part in parts)
        {
            // Case
            if (part is not ITemplateMalformedPart malformed) continue;
            // Yield
            yield return malformed;
        }
    }

    /// <summary>Get malformed parts</summary>
    public static IEnumerable<(int index, int length, ReadOnlyMemory<char>? malformed)> MalformedParts(this ITemplateBreakdown templateBreakdown)
    {
        // Iterate each
        foreach(var part in templateBreakdown.Parts)
        {
            // Case
            if (part is not ITemplateMalformedPart malformed) continue;
            // No correspondence to string
            if (!MemoryMarshal.TryGetString(part.Escaped, out string? text, out int index, out int length) || text == null) continue;
            // Yield info
            yield return (index, length, part.Escaped);
        }
    }

    /// <summary>Set escaped <paramref name="escapedText"/>.</summary>
    public static T SetEscaped<T>(this T part, ReadOnlyMemory<char> escapedText) where T : ITemplatePart
    {
        // Assign text
        part.Escaped = escapedText;
        // Return part
        return part;
    }

    /// <summary>Set escaped <paramref name="unescapedText"/>.</summary>
    public static T SetEscaped<T>(this T part, string unescapedText) where T : ITemplatePart
    {
        // Assign text
        part.Escaped = unescapedText.AsMemory();
        // Return part
        return part;
    }

    /// <summary>Set unescaped <paramref name="unescapedText"/>.</summary>
    public static T SetUnescaped<T>(this T part, ReadOnlyMemory<char> unescapedText) where T : ITemplatePart
    {
        // Assign text
        part.Unescaped = unescapedText;
        // Return part
        return part;
    }

    /// <summary>Set unescaped <paramref name="unescapedText"/>.</summary>
    public static T SetUnescaped<T>(this T part, string unescapedText) where T : ITemplatePart
    {
        // Assign text
        part.Unescaped = unescapedText.AsMemory();
        // Return part
        return part;
    }

    /// <summary>Set escaped and unescaped from <paramref name="text"/>.</summary>
    public static T SetTexts<T>(this T part, ReadOnlyMemory<char> text) where T : ITemplatePart
    {
        // Assign text
        part.Escaped = text;
        part.Unescaped = text;
        // Return part
        return part;
    }

    /// <summary>Set escaped and unescaped from <paramref name="text"/>.</summary>
    public static T SetTexts<T>(this T part, string text) where T : ITemplatePart
    {
        // Assign text
        part.Escaped = text.AsMemory();
        part.Unescaped = text.AsMemory();
        // Return part
        return part;
    }

    /// <summary>Set escaped and unescaped <paramref name="escapedText"/>.</summary>
    public static T SetTexts<T>(this T part, ReadOnlyMemory<char> escapedText, IEscaper escaper) where T : ITemplatePart
    {
        // Assign text
        part.Escaped = escapedText;
        part.Unescaped = escaper.Unescape2(escapedText);
        // Return part
        return part;
    }

    /// <summary>Set escaped and unescaped <paramref name="escapedText"/>.</summary>
    public static T SetTexts<T>(this T part, string escapedText, IEscaper escaper) where T : ITemplatePart
    {
        // Assign text
        part.Escaped = escapedText.AsMemory();
        part.Unescaped = escaper.Unescape2(escapedText.AsMemory());
        // Return part
        return part;
    }

    /// <summary>Set escaped and unescaped <paramref name="escapedText"/>.</summary>
    public static T SetTexts<T>(this T part, ReadOnlyMemory<char> escapedText, ReadOnlyMemory<char> unescapedText) where T : ITemplatePart
    {
        // Assign text
        part.Escaped = escapedText;
        part.Unescaped = unescapedText;
        // Return part
        return part;
    }

    /// <summary>Set escaped and unescaped <paramref name="escapedText"/>.</summary>
    public static T SetTexts<T>(this T part, string escapedText, string unescapedText) where T : ITemplatePart
    {
        // Assign text
        part.Escaped = escapedText.AsMemory();
        part.Unescaped = unescapedText.AsMemory();
        // Return part
        return part;
    }

    /// <summary></summary>
    public static T SetBreakdown<T>(this T part, ITemplateBreakdown templateBreakdown) where T : ITemplatePart
    {
        // Assign breakdown
        part.TemplateBreakdown = templateBreakdown;
        // Return part
        return part;
    }

}
