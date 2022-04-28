// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Text;
using Avalanche.Utilities;

/// <summary></summary>
public class TemplateBreakdownBase : ReadOnlyAssignableClass, ITemplateBreakdown
{
    /// <summary>Escaped format string "Welcome, \"{0}\". You received {1,15:N0} credit(s).</summary>
    protected string text = null!;
    /// <summary>Description of the format type</summary>
    protected ITemplateFormat? templateFormat = null!;
    /// <summary>Breakdown into sequence of text and placeholder parts.</summary>
    protected ITemplatePart[] parts = null!;
    /// <summary>Placeholders in order of occurance in <see cref="ITemplateText.Text"/>.</summary>
    protected ITemplatePlaceholderPart[] placeholders = null!;
    /// <summary>Parameters in order of <see cref="ITemplateParameterPart.ParameterIndex"/>.</summary>
    protected ITemplateParameterPart?[] parameters = null!;
    /// <summary>Parameters names in order of <see cref="ITemplateParameterPart.ParameterIndex"/>.</summary>
    protected string?[]? parameterNames;

    /// <summary>Escaped format string "Welcome, \"{0}\". You received {1,15:N0} credit(s).</summary>
    public virtual string Text { get => text; set => this.AssertWritable().text = value; }
    /// <summary>Template breakdown</summary>
    public virtual ITemplateBreakdown Breakdown { get => this; set => throw new InvalidOperationException(); }
    /// <summary>Description of the format type</summary>
    public virtual ITemplateFormat? TemplateFormat { get => templateFormat; set => this.AssertWritable().templateFormat = value; }
    /// <summary>Breakdown into sequence of text and placeholder parts.</summary>
    public virtual ITemplatePart[] Parts { get => parts; set => this.AssertWritable().parts = value; }
    /// <summary>Placeholders in order of occurance in <see cref="ITemplateText.Text"/>.</summary>
    public virtual ITemplatePlaceholderPart[] Placeholders { get => placeholders; set => this.AssertWritable().placeholders = value; }
    /// <summary>Placeholders in argument order.</summary>
    public virtual ITemplateParameterPart?[] Parameters { get => parameters; set => this.AssertWritable().parameters = value; }
    /// <summary>Parameter names</summary>
    public virtual string?[] ParameterNames { get => parameterNames!; set => this.AssertWritable().parameterNames = value; }

    /// <summary>Try print <paramref name="formatProvider"/> with <paramref name="arguments"/> and <paramref name="formatProvider"/>.</summary>
    public virtual string Print(IFormatProvider? formatProvider, object?[]? arguments = null) => TemplatePrintingExtensions.Print(this, formatProvider, arguments);
    /// <summary></summary>
    public virtual void AppendTo(StringBuilder sb, IFormatProvider? formatProvider, object?[]? arguments = null) => TemplatePrintingExtensions.AppendTo(this, sb, formatProvider, arguments);
    /// <summary></summary>
    public virtual void WriteTo(TextWriter textWriter, IFormatProvider? formatProvider, object?[]? arguments = null) => TemplatePrintingExtensions.WriteTo(this, textWriter, formatProvider, arguments);
    /// <summary></summary>
    public virtual bool TryPrintTo(Span<char> dst, out int length, IFormatProvider? formatProvider, object?[]? arguments = null) => TemplatePrintingExtensions.TryPrintTo(this, dst, out length, formatProvider, arguments);
    /// <summary></summary>
    public virtual bool TryEstimatePrintLength(out int length, IFormatProvider? formatProvider, object?[]? arguments = null) => TemplatePrintingExtensions.TryEstimatePrintLength(this, out length, formatProvider, arguments);

    /// <summary>Print text</summary>
    public override string? ToString() => Text;
}
