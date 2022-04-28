// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Diagnostics.CodeAnalysis;

/// <summary>Template text with emplacement decorability.</summary>
public interface ITemplateEmplacementable : ITemplatePrintableBase
{
    /// <summary>Create decorated version where <paramref name="emplacements"/> are embued into placeholders.</summary>
    bool TryPlace(ITemplateParameterEmplacement[] emplacements, [NotNullWhen(true)] out ITemplateText emplaced);
}
