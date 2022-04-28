// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Text;

// <docs>
/// <summary>Printable template text where format provider is internally assigned.</summary>
public interface ITemplatePrintable : ITemplatePrintableBase
{
    /// <summary>Print <paramref name="arguments"/> in placeholders.</summary>
    string Print(object?[]? arguments = null);
    /// <summary>Append to <paramref name="sb"/>.</summary>
    void AppendTo(StringBuilder sb, object?[]? arguments = null);
    /// <summary>Write to <paramref name="textWriter"/>.</summary>
    void WriteTo(TextWriter textWriter, object?[]? arguments = null);
    /// <summary>Try print <paramref name="arguments"/> into <paramref name="dst"/>.</summary>
    /// <param name="length">Number of characters written to <paramref name="dst"/>. If failed, the 0 is returned.</param>
    /// <returns>True if text was written, false if write failed.</returns>
    bool TryPrintTo(Span<char> dst, out int length, object?[]? arguments = null);
    /// <summary>Try estimate print length</summary>
    bool TryEstimatePrintLength(out int length, object?[]? arguments = null);
}
// </docs>
