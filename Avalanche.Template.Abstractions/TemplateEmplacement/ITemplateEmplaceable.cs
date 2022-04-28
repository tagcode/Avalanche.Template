// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

/// <summary>Template text with emplacement capability.</summary>
public interface ITemplateEmplaceable : ITemplatePrintableBase
{
    /// <summary>Emplacement assignments</summary>
    ITemplateParameterEmplacement[]? Emplacements { get; set; }
}
