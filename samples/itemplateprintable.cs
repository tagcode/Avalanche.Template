using System.Buffers;
using System.Globalization;
using System.Text;
using Avalanche.Template;
using static System.Console;

public class templateprintable
{
    public static void Run()
    {
        {
            ITemplatePrintable printable = new TemplateText("Time is {0}.", TemplateFormat.Brace)
                .WithFormat(CultureInfo.CurrentCulture);
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
        {
            // Create text without template format
            ITemplatePrintable printable = new TemplateText("Time is {0}.", TemplateFormat.Brace).WithFormat(CultureInfo.CurrentCulture);
            // Create arguments
            IDictionary<string, object?> argumentMap = new Dictionary<string, object?> { { "0", DateTime.Now } };
            // Print ok
            string print = printable.Print(argumentMap);
            // ""
            WriteLine(print);
        }
        {
            ITemplatePrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace).WithFormat(CultureInfo.GetCultureInfo("en"));
            // "You have no apples."
            printable.Print(new object?[] { 0 });
            // "You have an apple."
            printable.Print(new object?[] { 1 });
            // "You have 3 apples."
            printable.Print(new object?[] { 3 });
            // "Sinulla ei ole omenoita."
            printable.Print(new object?[] { 0 });
            // "Sinulla on yksi omena."
            printable.Print(new object?[] { 1 });
            // "Sinulla on 3 omenaa."
            printable.Print(new object?[] { 3 });
        }

        {
            // Create template
            ITemplatePrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace).WithFormat(CultureInfo.InvariantCulture);
            // Create arguments
            object[] arguments = { 3 };
            // Estimate length
            int length = printable.EstimatePrintLength(arguments);
            // Allocate span
            Span<char> span = length < 512 ? stackalloc char[length] : new char[length];
            // Print to span
            length = printable.PrintTo(span, arguments);
            // Print to stdout, zero heap.
            Console.Out.Write(span[..length]);
            WriteLine();
        }
        {
            // Create template
            ITemplatePrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace).WithFormat(CultureInfo.InvariantCulture);
            // Create arguments
            IDictionary<string, object?> argumentMap = new Dictionary<string, object?> { { "0", 3 } };
            // Estimate length
            int length = printable.EstimatePrintLength(argumentMap);
            // Allocate span
            Span<char> span = length < 512 ? stackalloc char[length] : new char[length];
            // Print to span
            length = printable.PrintTo(span, argumentMap);
            // Print to stdout, zero heap.
            Console.Out.Write(span[..length]);
            WriteLine();
        }

        {
            // Create template
            ITemplatePrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace).WithFormat(CultureInfo.InvariantCulture);
            // Create arguments
            object[] arguments = { 3 };
            // Estimate length
            int length = printable.EstimatePrintLength(arguments);
            // Allocate span
            Span<char> span = length < 512 ? stackalloc char[length] : new char[length];
            // Print to span
            if (printable.TryPrintTo(span, out length, arguments))
                Console.Out.Write(span[..length]);
            WriteLine();
        }
        {
            // Create template
            ITemplatePrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace).WithFormat(CultureInfo.InvariantCulture);
            // Create arguments
            IDictionary<string, object?> argumentMap = new Dictionary<string, object?> { { "0", 3 } };
            // Estimate length
            int length = printable.EstimatePrintLength(argumentMap);
            // Allocate span
            Span<char> span = length < 512 ? stackalloc char[length] : new char[length];
            // Print to span
            if (printable.TryPrintTo(span, out length, argumentMap))
                Console.Out.Write(span[..length]);
            WriteLine();
        }

        {
            // Create template
            ITemplatePrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace).WithFormat(CultureInfo.InvariantCulture);
            // Create arguments
            object?[] arguments = { 3 };
            // Create string builder
            StringBuilder sb = new(1024);
            // Append to string builder
            printable.AppendTo(sb, arguments);
            // Print
            WriteLine(sb); // "You have 3 apple(s)."
        }
        {
            // Create template
            ITemplatePrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace).WithFormat(CultureInfo.InvariantCulture);
            // Create arguments
            IDictionary<string, object?> argumentMap = new Dictionary<string, object?> { { "0", 3 } };
            // Create string builder
            StringBuilder sb = new(1024);
            // Append to string builder
            printable.AppendTo(sb, argumentMap);
            // Print
            WriteLine(sb); // "You have 3 apple(s)."
        }

        {
            // Create template
            ITemplatePrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace).WithFormat(CultureInfo.InvariantCulture);
            // Create arguments
            object?[] arguments = { 3 };
            // Assign text writer
            TextWriter textWriter = Console.Out;
            // Write to writer
            printable.WriteTo(textWriter, arguments); // "You have 3 apple(s)."
            WriteLine();
        }
        {
            // Create template
            ITemplatePrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace).WithFormat(CultureInfo.InvariantCulture);
            // Create arguments
            IDictionary<string, object?> argumentMap = new Dictionary<string, object?> { { "0", 3 } };
            // Assign text writer
            TextWriter textWriter = Console.Out;
            // Write to writer
            printable.WriteTo(textWriter, argumentMap); // "You have 3 apple(s)."
            WriteLine();
        }

        {
            // Create template
            ITemplatePrintable printable = new TemplateText("You have {0} apple(s).", TemplateFormat.Brace).WithFormat(CultureInfo.InvariantCulture);
            // Rent arguments
            object[] arguments = ArrayPool<object>.Shared.Rent(1);
            // Assign argument
            arguments[0] = "3";
            // Estimate length
            int length = printable.EstimatePrintLength(arguments);
            // Rent chars
            char[] buffer = ArrayPool<char>.Shared.Rent(length);
            // Print to buffer
            length = printable.PrintTo(buffer.AsSpan(), arguments);
            // Print to stdout, zero heap.
            Console.Out.Write(buffer.AsSpan()[..length]);
            // Return rentals
            ArrayPool<object>.Shared.Return(arguments);
            ArrayPool<char>.Shared.Return(buffer);            
            WriteLine();
        }
    }
}

