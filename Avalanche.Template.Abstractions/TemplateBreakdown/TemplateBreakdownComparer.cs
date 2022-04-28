// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>Compares format strings for equal parts.</summary>
public class TemplateBreakdownComparer : IEqualityComparer<ITemplateBreakdown>, IComparer<ITemplateBreakdown>
{
    /// <summary>Singleton</summary>
    static readonly TemplateBreakdownComparer instance = new TemplateBreakdownComparer();
    /// <summary>Singleton</summary>
    public static TemplateBreakdownComparer Instance => instance;

    /// <summary></summary>
    public int Compare(ITemplateBreakdown? x, ITemplateBreakdown? y)
    {
        // TODO
        string? tx = x?.Text ?? "", ty = y?.Text ?? "";
        int d = StringComparer.Ordinal.Compare(tx, ty);
        return d;
    }

    /// <summary></summary>
    public bool Equals(ITemplateBreakdown? x, ITemplateBreakdown? y)
    {
        // TODO
        return x?.Text == y?.Text;
    }

    /// <summary></summary>
    public int GetHashCode([DisallowNull] ITemplateBreakdown obj)
    {
        // TODO
        string? text = obj.Text;
        if (text == null) return 0;
        return text.GetHashCode();
    }
}
