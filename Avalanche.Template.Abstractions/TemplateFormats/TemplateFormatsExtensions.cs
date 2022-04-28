// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

/// <summary>Extension methods for <see cref="ITemplateFormats"/>.</summary>
public static class TemplatesFormatExtensions
{
    /// <summary>Assign template formats</summary>
    public static T SetAllTemplateFormats<T>(this T templateFormats, params ITemplateFormat[] allformats) where T : ITemplateFormats
    {
        // Assign
        templateFormats.AllFormats = allformats;
        // Return
        return templateFormats;
    }

    /// <summary>Assign template formats</summary>
    public static T AddTemplateFormat<T>(this T templateFormats, ITemplateFormat format) where T : ITemplateFormats
    {
        //
        lock (templateFormats)
        {
            // Assign
            templateFormats.AllFormats = templateFormats.AllFormats.Concat(Enumerable.Repeat(format, 1)).Distinct().ToArray();
        }
        // Return
        return templateFormats;
    }

    /// <summary>Assign template formats</summary>
    public static T AddTemplateFormats<T>(this T templateFormats, params ITemplateFormat[] formats) where T : ITemplateFormats
    {
        //
        lock (templateFormats)
        {
            // Assign
            templateFormats.AllFormats = templateFormats.AllFormats.Concat(formats).Distinct().ToArray();
        }
        // Return
        return templateFormats;
    }
}
