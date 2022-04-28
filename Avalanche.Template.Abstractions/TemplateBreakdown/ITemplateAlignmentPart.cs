// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

// <docs>
/// <summary>Alignment describes padding.</summary>
public interface ITemplateAlignmentPart : ITemplatePart
{
    /// <summary>Alignment, with positive value, padding goes on left side, with negative to right side.</summary>
    int Alignment { get; set; }
}
// </docs>
