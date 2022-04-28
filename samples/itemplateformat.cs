using System.Globalization;
using Avalanche.Template;
using static System.Console;

public class templateformat
{
    public static void Run()
    {
        {
            ITemplateText text = TemplateFormat.Parameterless.Text["Welcome!"];
        }
        {
            ITemplateText text = TemplateFormat.Parameterless.TextCached["Welcome!"];
        }
        {
            ITemplateBreakdown breakdown1 = TemplateFormat.BraceNumeric.Breakdown["Welcome, {0}"];
            ITemplateBreakdown breakdown2 = TemplateFormat.BraceAlphaNumeric.Breakdown["Welcome, {user}"];
            ITemplateBreakdown breakdown3 = TemplateFormat.Brace.Breakdown["Welcome, {0}"];
        }
        {
            ITemplateBreakdown breakdown1 = TemplateFormat.BraceNumeric.BreakdownCached["Welcome, {0}"];
            ITemplateBreakdown breakdown2 = TemplateFormat.BraceAlphaNumeric.BreakdownCached["Welcome, {user}"];
            ITemplateBreakdown breakdown3 = TemplateFormat.Brace.BreakdownCached["Welcome, {0}"];
        }
        {
            ITemplateFormat templateFormat = TemplateFormat.BraceAlphaNumeric;
            ITemplateBreakdown breakdown = templateFormat.Breakdown["Welcome, {user}"];
            WriteLine(templateFormat.Assemble[breakdown]); // "Welcome, {user}"
            WriteLine(breakdown.Print(CultureInfo.InvariantCulture, new object?[] { "Donald" }));
        }
        {
            ITemplateBreakdown breakdown = TemplateFormat.BraceAlphaNumeric.Breakdown["Welcome, {user}"];
            WriteLine(TemplateFormat.BraceAlphaNumeric.AssembleCached[breakdown]); // "Welcome, {user}"
            WriteLine(breakdown.Print(CultureInfo.InvariantCulture, new object?[] { "Donald" }));
        }
        {
            ITemplateBreakdown breakdown = TemplateFormat.Percent.Breakdown["Today is {%2}. Welcome, %1"];
            WriteLine(TemplateFormat.BraceNumeric.Assemble[breakdown]); // "Today is {{{1}}}. Welcome, {0}"
        }

    }
}

