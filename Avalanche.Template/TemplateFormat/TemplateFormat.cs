// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>String formatting description</summary>
public class TemplateFormat : ReadOnlyAssignableClass, ITemplateFormat
{
    /// <summary>Parameterless unescaped string format.</summary>
    public static ITemplateFormat Parameterless => ParameterlessTemplateFormat.Instance;

    /// <summary>
    /// String format that uses braces '{' and '}' for placeholders. '{' is escaped as "{{" and '}' as "}}".
    /// 
    /// Parameter name is detected whether is numeric or alphanumeric.
    /// 
    /// Placeholder use "{parameter[,alignment][:formatting]}" format, e.g. "{parameterName,-10:X8}". 
    /// </summary>
    public static ITemplateFormat Brace => BraceTemplateFormat.Auto;

    /// <summary>
    /// String format that uses braces '{' and '}' for placeholders. '{' is escaped as "{{" and '}' as "}}".
    /// 
    /// Placeholder use "{parameter[,alignment][:formatting]}" format, e.g. "{parameterName,-10:X8}". 
    /// Parameter name can contain number, alphabet and underscore.
    /// 
    /// This is the format used by Microsoft.Extensions.Logging.
    /// </summary>
    public static ITemplateFormat BraceAlphaNumeric => BraceTemplateFormat.AlphaNumeric;

    /// <summary>
    /// String format that uses braces '{' and '}' for placeholders. '{' is escaped as "{{" and '}' as "}}".
    /// 
    /// Placeholder use "{parameter[,alignment][:formatting]}" format, e.g. "{parameterName,-10:X8}". 
    /// Parameter name can contain number.
    /// 
    /// This is the format used by string.Format.
    /// </summary>
    public static ITemplateFormat BraceNumeric => BraceTemplateFormat.Numeric;

    /// <summary>String format that uses '%1' as placeholders.</summary>
    public static ITemplateFormat Percent => PercentTemplateFormat.Instance;

    /// <summary>Format name</summary>
    protected string name = null!;
    /// <summary>Provides text of format string</summary>
    protected IProvider<string, ITemplateText> text = null!;
    /// <summary>Provides and caches text of format string using weak-keyed reference cache.</summary>
    protected IProvider<string, ITemplateText> textCached = null!;
    /// <summary>Provides breakdown of format string</summary>
    protected IProvider<string, ITemplateBreakdown> breakdown = null!;
    /// <summary>Provides and caches breakdown of format string using weak-keyed reference cache.</summary>
    protected IProvider<string, ITemplateBreakdown> breakdownCached = null!;
    /// <summary>Assemble format string into string</summary>
    protected IProvider<ITemplateBreakdown, string> assemble = null!;
    /// <summary>Assemble format string into string using weak-keyed reference cache.</summary>
    protected IProvider<ITemplateBreakdown, string> assembleCached = null!;
    /// <summary>Print function</summary>
    protected Func<ITemplateBreakdown, IFormatProvider?, object?[]?, string>? printFunc;
    /// <summary>Policy whether format uses numeric parameters and assumes parameter order by numeric value.</summary>
    protected bool numberAssignedOrder;
    /// <summary>Text escaper</summary>
    protected IEscaper? escaper;

    /// <summary>Format name</summary>
    public string Name { get => name; set => this.AssertWritable().name = value; }
    /// <summary>Provides text of format string</summary>
    public IProvider<string, ITemplateText> Text { get => text; set => this.AssertWritable().text = value; }
    /// <summary>Provides and caches text of format string using weak-keyed reference cache.</summary>
    public IProvider<string, ITemplateText> TextCached { get => textCached; set => this.AssertWritable().textCached = value; }
    /// <summary>Provides breakdown of format string</summary>
    public IProvider<string, ITemplateBreakdown> Breakdown { get => breakdown; set => this.AssertWritable().breakdown = value; }
    /// <summary>Provides and caches breakdown of format string using weak-keyed reference cache.</summary>
    public IProvider<string, ITemplateBreakdown> BreakdownCached { get => breakdownCached; set => this.AssertWritable().breakdownCached = value; }
    /// <summary>Assemble format string into string</summary>
    public IProvider<ITemplateBreakdown, string> Assemble { get => assemble; set => this.AssertWritable().assemble = value; }
    /// <summary>Assemble format string into string using weak-keyed reference cache.</summary>
    public IProvider<ITemplateBreakdown, string> AssembleCached { get => assembleCached; set => this.AssertWritable().assembleCached = value; }
    /// <summary>Policy whether format uses numeric parameters and assumes parameter order by numeric value.</summary>
    public bool NumberAssignedOrder { get => numberAssignedOrder; set => this.AssertWritable().numberAssignedOrder = value; }
    /// <summary>Text escaper</summary>
    public IEscaper? Escaper { get => escaper; set => this.AssertWritable().escaper = value; }
    /// <summary>Print function</summary>
    public Func<ITemplateBreakdown, IFormatProvider?, object?[]?, string>? PrintFunc { get => printFunc; set => this.AssertWritable().printFunc = value; }

    /// <summary>Try print <paramref name="templateBreakdown"/> with <paramref name="arguments"/> and <paramref name="formatProvider"/>.</summary>
    public bool TryPrint(ITemplateBreakdown templateBreakdown, IFormatProvider? formatProvider, object?[]? arguments, out string text)
    {
        // No print func
        if (printFunc == null) { text = null!; return false; }
        // Print
        text = printFunc(templateBreakdown, formatProvider, arguments);
        return true;
    }

    /// <summary></summary>
    public override string ToString() => name ?? this.GetType().Name;
}
