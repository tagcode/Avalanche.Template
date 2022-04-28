// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using Avalanche.Template.Internal;
using Avalanche.Utilities;
using System.Diagnostics;

/// <summary>Extension methods for emplacement of template texts.</summary>
public static class TemplateEmplacementExtensions
{
    /// <summary>Parameter name separator between parent and emplaced parameter</summary>
    public const string ParameterNameSeparator = "_";

    /// <summary>Place <paramref name="emplacements"/> into placeholders in <paramref name="text"/>.</summary>
    public static ITemplateText Place(this ITemplateText text, params ITemplateParameterEmplacement[] emplacements)
    {
        // Place using ITemplateEmplacementable
        if (text is ITemplateEmplacementable emplacementable && emplacementable.TryPlace(emplacements, out ITemplateText emplaced)) return emplaced;
        // Recompose
        emplaced = Compose(text, emplacements);
        // Return
        return emplaced;
    }

    /// <summary>Place <paramref name="placement"/> into placeholder <paramref name="parameterName"/>.</summary>
    public static ITemplateText Place(this ITemplateText text, string parameterName, ITemplateText placement)
    {
        // 
        ITemplateParameterEmplacement[] emplacements = new ITemplateParameterEmplacement[] { new TemplateParameterEmplacement(parameterName, placement) };
        // Place using ITemplateEmplacementable
        if (text is ITemplateEmplacementable emplacementable && emplacementable.TryPlace(emplacements, out ITemplateText emplaced)) return emplaced;
        // Recompose
        emplaced = Compose(text, emplacements);
        // Return
        return emplaced;
    }

    /// <summary>Place <paramref name="placement"/> into placeholder <paramref name="parameterName"/>.</summary>
    public static ITemplateText Place(this ITemplateText text, string parameterName, ITemplateText placement, string parameterName2, ITemplateText placement2)
    {
        // 
        ITemplateParameterEmplacement[] emplacements = new ITemplateParameterEmplacement[] { 
            new TemplateParameterEmplacement(parameterName, placement),
            new TemplateParameterEmplacement(parameterName2, placement2)
        };
        // Place using ITemplateEmplacementable
        if (text is ITemplateEmplacementable emplacementable && emplacementable.TryPlace(emplacements, out ITemplateText emplaced)) return emplaced;
        // Recompose
        emplaced = Compose(text, emplacements);
        // Return
        return emplaced;
    }

    /// <summary>Place <paramref name="placement"/> into placeholder <paramref name="parameterName"/>.</summary>
    public static ITemplateText Place(this ITemplateText text, string parameterName, ITemplateText placement, string parameterName2, ITemplateText placement2, string parameterName3, ITemplateText placement3)
    {
        // 
        ITemplateParameterEmplacement[] emplacements = new ITemplateParameterEmplacement[] { 
            new TemplateParameterEmplacement(parameterName, placement),
            new TemplateParameterEmplacement(parameterName2, placement2),
            new TemplateParameterEmplacement(parameterName3, placement3),
        };
        // Place using ITemplateEmplacementable
        if (text is ITemplateEmplacementable emplacementable && emplacementable.TryPlace(emplacements, out ITemplateText emplaced)) return emplaced;
        // Recompose
        emplaced = Compose(text, emplacements);
        // Return
        return emplaced;
    }

    /// <summary>Place <paramref name="placement"/> into placeholder <paramref name="parameterName"/>.</summary>
    public static ITemplateText Place(this ITemplateText text, string parameterName, ITemplateText placement, string parameterName2, ITemplateText placement2, string parameterName3, ITemplateText placement3, string parameterName4, ITemplateText placement4)
    {
        // 
        ITemplateParameterEmplacement[] emplacements = new ITemplateParameterEmplacement[] { 
            new TemplateParameterEmplacement(parameterName, placement),
            new TemplateParameterEmplacement(parameterName2, placement2),
            new TemplateParameterEmplacement(parameterName3, placement3),
            new TemplateParameterEmplacement(parameterName4, placement4),
        };
        // Place using ITemplateEmplacementable
        if (text is ITemplateEmplacementable emplacementable && emplacementable.TryPlace(emplacements, out ITemplateText emplaced)) return emplaced;
        // Recompose
        emplaced = Compose(text, emplacements);
        // Return
        return emplaced;
    }

    /// <summary>Place <paramref name="placement"/> into placeholder <paramref name="parameterName"/>.</summary>
    public static ITemplateText Place(this ITemplateText text, string parameterName, ITemplateText placement, string parameterName2, ITemplateText placement2, string parameterName3, ITemplateText placement3, string parameterName4, ITemplateText placement4, string parameterName5, ITemplateText placement5)
    {
        // 
        ITemplateParameterEmplacement[] emplacements = new ITemplateParameterEmplacement[] { 
            new TemplateParameterEmplacement(parameterName, placement),
            new TemplateParameterEmplacement(parameterName2, placement2),
            new TemplateParameterEmplacement(parameterName3, placement3),
            new TemplateParameterEmplacement(parameterName4, placement4),
            new TemplateParameterEmplacement(parameterName5, placement5),
        };
        // Place using ITemplateEmplacementable
        if (text is ITemplateEmplacementable emplacementable && emplacementable.TryPlace(emplacements, out ITemplateText emplaced)) return emplaced;
        // Recompose
        emplaced = Compose(text, emplacements);
        // Return
        return emplaced;
    }

    /// <summary>Recompose breakdown to embed emplacements.</summary>
    [DebuggerHidden]
    public static ITemplateBreakdown Compose(ITemplateText text, ICollection<ITemplateParameterEmplacement> emplacements)
    {
        // Create mapping
        TemplateEmplacementMapping parameterMapping = TemplateEmplacementMapping.CreateEmplacementMapping(text, emplacements);
        // Compose text
        ITemplateBreakdown newComposition = Compose(parameterMapping);
        // Return new composition
        return newComposition;
    }

    /// <summary>Recompose breakdown to embed emplacements.</summary>
    public static ITemplateBreakdown Compose(TemplateEmplacementMapping parameterMapping, ITemplateText? text = null, ICollection<ITemplateParameterEmplacement>? emplacements = null)
    {
        // Get original text
        if (text == null) text = parameterMapping.Text;
        // Get emplacements
        if (emplacements == null) emplacements = parameterMapping.Emplacements;
        // Create mapping
        if (parameterMapping == null) parameterMapping = TemplateEmplacementMapping.CreateEmplacementMapping(text, emplacements);
        // Create breakdown for new result
        TemplateBreakdown breakdown = new TemplateBreakdown { TemplateFormat = text.TemplateFormat };
        // Place here parts
        StructList16<ITemplatePart> parts = new();
        // Get escaper
        IEscaper? escaper = text.TemplateFormat?.Escaper;
        // Embue parts
        foreach (ITemplatePart part in text.Breakdown.Parts)
        {
            // Not placeholder or has no parameter
            if (part is not ITemplatePlaceholderPart placeholderPart || placeholderPart.Parameter == null) { parts.Add(part.Clone(breakdown)); continue; }
            // Get parameter name
            string? origParameterName = placeholderPart.Parameter.Unescaped.AsString();
            // Find associated parameter emplacements
            parameterMapping.TryGetNewParametersByOrigParameterName(origParameterName, out TemplateEmplacementMapping.ParameterMapping[]? emplacedParameters);
            // Try get emplacement
            emplacements.TryGetEmplacement(origParameterName, out ITemplateParameterEmplacement parameterEmplacement);
            // Place here emplaced text
            ITemplateBreakdown? emplacementText = (emplacedParameters == null || emplacedParameters.Length == 0 ? null : emplacedParameters[0]?.EmplacementText?.Breakdown)??parameterEmplacement?.Emplacement?.Breakdown;
            // No associated emplacement
            if (emplacementText == null)
            {
                // Clone non-emplaced placeholder
                ITemplatePlaceholderPart clonedPlaceholder = (ITemplatePlaceholderPart)placeholderPart.Clone(breakdown);
                // Get parameter info and assign index and name
                if (parameterMapping.TryGetOrigParameterByOrigParameterName(origParameterName, out TemplateEmplacementMapping.ParameterMapping origParameterInfo))
                {
                    clonedPlaceholder.Parameter.ParameterIndex = origParameterInfo.NewParameterIndex;
                    clonedPlaceholder.Parameter.Unescaped = origParameterInfo.NewParameterName.AsMemory();
                    clonedPlaceholder.Parameter.Escaped = escaper == null ? origParameterInfo.NewParameterName.AsMemory() : escaper.Escape(origParameterInfo.NewParameterName).AsMemory();
                }
                // Add to parts
                parts.Add(clonedPlaceholder);
                //
                continue;
            }
            // Process emplacement
            else
            {
                // Process each part from emplacement
                foreach (ITemplatePart emplacementPart in emplacementText.Parts)
                {
                    // Add non-placeholder as is
                    if (emplacementPart is not ITemplatePlaceholderPart emplacementPlaceholder) { parts.Add(emplacementPart.Clone(breakdown)); continue; }
                    // Get corresponding part
                    if (!!emplacedParameters.TryGetByEmplacementName(emplacementPlaceholder.Parameter.Unescaped.AsString(), out TemplateEmplacementMapping.ParameterMapping emplacementMapping) && !emplacedParameters.TryGetByEmplacementIndex(emplacementPlaceholder.Parameter.ParameterIndex, out emplacementMapping)) { parts.Add(emplacementPart.Clone(breakdown)); continue; }
                    // Clone parameter
                    TemplateParameterPart newParameterPart = new TemplateParameterPart
                    {
                        TemplateBreakdown = breakdown,
                        ParameterIndex = emplacementMapping.NewParameterIndex,
                        Escaped = emplacementMapping.NewParameterName.AsMemory(),
                        Unescaped = escaper == null ? emplacementMapping.NewParameterName.AsMemory() : escaper.Escape(emplacementMapping.NewParameterName).AsMemory()
                    };
                    // Clone placeholder
                    TemplatePlaceholderPart newPlaceholderPart = new TemplatePlaceholderPart
                    {
                        TemplateBreakdown = breakdown,
                        Alignment = emplacementPlaceholder.Alignment,
                        Formatting = emplacementPlaceholder.Formatting,
                        Parameter = newParameterPart
                    };
                    // Add to parts
                    parts.Add(newPlaceholderPart);
                }
            }
        }

        // Assign parts
        breakdown.Parts = parts.ToArray();

        // Assign placeholder texts
        if (text.TemplateFormat != null)
        {
            // Process each part
            foreach (ITemplatePart? part in breakdown.Parts)
            {
                // Not placeholder
                if (part is not ITemplatePlaceholderPart placeholder) continue;
                // Already has text
                if (placeholder.Escaped.Length > 0 && placeholder.Unescaped.Length > 0) continue;
                // Create dummy breakdown
                TemplateBreakdown? tmpBreakdown = new TemplateBreakdown 
                {
                    TemplateFormat = text.TemplateFormat,
                    Parameters = new ITemplateParameterPart[] { placeholder.Parameter },
                    Parts = new ITemplatePart[] { placeholder }
                };
                // Create placeholder text, e.g. "{0_a}"
                string placeholderText = text.TemplateFormat.Assemble[tmpBreakdown];
                // No text
                if (placeholderText == null) continue;
                // Assign placeholder text
                placeholder.Unescaped = placeholderText.AsMemory();
                // Assign escaped text
                placeholder.Escaped = text.TemplateFormat.Escaper == null ? placeholderText.AsMemory() : text.TemplateFormat.Escaper.Escape(placeholderText).AsMemory();
            }
        }

        // Assign text
        if (text.TemplateFormat!=null) breakdown.Text = text.TemplateFormat!.Assemble[breakdown];

        // Immute
        breakdown.SetReadOnly();
        foreach (var p in breakdown.Parts) if (p is IReadOnly ro) ro.ReadOnly = true;
        foreach (var p in breakdown.Parameters) if (p is IReadOnly ro) ro.ReadOnly = true;
        // Return
        return breakdown;
    }
}
