// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Runtime.Serialization;
using Avalanche.Tokenizer;
using Avalanche.Utilities;

/// <summary>Tokenizes template text, e.g. "Hello, {user}".</summary>
public class TemplateTextTokenizer : TokenizerBase<CompositeToken>, IReadOnly
{
    /// <summary>Is read-only state</summary>
    protected bool @readonly;
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); @readonly = value; } }

    /// <summary></summary>
    protected ITokenizer<IToken> placeholderTokenizer = null!;
    /// <summary></summary>
    protected ITokenizer<IToken> textTokenizer = null!;
    /// <summary></summary>
    protected ITokenizer<IToken> malformedTokenizer = Avalanche.Tokenizer.MalformedTokenizer.Instance;

    /// <summary></summary>
    public ITokenizer<IToken> PlaceholderTokenizer { get => placeholderTokenizer; set => this.AssertWritable().placeholderTokenizer = value; }
    /// <summary></summary>
    public ITokenizer<IToken> TextTokenizer { get => textTokenizer; set => this.AssertWritable().textTokenizer = value; }
    /// <summary></summary>
    public ITokenizer<IToken> MalformedTokenizer { get => malformedTokenizer; set => this.AssertWritable().malformedTokenizer = value; }

    /// <summary></summary>
    public TemplateTextTokenizer() : base() { }
    /// <summary></summary>
    public TemplateTextTokenizer(ITokenizer<IToken> textTokenizer, ITokenizer<IToken> placeholderTokenizer, ITokenizer<IToken> malformedTokenizer) : base() 
    {
        this.TextTokenizer = textTokenizer;
        this.PlaceholderTokenizer = placeholderTokenizer;
        this.MalformedTokenizer = malformedTokenizer;
    }

    /// <summary>Try take format string.</summary>
    public override bool TryTake(ReadOnlyMemory<char> text, out CompositeToken token)
    {
        // Init
        token = text.As<CompositeToken>();
        // Put here tokens
        StructList6<IToken> tokens = new();
        // Read tokens until end
        while (text.Length > 0)
        {
            // Take place holder
            if (placeholderTokenizer.TryTake(text, out IToken placeholderToken))
            {
                // Add to tokens
                tokens.Add(placeholderToken);
                // Slice forward
                text = text.SliceAfter(placeholderToken.Memory);
            }
            // Take text token
            else if (textTokenizer.TryTake(text, out IToken textToken))
            {
                // Add to tokens
                tokens.Add(textToken);
                // Slice forward
                text = text.SliceAfter(textToken.Memory);
            }
            // Take as malformed token
            else if (malformedTokenizer.TryTake(text, out IToken malformedToken))
            {
                // Concatenate to prev token
                if (tokens.Count > 0 && tokens[tokens.Count - 1] is MalformedToken prevToken)
                {
                    malformedToken.Memory = malformedToken.Memory.UnifyStringWith(prevToken.Memory);
                    tokens[tokens.Count - 1] = malformedToken;
                }
                // Add as new token
                else tokens.Add(malformedToken);
                // Slice forward
                text = text.SliceAfter(malformedToken.Memory);
            }
            // Tokenization failed
            else { token = default!; return false; }
        }
        // Assign tokens
        token.Children = tokens.ToArray();
        // 
        return true;
    }
}
