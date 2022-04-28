// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Single parameter emplacement</summary>
public class TemplateParameterEmplacement : ITemplateParameterEmplacement
{
    /// <summary></summary>
    static IEqualityComparer<ITemplateParameterEmplacement[]> arrayComparer = new ArrayEqualityComparer<ITemplateParameterEmplacement>(EqualityComparer<ITemplateParameterEmplacement>.Default);
    /// <summary></summary>
    public static IEqualityComparer<ITemplateParameterEmplacement[]> ArrayComparer => arrayComparer;

    /// <summary>Parameter name</summary>
    protected string parameterName;
    /// <summary>Assigned emplacement</summary>
    protected ITemplateText emplacement;

    /// <summary>Parameter name</summary>
    public string ParameterName { get => parameterName; set => throw new InvalidOperationException(); }
    /// <summary>Assigned emplacement</summary>
    public ITemplateText Emplacement { get => emplacement; set => throw new InvalidOperationException(); }

    /// <summary>Create parameter emplacement</summary>
    public TemplateParameterEmplacement(string parameterName, ITemplateText text)
    {
        this.parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        this.emplacement = text ?? throw new ArgumentNullException(nameof(text));
    }

    /// <summary>Calculate hashcode</summary>
    public override int GetHashCode() => 0x2350 ^ parameterName.GetHashCode() ^ emplacement.GetHashCode();

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Not emplacement
        if (obj is not ITemplateParameterEmplacement other) return false;
        // Disqualify by parameter name
        if (other.ParameterName != this.parameterName) return false;
        // Disqualify by template text
        if (other.Emplacement != this.emplacement && !other.Emplacement.Equals(this.Emplacement)) return false;
        // Equals
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{ParameterName}={Emplacement}";
}
