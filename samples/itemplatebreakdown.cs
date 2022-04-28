using System.Globalization;
using Avalanche.Template;
using Avalanche.Utilities;
using static System.Console;

public class templatebreakdown
{
    public static void Run()
    {
        {
            // Create format string
            string text = "Welcome, {user}";
            ReadOnlyMemory<char> mem = text.AsMemory();
            TemplateBreakdown breakdown = new TemplateBreakdown { Text = text };
            var textPart = new TemplateTextPart().SetBreakdown(breakdown).SetTexts(mem[..9], Escaper.Brace);
            var parameter = new TemplateParameterPart().SetBreakdown(breakdown).SetTexts(mem[10..14]);
            var placeholder = new TemplatePlaceholderPart().SetBreakdown(breakdown).SetTexts(mem[9..15]).SetParameter(parameter);
            breakdown.Parts = new ITemplatePart[] { textPart, placeholder };
            breakdown.SetReadOnly();
            // Print
            WriteLine(breakdown.Print(CultureInfo.InvariantCulture, new object?[] { "Donald Duck" }));
            WriteLine(textPart.Unescaped);
            WriteLine(TemplateFormat.BraceNumeric.Assemble[breakdown]);
        }
        {
            ITemplateBreakdown breakdown1 = TemplateFormat.BraceNumeric.Breakdown["Welcome, {0}"];
            ITemplateBreakdown breakdown2 = TemplateFormat.BraceAlphaNumeric.Breakdown["Welcome, {user}"];
        }
        {
            ITemplateBreakdown breakdown = TemplateFormat.BraceAlphaNumeric.Breakdown["Welcome, {user}"];
            WriteLine(breakdown.Print(CultureInfo.InvariantCulture, new object?[] { "Donald Duck" })); // "Welcome, Donald Duck"
        }
        {
            ITemplateBreakdown breakdown = TemplateFormat.BraceAlphaNumeric.Breakdown["Welcome, {}"];
            // Get malformed parts
            foreach ((int index, int length, ReadOnlyMemory<char>? malformed) in breakdown.MalformedParts())
            {
                // Write
                WriteLine($"Malformed '{malformed}' at: {index}:{index + length}"); // Malformed at: 9:11
            }
        }
        {
            // Create breakdown
            ITemplateBreakdown breakdown = TemplateFormat.Brace.Breakdown["Error code: {0} (0x{0:X4})."];
            // "Error code: 256 (0x0100)."
            WriteLine(breakdown.Print(null, new object?[] { 0x100 }));

            // Placeholder: "{0}", Placeholder: "{0:X8}"
            WriteLine(string.Join(", ", (IEnumerable<ITemplatePlaceholderPart>)breakdown.Placeholders));
            // Parameter: "0"
            WriteLine(string.Join(", ", (IEnumerable<ITemplateParameterPart>)breakdown.Parameters));
            WriteLine(TemplateFormat.BraceAlphaNumeric.Assemble[breakdown]);
            WriteLine(TemplateFormat.BraceNumeric.Assemble[breakdown]);
        }
        {
            // Create breakdown
            ITemplateBreakdown breakdown = TemplateFormat.Brace.Breakdown["Error code: {code} (0x{code:X4})."];
            // "Error code: 256 (0x0100)."
            WriteLine(breakdown.Print(null, new object?[] { 0x100 }));

            // Placeholder: "{code}", Placeholder: "{code:X4}"
            WriteLine(string.Join(", ", (IEnumerable<ITemplatePlaceholderPart>)breakdown.Placeholders));
            // Parameter: "code"
            WriteLine(string.Join(", ", (IEnumerable<ITemplateParameterPart>)breakdown.Parameters));
            WriteLine(TemplateFormat.BraceAlphaNumeric.Assemble[breakdown]);
            WriteLine(TemplateFormat.BraceNumeric.Assemble[breakdown]);
        }
    }
}

