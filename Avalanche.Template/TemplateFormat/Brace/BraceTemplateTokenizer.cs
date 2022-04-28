// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template.Internal;
using Avalanche.Utilities;
using Avalanche.Tokenizer;

/// <summary>Tokenizes string into <see cref="CompositeToken"/>.</summary>
public class BraceTemplateTokenizer : TemplateTextTokenizer
{
    /// <summary>Singleton</summary>
    static BraceTemplateTokenizer alphaNumeric = new BraceTemplateTokenizer(Avalanche.Template.Internal.PlaceholderTokenizer.AlphaNumeric);
    /// <summary>Singleton</summary>
    static BraceTemplateTokenizer numeric = new BraceTemplateTokenizer(Avalanche.Template.Internal.PlaceholderTokenizer.Numeric);
    /// <summary>Singleton</summary>
    public static BraceTemplateTokenizer AlphaNumeric => alphaNumeric;
    /// <summary>Singleton</summary>
    public static BraceTemplateTokenizer Numeric => numeric;

    /// <summary></summary>
    public BraceTemplateTokenizer(ITokenizer<IToken> placeholderTokenizer)
    {
        this.TextTokenizer = BraceEscapeTokenizer.Instance;
        this.placeholderTokenizer = placeholderTokenizer;
        this.malformedTokenizer = Avalanche.Tokenizer.MalformedTokenizer.Instance;
        this.SetReadOnly();
    }

    /// <summary>Brace format text token tokenizer.</summary>
    public class BraceEscapeTokenizer : TokenizerBase<TextToken>
    {
        /// <summary>Singleton</summary>
        static BraceEscapeTokenizer instance = new BraceEscapeTokenizer();
        /// <summary>Singleton</summary>
        public static BraceEscapeTokenizer Instance => instance;

        /// <summary>Try take format string.</summary>
        public override bool TryTake(ReadOnlyMemory<char> text, out TextToken token)
        {
            // Empty length
            if (text.IsEmpty) { token = default!; return false; }
            // Get span
            ReadOnlySpan<char> span = text.Span;
            // Number of chars accepted as token
            int ix = 0;
            // Scan valid chars
            for (int i = 0; i < span.Length; i++)
            {
                // Get char and next char
                char c = span[i], nc = i < span.Length - 1 ? span[i + 1] : '\0';
                // Accept char
                if (c != '{' && c != '}') ix = i + 1;
                // "{{"
                else if (c == '{' && nc == '{') { ix = i + 2; i++; }
                // "}}"
                else if (c == '}' && nc == '}') { ix = i + 2; i++; }
                //
                else break;
            }
            // No accepted chars
            if (ix == 0) { token = default!; return false; }
            // Return
            token = text.As<TextToken>(0, ix);
            return true;
        }
    }

}
