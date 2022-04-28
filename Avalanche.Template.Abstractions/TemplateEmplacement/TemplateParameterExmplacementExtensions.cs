// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Diagnostics.CodeAnalysis;

/// <summary>Extension methods for <see cref="ITemplateParameterEmplacement"/>.</summary>
public static class TemplateParameterExmplacementExtensions
{
    /// <summary>Try find emplacement by <paramref name="parameterName"/>.</summary>
    public static bool TryGetEmplacement(this ICollection<ITemplateParameterEmplacement> emplacements, string parameterName, [NotNullWhen(true)] out ITemplateParameterEmplacement parameterEmplacement)
    {
        // No array
        if (emplacements == null || parameterName == null) { parameterEmplacement = null!; return false; }
        // Find matching
        foreach (ITemplateParameterEmplacement _parameterEmplacement in emplacements)
            if (_parameterEmplacement.ParameterName == parameterName) { parameterEmplacement = _parameterEmplacement; return true; };
        // No match
        parameterEmplacement = null!;
        return false;
    }
}
