// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Base class for string format.</summary>
public abstract class TemplateFormatBase : ITemplateFormat
{
    /// <summary>Format name</summary>
    protected string name;
    /// <summary>Provides text of format string</summary>
    protected IProvider<string, ITemplateText> text;
    /// <summary>Provides and caches text of format string using weak-keyed reference cache.</summary>
    protected IProvider<string, ITemplateText> textCached = null!;
    /// <summary>Provides breakdown of format string</summary>
    protected IProvider<string, ITemplateBreakdown> breakdown;
    /// <summary>Provides and caches breakdown of format string using weak-keyed reference cache.</summary>
    protected IProvider<string, ITemplateBreakdown> breakdownCached = null!;
    /// <summary>Assemble format string into string</summary>
    protected IProvider<ITemplateBreakdown, string> assemble = null!;
    /// <summary>Assemble format string into string using weak-keyed reference cache.</summary>
    protected IProvider<ITemplateBreakdown, string> assembleCached = null!;
    /// <summary>Policy whether format uses numeric parameters and assumes parameter order by numeric value.</summary>
    protected bool numberAssignedOrder;
    /// <summary>Text escaper</summary>
    protected IEscaper? escaper;

    /// <summary>Create string format</summary>
    protected TemplateFormatBase(string name)
    {
        this.name = name;
        this.text = Providers.Func<string, ITemplateText>(tryCreate: TryText);
        this.textCached = this.text.AsReadOnly().WeakCached();
        this.breakdown = Providers.Func<string, ITemplateBreakdown>(tryCreate: TryBreakdown);
        this.breakdownCached = this.breakdown.AsReadOnly().WeakCached();
        this.assemble = Providers.Func<ITemplateBreakdown, string>(tryCreate: TryAssemble);
        this.assembleCached = this.assemble.AsReadOnly().WeakCached();
    }

    /// <summary>Format name</summary>
    public string Name { get => name; set => throw new InvalidOperationException(); }
    /// <summary>Provides text of format string</summary>
    public IProvider<string, ITemplateText> Text { get => text; set => throw new InvalidOperationException(); }
    /// <summary>Provides and caches text of format string using weak-keyed reference cache.</summary>
    public IProvider<string, ITemplateText> TextCached { get => textCached; set => throw new InvalidOperationException(); }
    /// <summary>Provides breakdown of format string</summary>
    public IProvider<string, ITemplateBreakdown> Breakdown { get => breakdown; set => throw new InvalidOperationException(); }
    /// <summary>Provides and caches breakdown of format string using weak-keyed reference cache.</summary>
    public IProvider<string, ITemplateBreakdown> BreakdownCached { get => breakdownCached; set => throw new InvalidOperationException(); }
    /// <summary>Assemble format string into string</summary>
    public IProvider<ITemplateBreakdown, string> Assemble { get => assemble; set => throw new InvalidOperationException(); }
    /// <summary>Assemble format string into string using weak-keyed reference cache.</summary>
    public IProvider<ITemplateBreakdown, string> AssembleCached { get => assembleCached; set => throw new InvalidOperationException(); }
    /// <summary>Policy whether format uses numeric parameters and assumes parameter order by numeric value.</summary>
    public bool NumberAssignedOrder { get => numberAssignedOrder; set => throw new InvalidOperationException(); }
    /// <summary>Text escaper</summary>
    public IEscaper? Escaper { get => escaper; set => throw new InvalidOperationException(); }

    /// <summary>Parse text from <paramref name="text"/> into <see cref="ITemplateText"/></summary>
    protected abstract bool TryText(string text, out ITemplateText result);
    /// <summary>Breakdown <paramref name="text"/> into format string</summary>
    protected abstract bool TryBreakdown(string text, out ITemplateBreakdown result);
    /// <summary>Assemble <paramref name="templateBreakdown"/> into text</summary>
    protected abstract bool TryAssemble(ITemplateBreakdown templateBreakdown, out string result);

    /// <summary></summary>
    public override string ToString() => name ?? this.GetType().Name;

    /// <summary>In this version texts will be same as breakdown instances, and they will be cached in same map.</summary>
    public abstract class TextIsBreakdown : TemplateFormatBase
    {
        /// <summary></summary>
        public TextIsBreakdown(string name) : base(name)
        {
            this.textCached = Providers.Func<string, ITemplateText>(tryCreate: TryTextCached);
        }

        /// <summary>Parse text from <paramref name="text"/> into <see cref="ITemplateText"/></summary>
        protected override bool TryText(string text, out ITemplateText result)
        {
            bool ok = this.Breakdown.TryGetValue(text, out ITemplateBreakdown breakdown);
            result = breakdown;
            return ok;
        }

        /// <summary>Parse text from <paramref name="text"/> into <see cref="ITemplateText"/></summary>
        protected virtual bool TryTextCached(string text, out ITemplateText result)
        {
            bool ok = this.BreakdownCached.TryGetValue(text, out ITemplateBreakdown breakdown);
            result = breakdown;
            return ok;
        }
    }
}
