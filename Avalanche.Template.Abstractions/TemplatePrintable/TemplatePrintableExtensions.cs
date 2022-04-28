// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>Extension methods for <see cref="ITemplatePrintable"/>.</summary>
public static class TemplatePrintableExtensions
{
    /// <summary>Print <paramref name="templatePrintable"/> to <paramref name="dst"/>.</summary>
    /// <returns>Number of characters written to <paramref name="dst"/>, or -1 if failed</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int PrintTo(this ITemplatePrintable templatePrintable, Span<char> dst, object?[]? arguments)
    {
        //
        if (templatePrintable == null) return -1;
        // Try print
        if (templatePrintable.TryPrintTo(dst, out int length, arguments)) return length;
        // Failed
        return -1;
    }

    /// <summary>Try estimate print length</summary>
    /// <exception cref="InvalidOperationException">If estimate fialed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int EstimatePrintLength(this ITemplatePrintable templatePrintable, object?[]? arguments)
    {
        // Try estimate
        if (templatePrintable.TryEstimatePrintLength(out int length, arguments)) return length;
        // Failed
        throw new InvalidOperationException("Failed to estimate length.");
    }

    /// <summary>Print <paramref name="argumentMap"/> in placeholders.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static string Print(this ITemplatePrintable templatePrintable, IDictionary<string, object?>? argumentMap)
    {
        // Get parameter names
        string?[] parameterNames = templatePrintable.ParameterNames;
        // Rent arguments
        object?[] arguments = ArrayPool<object?>.Shared.Rent(parameterNames.Length);
        //
        try
        {
            //
            for (int i = 0; i < parameterNames.Length; i++)
            {
                // Get parameter name
                string? parameterName = parameterNames[i];
                // No parameter name
                if (parameterName == null) continue;
                // Get argument
                object? argument = argumentMap == null ? null : argumentMap.TryGetValue(parameterName, out object? value) ? value : null;
                // Assign to array
                arguments[i] = argument;
            }
            // Call forward
            string print = templatePrintable.Print(arguments);
            // Return print
            return print;
        }
        finally
        {
            // Return rental
            ArrayPool<object?>.Shared.Return(arguments);
        }
    }

    /// <summary>Append to <paramref name="sb"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static void AppendTo(this ITemplatePrintable templatePrintable, StringBuilder sb, IDictionary<string, object?>? argumentMap)
    {
        // Get parameter names
        string?[] parameterNames = templatePrintable.ParameterNames;
        // Rent arguments
        object?[] arguments = ArrayPool<object?>.Shared.Rent(parameterNames.Length);
        //
        try
        {
            //
            for (int i = 0; i < parameterNames.Length; i++)
            {
                // Get parameter name
                string? parameterName = parameterNames[i];
                // No parameter name
                if (parameterName == null) continue;
                // Get argument
                object? argument = argumentMap == null ? null : argumentMap.TryGetValue(parameterName, out object? value) ? value : null;
                // Assign to array
                arguments[i] = argument;
            }
            // Call forward
            templatePrintable.AppendTo(sb, arguments);
        }
        finally
        {
            // Return rental
            ArrayPool<object?>.Shared.Return(arguments);
        }
    }

    /// <summary>Write to <paramref name="textWriter"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static void WriteTo(this ITemplatePrintable templatePrintable, TextWriter textWriter, IDictionary<string, object?>? argumentMap)
    {
        // Get parameter names
        string?[] parameterNames = templatePrintable.ParameterNames;
        // Rent arguments
        object?[] arguments = ArrayPool<object?>.Shared.Rent(parameterNames.Length);
        //
        try
        {
            //
            for (int i = 0; i < parameterNames.Length; i++)
            {
                // Get parameter name
                string? parameterName = parameterNames[i];
                // No parameter name
                if (parameterName == null) continue;
                // Get argument
                object? argument = argumentMap == null ? null : argumentMap.TryGetValue(parameterName, out object? value) ? value : null;
                // Assign to array
                arguments[i] = argument;
            }
            // Call forward
            templatePrintable.WriteTo(textWriter, arguments);
        }
        finally
        {
            // Return rental
            ArrayPool<object?>.Shared.Return(arguments);
        }
    }

    /// <summary>Try print <paramref name="argumentMap"/> using <paramref name="formatProvider"/> into <paramref name="dst"/>.</summary>
    /// <param name="length">Number of characters written to <paramref name="dst"/>. If failed, the 0 is returned.</param>
    /// <returns>True if text was written, false if write failed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static bool TryPrintTo(this ITemplatePrintable templatePrintable, Span<char> dst, out int length, IDictionary<string, object?>? argumentMap)
    {
        // Get parameter names
        string?[] parameterNames = templatePrintable.ParameterNames;
        // Rent arguments
        object?[] arguments = ArrayPool<object?>.Shared.Rent(parameterNames.Length);
        //
        try
        {
            //
            for (int i = 0; i < parameterNames.Length; i++)
            {
                // Get parameter name
                string? parameterName = parameterNames[i];
                // No parameter name
                if (parameterName == null) continue;
                // Get argument
                object? argument = argumentMap == null ? null : argumentMap.TryGetValue(parameterName, out object? value) ? value : null;
                // Assign to array
                arguments[i] = argument;
            }
            // Call forward
            bool ok = templatePrintable.TryPrintTo(dst, out length, arguments);
            // Return
            return ok;
        }
        finally
        {
            // Return rental
            ArrayPool<object?>.Shared.Return(arguments);
        }
    }

    /// <summary>Try estimate print length</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static bool TryEstimatePrintLength(this ITemplatePrintable templatePrintable, out int length, IDictionary<string, object?>? argumentMap)
    {
        // Get parameter names
        string?[] parameterNames = templatePrintable.ParameterNames;
        // Rent arguments
        object?[] arguments = ArrayPool<object?>.Shared.Rent(parameterNames.Length);
        //
        try
        {
            //
            for (int i = 0; i < parameterNames.Length; i++)
            {
                // Get parameter name
                string? parameterName = parameterNames[i];
                // No parameter name
                if (parameterName == null) continue;
                // Get argument
                object? argument = argumentMap == null ? null : argumentMap.TryGetValue(parameterName, out object? value) ? value : null;
                // Assign to array
                arguments[i] = argument;
            }
            // Call forward
            bool ok = templatePrintable.TryEstimatePrintLength(out length, arguments);
            //
            return ok;
        }
        finally
        {
            // Return rental
            ArrayPool<object?>.Shared.Return(arguments);
        }
    }

    /// <summary>Print <paramref name="templatePrintable"/> to <paramref name="dst"/>.</summary>
    /// <returns>Number of characters written to <paramref name="dst"/>, or -1 if failed</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int PrintTo(this ITemplatePrintable templatePrintable, Span<char> dst, IDictionary<string, object?>? argumentMap)
    {
        // Get parameter names
        string?[] parameterNames = templatePrintable.ParameterNames;
        // Rent arguments
        object?[] arguments = ArrayPool<object?>.Shared.Rent(parameterNames.Length);
        //
        try
        {
            //
            for (int i = 0; i < parameterNames.Length; i++)
            {
                // Get parameter name
                string? parameterName = parameterNames[i];
                // No parameter name
                if (parameterName == null) continue;
                // Get argument
                object? argument = argumentMap == null ? null : argumentMap.TryGetValue(parameterName, out object? value) ? value : null;
                // Assign to array
                arguments[i] = argument;
            }
            // Call forward
            bool ok = templatePrintable.TryPrintTo(dst, out int length, arguments);
            // Return
            return ok ? length : -1;
        }
        finally
        {
            // Return rental
            ArrayPool<object?>.Shared.Return(arguments);
        }
    }

    /// <summary>Try estimate print length</summary>
    /// <exception cref="InvalidOperationException">If estimate fialed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int EstimatePrintLength(this ITemplatePrintable templatePrintable, IDictionary<string, object?>? argumentMap)
    {
        // Get parameter names
        string?[] parameterNames = templatePrintable.ParameterNames;
        // Rent arguments
        object?[] arguments = ArrayPool<object?>.Shared.Rent(parameterNames.Length);
        //
        try
        {
            //
            for (int i = 0; i < parameterNames.Length; i++)
            {
                // Get parameter name
                string? parameterName = parameterNames[i];
                // No parameter name
                if (parameterName == null) continue;
                // Get argument
                object? argument = argumentMap == null ? null : argumentMap.TryGetValue(parameterName, out object? value) ? value : null;
                // Assign to array
                arguments[i] = argument;
            }
            // Call forward
            bool ok = templatePrintable.TryEstimatePrintLength(out int length, arguments);
            // Return
            return ok ? length : -1;
        }
        finally
        {
            // Return rental
            ArrayPool<object?>.Shared.Return(arguments);
        }
    }

}
