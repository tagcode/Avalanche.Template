// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;
using System.Collections.Concurrent;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>
/// Configurable table of template formats.
/// 
/// <see cref="ByName"/> and <see cref="Detect"/> are assigned on construction, have constant reference, cannot be replaced, and are updated when <see cref="AllFormats"/> is changed.
/// </summary>
public class TemplateFormats : ReadOnlyAssignableClass, ITemplateFormats
{
    /// <summary>All default template formats</summary>
    static Lazy<ITemplateFormats> all = new Lazy<ITemplateFormats>(
        () => new TemplateFormats()
                .SetAllTemplateFormats(
                    ParameterlessTemplateFormat.Instance, 
                    BraceTemplateFormat.Numeric, 
                    BraceTemplateFormat.AlphaNumeric, 
                    BraceTemplateFormat.Auto, 
                    PercentTemplateFormat.Instance
                ).SetReadOnly()
        );
    /// <summary>All default template formats</summary>
    public static ITemplateFormats All => all.Value;

    /// <summary>Shared lock</summary>
    protected object mLock => allFormats.SyncRoot;
    /// <summary>List of all formats</summary>
    protected ArrayList<ITemplateFormat> allFormats = new();
    /// <summary>Format by name</summary>
    protected ConcurrentDictionary<string, ITemplateFormat> byNameCache = new();
    /// <summary>Format by name</summary>
    protected IProvider<string, ITemplateFormat> byName;
    /// <summary>Auto-detect format</summary>
    protected DetectTemplateFormat detect = new DetectTemplateFormat("Detect");

    /// <summary>List of all formats</summary>
    public virtual ITemplateFormat[] AllFormats { get => allFormats.Array; set => setAllFormats(value); }
    /// <summary>Format by name</summary>
    public virtual IProvider<string, ITemplateFormat> ByName { get => byName; set => throw new InvalidOperationException(); }
    /// <summary>Auto-detect format</summary>
    public virtual ITemplateFormat Detect { get => detect; set => throw new InvalidOperationException(); }

    /// <summary>Create formats</summary>
    public TemplateFormats() : base() {
        byName = TemplateFormatsBase.CreateByNameProvider(this).Cached(byNameCache);
    }
    /// <summary>Create formats</summary>
    public TemplateFormats(params ITemplateFormat[] formats) : this() { setAllFormats(formats); }
    /// <summary>Create formats</summary>
    public TemplateFormats(IEnumerable<ITemplateFormat> formats) : this() { setAllFormats(formats); }

    /// <summary>Find template format by name from array snapshot.</summary>
    protected virtual bool findTemplateFormat(string name, out ITemplateFormat templateFormat)
    {
        // Find by name
        foreach (ITemplateFormat _templateFormat in allFormats.Array)
        {
            if (_templateFormat.Name == name)
            {
                templateFormat = _templateFormat;
                return true;
            }
        }
        // No result
        templateFormat = null!;
        return false;
    }

    /// <summary>Overridde this to manage format changes</summary>
    protected virtual void setAllFormats(IEnumerable<ITemplateFormat> formats)
    {
        // Assert writable
        this.AssertWritable();
        // 
        lock (mLock)
        {
            // Assert writable, again
            this.AssertWritable();
            // Assign array
            allFormats.Clear();
            allFormats.AddRange(formats);
            // Get snapshot
            var array = allFormats.Array;
            // Assign snapshot
            this.detect.Formats = array;
            // Clear cache
            byNameCache.Clear();
        }
    }

    /// <summary>Thread safe add</summary>
    public virtual TemplateFormats Add(ITemplateFormat format)
    {
        // 
        this.AssertWritable();
        // Already in array
        if (this.AllFormats.Contains(format)) return this;
        // Add
        lock (mLock)
        {
            // Already in array
            if (this.AllFormats.Contains(format)) return this;
            // Add under lock
            this.AllFormats = ArrayUtilities.Append(this.AllFormats, format);
        }
        // Return
        return this;
    }

    /// <summary>Print names</summary>
    public override string ToString() => String.Join(", ", AllFormats?.Select(f => f.Name) ?? Array.Empty<string>());
}
