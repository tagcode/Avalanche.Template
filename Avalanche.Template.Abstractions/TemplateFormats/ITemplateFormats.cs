// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using Avalanche.Utilities.Provider;

// <docs>
/// <summary>Configurable table of template formats.</summary>
public interface ITemplateFormats
{
    /// <summary>List of all formats, excluding detect format</summary>
    ITemplateFormat[] AllFormats { get; set; }
    /// <summary>Format by name, including <see cref="Detect"/>.</summary>
    IProvider<string, ITemplateFormat> ByName { get; set; }
    /// <summary>Auto-detect format</summary>
    ITemplateFormat Detect { get; set; }
}
// </docs>
