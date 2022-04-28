// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

/// <summary>Extension methods for <see cref="ITemplatePlaceholderPart"/>.</summary>
public static class TemplatePlaceholderPartExtensions
{
        /// <summary>Parameter name or number, e.g. "object" or "0"</summary>
    public static T SetParameter<T>(this T part, ITemplateParameterPart parameterPart) where T : ITemplatePlaceholderPart
    {
        // Assign 
        part.Parameter = parameterPart;
        // Return 
        return part;
    }

    /// <summary>Formatting descriptions, e.g. ",10:N0" or ":X8".</summary>
    public static T SetFormatting<T>(this T part, ITemplateFormattingPart? formatting) where T : ITemplatePlaceholderPart
    {
        // Assign 
        part.Formatting = formatting;
        // Return 
        return part;
    }

    /// <summary>Alignment describes padding.</summary>
    public static T SetAlignment<T>(this T part, ITemplateAlignmentPart? alignment) where T : ITemplatePlaceholderPart
    {
        // Assign 
        part.Alignment = alignment;
        // Return 
        return part;
    }
}
