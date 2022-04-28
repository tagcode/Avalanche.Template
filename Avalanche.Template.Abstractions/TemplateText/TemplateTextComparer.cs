// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>Compares format strings for equal parts.</summary>
public class TemplateTextComparer : IEqualityComparer<ITemplateText>, IComparer<ITemplateText>
{
    /// <summary>Singleton</summary>
    static readonly TemplateTextComparer instance = new TemplateTextComparer();
    /// <summary>Singleton</summary>
    public static TemplateTextComparer Instance => instance;

    /// <summary></summary>
    public int Compare(ITemplateText? x, ITemplateText? y)
    {
        // TODO
        string? tx = x?.Text ?? "", ty = y?.Text ?? "";
        int d = StringComparer.Ordinal.Compare(tx, ty);
        return d;
    }

    /// <summary></summary>
    public bool Equals(ITemplateText? x, ITemplateText? y)
    {
        // TODO
        return x?.Text == y?.Text;
    }

    /// <summary></summary>
    public int GetHashCode([DisallowNull] ITemplateText obj)
    {
        // TODO
        string? text = obj.Text;
        if (text == null) return 0;
        return text.GetHashCode();
    }
}
