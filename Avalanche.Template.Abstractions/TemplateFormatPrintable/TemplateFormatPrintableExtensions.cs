// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="ITemplateFormatPrintable"/>.</summary>
public static class TemplateFormatPrintableExtensions
{
    /// <summary>Decorates <paramref name="templatePrintable"/> to use <paramref name="formatProvider"/>.</summary>
    public static ITemplatePrintable WithFormat(this ITemplateFormatPrintable templatePrintable, IFormatProvider formatProvider)
    {
        // Already decorated
        if (templatePrintable is TemplatePrintableFormatDecorated decorated)
        {
            // Same
            if (decorated.FormatProvider == formatProvider) return decorated;
            // Recdecorate
            return new TemplatePrintableFormatDecorated(decorated.TemplatePrintable, formatProvider);
        }
        // Decorate
        return new TemplatePrintableFormatDecorated(templatePrintable, formatProvider);
    }

    /// <summary>Print <paramref name="templatePrintable"/> to <paramref name="dst"/>.</summary>
    /// <returns>Number of characters written to <paramref name="dst"/>, or -1 if failed</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int PrintTo(this ITemplateFormatPrintable templatePrintable, Span<char> dst, IFormatProvider? formatProvider, object?[]? arguments)
    {
        //
        if (templatePrintable == null) return -1;
        // Try print
        if (templatePrintable.TryPrintTo(dst, out int length, formatProvider, arguments)) return length;
        // Failed
        return -1;
    }

    /// <summary>Try estimate print length</summary>
    /// <exception cref="InvalidOperationException">If estimate fialed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int EstimatePrintLength(this ITemplateFormatPrintable templatePrintable, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // Try estimate
        if (templatePrintable.TryEstimatePrintLength(out int length, formatProvider, arguments)) return length;
        // Failed
        throw new InvalidOperationException("Failed to estimate length.");
    }

    /// <summary>Print <paramref name="argumentMap"/> in placeholders using <paramref name="formatProvider"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static string Print(this ITemplateFormatPrintable templatePrintable, IFormatProvider? formatProvider, IDictionary<string, object?>? argumentMap)
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
            string print = templatePrintable.Print(formatProvider, arguments);
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
    public static void AppendTo(this ITemplateFormatPrintable templatePrintable, StringBuilder sb, IFormatProvider? formatProvider, IDictionary<string, object?>? argumentMap)
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
            templatePrintable.AppendTo(sb, formatProvider, arguments);
        }
        finally
        {
            // Return rental
            ArrayPool<object?>.Shared.Return(arguments);
        }
    }

    /// <summary>Write to <paramref name="textWriter"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static void WriteTo(this ITemplateFormatPrintable templatePrintable, TextWriter textWriter, IFormatProvider? formatProvider, IDictionary<string, object?>? argumentMap)
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
            templatePrintable.WriteTo(textWriter, formatProvider, arguments);
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
    public static bool TryPrintTo(this ITemplateFormatPrintable templatePrintable, Span<char> dst, out int length, IFormatProvider? formatProvider, IDictionary<string, object?>? argumentMap)
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
            bool ok = templatePrintable.TryPrintTo(dst, out length, formatProvider, arguments);
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
    public static bool TryEstimatePrintLength(this ITemplateFormatPrintable templatePrintable, out int length, IFormatProvider? formatProvider, IDictionary<string, object?>? argumentMap)
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
            bool ok = templatePrintable.TryEstimatePrintLength(out length, formatProvider, arguments);
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
    public static int PrintTo(this ITemplateFormatPrintable templatePrintable, Span<char> dst, IFormatProvider? formatProvider, IDictionary<string, object?>? argumentMap)
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
            bool ok = templatePrintable.TryPrintTo(dst, out int length, formatProvider, arguments);
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
    public static int EstimatePrintLength(this ITemplateFormatPrintable templatePrintable, IFormatProvider? formatProvider, IDictionary<string, object?>? argumentMap)
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
            bool ok = templatePrintable.TryEstimatePrintLength(out int length, formatProvider, arguments);
            // Return
            return ok ? length : -1;
        }
        finally
        {
            // Return rental
            ArrayPool<object?>.Shared.Return(arguments);
        }
    }

    /// <summary>Template printable</summary>
    class TemplatePrintableFormatDecorated : ITemplatePrintable, IDecoration
    {
        /// <summary></summary>
        protected ITemplateFormatPrintable templatePrintable;
        /// <summary></summary>
        protected IFormatProvider formatProvider;
        /// <summary></summary>
        public ITemplateFormatPrintable TemplatePrintable => templatePrintable;
        /// <summary></summary>
        public IFormatProvider FormatProvider => formatProvider;

        /// <summary></summary>
        public TemplatePrintableFormatDecorated(ITemplateFormatPrintable templatePrintable, IFormatProvider formatProvider)
        {
            this.templatePrintable = templatePrintable ?? throw new ArgumentNullException(nameof(templatePrintable));
            this.formatProvider = formatProvider ?? throw new ArgumentNullException(nameof(formatProvider));
        }

        /// <summary></summary>
        public string?[] ParameterNames { get => templatePrintable.ParameterNames; set => templatePrintable.ParameterNames = value; }
        /// <summary></summary>
        public bool IsDecoration { get => true; set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public object? Decoree { get => templatePrintable; set => throw new InvalidOperationException(); }

        /// <summary></summary>
        public void AppendTo(StringBuilder sb, object?[]? arguments = null) => templatePrintable.AppendTo(sb, formatProvider, arguments);
        /// <summary></summary>
        public string Print(object?[]? arguments = null) => templatePrintable.Print(formatProvider, arguments);
        /// <summary></summary>
        public bool TryEstimatePrintLength(out int length, object?[]? arguments = null) => templatePrintable.TryEstimatePrintLength(out length, formatProvider, arguments);
        /// <summary></summary>
        public bool TryPrintTo(Span<char> dst, out int length, object?[]? arguments = null) => templatePrintable.TryPrintTo(dst, out length, formatProvider, arguments);
        /// <summary></summary>
        public void WriteTo(TextWriter textWriter, object?[]? arguments = null) => templatePrintable.WriteTo(textWriter, formatProvider, arguments);
        /// <summary></summary>
        public override string? ToString() => templatePrintable?.ToString();
    }

}
