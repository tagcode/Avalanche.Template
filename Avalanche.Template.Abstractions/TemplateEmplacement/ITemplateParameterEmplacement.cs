// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

/// <summary>Parameter emplacement assignment</summary>
public interface ITemplateParameterEmplacement
{
    /// <summary>Parameter name</summary>
    string ParameterName { get; set; }
    /// <summary>Emplaced text</summary>
    ITemplateText Emplacement { get; set; }
}
