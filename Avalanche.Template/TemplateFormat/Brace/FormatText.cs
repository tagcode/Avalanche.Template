// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalanche.Template.Internal;
using Avalanche.Utilities;
using Avalanche.Tokenizer;

/// <summary>Represents template text that uses numeric brace placeholders, e.g. "{0}" and uses <see cref="String.Format(string, object?[])"/> for printing.</summary>
public class FormatText : ITemplateBreakdown
{
    /// <summary>String in its escaped format, e.g. "Welcome, {{{0}}}.\nYou received {1,15:N0} credit(s).</summary>
    protected string templateText;
    /// <summary>Template breakdown</summary>
    protected ITemplateBreakdown breakdown = null!;
    /// <summary>Cached parameter names</summary>
    protected string?[]? parameterNames;

    /// <summary>String in its escaped format, e.g. "Welcome, {{{0}}}.\nYou received {1,15:N0} credit(s).</summary>
    public virtual string Text { get => templateText; set => throw new InvalidOperationException(); }
    /// <summary>Template breakdown</summary>
    public virtual ITemplateBreakdown Breakdown { get => breakdown ?? (breakdown = Avalanche.Template.TemplateFormat.BraceNumeric.Breakdown[templateText]); set => throw new InvalidOperationException(); }
    /// <summary>Template format</summary>
    public ITemplateFormat? TemplateFormat { get => Avalanche.Template.TemplateFormat.BraceNumeric; set => throw new InvalidOperationException(); }
    /// <summary>Parameter names</summary>
    public string?[] ParameterNames { get => parameterNames ?? (parameterNames = createParametersNames(ExtractPlaceholders(templateText))!); set => throw new InvalidOperationException(); }

    /// <summary>Breakdown into sequence of text and placeholder parts.</summary>
    ITemplatePart[] ITemplateBreakdown.Parts { get => Breakdown.Parts; set => throw new InvalidOperationException(); }
    /// <summary>Placeholders in order of occurance in <see cref="ITemplateText.Text"/>.</summary>
    ITemplatePlaceholderPart[] ITemplateBreakdown.Placeholders { get => Breakdown.Placeholders; set => throw new InvalidOperationException(); }
    /// <summary>Parameters in argument order.</summary>
    ITemplateParameterPart?[] ITemplateBreakdown.Parameters { get => Breakdown.Parameters; set => throw new InvalidOperationException(); }

    /// <summary>Create format string</summary>
    public FormatText(string templateText)
    {
        this.templateText = templateText;
    }

    /// <summary>Print template text</summary>
    public override string? ToString() => Text;

    /// <summary>Extract unique placeholders from <paramref name="templateText"/>.</summary>
    /// <param name="templateText">Brace with numeric parameter names "{0}"</param>
    /// <returns>placeholders by parameter name</returns>
    protected static string[]? ExtractPlaceholders(string? templateText)
    {
        // No text
        if (templateText == null) return null;
        // Tokenize
        if (!BraceTemplateTokenizer.Numeric.TryTake(templateText.AsMemory(), out CompositeToken textToken)) return Array.Empty<string>();
        // Place here parameters
        StructList8<string> _placeholders = new();
        // 
        foreach (IToken token in textToken.Children)
        {
            // Cast
            if (token is not PlaceholderToken placeholderToken) continue;
            // Get parameter name
            string parameterName = placeholderToken.Parameter == null ? "" : placeholderToken.Parameter.Memory.AsString();
            // Add to list
            _placeholders.Add(parameterName);
        }
        // Return array of paramter names
        return _placeholders.ToArray();
    }

    /// <summary>Create parameters names.</summary>
    protected static string[]? createParametersNames(string[]? placeholders)
    {
        // No placeholders
        if (placeholders == null) return null;
        // Index of last found argument
        int highestParameterIx = int.MinValue;
        // Find highest parameter index
        foreach (string placeholder in placeholders)
        {
            // Parse 
            int parameterIx = int.TryParse(placeholder, out int _ix) ? _ix : 0;
            // Up highest index
            if (parameterIx > highestParameterIx) highestParameterIx = parameterIx;
        }
        // No arguments
        if (highestParameterIx == int.MinValue) return Array.Empty<string>();
        // Parameter count
        int parameterCount = highestParameterIx + 1;
        // Fallback to occurance order: Too many parameters in relation to placeholders
        if (parameterCount < 0 || parameterCount > placeholders.Length + 8) return placeholders;
        // Index order
        else
        {
            // Allocate
            string[] parameters = new string[parameterCount];
            // Assign each
            foreach (string placeholder in placeholders) 
            {
                // Parse 
                int parameterIx = int.TryParse(placeholder, out int _ix) ? _ix : 0;
                // Assign
                parameters[parameterIx] = placeholder;
            }
            // Return
            return parameters;
        }
    }

    /// <summary>Try print <paramref name="arguments"/> on placeholders using <paramref name="formatProvider"/>.</summary>
    public virtual string Print(IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Null template text
        if (this.templateText == null) return "";
        // Formulate
        string printOut = arguments == null || arguments.Length == 0 ? string.Format(formatProvider, this.templateText) : string.Format(formatProvider, this.templateText, arguments);
        // Return
        return printOut;
    }
    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, IFormatProvider? formatProvider, object?[]? arguments) => TemplatePrintingExtensions.TryPrintTo(Breakdown, dst, out length, formatProvider, arguments);
    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, IFormatProvider? formatProvider, object?[]? arguments = null) => TemplatePrintingExtensions.TryEstimatePrintLength(Breakdown, out length, formatProvider, arguments);
    /// <summary></summary>
    public void AppendTo(StringBuilder sb, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // No arguments array
        if (arguments == null) sb.AppendFormat(provider: formatProvider, Text);
        // Got arguments array
        else sb.AppendFormat(provider: formatProvider, Text, arguments);
    }
    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, IFormatProvider? formatProvider, object?[]? arguments = null) => Breakdown.WriteTo(textWriter, formatProvider, arguments);
}
