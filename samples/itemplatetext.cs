using Avalanche.Template;
using Avalanche.Utilities;
using static System.Console;

public class templatetext
{
    public static void Run()
    {
        {
            ITemplateText templateText = new TemplateText("Error code {0} (0x{0:X4}).", TemplateFormat.Brace).SetReadOnly();
            WriteLine(templateText.Print(null, new object?[] { 0x100 })); // "Error code 256 (0x0100)."
        }
        {
            ITemplateText templateText = new FormatText("Error code {0} (0x{0:X4}).");
            string print1 = string.Format(templateText.Text, 0x100);
            string print2 = templateText.Print(null, new object?[] { 0x100 });
            WriteLine(print2); // "Error code 256 (0x0100)."
        }
        {
            ITemplateText templateText = new FormatText("Error code {2} (0x{0:X4}).");
            WriteLine(templateText.ParameterNames[0]); // "0"
            WriteLine(templateText.ParameterNames[1]); // ""
            WriteLine(templateText.ParameterNames[2]); // "2"
        }
        {
            ITemplateText templateText = new ParameterlessText("Application started.");
            WriteLine(templateText.Print(null));
        }

    }
}

