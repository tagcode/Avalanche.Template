// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

// <docs>
/// <summary>A part of a <see cref="ITemplateBreakdown"/>.</summary>
public interface ITemplatePart
{
    /// <summary>Reference to breakdown root</summary>
    ITemplateBreakdown? TemplateBreakdown { get; set; }
    /// <summary>Escaped text that is a slice of <see cref="ITemplateText.Text"/>.</summary>
    ReadOnlyMemory<char> Escaped { get; set; }
    /// <summary>Unescaped text that is either exact same slice than <see cref="Escaped"/>, or its derived unescape.</summary>
    ReadOnlyMemory<char> Unescaped { get; set; }
}
// </docs>
