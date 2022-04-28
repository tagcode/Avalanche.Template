// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System;
using System.Text;

/// <summary>Parameterless template string</summary>
public class ParameterlessText : ITemplateBreakdown, ITemplateTextPart, ITemplatePrintable
{
    /// <summary>Text</summary>
    protected string? text;
    /// <summary>Create text</summary>
    public ParameterlessText(string? text)
    {
        this.text = text;
    }

    /// <summary></summary>
    public ITemplatePart[] Parts { get => new ITemplatePart[] { this }; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplatePlaceholderPart[] Placeholders { get => Array.Empty<ITemplatePlaceholderPart>(); set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateParameterPart?[] Parameters { get => Array.Empty<ITemplateParameterPart>(); set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Text { get => text!; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateFormat? TemplateFormat { get => ParameterlessTemplateFormat.Instance; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string?[] ParameterNames { get => Array.Empty<string>(); set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateBreakdown Breakdown { get => this; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateBreakdown? TemplateBreakdown { get => this; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    public ReadOnlyMemory<char> Escaped { get => text == null ? default : text.AsMemory(); set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ReadOnlyMemory<char> Unescaped { get => text == null ? default : text.AsMemory(); set => throw new InvalidOperationException(); }

    /// <summary>Print</summary>
    public string Print(IFormatProvider? formatProvider, object?[]? arguments = null) => Text ?? "";
    /// <summary></summary>
    public string Print(object?[]? arguments = null) => Text ?? "";
    /// <summary>Print to <paramref name="dst"/>.</summary>
    public bool TryPrintTo(Span<char> dst, out int length, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Check length
        if (text == null || dst.Length < text.Length) { length = 0; return false; }
        // Write
        Text.CopyTo(dst);
        length = Text.Length;
        return true;
    }
    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, object?[]? arguments = null)
    {
        // Check length
        if (text == null || dst.Length < text.Length) { length = 0; return false; }
        // Write
        Text.CopyTo(dst);
        length = Text.Length;
        return true;
    }
    /// <summary>Get length</summary>
    public bool TryEstimatePrintLength(out int length, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        length = text == null ? 0 : text.Length;
        return true;
    }
    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, object?[]? arguments = null)
    {
        length = text == null ? 0 : text.Length;
        return true;
    }
    /// <summary>Append to <paramref name="sb"/>.</summary>
    public void AppendTo(StringBuilder sb, IFormatProvider? formatProvider, object?[]? arguments = null) => sb.Append(Text);
    /// <summary></summary>
    public void AppendTo(StringBuilder sb, object?[]? arguments = null) => sb.Append(Text);
    /// <summary>Write to <paramref name="textWriter"/>.</summary>
    public void WriteTo(TextWriter textWriter, IFormatProvider? formatProvider, object?[]? arguments = null) => textWriter.Write(Text);
    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, object?[]? arguments = null) => textWriter.Write(Text);

    /// <summary>Print information</summary>
    public override string? ToString() => text;
}
