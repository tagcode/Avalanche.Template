// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

// <docs>
/// <summary>A placeholder part of <see cref="ITemplateBreakdown"/>.</summary>
public interface ITemplatePlaceholderPart : ITemplatePart
{
    /// <summary>Parameter name or number, e.g. "object" or "0"</summary>
    ITemplateParameterPart Parameter { get; set; }
    /// <summary>Formatting descriptions, e.g. ",10:N0" or ":X8".</summary>
    ITemplateFormattingPart? Formatting { get; set; }
    /// <summary>Alignment describes padding.</summary>
    ITemplateAlignmentPart? Alignment { get; set; }
}
// </docs>
