// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using Avalanche.Utilities;

/// <summary>
/// Template text.
/// 
/// Constructs lazily <see cref="Breakdown"/> and <see cref="ParameterNames"/>.
/// </summary>
public class TemplateText : TemplateTextBase
{
    /// <summary>Template breakdown</summary>
    public override ITemplateBreakdown Breakdown { get => breakdown ?? (breakdown = createBreakdown()!); set => this.AssertWritable().breakdown = value; }
    /// <summary>Parameter names</summary>
    public override string?[] ParameterNames { get => parameterNames ?? (parameterNames = createParameterNames() ?? Array.Empty<string?>()); set => this.AssertWritable().parameterNames = value; }

    /// <summary>Create template text</summary>
    public TemplateText() : base() { }
    /// <summary>Create template text</summary>
    public TemplateText(string text, ITemplateFormat? templateFormat = default) : base()
    {
        this.text = text;
        this.templateFormat = templateFormat;
    }

    /// <summary>Create breakdown, for lazy construction</summary>
    protected virtual ITemplateBreakdown? createBreakdown()
    {
        // Get text
        string _text = Text;
        // No text
        if (_text == null) return null;
        // Get template format
        ITemplateFormat? _templateFormat = TemplateFormat;
        // No template format
        if (_templateFormat == null) return null;
        // Breakdown
        if (!_templateFormat.Breakdown.TryGetValue(_text, out ITemplateBreakdown templateBreakdown0)) return null;
        // Return breakdown
        return templateBreakdown0;
    }

    /// <summary>Create parameter names, for lazy construction</summary>
    protected virtual string?[]? createParameterNames()
    {
        // Get breakdown
        ITemplateBreakdown? _breakdown = Breakdown;
        // No breakdown
        if (_breakdown == null) return null;
        // Get parameters
        ITemplateParameterPart?[]? _parameters = _breakdown.Parameters;
        // No parameters
        if (_parameters == null) return null;
        // Allocate strings
        string?[] names = new string[_parameters.Length];
        // Assign
        for (int i = 0; i < names.Length; i++) if (_parameters[i] != null) names[i] = _parameters[i]!.Unescaped.AsString();
        // Return
        return names;
    }
}
