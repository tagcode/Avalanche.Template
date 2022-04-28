// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="ITemplateBreakdown"/>.</summary>
public static class TemplatePrintingExtensions
{
    /// <summary>Print <paramref name="argument"/></summary>
    /// <param name="buf">Optional buffer to slice memories from. If needs pinning, the caller must pin it.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<char> PrintArgument(IFormatProvider? formatProvider, ReadOnlyMemory<char> format, object? argument, ref Memory<char> buf)
    {
        // No argument
        if (argument == null) return default;
        // Got string
        if (argument is String str) return str.AsMemory();
        // ISpanFormattable
        if (argument is ISpanFormattable spanformattable)
        {
            //
            ReadOnlySpan<char> formatSpan = format.Span;
            // Print to 'buf'
            if (buf.Length > 0 && spanformattable.TryFormat(buf.Span, out int written, formatSpan, formatProvider))
            {
                // Wrap to print out
                ReadOnlyMemory<char> result = buf.Slice(0, written);
                // Slice 'buf'
                buf = buf.Slice(written);
                // Return
                return result;
            }
        }

        // Place print here
        string? print;
        // IFormattable
        if (argument is IFormattable formattable) print = formattable.ToString(format.AsString(), formatProvider);
        // ToString()
        else print = argument?.ToString();
        // Empty
        if (print == null || print.Length == 0) return default;
        // Wrap string       
        return print.AsMemory();
    }

    /// <summary></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryEstimatePrintLength(ITemplateBreakdown? templateBreakdown, out int length, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // Get parts
        ITemplatePart[]? parts = templateBreakdown?.Parts;
        // No parts
        if (parts == null || parts.Length == 0) { length = 0; return false; }
        // Get print length
        length = parts.PrintLength(formatProvider, arguments);
        // Done
        return true;
    }

    /// <summary>Calculate parts print length</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int PrintLength(this ITemplatePart[]? parts, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // No parts
        if (parts == null) return 0;
        // Place length here
        int length = 0;
        // Allocate buffer
        char[] buffer = ArrayPool<char>.Shared.Rent(4096);
        try
        {
            // Visit parts
            foreach (ITemplatePart part in parts)
            {
                // IPlaceholder
                if (part is ITemplatePlaceholderPart placeholder)
                {
                    // Get parameter index
                    int parameterIndex = placeholder.Parameter.ParameterIndex;
                    // Get format
                    ReadOnlyMemory<char> format = placeholder.Formatting == null ? default : placeholder.Formatting.Unescaped;
                    // Get argument
                    object? argument = arguments == null ? null : parameterIndex >= arguments.Length ? null : arguments[parameterIndex];
                    // 
                    Memory<char> buf = buffer.AsMemory();
                    // Print
                    ReadOnlyMemory<char> print = PrintArgument(formatProvider, format, argument, ref buf);
                    // Get padding
                    int padding = placeholder.Alignment == null ? 0 : Math.Abs(placeholder.Alignment.Alignment);
                    // Add length
                    length += padding > print.Length ? padding : print.Length;
                }
                // Get unescaped length
                else
                {
                    // Add length
                    length += part.Unescaped.Length;
                }
            }
            // Return length
            return length;
        }
        finally
        {
            // Return array
            ReturnToPool(null, buffer);
        }
    }
    
    /// <summary>Print <paramref name="parts"/>.</summary>
    /// <return>Length required of required string including padding.</return>
    /// <remarks>Prints, length, rental buffer</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (ReadOnlyMemory<char>[]?, int, char[]?) Print(this ITemplatePart[]? parts, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // No parts
        if (parts == null) return (null, 0, null);
        // Place length here
        int length = 0;
        // Allocate print slots
        ReadOnlyMemory<char>[] prints = ArrayPool<ReadOnlyMemory<char>>.Shared.Rent(parts.Length);
        // Allocate working buffer
        char[] rental = ArrayPool<char>.Shared.Rent(4096);
        // As memory
        Memory<char> buf = rental.AsMemory();

        // Try printing
        try
        {
            // Visit parts
            for (int ix = 0; ix < parts.Length; ix++)
            {
                // Index
                ITemplatePart part = parts[ix];
                // IPlaceholder
                if (part is ITemplatePlaceholderPart placeholder)
                {
                    // Get parameter index
                    int parameterIndex = placeholder.Parameter.ParameterIndex;
                    // Get format
                    ReadOnlyMemory<char> format = placeholder.Formatting == null ? default :placeholder.Formatting.Unescaped;
                    // Get argument
                    object? argument = arguments == null ? null : parameterIndex >= arguments.Length ? null : arguments[parameterIndex];
                    // Print
                    ReadOnlyMemory<char> print = PrintArgument(formatProvider, format, argument, ref buf);
                    // Add to list
                    prints[ix] = print;
                    // Get padding
                    int padding = placeholder.Alignment == null ? 0 : Math.Abs(placeholder.Alignment.Alignment);
                    // Add length
                    length += padding > print.Length ? padding : print.Length;
                }
                // Get unescaped length
                else
                {
                    // Get memory
                    ReadOnlyMemory<char> partMemory = part.Unescaped;
                    // Get memory
                    prints[ix] = partMemory;
                    // Add length
                    length += partMemory.Length;
                }
            }
            // Rental was used
            if (buf.Length != rental.Length) return (prints, length, rental);
            // Return rental
            ArrayPool<char>.Shared.Return(rental);
            // Return without rental
            return (prints, length, null);
        }
        catch (Exception)
        {
            // Clean up
            ReturnToPool(prints, rental);
            // Let fly
            throw;
        }
    }
    
    /// <summary>Return to pool</summary>
    public static void ReturnToPool(ReadOnlyMemory<char>[]? prints, char[]? rental)
    {
        // Return 
        if (prints != null) ArrayPool<ReadOnlyMemory<char>>.Shared.Return(prints, clearArray: true /*Keep true so that pins are released.*/);
        // Return 
        if (rental != null) ArrayPool<char>.Shared.Return(rental, clearArray: false);
    }

    /// <summary>Print <paramref name="templateBreakdown"/> with <paramref name="arguments"/> and <paramref name="formatProvider"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Print(ITemplateBreakdown? templateBreakdown, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // Get parts
        ITemplatePart[]? parts = templateBreakdown?.Parts;
        // No parts
        if (parts == null || parts.Length == 0) return String.Empty;
        // Single text part
        if (parts.Length == 1 && parts[0] is ITemplateTextPart textPart) return textPart.Unescaped.AsString();

        // Print buffer here
        char[] printBuf = null!;
        // Place prints here
        (ReadOnlyMemory<char>[]? prints, int length, char[]? rental) = parts.Print(formatProvider, arguments);

        try
        {
            // Return singleton
            if (prints == null || length == 0) return String.Empty;
            // Allocate print buffer
            printBuf = ArrayPool<char>.Shared.Rent(length);

            // Get 'dst' span
            Memory<char> dst = printBuf;
            // Copy parts to 'dst'
            for (int ix = 0; ix < parts.Length; ix++)
            {
                // Get part
                ITemplatePart part = parts[ix];
                // Get print
                ReadOnlyMemory<char> print = prints[ix];
                // Placeholder part
                if (part is ITemplatePlaceholderPart placeholder)
                {
                    // Get padding
                    int padding = placeholder.Alignment == null ? 0 : placeholder.Alignment.Alignment;
                    // Left pad
                    for (int i = print.Length; i < padding; i++) { dst.Span[0] = ' '; dst = dst.Slice(1); }
                    // Copy chars
                    print.CopyTo(dst);
                    // Slice
                    dst = dst.Slice(print.Length);
                    // Right pad
                    for (int i = print.Length; i < -padding; i++) { dst.Span[0] = ' '; dst = dst.Slice(1); }
                }
                // Other part, e.g. Malformed
                else
                {
                    // Copy text
                    part.Unescaped.CopyTo(dst);
                    // Slice
                    dst = dst.Slice(part.Unescaped.Span.Length);
                }
            }
            // Put together
            string text = new string(printBuf, 0, length);
            // Return
            return text;
        }
        finally
        {
            // Return print buffer
            if (printBuf != null) ArrayPool<char>.Shared.Return(printBuf);
            // Release rents
            ReturnToPool(prints, rental);
        }
    }
    
    /// <summary></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryPrintTo(ITemplateBreakdown? templateBreakdown, Span<char> dst, out int written, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // Get parts
        ITemplatePart[]? parts = templateBreakdown?.Parts;
        // No parts
        if (parts == null || parts.Length == 0) { written = 0; return false; }

        // Place prints here
        (ReadOnlyMemory<char>[]? prints, int length, char[]? rental) = parts.Print(formatProvider, arguments);
        try
        {
            // There is enough room in 'dst'
            if (dst.Length < length) { written = 0; return false; }
            // No prints
            if (prints == null || length == 0) { written = 0; return true; }

            // Copy parts to 'dst'
            for (int ix = 0; ix < parts.Length; ix++)
            {
                // Get part
                ITemplatePart part = parts[ix];
                // Get print
                ReadOnlyMemory<char> print = prints[ix];
                // Placeholder part
                if (part is ITemplatePlaceholderPart placeholder)
                {
                    // Get padding
                    int padding = placeholder.Alignment == null ? 0 : placeholder.Alignment.Alignment;
                    // Left pad
                    for (int i = print.Length; i < padding; i++) { dst[0] = ' '; dst = dst.Slice(1); }
                    // Copy chars
                    print.Span.CopyTo(dst);
                    // Slice
                    dst = dst.Slice(print.Length);
                    // Right pad
                    for (int i = print.Length; i < -padding; i++) { dst[0] = ' '; dst = dst.Slice(1); }
                }
                // Other part, e.g. Malformed
                else
                {
                    // Copy text
                    part.Unescaped.Span.CopyTo(dst);
                    // Slice
                    dst = dst.Slice(part.Unescaped.Length);
                }
            }
            // Return
            written = length;
            return true;
        }
        finally
        {
            // Release rents
            ReturnToPool(prints, rental);
        }
    }
    
    /// <summary></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendTo(ITemplateBreakdown? templateBreakdown, StringBuilder stringBuilder, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // No breakdown
        if (templateBreakdown == null) return;
        // Work buffer
        char[] buffer = ArrayPool<char>.Shared.Rent(4096);
        try
        {
            // Calculate length
            foreach (ITemplatePart part in templateBreakdown.Parts)
            {
                // Placeholder part
                if (part is ITemplatePlaceholderPart placeholder)
                {
                    // Get parameter index
                    int parameterIndex = placeholder.Parameter.ParameterIndex;
                    // Get argument
                    object? argument = arguments == null ? null : parameterIndex >= arguments.Length ? null : arguments[parameterIndex];
                    // Get format
                    ReadOnlyMemory<char> format = placeholder.Formatting == null ? default : placeholder.Formatting.Unescaped;
                    // Restore buffer
                    Memory<char> buf = buffer.AsMemory();
                    // Print
                    ReadOnlyMemory<char> print = PrintArgument(formatProvider, format, argument, ref buf);
                    // Get padding
                    int padding = placeholder.Alignment == null ? 0 : placeholder.Alignment.Alignment;
                    // Left pad
                    for (int i = print.Length; i < padding; i++) stringBuilder.Append(' ');
                    // Copy chars
                    stringBuilder.Append(print);
                    // Right pad
                    for (int i = print.Length; i < -padding; i++) stringBuilder.Append(' ');
                }
                // Other part
                else stringBuilder.Append(part.Unescaped);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }
    /// <summary></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteTo(ITemplateBreakdown? templateBreakdown, TextWriter textWriter, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // No breakdown
        if (templateBreakdown == null) return;
        // Work buffer
        char[] buffer = ArrayPool<char>.Shared.Rent(4096);
        try
        {
            // Calculate length
            foreach (ITemplatePart part in templateBreakdown.Parts)
            {
                // Placeholder part
                if (part is ITemplatePlaceholderPart placeholder)
                {
                    // Get parameter index
                    int parameterIndex = placeholder.Parameter.ParameterIndex;
                    // Get argument
                    object? argument = arguments == null ? null : parameterIndex >= arguments.Length ? null : arguments[parameterIndex];
                    // Get format
                    ReadOnlyMemory<char> format = placeholder.Formatting == null ? default : placeholder.Formatting.Unescaped;
                    // Restore buffer
                    Memory<char> buf = buffer.AsMemory();
                    // Print
                    ReadOnlyMemory<char> print = PrintArgument(formatProvider, format, argument, ref buf);
                    // Get padding
                    int padding = placeholder.Alignment == null ? 0 : placeholder.Alignment.Alignment;
                    // Left pad
                    for (int i = print.Length; i < padding; i++) textWriter.Write(' ');
                    // Copy chars
                    textWriter.Write(print);
                    // Right pad
                    for (int i = print.Length; i < -padding; i++) textWriter.Write(' ');
                }
                // Other part
                else textWriter.Write(part.Unescaped);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

}
