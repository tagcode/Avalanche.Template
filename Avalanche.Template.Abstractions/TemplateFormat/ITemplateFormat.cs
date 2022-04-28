// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

// <docs>
/// <summary>String format description</summary>
public interface ITemplateFormat
{
    /// <summary>Format name</summary>
    string Name { get; set; }
    /// <summary>Provides <see cref="ITemplateText"/> for format string</summary>
    IProvider<string, ITemplateText> Text { get; set; }
    /// <summary>Provides <see cref="ITemplateText"/> for format string and caches into a weak-keyed map.</summary>
    IProvider<string, ITemplateText> TextCached { get; set; }
    /// <summary>Provides breakdown of format string</summary>
    IProvider<string, ITemplateBreakdown> Breakdown { get; set; }
    /// <summary>Provides breakdown of format string and caches in a weak-keyed map.</summary>
    IProvider<string, ITemplateBreakdown> BreakdownCached { get; set; }
    /// <summary>Assembles breakdown into string</summary>
    IProvider<ITemplateBreakdown, string> Assemble { get; set; }
    /// <summary>Assembles breakdown into string and caches in a weak-keyed map.</summary>
    IProvider<ITemplateBreakdown, string> AssembleCached { get; set; }
    /// <summary>Policy whether format uses numeric parameters and assumes parameter order by numeric value.</summary>
    bool NumberAssignedOrder { get; set; }
    /// <summary>Text escaper</summary>
    IEscaper? Escaper { get; set; }
}
// </docs>
