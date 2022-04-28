// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System;
using Avalanche.Template.Internal;
using Avalanche.Tokenizer;
using Avalanche.Utilities;

/// <summary>String format that uses dash placeholders "#parameter#". </summary>
public class DashTemplateFormat : TemplateFormatBase.TextIsBreakdown
{
    /// <summary>Singleton</summary>
    static DashTemplateFormat instance = new DashTemplateFormat();
    /// <summary>Singleton</summary>
    public static DashTemplateFormat Instance => instance;

    /// <summary>'#'</summary>
    protected static readonly ConstantTokenizer dashTokenizer = new ConstantTokenizer("#");
    /// <summary>'#name#'</summary>
    protected static readonly PlaceholderTokenizer placeholderTokenizer = new PlaceholderTokenizer(dashTokenizer, CharTokenizer.AlphaNumeric, IntegerTokenizer.Instance, new UntilTokenizer(dashTokenizer, false, null), dashTokenizer);
    /// <summary>Singleton</summary>
    static TemplateTextTokenizer tokenizer = new TemplateTextTokenizer(new UntilTokenizer(dashTokenizer, false, null), placeholderTokenizer, MalformedTokenizer.Instance);
    /// <summary>Singleton</summary>
    public static TemplateTextTokenizer Tokenizer => tokenizer;

    /// <summary></summary>
    public DashTemplateFormat() : base("Dash") 
    {
        numberAssignedOrder = false;
    }

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
                    part = new TemplateTextPart().SetBreakdown(templateBreakdown).SetTexts(token.Memory).SetReadOnly();
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
                    part = new TemplateMalformedPart { TemplateBreakdown = templateBreakdown }.SetTexts(token.Memory).SetReadOnly();
                    break;
            }
            //
            templateBreakdown.Parts[i] = part;
        }
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
        if (templateBreakdown.TemplateFormat == this && templateBreakdown.Text != null) { result = templateBreakdown.Text; return true; }
        // Place length here
        int length = 0;
        // Calculate length
        foreach (ITemplatePart part in templateBreakdown.Parts)
        {
            if (part is ITemplateTextPart text)
            {
                // Estimate re-escaped length
                length += text.Length();
            }
            else if (part is ITemplatePlaceholderPart placeholder)
            {
                // '#' and '#'
                length += 2;
                // parameter
                if (placeholder.Parameter != null)
                {
                    length += placeholder.Parameter.Length();
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
                // Use as is
                text.Escaped.Span.CopyTo(buf); buf = buf.Slice(text.Escaped.Length);
            }
            else if (part is ITemplatePlaceholderPart placeholder)
            {
                // '#' and '#'
                buf[0] = '#'; buf = buf.Slice(1);
                // parameter
                if (placeholder.Parameter != null)
                {
                    placeholder.Parameter.Escaped.Span.CopyTo(buf); buf = buf.Slice(placeholder.Parameter.Escaped.Length);
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
                // '#'
                buf[0] = '#'; buf = buf.Slice(1);
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
