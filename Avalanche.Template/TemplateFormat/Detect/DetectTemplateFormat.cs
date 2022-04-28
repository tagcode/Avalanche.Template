// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Runtime.Serialization;
using Avalanche.Utilities;

/// <summary>
/// Detects template format by trying every assigned component format. 
/// Chooses the template format whose breakdown has most <see cref="ITemplatePlaceholderPart"/> and least <see cref="ITemplateMalformedPart"/>.
/// </summary>
public class DetectTemplateFormat : TemplateFormatBase.TextIsBreakdown, IReadOnly
{
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] protected bool @readonly;
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); if (value) setReadOnly(); } }
    /// <summary>Override this to modify assignment action.</summary>
    protected virtual void setReadOnly() => @readonly = true;

    /// <summary>Child formats</summary>
    protected ITemplateFormat[] formats;
    /// <summary>Child formats</summary>
    public ITemplateFormat[] Formats { get => formats; set => this.AssertWritable().formats = value; }

    /// <summary>Create auto-detecting template format</summary>
    public DetectTemplateFormat(string name, params ITemplateFormat[] formats) : base(name)
    {
        this.formats = formats.ToArray();
    }

    /// <summary>Assembly using associated or first format that can assemble</summary>
    protected override bool TryAssemble(ITemplateBreakdown templateBreakdown, out string result)
    {
        // Try associated format
        if (templateBreakdown.TemplateFormat != null && templateBreakdown.TemplateFormat.Assemble.TryGetValue(templateBreakdown, out result)) return true;
        // Get snapshot of formats
        var _format = Formats;
        // Try each format, choose first working
        foreach(ITemplateFormat templateFormat in _format)
        {
            if (templateFormat.Assemble.TryGetValue(templateBreakdown, out result)) return true;
        }
        // None worked
        result = null!;
        return false;
    }

    /// <summary>Breakdown with each, chooses the format with most <see cref="ITemplatePlaceholderPart"/> and least <see cref="ITemplateMalformedPart"/>.</summary>
    protected override bool TryBreakdown(string text, out ITemplateBreakdown templateBreakdown)
    {
        // Get snapshot of formats
        var _format = Formats;
        // Place here
        ITemplateBreakdown? result = null;
        int bestScore = int.MinValue;
        // try each
        foreach (ITemplateFormat templateFormat in _format)
        {
            // Try breakdown
            if (!templateFormat.Breakdown.TryGetValue(text, out ITemplateBreakdown? breakdown1)) continue;
            // Calculate score
            int score = 0;
            foreach (ITemplatePart part in breakdown1.Parts)
            {
                if (part is ITemplateMalformedPart) score -= 3;
                if (part is ITemplatePlaceholderPart) score++;
            }
            // Not best score
            if (score <= bestScore) continue;
            // Assign as currently best known
            bestScore = score;
            result = breakdown1;
        }
        // Return best
        templateBreakdown = result!;
        return result != null;
    }
}
