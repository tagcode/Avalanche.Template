// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

/// <summary>Extension methods for <see cref="ITemplateAlignmentPart"/>.</summary>
public static class TemplateAlignmentPartExtensions
{
    /// <summary>Set <paramref name="alignment"/>. With positive value, padding goes on left side, with negative to right side.</summary>
    public static T SetAlignment<T>(this T part, int alignment) where T : ITemplateAlignmentPart
    {
        // Assign 
        part.Alignment = alignment;
        // Return 
        return part;
    }
}
