// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

// <docs>
/// <summary>Parameter name or index number part</summary>
public interface ITemplateParameterPart : ITemplatePart
{
    /// <summary>Parameter index in order of arguments and <see cref="ITemplateBreakdown.Parameters"/>.</summary>
    int ParameterIndex { get; set; }
}
// </docs>
