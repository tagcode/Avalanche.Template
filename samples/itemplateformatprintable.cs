using System.Buffers;
using System.Globalization;
using System.Text;
using Avalanche.Template;
using static System.Console;

public class templateformatprintable
{
    public static void Run()
    {
        {
            // Create text without template format
            ITemplateFormatPrintable printable = new TemplateText("Time is {0}.", TemplateFormat.Brace);
            // Create arguments
            object?[] arguments = { DateTime.Now };
            // Print ok
            string print = printable.Print(CultureInfo.CurrentCulture, arguments);
            // ""
            WriteLine(print);
        }
        {
            // Create text without template format
            ITemplateFormatPrintable printable = new TemplateText("Time is {0}.", TemplateFormat.Brace);
            // Create arguments
            IDictionary<string, object?> argumentMap = new Dictionary<string, object?> { { "0", DateTime.Now } };
            // Print ok
            string print = printable.Print(CultureInfo.CurrentCulture, argumentMap);
            // ""
            WriteLine(print);
        }
        {
            ITemplateFormatPrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace);
            // "You have no apples."
            printable.Print(CultureInfo.GetCultureInfo("en"), new object?[] { 0 });
            // "You have an apple."
            printable.Print(CultureInfo.GetCultureInfo("en"), new object?[] { 1 });
            // "You have 3 apples."
            printable.Print(CultureInfo.GetCultureInfo("en"), new object?[] { 3 });
            // "Sinulla ei ole omenoita."
            printable.Print(CultureInfo.GetCultureInfo("fi"), new object?[] { 0 });
            // "Sinulla on yksi omena."
            printable.Print(CultureInfo.GetCultureInfo("fi"), new object?[] { 1 });
            // "Sinulla on 3 omenaa."
            printable.Print(CultureInfo.GetCultureInfo("fi"), new object?[] { 3 });
        }

        {
            // Create template
            ITemplateFormatPrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace);
            // Create arguments
            object[] arguments = { 3 };
            // Estimate length
            int length = printable.EstimatePrintLength(CultureInfo.InvariantCulture, arguments);
            // Allocate span
            Span<char> span = length < 512 ? stackalloc char[length] : new char[length];
            // Print to span
            length = printable.PrintTo(span, CultureInfo.InvariantCulture, arguments);
            // Print to stdout, zero heap.
            Console.Out.Write(span[..length]);
            WriteLine();
        }
        {
            // Create template
            ITemplateFormatPrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace);
            // Create arguments
            IDictionary<string, object?> argumentMap = new Dictionary<string, object?> { { "0", 3 } };
            // Estimate length
            int length = printable.EstimatePrintLength(CultureInfo.InvariantCulture, argumentMap);
            // Allocate span
            Span<char> span = length < 512 ? stackalloc char[length] : new char[length];
            // Print to span
            length = printable.PrintTo(span, CultureInfo.InvariantCulture, argumentMap);
            // Print to stdout, zero heap.
            Console.Out.Write(span[..length]);
            WriteLine();
        }

        {
            // Create template
            ITemplateFormatPrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace);
            // Create arguments
            object[] arguments = { 3 };
            // Estimate length
            int length = printable.EstimatePrintLength(CultureInfo.InvariantCulture, arguments);
            // Allocate span
            Span<char> span = length < 512 ? stackalloc char[length] : new char[length];
            // Print to span
            if (printable.TryPrintTo(span, out length, CultureInfo.InvariantCulture, arguments))
                Console.Out.Write(span[..length]);
            WriteLine();
        }
        {
            // Create template
            ITemplateFormatPrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace);
            // Create arguments
            IDictionary<string, object?> argumentMap = new Dictionary<string, object?> { { "0", 3 } };
            // Estimate length
            int length = printable.EstimatePrintLength(CultureInfo.InvariantCulture, argumentMap);
            // Allocate span
            Span<char> span = length < 512 ? stackalloc char[length] : new char[length];
            // Print to span
            if (printable.TryPrintTo(span, out length, CultureInfo.InvariantCulture, argumentMap))
                Console.Out.Write(span[..length]);
            WriteLine();
        }

        {
            // Create template
            ITemplateFormatPrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace);
            // Create arguments
            object?[] arguments = { 3 };
            // Create string builder
            StringBuilder sb = new(1024);
            // Append to string builder
            printable.AppendTo(sb, CultureInfo.InvariantCulture, arguments);
            // Print
            WriteLine(sb); // "You have 3 apple(s)."
        }
        {
            // Create template
            ITemplateFormatPrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace);
            // Create arguments
            IDictionary<string, object?> argumentMap = new Dictionary<string, object?> { { "0", 3 } };
            // Create string builder
            StringBuilder sb = new(1024);
            // Append to string builder
            printable.AppendTo(sb, CultureInfo.InvariantCulture, argumentMap);
            // Print
            WriteLine(sb); // "You have 3 apple(s)."
        }

        {
            // Create template
            ITemplateFormatPrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace);
            // Create arguments
            object?[] arguments = { 3 };
            // Assign text writer
            TextWriter textWriter = Console.Out;
            // Write to writer
            printable.WriteTo(textWriter, CultureInfo.InvariantCulture, arguments); // "You have 3 apple(s)."
            WriteLine();
        }
        {
            // Create template
            ITemplateFormatPrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace);
            // Create arguments
            IDictionary<string, object?> argumentMap = new Dictionary<string, object?> { { "0", 3 } };
            // Assign text writer
            TextWriter textWriter = Console.Out;
            // Write to writer
            printable.WriteTo(textWriter, CultureInfo.InvariantCulture, argumentMap); // "You have 3 apple(s)."
            WriteLine();
        }

        {
            // Create template
            ITemplateFormatPrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace);
            // Rent arguments
            object[] arguments = ArrayPool<object>.Shared.Rent(1);
            // Assign argument
            arguments[0] = "3";
            // Estimate length
            int length = printable.EstimatePrintLength(CultureInfo.InvariantCulture, arguments);
            // Rent chars
            char[] buffer = ArrayPool<char>.Shared.Rent(length);
            // Print to buffer
            length = printable.PrintTo(buffer.AsSpan(), CultureInfo.InvariantCulture, arguments);
            // Print to stdout, zero heap.
            Console.Out.Write(buffer.AsSpan()[..length]);
            // Return rentals
            ArrayPool<object>.Shared.Return(arguments);
            ArrayPool<char>.Shared.Return(buffer);            
            WriteLine();
        }
    }
}

