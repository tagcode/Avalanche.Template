// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template.Internal;
using Avalanche.Tokenizer;
using Avalanche.Utilities;

/// <summary>Brace format placeholder token tokenizer.</summary>
public class PlaceholderTokenizer : TokenizerBase<PlaceholderToken>
{
    /// <summary>Singleton</summary>
    static PlaceholderTokenizer alphaNumeric = new PlaceholderTokenizer(
        new ConstantTokenizer("{"),
        Avalanche.Tokenizer.CharTokenizer.AlphaNumeric,
        IntegerTokenizer.Instance,
        Avalanche.Tokenizer.CharTokenizer.NonBrace,
        new ConstantTokenizer("}")
        );

    /// <summary>Singleton</summary>
    static PlaceholderTokenizer numeric = new PlaceholderTokenizer(
        new ConstantTokenizer("{"),
        Avalanche.Tokenizer.CharTokenizer.Numeric,
        IntegerTokenizer.Instance,
        Avalanche.Tokenizer.CharTokenizer.NonBrace,
        new ConstantTokenizer("}")
        );

    /// <summary>Singleton</summary>
    public static PlaceholderTokenizer AlphaNumeric => alphaNumeric;
    /// <summary>Singleton</summary>
    public static PlaceholderTokenizer Numeric => numeric;

    /// <summary>Tokenizer that reads placeholder opener.</summary>
    protected ITokenizer<IToken> openerTokenizer = null!;
    /// <summary></summary>
    protected ITokenizer<IToken> parameterTokenizer;
    /// <summary></summary>
    protected ITokenizer<IToken>? alignmentTokenizer;
    /// <summary></summary>
    protected ITokenizer<IToken>? formattingTokenizer;
    /// <summary>Tokenizer that reads placeholder closer.</summary>
    protected ITokenizer<IToken> closerTokenizer;

    /// <summary>Tokenizer that reads placeholder opener.</summary>
    public virtual ITokenizer<IToken> OpenerTokenizer => openerTokenizer;
    /// <summary></summary>
    public virtual ITokenizer<IToken> ParameterTokenizer => parameterTokenizer;
    /// <summary></summary>
    public virtual ITokenizer<IToken>? AlignmentTokenizer => alignmentTokenizer;
    /// <summary></summary>
    public virtual ITokenizer<IToken>? DormattingTokenizer => formattingTokenizer;
    /// <summary>Tokenizer that reads placeholder closer.</summary>
    public virtual ITokenizer<IToken> CloserTokenizer => closerTokenizer;

    /// <summary></summary>
    public PlaceholderTokenizer(
        ITokenizer<IToken> openerTokenizer, 
        ITokenizer<IToken> parameterTokenizer, 
        ITokenizer<IToken>? alignmentTokenizer, 
        ITokenizer<IToken>? formattingTokenizer,
        ITokenizer<IToken> closerTokenizer)
    {
        this.openerTokenizer = openerTokenizer ?? throw new ArgumentNullException(nameof(openerTokenizer));
        this.parameterTokenizer = parameterTokenizer ?? throw new ArgumentNullException(nameof(parameterTokenizer));
        this.alignmentTokenizer = alignmentTokenizer;
        this.formattingTokenizer = formattingTokenizer;
        this.closerTokenizer = closerTokenizer ?? throw new ArgumentNullException(nameof(closerTokenizer));
    }

    /// <summary>Try take format string.</summary>
    public override bool TryTake(ReadOnlyMemory<char> text, out PlaceholderToken token)
    {
        // Empty length
        if (text.IsEmpty) { token = default!; return false; }
        // Initialize
        token = new PlaceholderToken();
        // Take block opening token, e.g. '{'
        if (!openerTokenizer.TryTake(text, out IToken openerToken)) { token = default!; return false; }
        // Slice
        text = text.SliceAfter(openerToken.Memory);
        // Take parameter
        if (!parameterTokenizer.TryTake(text, out token.Parameter)) { token = default!; return false; }
        // Slice
        text = text.SliceAfter(token.Parameter.Memory);
        // Take ',' for alignment
        if (alignmentTokenizer != null && text.Length > 0 && text.Span[0] == ',')
        {
            // Slice off ','
            text = text.Slice(1);
            // Take alignment
            if (!alignmentTokenizer.TryTake(text, out token.Alignment)) { token = default!; return false; }
            // Slice
            text = text.SliceAfter(token.Alignment.Memory);
        }
        // Take ':' for formatting
        if (formattingTokenizer != null && text.Length >0 && text.Span[0] == ':')
        {
            // Slice off ':'
            text = text.Slice(1);
            // Take alignment
            if (!formattingTokenizer.TryTake(text, out IToken formattingToken)) { token = default!; return false; }
            // Slice
            text = text.SliceAfter(formattingToken.Memory);
        }
        // Take block closing token, e.g. '{'
        if (!closerTokenizer.TryTake(text, out IToken closerToken)) { token = default!; return false; }
        // Slice
        //text = text.SliceAfter(closerToken.Memory);
        // Return
        token.Memory = Avalanche.Utilities.MemoryExtensions.UnifyStringWith(openerToken.Memory, closerToken.Memory);
        return true;
    }
}
