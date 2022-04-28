// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System;
using Avalanche.Template.Internal;
using Avalanche.Utilities;
using Avalanche.Utilities.Internal;
using Avalanche.Tokenizer;

/// <summary>
/// String format that uses braces '{' and '}' for placeholders. '{' is escaped as "{{" and '}' as "}}".
/// 
/// Placeholder use "{parameter[,alignment][:formatting]}" format, e.g. "{parameterName,-10:X8}". 
/// Parameter name can contain number, alphabet and underscore.
/// </summary>
public class BraceTemplateFormat : TemplateFormatBase.TextIsBreakdown
{
    /// <summary>Singleton</summary>
    static BraceTemplateFormat alphaNumeric = new BraceTemplateFormat("BraceAlphaNumeric", BraceTemplateTokenizer.AlphaNumeric, Kind.AlphaNumeric);
    /// <summary>Singleton</summary>
    static BraceTemplateFormat numeric = new BraceTemplateFormat("BraceNumeric", BraceTemplateTokenizer.Numeric, Kind.Numeric);
    /// <summary>Singleton</summary>
    static BraceTemplateFormat auto = new BraceTemplateFormat("Brace", BraceTemplateTokenizer.AlphaNumeric, Kind.Auto);
    /// <summary>Singleton, accessable facade is in <see cref="TemplateFormat.BraceAlphaNumeric"/>.</summary>
    public static BraceTemplateFormat AlphaNumeric => alphaNumeric;
    /// <summary>Singleton, accessable facade is in <see cref="TemplateFormat.BraceNumeric"/>.</summary>
    public static BraceTemplateFormat Numeric => numeric;
    /// <summary>Singleton, accessable facade is in <see cref="TemplateFormat.Brace"/>.</summary>
    public static BraceTemplateFormat Auto => auto;

    /// <summary>Tokenizer</summary>
    protected ITokenizer<CompositeToken> tokenizer;

    /// <summary>Parameter kind</summary>
    protected Kind parameterKind;
    /// <summary>Parameter kind</summary>
    public Kind ParameterKind => parameterKind;

    /// <summary>Brace format kind</summary>
    public enum Kind
    {
        /// <summary>Auto detects each text</summary>
        Auto,
        /// <summary>Numeric placeholders "{0}"</summary>
        Numeric,
        /// <summary>Alphanumeric placeholders "{user}"</summary>
        AlphaNumeric,
    }

    /// <summary></summary>
    protected BraceTemplateFormat(string name, ITokenizer<CompositeToken> tokenizer, Kind parameterKind) : base(name)
    {
        this.tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        this.parameterKind = parameterKind;
        this.numberAssignedOrder = parameterKind == Kind.Numeric;
        this.escaper = BraceEscaper.Instance!;
    }

    /// <summary>Evaluate whether <paramref name="c"/> is parameter name.</summary>
    protected virtual bool IsParameterChar(char c) => char.IsLetterOrDigit(c) || c == '_';
    /// <summary>Evaluate whether <paramref name="c"/> is formatting .</summary>
    protected virtual bool IsFormattingChar(char c) => c != '{' && c != '}';

    /// <summary>Breakdown <paramref name="text"/> into format string.</summary>
    protected override bool TryBreakdown(string text, out ITemplateBreakdown result)
    {
        // Handle null
        if (text == null)
        {
            TemplateBreakdown formatString_ = new TemplateBreakdown();
            formatString_.TemplateFormat = this;
            result = formatString_;
            return true;
        }
        // Create format string
        TemplateBreakdown templateBreakdown = new TemplateBreakdown();
        templateBreakdown.Text = text;
        // Tokenize
        if (!tokenizer.TryTake(text, out CompositeToken formatStringToken)) { result = default!; return false; }
        // Place parts here
        templateBreakdown.Parts = new ITemplatePart[formatStringToken.Children.Length];
        StructList6<ITemplatePlaceholderPart> placeholders = new();
        // Convert tokens to parts
        for (int i = 0; i < formatStringToken.Children.Length; i++)
        {
            // Get token
            IToken token = formatStringToken.Children[i];
            // Place part here
            ITemplatePart part;
            // Convert token to part
            switch (token)
            {
                case TextToken textToken:
                    part = new TemplateTextPart().SetBreakdown(templateBreakdown).SetTexts(token.Memory, escaper!).SetReadOnly();
                    break;
                case PlaceholderToken placeholderToken:
                    var parameterPart = placeholderToken.Parameter == null ? null : new TemplateParameterPart().SetBreakdown(templateBreakdown).SetTexts(placeholderToken.Parameter.Memory);
                    var alignmentPart = placeholderToken.Alignment == null ? null : new TemplateAlignmentPart().SetBreakdown(templateBreakdown).SetTexts(placeholderToken.Alignment.Memory).SetReadOnly();
                    var formattingPart = placeholderToken.Formatting == null ? null : new TemplateFormattingPart().SetBreakdown(templateBreakdown).SetTexts(placeholderToken.Formatting.Memory).SetReadOnly();
                    var part2 = new TemplatePlaceholderPart
                    {
                        TemplateBreakdown = templateBreakdown,
                        Parameter = parameterPart!,
                        Alignment = alignmentPart,
                        Formatting = formattingPart,
                    }.SetTexts(token.Memory).SetReadOnly();
                    part = part2;
                    placeholders.Add(part2);
                    break;
                default:
                    part = new TemplateMalformedPart().SetBreakdown(templateBreakdown).SetTexts(token.Memory).SetReadOnly();
                    break;
            }
            //
            templateBreakdown.Parts[i] = part;
        }
        // Choose parameter kind
        Kind _kind = parameterKind == Kind.Auto ? DetectFormatKind(templateBreakdown.Parts) : parameterKind;
        //
        if (_kind == Kind.Numeric)
        {
            // Assign parameter indices
            foreach (ITemplatePart part in templateBreakdown.Parts)
            {
                // Cast
                if (part is not ITemplatePlaceholderPart placeholder) continue;
                // Try parse
                if (int.TryParse(placeholder.Parameter.Unescaped.Span, out int parameterIx)) placeholder.Parameter.ParameterIndex = parameterIx;
                // Set read-only
                ((IReadOnly)placeholder.Parameter).SetReadOnly();
            }
        }
        else
        {
            //
            StructList8<ReadOnlyMemory<char>> parameterNames = new(MemoryEqualityComparer<char>.Instance);
            // Assign parameter indices
            foreach (ITemplatePart part in templateBreakdown.Parts)
            {
                // Cast
                if (part is not ITemplatePlaceholderPart placeholder) continue;
                // Get parameter name
                ReadOnlyMemory<char> parameterName = placeholder.Parameter.Unescaped;
                // Search
                int parameterIx = parameterNames.IndexOf(parameterName);
                // Add new
                if (parameterIx < 0) { parameterIx = parameterNames.Count; parameterNames.Add(parameterName); }
                // Assign
                placeholder.Parameter.ParameterIndex = parameterIx;
                // Set read-only
                ((IReadOnly)placeholder.Parameter).SetReadOnly();
            }
        }
        // Assign template format
        if (this.ParameterKind == Kind.Auto && this == BraceTemplateFormat.Auto && _kind != Kind.Auto)
            templateBreakdown.TemplateFormat = _kind == Kind.Numeric ? BraceTemplateFormat.Numeric : BraceTemplateFormat.AlphaNumeric;
        else
            templateBreakdown.TemplateFormat = this;
        // Assign result
        templateBreakdown.SetReadOnly();
        result = templateBreakdown;
        return true;
    }

    /// <summary>
    /// Identify format kind.
    /// 
    /// Numeric format has positive integer numbers, and largest argument index is not larger than placeholder count + 8.
    /// </summary>
    /// <param name="parts">parts or placeholder parts</param>
    /// <returns>Either <see cref="Kind.Numeric"/> or <see cref="Kind.AlphaNumeric"/></returns>
    public static Kind DetectFormatKind(IEnumerable<ITemplatePart> parts)
    {
        // Count placeholders and largest argument index
        int placeholderCount = 0, highestArgIx = int.MinValue;
        //
        foreach (ITemplatePart part in parts)
        {
            // Cast
            if (part is not ITemplatePlaceholderPart placeholder) continue;
            // 
            placeholderCount++;
            // Get parameter
            var _parameter = placeholder.Parameter;
            // No parameter
            if (_parameter == null) return Kind.AlphaNumeric;
            // Try parse
            if (!int.TryParse(_parameter.Unescaped.Span, out int argumentIndex)) return Kind.AlphaNumeric;
            // Up highestArgIx
            if (argumentIndex > highestArgIx) highestArgIx = argumentIndex;
        }
        // No placeholders
        if (placeholderCount == 0) return Kind.Numeric;
        // Got negative argument index
        if (highestArgIx < 0) return Kind.AlphaNumeric;
        // Get argument count
        int argumentCount = highestArgIx + 1;
        // Too high argument value, e.g. "{9999}"
        if (argumentCount > placeholderCount + 8) return Kind.AlphaNumeric;
        // Ok must be numeric
        return Kind.Numeric;
    }

    /// <summary>Put format string together.</summary>
    protected override bool TryAssemble(ITemplateBreakdown templateBreakdown, out string result)
    {
        // Got null
        if (templateBreakdown == null) { result = null!; return false; }
        // Return original string
        if (templateBreakdown.TemplateFormat == this && templateBreakdown is BraceTemplateFormat braceTemplateFormat0 && braceTemplateFormat0.text != null) { result = templateBreakdown.Text; return true; }
        // Place length here
        int length = 0;
        // Target kind
        Kind dstKind = this.ParameterKind;
        //
        if (dstKind == Kind.Auto)
        {
            // Choose from template format
            if (templateBreakdown.TemplateFormat is BraceTemplateFormat braceTemplateFormat1) dstKind = braceTemplateFormat1.ParameterKind;
            // Auto detect
            if (dstKind == Kind.Auto) dstKind = DetectFormatKind(templateBreakdown.Parts);
        }
        // Calculate length
        foreach (ITemplatePart part in templateBreakdown.Parts)
        {
            // Text part
            if (part is ITemplateTextPart text) length += escaper!.EstimateEscapedLength(text.Unescaped.Span);
            // Placeholder part
            else if (part is ITemplatePlaceholderPart placeholder)
            {
                // '{' and '}'
                length += 2;
                // parameter
                if (placeholder.Parameter != null)
                {
                    if (dstKind == Kind.Numeric)
                    {
                        //
                        int parameterIx = placeholder.Parameter.ParameterIndex;
                        // Calculate digits in placeholder number '{0}'
                        length++;
                        for (int paIx = parameterIx / 10; paIx > 0; paIx /= 10) length++;
                    }
                    else
                    {
                        length += placeholder.Parameter.Length();
                    }
                }
                // alignment
                if (placeholder.Alignment != null)
                {
                    length += placeholder.Alignment.Length() + 1;
                }
                // formatting
                if (placeholder.Formatting != null)
                {
                    length += placeholder.Formatting.Length() + 1;
                }
            }
            else
            {
                // Append as is
                length += part.Length();
            }
        }
        // Allocate
        Span<char> buf = length < 256 ? stackalloc char[length] : new char[length];
        ReadOnlySpan<char> buf2 = buf;
        // Append parts
        foreach (ITemplatePart part in templateBreakdown.Parts)
        {
            if (part is ITemplateTextPart text)
            {
                // Re-escape
                buf = buf.Slice(escaper!.Escape(text.Unescaped.Span, buf));
            }
            else if (part is ITemplatePlaceholderPart placeholder)
            {
                // '{' and '}'
                buf[0] = '{'; buf = buf.Slice(1);
                // parameter
                if (placeholder.Parameter != null)
                {
                    if (dstKind == Kind.Numeric)
                    {
                        //
                        int paIx = placeholder.Parameter.ParameterIndex;
                        char digit = (char)('0' + (paIx % 10));
                        buf[0] = digit; buf = buf.Slice(1);
                        for (int phix = paIx / 10; phix > 0; phix /= 10)
                        {
                            digit = (char)('0' + (phix % 10));
                            buf[0] = digit; buf = buf.Slice(1);
                        }
                    }
                    else
                    {
                        placeholder.Parameter.Escaped.Span.CopyTo(buf); buf = buf.Slice(placeholder.Parameter.Escaped.Length);
                    }
                }
                // alignment
                if (placeholder.Alignment != null)
                {
                    // ','
                    buf[0] = ','; buf = buf.Slice(1);
                    placeholder.Alignment.Escaped.Span.CopyTo(buf);
                    buf = buf.Slice(placeholder.Alignment.Escaped.Length);
                }
                // formatting
                if (placeholder.Formatting != null)
                {
                    // ':'
                    buf[0] = ':'; buf = buf.Slice(1);
                    placeholder.Formatting.Escaped.Span.CopyTo(buf);
                    buf = buf.Slice(placeholder.Formatting.Escaped.Length);
                }
                // '}'
                buf[0] = '}'; buf = buf.Slice(1);
            }
            else
            {
                // Append as is
                part.Escaped.Span.CopyTo(buf);
                buf = buf.Slice(part.Escaped.Length);
            }
        }

        // Create string
        result = new string(buf2);
        // Ok
        return true;
    }
}
