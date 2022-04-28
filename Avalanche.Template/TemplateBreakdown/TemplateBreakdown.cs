// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using Avalanche.Utilities;

/// <summary></summary>
public class TemplateBreakdown : TemplateBreakdownBase, ITemplateBreakdown
{
    /*
    /// <summary>Operation successful</summary>
    static IMessageDescription s_ok = new MessageDescription("NULL.S_OK", 0x00000000, new ParameterlessText("Operation successful")).SetHResult(0x00000000).SetDescription("Operation successful.");
    /// <summary>Object is invalid state</summary>
    static IMessageDescription invalidOperationException = new MessageDescription().SetCode(0x80131509).SetHResult(0x80131509).SetKey("Arg_InvalidOperationException").SetDescription("Object is invalid state").SetTemplate(new ParameterlessText("Invalid Operation")).SetException(typeof(InvalidOperationException)).SetReadOnly();
    /// <summary>One or more arguments are not valid</summary>
    static IMessageDescription argumentException = new MessageDescription().SetCode(0x80070057).SetHResult(0x80070057).SetKey("Arg_ArgumentException").SetDescription("Arguments is not valid.").SetTemplate(new FormatText("Invalid argument '{0}'")).SetException(typeof(ArgumentException)).SetReadOnly();
    /// <summary>Malformed format string.</summary>
    static IMessageDescription malformedFormat = new MessageDescription().SetCode(0x80131537).SetHResult(0x80131537).SetKey("COR_E_FORMAT").SetDescription("Malformed format text").SetTemplate(new FormatText("Malformed \"{0}\" at {1}:{2}")).SetException(typeof(FormatException)).SetReadOnly();
    */
    /// <summary>Placeholders in order of occurance in <see cref="ITemplateText.Text"/>.</summary>
    public override ITemplatePlaceholderPart[] Placeholders { get => placeholders ?? (placeholders = createPlaceholdersArray()!); set => this.AssertWritable().placeholders = value; }
    /// <summary>Parameters in order of <see cref="ITemplateParameterPart.ParameterIndex"/>.</summary>
    public override ITemplateParameterPart?[] Parameters { get => parameters ?? (parameters = createParametersArray()!); set => this.AssertWritable().parameters = value; }
    /// <summary>Parameters names in order of <see cref="ITemplateParameterPart.ParameterIndex"/>.</summary>
    public override string?[] ParameterNames { get => parameterNames ?? (parameterNames = createParameterNamesArray()!); set => this.AssertWritable().parameterNames = value; }

    /// <summary>Create placeholders array, for lazy initialization.</summary>
    protected virtual ITemplatePlaceholderPart[]? createPlaceholdersArray()
    {
        // Place here placeholder count
        int count = 0;
        // Get reference
        var parts = this.parts;
        // Count
        foreach (ITemplatePart part in parts) if (part is ITemplatePlaceholderPart) count++;
        // No placeholders
        if (count == 0) return Array.Empty<ITemplatePlaceholderPart>();
        // Allocate
        ITemplatePlaceholderPart[] placeholders = new ITemplatePlaceholderPart[count];
        // Assign
        int ix = 0;
        foreach (ITemplatePart part in parts) if (part is ITemplatePlaceholderPart placeholder) placeholders[ix++] = placeholder;
        // Return
        return placeholders;
    }

    /// <summary>Create parameters array, for lazy initialization.</summary>
    protected virtual ITemplateParameterPart?[]? createParametersArray()
    {
        // Index of last found argument
        int highestParameterIx = int.MinValue;
        // Get reference
        var parts = this.parts;
        // Count placeholder count
        int placeholderCount = 0;
        // Find highest parameter index
        foreach (ITemplatePart part in parts)
        {
            // Cast to placeholder
            if (part is not ITemplatePlaceholderPart placeholder0) continue;
            // Increment count
            placeholderCount++;
            // Up highest index
            if (placeholder0.Parameter.ParameterIndex > highestParameterIx) highestParameterIx = placeholder0.Parameter.ParameterIndex;
        }
        // No arguments
        if (highestParameterIx == int.MinValue) return Array.Empty<ITemplateParameterPart>();
        // Parameter count
        int parameterCount = highestParameterIx + 1;
        // Fallback to occurance order: Too high parameter index
        if (parameterCount < 0 || parameterCount > placeholderCount + 8)
        {
            // Allocate
            ITemplateParameterPart[] parameters = new ITemplateParameterPart[placeholderCount];
            // Index
            int ix = 0;
            // Assign
            foreach (ITemplatePart part in parts) if (part is ITemplatePlaceholderPart placeholder) parameters[ix++] = placeholder.Parameter;
            // Return
            return parameters;
        }
        // Index order
        else
        {
            // Allocate
            ITemplateParameterPart[] parameters = new ITemplateParameterPart[parameterCount];
            // Assign
            foreach (ITemplatePart part in parts) if (part is ITemplatePlaceholderPart placeholder) parameters[placeholder.Parameter.ParameterIndex] = placeholder.Parameter;
            // Return
            return parameters;
        }
    }

    /// <summary>Create parameter names, lazy construction</summary>
    protected virtual string?[]? createParameterNamesArray()
    {
        // Get parameters
        ITemplateParameterPart?[]? _parameters = Parameters;
        // No parameters
        if (_parameters == null) return null;
        // Allocate strings
        string?[] names = new string[_parameters.Length];
        // Assign
        for (int i = 0; i < names.Length; i++) names[i] = _parameters[i]?.Unescaped.AsString();
        // Return
        return names;
    }
}
