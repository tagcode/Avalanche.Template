// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="ITemplateText"/>.</summary>
public static class TemplateTextExtensions_
{
    /// <summary>Create writable clone. Clone can be set immutable with <see cref="IReadOnly"/> interface.</summary>
    public static ITemplateBreakdown Clone(this ITemplateText templateText)
    {
        // Get breakdown
        ITemplateBreakdown sourceText = templateText is ITemplateBreakdown templateBreakdown0 ? templateBreakdown0 : templateText.Breakdown;
        // Put here part copies
        StructList10 <ITemplatePart> parts = new();
        StructList10<ITemplatePlaceholderPart> placeholderParts = new();
        StructList10<ITemplateParameterPart> parametersParts = new();
        // 
        TemplateBreakdownBase copy = new TemplateBreakdownBase
        {
            TemplateFormat = sourceText.TemplateFormat,
            ParameterNames = sourceText.ParameterNames,
            Text = sourceText.Text,
        };
        // Visit parts
        foreach (ITemplatePart? part in sourceText.Parts)
        {
            // No part
            if (part == null) continue;
            // Clone placeholder part
            if (part is ITemplatePlaceholderPart placeholderPart)
            {
                // Create copy
                ITemplatePlaceholderPart partCopy = new TemplatePlaceholderPart { Escaped = placeholderPart.Escaped, Unescaped = placeholderPart.Unescaped, TemplateBreakdown = copy };
                // Assign format
                if (placeholderPart.Formatting != null)
                {
                    // 
                    var partSource = placeholderPart.Formatting;
                    // 
                    ITemplateFormattingPart formattingCopy = new TemplateFormattingPart { Escaped = partSource.Escaped, Unescaped = partSource.Unescaped, TemplateBreakdown = copy };
                    //
                    partCopy.Formatting = formattingCopy;
                }
                // Assign alignment
                if (placeholderPart.Alignment != null)
                {
                    // 
                    var partSource = placeholderPart.Alignment;
                    // 
                    ITemplateAlignmentPart alignmentCopy = new TemplateAlignmentPart { Escaped = partSource.Escaped, Unescaped = partSource.Unescaped, Alignment = partSource.Alignment, TemplateBreakdown = copy };
                    //
                    partCopy.Alignment = alignmentCopy;
                }
                // Assign parameter
                if (placeholderPart.Parameter != null)
                {
                    // 
                    var partSource = placeholderPart.Parameter;
                    // 
                    ITemplateParameterPart parameterCopy = new TemplateParameterPart { Escaped = partSource.Escaped, Unescaped = partSource.Unescaped, ParameterIndex = partSource.ParameterIndex, TemplateBreakdown = copy };
                    //
                    parametersParts.Add(parameterCopy);
                    partCopy.Parameter = parameterCopy;
                }

                parts.Add(partCopy);
                placeholderParts.Add(partCopy);
            }
            // Clone text part
            else if (part is ITemplateTextPart textPart)
            {
                ITemplateTextPart partCopy = new TemplateTextPart { Escaped = textPart.Escaped, Unescaped = textPart.Unescaped, TemplateBreakdown = copy };
                parts.Add(partCopy);
            }
            // Clone malformed part
            else if (part is ITemplateMalformedPart malformedPart)
            {
                // 
                ITemplateMalformedPart partCopy = new TemplateMalformedPart { Escaped = malformedPart.Escaped, Unescaped = malformedPart.Unescaped, TemplateBreakdown = copy };
                //
                parts.Add(partCopy);
            }
        }
        // Assign arrays
        copy.Parts = parts.ToArray();
        copy.Placeholders = placeholderParts.ToArray();
        copy.Parameters = parametersParts.ToArray();
        //
        return copy;
    }
}
