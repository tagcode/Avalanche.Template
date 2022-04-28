// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

// <docs>
/// <summary>Root interface printable template.</summary>
public interface ITemplatePrintableBase
{
    /// <summary>Parameters in order of arguments and correspondence to <see cref="ITemplateParameterPart.ParameterIndex"/>.</summary>
    string?[] ParameterNames { get; set; }
}
// </docs>
