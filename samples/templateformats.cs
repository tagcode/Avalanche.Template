using System.Globalization;
using Avalanche.Template;
using static System.Console;

public class templateformats
{
    public static void Run()
    {
        {
            ITemplateFormats templateFormats = TemplateFormats.All;
            var format = templateFormats.ByName["Detect"];
        }
        {
            ITemplateFormat templateFormat = TemplateFormat.BraceNumeric;
            ITemplateBreakdown breakdown1 = templateFormat.Breakdown["Today is {1}. Welcome, {0}"];
        }
        {
            ITemplateFormat templateFormat = TemplateFormat.BraceAlphaNumeric;
            ITemplateBreakdown breakdown2 = templateFormat.Breakdown["Today is {time}. Welcome, {user}"];
        }
        {
            ITemplateFormat templateFormat = TemplateFormat.Brace;
            ITemplateBreakdown breakdown2 = templateFormat.Breakdown["Today is {time}. Welcome, {user}"];
        }
        {
            ITemplateBreakdown breakdown1 = TemplateFormat.BraceNumeric.Breakdown["Today is {1}. Welcome, {0}"];
            ITemplateBreakdown breakdown2 = TemplateFormat.BraceAlphaNumeric.Breakdown["Today is {time}. Welcome, {user}"];
            WriteLine(breakdown1.Print(null, new object?[] { "User", DateTime.Now })); // "Today is 5.1.2022 20.02.40. Welcome, User"
            WriteLine(breakdown2.Print(null, new object?[] { DateTime.Now, "User" })); // "Today is 5.1.2022 20.02.40. Welcome, User"
        }
        {
            ITemplateBreakdown breakdown1 = TemplateFormat.Percent.Breakdown["Today is %2. Welcome, %1"];
            WriteLine(breakdown1.Print(null, new object?[] { "User", DateTime.Now })); // "Today is 5.1.2022 20.02.40. Welcome, User"
            WriteLine(TemplateFormat.BraceNumeric.Assemble[breakdown1]); // "Today is {1}. Welcome, {0}"
        }

        {
            ITemplateBreakdown breakdown = TemplateFormat.BraceAlphaNumeric.Breakdown["Error 0x{code:X8}."];
            WriteLine(breakdown.Print(CultureInfo.InvariantCulture, new object?[] { 0x100 })); // "Error 0x00000100."
        }
        {
            ITemplateBreakdown breakdown = TemplateFormat.BraceAlphaNumeric.Breakdown["User \"{user,13}\""];
            WriteLine(breakdown.Print(CultureInfo.InvariantCulture, new object?[] { "Donald Duck" })); // "User "  Donald Duck""
        }
        {
            ITemplateBreakdown breakdown = TemplateFormat.BraceAlphaNumeric.Breakdown["Welcome, {{{user}}}"];
            WriteLine(breakdown.Print(CultureInfo.InvariantCulture, new object?[] { "Donald Duck" })); // "Welcome, {Donald Duck}"
        }
        {
            ITemplateBreakdown breakdown = TemplateFormat.Parameterless.Breakdown["void Main() {}"];
            WriteLine(breakdown.Print(null, arguments: (object?[]?)null)); // "void Main() {}"
        }
        {
            ITemplateBreakdown breakdown = TemplateFormat.Parameterless.Breakdown["void Main() {}"];
            // Reassembled is escaped
            WriteLine(TemplateFormat.Brace.Assemble[breakdown]); // "void Main() {{}}"
        }
        {
            ITemplateBreakdown breakdown0 = TemplateFormats.All.Detect.Breakdown["Today is now. Welcome, user"];
            ITemplateBreakdown breakdown1 = TemplateFormats.All.Detect.Breakdown["Today is {0}. Welcome, {1}"];
            ITemplateBreakdown breakdown2 = TemplateFormats.All.Detect.Breakdown["Today is {time}. Welcome, {user}"];
            ITemplateBreakdown breakdown3 = TemplateFormats.All.Detect.Breakdown["Today is %1. Welcome, %2"];

            WriteLine(breakdown0.TemplateFormat!.Name); // "Parameterless"
            WriteLine(breakdown1.TemplateFormat!.Name); // "BraceNumeric"
            WriteLine(breakdown2.TemplateFormat!.Name); // "BraceAlphaNumeric"
            WriteLine(breakdown3.TemplateFormat!.Name); // "Percent"

            WriteLine(TemplateFormats.All.Detect.Assemble[breakdown1]);
        }
        {
            // Create detect format
            ITemplateFormat detect = new DetectTemplateFormat("FormatName", TemplateFormat.Parameterless, TemplateFormat.BraceNumeric);
            // Parse string and detect template format
            ITemplateText templateText = detect.TextCached["Today is {0}. Welcome, {1}."];
            // Print straight to console
            templateText.WriteTo(Console.Out, CultureInfo.InvariantCulture, new object?[] { DateTime.Now, "User" });
        }
        {
            // Builtin template formats
            ITemplateFormats templateFormats = TemplateFormats.All;
            // Get by name
            ITemplateFormat templateFormat = templateFormats.ByName["BraceNumeric"];
        }
    }
}

