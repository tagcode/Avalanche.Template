using System.Buffers;
using System.Globalization;
using Avalanche.Template;
using static System.Console;

public class templateextractpattern
{
    public static void Run()
    {
        {
            // Create template
            ITemplateBreakdown breakdown = TemplateFormat.Brace.BreakdownCached["You have {0} apple(s)."];
            // Create arguments
            object?[] arguments = { 3 };
            // Print
            string print = breakdown.Print(CultureInfo.InvariantCulture, arguments); // "You have 3 apple(s)."

            // Extract arguments
            if (breakdown.TryExtractArguments(print, out ReadOnlyMemory<char>[] extractedArguments))
            {
                // Print extracted argument
                WriteLine(extractedArguments[0]); // "3"
            }
        }
        {
            // Create template
            ITemplateBreakdown breakdown = TemplateFormat.Brace.BreakdownCached["You have {0} apple(s)."];
            // Create arguments
            object?[] arguments = { 3 };
            // Print
            ReadOnlyMemory<char> print = breakdown.Print(CultureInfo.InvariantCulture, arguments).AsMemory(); // "You have 3 apple(s)."
            // Extract arguments
            if (breakdown.TryExtractArguments(print, out ReadOnlyMemory<char>[] extractedArguments))
            {
                // Print extracted argument
                WriteLine(extractedArguments[0]); // "3"
            }
        }
        {
            // Create template
            ITemplateBreakdown breakdown = TemplateFormat.Brace.BreakdownCached["Line 1: {2}\nLine2: {1}\nLine3: {0}"];
            // Create arguments
            object?[] arguments = { "Hello\nWorld", "Clown\nShow", "More\nFood" };
            // Print
            string print = breakdown.Print(CultureInfo.InvariantCulture, arguments);

            // Extract arguments
            if (breakdown.TryExtractArguments(print, out ReadOnlyMemory<char>[] extractedArguments))
            {
                // Print extracted argument
                WriteLine(extractedArguments[0]); // "Hello\nWorld"
                WriteLine(extractedArguments[1]); // "Clown\nShow"
                WriteLine(extractedArguments[2]); // "More\nFood"
            }
        }
        {
            // Create template
            ITemplateBreakdown breakdown = TemplateFormat.Brace.BreakdownCached["You have {0} apple(s)."];
            // Create arguments
            object?[] arguments = { 3 };
            // Print
            string print = breakdown.Print(CultureInfo.InvariantCulture, arguments); // "You have 3 apple(s)."

            // Allocate
            ReadOnlyMemory<char>[] argumentsToBeExtracted = ArrayPool<ReadOnlyMemory<char>>.Shared.Rent(1);
            // Extract arguments
            if (breakdown.TryExtractArgumentsTo(print, argumentsToBeExtracted))
            {
                // Print extracted argument
                WriteLine(argumentsToBeExtracted[0]); // "3"
            }
            // Return arguments
            ArrayPool<ReadOnlyMemory<char>>.Shared.Return(argumentsToBeExtracted);
        }
        {
            // Create template
            ITemplateBreakdown breakdown = TemplateFormat.Brace.BreakdownCached["You have {0} apple(s)."];
            // Create arguments
            object?[] arguments = { 3 };
            // Print
            string print = breakdown.Print(CultureInfo.InvariantCulture, arguments); // "You have 3 apple(s)."

            // Allocate
            ReadOnlyMemory<char>[] extractedArguments = breakdown.ExtractArguments(print);
            // Print extracted argument
            WriteLine(extractedArguments[0]); // "3"
        }
    }
}

