// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template.Internal;
using Avalanche.Tokenizer;
using Avalanche.Utilities;

/// <summary>Token that spans over placeholder</summary>
public struct PlaceholderToken : IToken
{
    /// <summary>Text source</summary>
    public ReadOnlyMemory<char> Memory { get; set; }
    /// <summary>Accept visitor.</summary>
    public bool Accept(ITokenVisitor visitor) { if (visitor is ITokenVisitor<PlaceholderToken> c) { c.Visit(this); return true; } else return false; }

    /// <summary>Parameter token</summary>
    public IToken? Parameter;
    /// <summary>Formatting tokens</summary>
    public IToken? Formatting;
    /// <summary>Alignment token</summary>
    public IToken? Alignment;

    /// <summary>Children of structural token. Each child must be contained in the range of this parent token.</summary>
    public IToken[] Children
    {
        get
        {
            StructList3<IToken> result = new();
            if (Parameter != null && !Parameter.Memory.IsEmpty) result.Add(Parameter);
            if (Alignment != null && !Alignment.Memory.IsEmpty) result.Add(Alignment);
            if (Formatting != null && !Formatting.Memory.IsEmpty) result.Add(Formatting);
            return result.ToArray();
        }
        set => throw new InvalidOperationException();
    }
    /// <summary>Print information</summary>
    public override string ToString() { int index = Memory.Index(); return $"[{index}:{index + Memory.Length}] {GetType().Name} \"{Memory}\""; }
}

