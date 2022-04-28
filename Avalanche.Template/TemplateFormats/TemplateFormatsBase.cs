// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Template formats table where formats configurable and accessible by name.</summary>
public class TemplateFormatsBase : ReadOnlyAssignableClass, ITemplateFormats, ICached
{
    /// <summary>List of all formats</summary>
    protected ITemplateFormat[] allFormats = null!;
    /// <summary>Format by name</summary>
    protected IProvider<string, ITemplateFormat> byName = null!;
    /// <summary>Auto-detect format</summary>
    protected ITemplateFormat detect = null!;

    /// <summary>List of all formats</summary>
    public virtual ITemplateFormat[] AllFormats { get => allFormats; set => this.AssertWritable().setAllFormats(value); }
    /// <summary>Format by name</summary>
    public virtual IProvider<string, ITemplateFormat> ByName { get => byName; set => this.AssertWritable().byName = value; }
    /// <summary>Auto-detect format</summary>
    public virtual ITemplateFormat Detect { get => detect; set => this.AssertWritable().detect = value; }
    /// <summary></summary>
    bool ICached.IsCached { get => true; set => throw new InvalidOperationException(); }

    /// <summary>Overridde this to manage format changes</summary>
    protected virtual void setAllFormats(ITemplateFormat[] formats)
    {
        this.allFormats = formats;
    }

    /// <summary>Print names</summary>
    public override string ToString() => String.Join(", ", AllFormats?.Select(f => f.Name) ?? Array.Empty<string>());

    /// <summary>Create find by name provider that compares to <see cref="ITemplateFormats.AllFormats"/>. Does not cache.</summary>
    public static IProvider<string, ITemplateFormat> CreateByNameProvider(ITemplateFormats templateFormats) => new ByNameProvider(templateFormats);

    /// <summary></summary>
    public virtual void InvalidateCache(bool deep = false)
    {
        ByName.InvalidateCache(deep);
    }

    /// <summary>Simple <see cref="ByName"/> lookup provider</summary>
    class ByNameProvider : ProviderBase<string, ITemplateFormat>
    {
        /// <summary></summary>
        ITemplateFormats templateFormats;
        /// <summary></summary>
        public ByNameProvider(ITemplateFormats templateFormats)
        {
            this.templateFormats = templateFormats ?? throw new ArgumentNullException(nameof(templateFormats));
        }

        /// <summary>Find by <paramref name="name"/>. Compares to <see cref="ITemplateFormats.AllFormats"/>.</summary>
        /// <param name="name">Template format to seach</param>
        /// <param name="result">Template format result</param>
        public override bool TryGetValue(string name, out ITemplateFormat result)
        {
            //
            var array = templateFormats.AllFormats;
            // Find by name
            foreach (ITemplateFormat _templateFormat in array)
            {
                // Found by name
                if (_templateFormat.Name == name) { result = _templateFormat; return true; }
            }
            // Compare to detect
            var _detect = templateFormats.Detect;
            if (_detect!=null && _detect.Name == name) { result = _detect; return true; }
            // No result
            result = null!;
            return false;

        }
    }
}
