// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template.Internal;
using Avalanche.Utilities;
using Avalanche.Tokenizer;

/// <summary>Tokenizes string into <see cref="CompositeToken"/>.</summary>
public class PercentTemplateTokenizer : TemplateTextTokenizer
{
    /// <summary>Singleton</summary>
    static PercentTemplateTokenizer instance = new PercentTemplateTokenizer(PlaceholderTokenizer.Instance);
    /// <summary>Singleton</summary>
    public static PercentTemplateTokenizer Instance => instance;

    /// <summary></summary>
    public PercentTemplateTokenizer(ITokenizer<IToken> placeholderTokenizer)
    {
        this.TextTokenizer = PercentEscapeTokenizer.Instance;
        this.placeholderTokenizer = placeholderTokenizer;
        this.malformedTokenizer = Avalanche.Tokenizer.MalformedTokenizer.Instance;
        this.SetReadOnly();
    }

    /// <summary>Percent format text token tokenizer.</summary>
    public class PercentEscapeTokenizer : TokenizerBase<TextToken>
    {
        /// <summary>Singleton</summary>
        static PercentEscapeTokenizer instance = new PercentEscapeTokenizer();
        /// <summary>Singleton</summary>
        public static PercentEscapeTokenizer Instance => instance;

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
                if (c != '%') ix = i + 1;
                // "%%"
                else if (c == '%' && nc == '%') { ix = i + 2; i++; }
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

    /// <summary>Percent format placeholder token tokenizer.</summary>
    public new class PlaceholderTokenizer : TokenizerBase<PlaceholderToken>
    {
        /// <summary>Singleton</summary>
        static PlaceholderTokenizer instance = new PlaceholderTokenizer(Avalanche.Tokenizer.CharTokenizer.Numeric);
        /// <summary>Singleton</summary>
        public static PlaceholderTokenizer Instance => instance;

        /// <summary></summary>
        protected ITokenizer<IToken> parameterTokenizer;

        /// <summary></summary>
        public PlaceholderTokenizer(ITokenizer<IToken> parameterTokenizer)
        {
            this.parameterTokenizer = parameterTokenizer;
        }

        /// <summary>Try take format string.</summary>
        public override bool TryTake(ReadOnlyMemory<char> text, out PlaceholderToken token)
        {
            // Empty length
            if (text.IsEmpty) { token = default!; return false; }
            // Get span
            ReadOnlySpan<char> span = text.Span;
            // Number of chars accepted as token
            int ix = 0;
            // Take '%'
            if (span.Length >= 2 && span[0] == '%') { ix++; span = span.Slice(1); } else { token = default!; return false; }
            // Initialize
            token = new PlaceholderToken();
            // Take parameter
            if (parameterTokenizer.TryTake(text.Slice(ix), out IToken parameterToken)) { token.Parameter = parameterToken; ix += parameterToken.Memory.Length; span = span.Slice(parameterToken.Memory.Length); } else { token = default!; return false; }
            // Don't regard %0 as placeholder
            if (parameterToken.Memory.Length == 1 && parameterToken.Memory.Span[0] == '0') { token = default!; return false; }
            // Return
            token.Memory = text.Slice(0, ix);
            return true;
        }
    }


}
