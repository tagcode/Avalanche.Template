// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

// <docs>
/// <summary>Template text.</summary>
public interface ITemplateText : ITemplateFormatPrintable
{
    /// <summary>Template text in its escaped format, e.g. "Welcome, {{{0}}}.\nYou received {1,15:N0} credit(s).</summary>
    string Text { get; set; }
    /// <summary>(optional) Description of the format type</summary>
    ITemplateFormat? TemplateFormat { get; set; }
    /// <summary>Template breakdown</summary>
    ITemplateBreakdown Breakdown { get; set; }
}
// </docs>
