// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

// <docs>
/// <summary>Template string breakdown into parts, e.g. "Welcome, {0}!", into text and placeholder parts. </summary>
public interface ITemplateBreakdown : ITemplateText
{
    /// <summary>Breakdown into sequence of text and placeholder parts.</summary>
    ITemplatePart[] Parts { get; set; }
    /// <summary>Placeholders in order of occurance in <see cref="ITemplateText.Text"/>.</summary>
    ITemplatePlaceholderPart[] Placeholders { get; set; }
    /// <summary>Parameters in order of arguments and <see cref="ITemplateParameterPart.ParameterIndex"/>.</summary>
    ITemplateParameterPart?[] Parameters { get; set; }
}
// </docs>
