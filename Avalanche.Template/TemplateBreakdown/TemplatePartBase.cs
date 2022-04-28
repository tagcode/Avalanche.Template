// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using Avalanche.Utilities;

/// <summary></summary>
public abstract class TemplatePartBase : ReadOnlyAssignableClass, ITemplatePart
{
    /// <summary>Associated breakdown</summary>
    protected ITemplateBreakdown? templateBreakdown = null!;
    /// <summary>Escaped text.</summary>
    protected ReadOnlyMemory<char> escaped = default;
    /// <summary>Escaped text.</summary>
    protected ReadOnlyMemory<char> unescaped = default;

    /// <summary>Associated string</summary>
    public virtual ITemplateBreakdown? TemplateBreakdown { get => templateBreakdown; set => this.AssertWritable().templateBreakdown = value; }
    /// <summary>Escaped text.</summary>
    public virtual ReadOnlyMemory<char> Escaped { get => escaped; set => this.AssertWritable().escaped = value; }
    /// <summary>Escaped text.</summary>
    /// <remarks>Implementation may lazily derive from assigned <see cref="Escaped"/>, see impl specific instructions.</remarks>
    public virtual ReadOnlyMemory<char> Unescaped { get => unescaped; set => this.AssertWritable().unescaped = value; }

    /// <summary></summary>
    public TemplatePartBase() : base() { }

    /// <summary>Print information</summary>
    public override string ToString() => $"{GetType().Name}: \"{Unescaped}\"";
}

/// <summary>Placeholder part.</summary>
public class TemplatePlaceholderPartBase : TemplatePartBase, ITemplatePlaceholderPart
{
    /// <summary>Parameter name or number, e.g. "object" or "0"</summary>
    protected ITemplateParameterPart parameter = null!;
    /// <summary>Alignment</summary>
    protected ITemplateAlignmentPart? alignment = null;
    /// <summary>Formatting descriptions, e.g. ",10:N0" or ":X8".</summary>
    protected ITemplateFormattingPart? formatting = null;

    /// <summary>Alignment</summary>
    public virtual ITemplateAlignmentPart? Alignment { get => alignment; set => this.AssertWritable().alignment = value; }
    /// <summary>Parameter name or number, e.g. "object" or "0"</summary>
    public virtual ITemplateParameterPart Parameter { get => parameter; set => this.AssertWritable().parameter = value; }
    /// <summary>Formatting description, e.g. ",10:N0" or ":X8".</summary>
    public virtual ITemplateFormattingPart? Formatting { get => formatting; set => this.AssertWritable().formatting = value; }
}

/// <summary>Text part.</summary>
public class TemplateTextPartBase : TemplatePartBase, ITemplateTextPart
{
}

/// <summary>Parameter name or index number part</summary>
public class TemplateParameterPartBase : TemplatePartBase, ITemplateParameterPart
{
    /// <summary>Parameter index, correlates with arguments and <see cref="ITemplateBreakdown.Parameters"/>.</summary>
    protected int parameterIndex;
    /// <summary>Parameter index, correlates with arguments and <see cref="ITemplateBreakdown.Parameters"/>.</summary>
    public virtual int ParameterIndex { get => parameterIndex; set => this.AssertWritable().parameterIndex = value; }
}

/// <summary>Formatting description, e.g. ",10:N0" or ":X8".</summary>
public class TemplateFormattingPartBase : TemplatePartBase, ITemplateFormattingPart { }

/// <summary>Alignment description</summary>
public class TemplateAlignmentPartBase : TemplatePartBase, ITemplateAlignmentPart
{
    /// <summary>Cached padding value</summary>
    protected int? padding;
    /// <summary></summary>
    public TemplateAlignmentPartBase() : base() { }
    /// <summary>Padding</summary>
    public virtual int Alignment { get => padding.HasValue ? padding.Value : 0; set => this.AssertWritable().padding = value; }
}

/// <summary>Malformed part.</summary>
public class TemplateMalformedPartBase : TemplatePartBase, ITemplateMalformedPart { }

