using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

public class itemplateformats
{
    public static void Run()
    {
        {
            // Create template formats table
            TemplateFormatsBase templateFormats = new TemplateFormatsBase();
            // Assign all
            templateFormats.AllFormats = new ITemplateFormat[] { BraceTemplateFormat.AlphaNumeric, BraceTemplateFormat.Numeric };
            templateFormats.Detect = new DetectTemplateFormat("Detect", templateFormats.AllFormats);
            templateFormats.ByName = TemplateFormatsBase.CreateByNameProvider(templateFormats).Cached();
            // Make immutable
            templateFormats.SetReadOnly();
            var format = templateFormats.ByName["Detect"];
            format = templateFormats.ByName["BraceNumeric"];
        }
        {
            ITemplateFormats templateFormats = new TemplateFormats(BraceTemplateFormat.AlphaNumeric, BraceTemplateFormat.Numeric).SetReadOnly();
            var format = templateFormats.ByName["Detect"];
        }
        {
            TemplateFormats templateFormats = new TemplateFormats()
                .Add(BraceTemplateFormat.AlphaNumeric)
                .Add(BraceTemplateFormat.Numeric)
                .SetReadOnly();
            var format = templateFormats.ByName["Detect"];
        }
        {
            ITemplateFormats templateFormats = new TemplateFormats(BraceTemplateFormat.AlphaNumeric, BraceTemplateFormat.Numeric).SetReadOnly();
            ITemplateFormat templateFormat = templateFormats.ByName["BraceNumeric"];
            var format = templateFormats.ByName["Detect"];
        }
        {
            // Create format table
            ITemplateFormats templateFormats = new TemplateFormats(BraceTemplateFormat.AlphaNumeric, BraceTemplateFormat.Numeric).SetReadOnly();
            // Parse template into breakdown
            ITemplateBreakdown templateText = templateFormats.Detect.Breakdown["Hello {user}, welcome to {location}."];
            var format = templateFormats.ByName["Detect"];
        }
    }
}

