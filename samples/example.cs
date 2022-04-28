using System.Globalization;
using Avalanche.Template;
using Avalanche.Utilities;
using static System.Console;

class example
{
    public static void Run()
    {
        {
            ITemplateText templateText = new TemplateText("Error code {0} (0x{0:X4}).", TemplateFormat.Brace).SetReadOnly();
            WriteLine(templateText.Print(null, new object?[] { 0x100 })); // "Error code 256 (0x0100)."
        }
        {
            ITemplateFormat templateFormat = TemplateFormat.Brace;
            ITemplateBreakdown breakdown2 = templateFormat.Breakdown["Today is {time}. Welcome, {user}"];
        }
        {
            // Create text without template format
            ITemplatePrintable printable = new TemplateText("Time is {0}.", TemplateFormat.Brace).WithFormat(CultureInfo.CurrentCulture);
            // Create arguments
            object?[] arguments = { DateTime.Now };
            // Print ok
            string print = printable.Print(arguments);
            // ""
            WriteLine(print);
        }
    }
}
