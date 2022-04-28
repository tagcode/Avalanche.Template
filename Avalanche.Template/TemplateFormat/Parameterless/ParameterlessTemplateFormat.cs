// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System;

/// <summary>Parameterless unescaped string template foramt</summary>
public class ParameterlessTemplateFormat : TemplateFormatBase.TextIsBreakdown
{
    /// <summary>Singleton</summary>
    static ParameterlessTemplateFormat instance = new ParameterlessTemplateFormat();
    /// <summary>Singleton, accessable facade is in <see cref="TemplateFormat.Parameterless"/>.</summary>
    public static ParameterlessTemplateFormat Instance => instance;

    /// <summary></summary>
    public ParameterlessTemplateFormat() : base("Parameterless") { }

    /// <summary>Breakdown <paramref name="text"/> without parameters</summary>
    protected override bool TryBreakdown(string text, out ITemplateBreakdown result)
    {
        result = new ParameterlessText(text);
        return true;
    }

    /// <summary>Put together parts using their unescaped texts</summary>
    protected override bool TryAssemble(ITemplateBreakdown templateBreakdown, out string result)
    {
        // Got null
        if (templateBreakdown == null) { result = null!; return false; }
        // Return original string
        if (templateBreakdown.TemplateFormat == this && templateBreakdown.Text!=null) { result = templateBreakdown.Text; return true; }
        // Place length here
        int length = 0;
        // Calculate length
        foreach (ITemplatePart part in templateBreakdown.Parts)
        {
            // Append length
            length += part.Unescaped.Length;
        }
        // Allocate
        Span<char> buf = length < 256 ? stackalloc char[length] : new char[length];
        ReadOnlySpan<char> buf2 = buf;
        // Append parts
        foreach (ITemplatePart part in templateBreakdown.Parts)
        {
            // Copy
            part.Unescaped.Span.CopyTo(buf);
            buf = buf.Slice(part.Unescaped.Length);
        }
        // Create string
        result = new string(buf2);
        // Ok
        return true;
    }
}
