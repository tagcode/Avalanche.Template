// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

/// <summary>Extension methods for <see cref="ITemplateBreakdown"/>.</summary>
public static class TemplateBreakdownExtensions_
{
    /// <summary>Get string.Format() compatible template string</summary>
    public static string FormatTemplate(this ITemplateBreakdown formatString)
    {
        // Get format
        ITemplateFormat? format = formatString.TemplateFormat;
        // Parameterless text
        if (format == TemplateFormat.Parameterless) return formatString.Text;
        // Numeric placeholder '{0}'
        if (format == TemplateFormat.BraceNumeric) return formatString.Text;
        // Fallback reconstruction
        return TemplateFormat.BraceNumeric.AssembleCached[formatString];
    }

    /// <summary>Get ILogger compatible template string</summary>
    public static string LoggerTemplate(this ITemplateBreakdown formatString)
    {
        // Get format
        ITemplateFormat? format = formatString.TemplateFormat;
        // Parameterless text
        if (format == TemplateFormat.Parameterless) return formatString.Text;
        // AlphaNumeric placeholder '{user}'
        if (format == TemplateFormat.BraceAlphaNumeric) return formatString.Text;
        // Fallback reconstruction
        return TemplateFormat.BraceAlphaNumeric.AssembleCached[formatString];
    }

    /// <summary>Assign parameters into parameter array</summary>
    public static R[] AssignByParameterIndex<T, R>(IEnumerable<T> parameters, Func<T, int> parameterIndexSelector, Func<T, R> resultSelector)
    {
        // Place highest parameter index
        int highestParameterIx = int.MinValue;
        int parameterCount = 0;
        // Find highest parameter index and parameter count
        if (parameters != null)
            foreach (T parameter in parameters)
            {
                // Increment count
                parameterCount++;
                // Get parameter index
                int parameterIx = parameterIndexSelector(parameter);
                // Up ix
                if (parameterIx > highestParameterIx) highestParameterIx = parameterIx;
            }
        // No parameters
        if (highestParameterIx < 0) return Array.Empty<R>();
        // Parameter array length
        int parameterLength = highestParameterIx + 1;
        // Fallback to occurance order: Too high parameter index
        if (parameterLength < 0 || parameterLength > parameterCount + 8)
        {
            // Allocate
            R[] result = new R[parameterCount];
            // Index
            int ix = 0;
            // Assign
            if (parameters != null) foreach (T parameter in parameters) result[ix++] = resultSelector(parameter);
            // Return
            return result;
        }
        // Assign by parameter index
        else
        {
            // Allocate
            R[] result = new R[parameterLength];
            // Assign
            if (parameters != null)
                foreach (T parameter in parameters)
                {
                    // Get parameter index
                    int parameterIx = parameterIndexSelector(parameter);
                    // Assign parameter
                    if (parameterIx >= 0 && parameterIx < parameterLength) result[parameterIx] = resultSelector(parameter);
                }
            // Return
            return result;
        }
    }
}
