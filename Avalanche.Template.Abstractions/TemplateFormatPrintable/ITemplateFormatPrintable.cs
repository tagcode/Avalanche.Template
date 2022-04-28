// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Text;

// <docs>
/// <summary>Printable template text. Printing assigns arguments to placeholders.</summary>
public interface ITemplateFormatPrintable : ITemplatePrintableBase
{
    /// <summary>Print <paramref name="arguments"/> in placeholders using <paramref name="formatProvider"/>.</summary>
    string Print(IFormatProvider? formatProvider, object?[]? arguments = null);
    /// <summary>Append to <paramref name="sb"/>.</summary>
    void AppendTo(StringBuilder sb, IFormatProvider? formatProvider, object?[]? arguments = null);
    /// <summary>Write to <paramref name="textWriter"/>.</summary>
    void WriteTo(TextWriter textWriter, IFormatProvider? formatProvider, object?[]? arguments = null);
    /// <summary>Try print <paramref name="arguments"/> using <paramref name="formatProvider"/> into <paramref name="dst"/>.</summary>
    /// <param name="length">Number of characters written to <paramref name="dst"/>. If failed, the 0 is returned.</param>
    /// <returns>True if text was written, false if write failed.</returns>
    bool TryPrintTo(Span<char> dst, out int length, IFormatProvider? formatProvider, object?[]? arguments = null);
    /// <summary>Try estimate print length</summary>
    bool TryEstimatePrintLength(out int length, IFormatProvider? formatProvider, object?[]? arguments = null);
}
// </docs>
