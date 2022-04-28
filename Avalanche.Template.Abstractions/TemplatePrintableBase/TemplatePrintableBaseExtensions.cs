// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

/// <summary>Extension methods for <see cref="ITemplatePrintableBase"/>.</summary>
public static class TemplatePrintableBaseExtensions
{
    /// <summary>Get non-null paramter names.</summary>
    public static IEnumerable<string> GetNonNullParameterNames(this ITemplatePrintableBase templateText)
    {
        // Get paramter names
        var _parameternames = templateText?.ParameterNames;
        // Visit parameter names
        if (_parameternames != null)
            foreach (var _parametername in _parameternames)
                if (_parametername != null)
                    yield return _parametername;
    }
}
