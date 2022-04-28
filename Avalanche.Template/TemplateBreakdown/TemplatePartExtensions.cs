// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

/// <summary>Extension methods for <see cref="ITemplatePart"/>.</summary>
public static class TemplatePartExtensions_
{
    /// <summary>Clone to use new parent <paramref name="breakdown"/>.</summary>
    public static ITemplatePart Clone(this ITemplatePart part, ITemplateBreakdown? breakdown = null)
    {
        //
        if (breakdown == null) breakdown = part.TemplateBreakdown;
        // Clone placeholder part
        if (part is ITemplatePlaceholderPart placeholderPart)
        {
            // Create copy
            ITemplatePlaceholderPart partCopy = new TemplatePlaceholderPart { Escaped = placeholderPart.Escaped, Unescaped = placeholderPart.Unescaped, TemplateBreakdown = breakdown };
            // Assign format
            if (placeholderPart.Formatting != null)
            {
                // 
                var partSource = placeholderPart.Formatting;
                // 
                ITemplateFormattingPart formattingCopy = new TemplateFormattingPart { Escaped = partSource.Escaped, Unescaped = partSource.Unescaped, TemplateBreakdown = breakdown };
                //
                partCopy.Formatting = formattingCopy;
            }
            // Assign alignment
            if (placeholderPart.Alignment != null)
            {
                // 
                var partSource = placeholderPart.Alignment;
                // 
                ITemplateAlignmentPart alignmentCopy = new TemplateAlignmentPart { Escaped = partSource.Escaped, Unescaped = partSource.Unescaped, Alignment = partSource.Alignment, TemplateBreakdown = breakdown };
                //
                partCopy.Alignment = alignmentCopy;
            }
            // Assign parameter
            if (placeholderPart.Parameter != null)
            {
                // 
                var partSource = placeholderPart.Parameter;
                // 
                ITemplateParameterPart parameterCopy = new TemplateParameterPart { Escaped = partSource.Escaped, Unescaped = partSource.Unescaped, ParameterIndex = partSource.ParameterIndex, TemplateBreakdown = breakdown };
                //
                partCopy.Parameter = parameterCopy;
            }
            //
            return partCopy;
        }
        // Clone text part
        else if (part is ITemplateTextPart textPart)
        {
            ITemplateTextPart partCopy = new TemplateTextPart { Escaped = textPart.Escaped, Unescaped = textPart.Unescaped, TemplateBreakdown = breakdown };
            //
            return partCopy;
        }
        // Clone malformed part
        else if (part is ITemplateMalformedPart malformedPart)
        {
            // 
            ITemplateMalformedPart partCopy = new TemplateMalformedPart { Escaped = malformedPart.Escaped, Unescaped = malformedPart.Unescaped, TemplateBreakdown = breakdown };
            //
            return partCopy;
        }

        // Unknown part type
        throw new ArgumentException($"Cannot clone {part.GetType().FullName}");
    }

}
