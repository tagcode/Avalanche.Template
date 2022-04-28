using Avalanche.Template;
using static System.Console;

public class templateemplacement
{
    public static void Run()
    {
        {
            // Create texts
            ITemplateText and = new TemplateText("{0} and {1}", TemplateFormat.Brace);
            ITemplateText cat = new ParameterlessText("a cat");
            ITemplateText dog = new ParameterlessText("a dog");
            // Embue "a cat and a dog" 
            ITemplateText catAndDog = and.Place("0", cat, "1", dog);
            // Print
            WriteLine(catAndDog.Print(null, null)); // "a cat and a dog" 
        }
        {
            // Create texts
            ITemplateText and = new TemplateText("{0} and {1}", TemplateFormat.Brace);
            ITemplateText cat = new TemplateText("{count} cat(s)", TemplateFormat.Brace);
            ITemplateText dog = new TemplateText("{count} dog(s)", TemplateFormat.Brace);
            // Embue 0=cat, 1=dog,
            ITemplateText catsDogs = and.Place("0", cat, "1", dog);

            // Create arguments
            object?[] arguments = { 2, 3 };
            // Print
            string print = catsDogs.Print(null, arguments);
            // Write
            WriteLine(print); // "2 cat(s) and 3 dog(s)" 

            // Print the new composition
            WriteLine(catsDogs); // "{0_count} cat(s) and {1_count}"
            // Print parameters
            WriteLine(string.Join(", ", catsDogs.ParameterNames)); // "0_count, 1_count"
        }
        {
            // Create texts
            ITemplateText ands = new TemplateText("{0}, {1} and {2}", TemplateFormat.BraceNumeric);
            ITemplateText cat = new TemplateText("{count} cat(s)", TemplateFormat.Brace);
            ITemplateText dog = new TemplateText("{count} dog(s)", TemplateFormat.Brace);
            ITemplateText pony = new TemplateText("{count} poni(es)", TemplateFormat.Brace);
            // Embue "{0} cat(s), {1} dog(s) and {2} poni(es)"
            ITemplateText catsDogsPonies = ands.Place("0", cat, "1", dog, "2", pony);
            // Write parameters
            WriteLine(string.Join(", ", catsDogsPonies.ParameterNames)); // "0, 1, 2"
        }

        {
            // Create texts
            ITemplateText line = new TemplateText("{0,10}, {1,10}", TemplateFormat.Brace);
            ITemplateText count = new TemplateText("Count = {count,-5}", TemplateFormat.Brace);
            ITemplateText id = new TemplateText("Id    = {id,-5}", TemplateFormat.Brace);
            // Embue "Count = {0_count,-5}, Id    = {1_id,-5}"
            ITemplateText merge = line.Place("0", count, "1", id);
            // Print
            WriteLine(merge.Print(null, new object?[] { 1, 1 })); // "Count = 1    , Id    = 1    "
        }

        {
            // Create texts
            ITemplateText and = new TemplateText("{0} and {1}", TemplateFormat.Brace);
            ITemplateText cat = new TemplateText("{count} cat(s)", TemplateFormat.Brace);
            ITemplateText dog = new TemplateText("{count} dog(s)", TemplateFormat.Brace);
            // Create "2 cat(s)" and "3 dog(s)"
            ITemplateText cat2 = cat.Place("count", new ParameterlessText("2"));
            ITemplateText dog3 = dog.Place("count", new ParameterlessText("3"));
            // Print
            WriteLine(and.Print(null, new object?[] { cat2, dog3 })); // "2 cat(s) and 3 dog(s)"
        }

    }
}

