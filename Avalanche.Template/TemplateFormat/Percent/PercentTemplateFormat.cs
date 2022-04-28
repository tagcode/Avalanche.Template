// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System;
using Avalanche.Template.Internal;
using Avalanche.Utilities;
using Avalanche.Utilities.Internal;
using Avalanche.Tokenizer;

/// <summary>String format that uses '%1' as placeholders.</summary>
public class PercentTemplateFormat : TemplateFormatBase.TextIsBreakdown
{
    /// <summary>Singleton</summary>
    static PercentTemplateFormat instance = new PercentTemplateFormat("Percent", PercentTemplateTokenizer.Instance);
    /// <summary>Singleton</summary>
    public static PercentTemplateFormat Instance => instance;

    /// <summary>Tokenizer</summary>
    protected ITokenizer<CompositeToken> tokenizer;

    /// <summary></summary>
    protected PercentTemplateFormat(string name, ITokenizer<CompositeToken> tokenizer) : base(name)
    {
        this.tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        this.numberAssignedOrder = true;
        this.escaper = PercentEscaper.Instance;
    }

    /// <summary>Evaluate whether <paramref name="c"/> is parameter name.</summary>
    protected virtual bool IsParameterChar(char c) => char.IsDigit(c);
    /// <summary>Evaluate whether <paramref name="c"/> is formatting .</summary>
    protected virtual bool IsFormattingChar(char c) => c != '%';

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
        if (!tokenizer.TryTake(text, out CompositeToken compositeToken)) { result = default!; return false; }
        // Place parts here
        templateBreakdown.Parts = new ITemplatePart[compositeToken.Children.Length];
        StructList6<ITemplatePlaceholderPart> placeholders = new();
        // Convert tokens to parts
        for (int i = 0; i < compositeToken.Children.Length; i++)
        {
            // Get token
            IToken token = compositeToken.Children[i];
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
                    }.SetEscaped(token.Memory).SetReadOnly();
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
        // Assign parameter indices
        foreach (ITemplatePart part in templateBreakdown.Parts)
        {
            // Cast
            if (part is not ITemplatePlaceholderPart placeholder) continue;
            // Try parse
            if (int.TryParse(placeholder.Parameter.Unescaped.Span, out int parameterIx)) placeholder.Parameter.ParameterIndex = parameterIx - 1;
            // Set read-only
            ((IReadOnly)placeholder.Parameter).SetReadOnly();
        }
        // Assign template format
        templateBreakdown.TemplateFormat = this;
        // Assign result
        templateBreakdown.SetReadOnly();
        result = templateBreakdown;
        return true;
    }

    /// <summary>Put format string together.</summary>
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
            // Estimate re-escaped length
            if (part is ITemplateTextPart text) length += escaper!.EstimateEscapedLength(text.Unescaped);
            // Placeholder part
            else if (part is ITemplatePlaceholderPart placeholder)
            {
                // '%'
                length += 1;
                // parameter
                if (placeholder.Parameter != null)
                {
                    //
                    int parameterIx = placeholder.Parameter.ParameterIndex + 1;
                    // Calculate digits in placeholder number '{0}'
                    length++;
                    for (int paIx = parameterIx / 10; paIx > 0; paIx /= 10) length++;
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
            // Placeholder part
            else if (part is ITemplatePlaceholderPart placeholder)
            {
                // '%'
                buf[0] = '%'; buf = buf.Slice(1);
                // parameter
                if (placeholder.Parameter != null)
                {
                    //
                    int paIx = placeholder.Parameter.ParameterIndex + 1;
                    char digit = (char)('0' + (paIx % 10));
                    buf[0] = digit; buf = buf.Slice(1);
                    for (int phix = paIx / 10; phix > 0; phix /= 10)
                    {
                        digit = (char)('0' + (phix % 10));
                        buf[0] = digit; buf = buf.Slice(1);
                    }
                }
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
