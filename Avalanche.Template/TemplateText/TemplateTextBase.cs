// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Text;
using Avalanche.Utilities;

/// <summary>Template text</summary>
public class TemplateTextBase : ReadOnlyAssignableClass, ITemplateText
{
    /// <summary>String in its escaped format, e.g. "Welcome, {{{0}}}.\nYou received {1,15:N0} credit(s).</summary>
    protected string? text;
    /// <summary>Description of the format type</summary>
    protected ITemplateFormat? templateFormat;
    /// <summary>Parameter names</summary>
    protected string?[]? parameterNames;
    /// <summary>Template breakdown</summary>
    protected ITemplateBreakdown? breakdown;

    /// <summary>String in its escaped format, e.g. "Welcome, {{{0}}}.\nYou received {1,15:N0} credit(s).</summary>
    public virtual string Text { get => text!; set => this.AssertWritable().text = value; }
    /// <summary>Description of the format type</summary>
    public virtual ITemplateFormat? TemplateFormat { get => templateFormat; set => this.AssertWritable().templateFormat = value; }
    /// <summary>Parameter names</summary>
    public virtual string?[] ParameterNames { get => parameterNames!; set => this.AssertWritable().parameterNames = value; }
    /// <summary>Template breakdown</summary>
    public virtual ITemplateBreakdown Breakdown { get => breakdown!; set => this.AssertWritable().breakdown = value; }

    /// <summary>Create template text</summary>
    public TemplateTextBase() : base() { }

    /// <summary>Print <paramref name="arguments"/> on placeholders using <paramref name="formatProvider"/>.</summary>
    public virtual string Print(IFormatProvider? formatProvider, object?[]? arguments) => Breakdown?.Print(formatProvider, arguments) ?? "";
    /// <summary>Append to <paramref name="sb"/>.</summary>
    public virtual void AppendTo(StringBuilder sb, IFormatProvider? formatProvider, object?[]? arguments) => TemplatePrintingExtensions.AppendTo(Breakdown, sb, formatProvider, arguments);
    /// <summary>Write to <paramref name="textWriter"/>.</summary>
    public virtual void WriteTo(TextWriter textWriter, IFormatProvider? formatProvider, object?[]? arguments) => TemplatePrintingExtensions.WriteTo(Breakdown, textWriter, formatProvider, arguments);
    /// <summary></summary>
    public virtual bool TryPrintTo(Span<char> dst, out int length, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // Get breakdown
        ITemplateBreakdown? _breakdown = Breakdown;
        // No breakdown
        if (_breakdown == null) { length = 0; return false; }
        // Try print with breakdown
        return _breakdown.TryPrintTo(dst, out length, formatProvider, arguments);
    }
    /// <summary></summary>
    public virtual bool TryEstimatePrintLength(out int length, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // Get breakdown
        ITemplateBreakdown? _breakdown = Breakdown;
        // No breakdown
        if (_breakdown == null) { length = 0; return false; }
        // Try print with breakdown
        return _breakdown.TryEstimatePrintLength(out length, formatProvider, arguments);
    }

    /// <summary>Print template text</summary>
    public override string? ToString() => text;
}
