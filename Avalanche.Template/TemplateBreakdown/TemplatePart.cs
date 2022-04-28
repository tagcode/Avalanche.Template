// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Text part.</summary>
public class TemplateTextPart : TemplateTextPartBase { }
/// <summary>Alignment description. Parses padding lazily.</summary>
public class TemplateAlignmentPart : TemplateAlignmentPartBase
{
    /// <summary>Get padding</summary>
    public override int Alignment
    {
        get
        {
            // Return already assigned padding
            if (padding.HasValue) return padding.Value;
            // Lazy parse padding
            padding = int.Parse(this.Unescaped.Span);
            // Return parsed
            return padding.Value;
        }
        // Assign value
        set => this.AssertWritable().padding = value;
    }
}

/// <summary>Placeholder part.</summary>
public class TemplatePlaceholderPart : TemplatePlaceholderPartBase { }
/// <summary>Parameter name or index number part</summary>
public class TemplateParameterPart : TemplateParameterPartBase { }
/// <summary>Formatting description, e.g. ",10:N0" or ":X8".</summary>
public class TemplateFormattingPart : TemplateFormattingPartBase { }
/// <summary>Malformed part.</summary>
public class TemplateMalformedPart : TemplateMalformedPartBase { }
