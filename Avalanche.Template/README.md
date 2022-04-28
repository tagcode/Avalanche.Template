<b>Avalanche.Template</b> contains classes for text templating,
[[git]](https://github.com/tagcode/Avalanche.Template/Avalanche.Template), 
[[www]](https://avalanche.fi/Avalanche.Core/Avalanche.Template/docs/), 
[[licensing]](https://avalanche.fi/Avalanche.Core/license/index.html).

Add package reference to .csproj.
```xml
<PropertyGroup>
    <RestoreAdditionalProjectSources>https://avalanche.fi/Avalanche.Core/nupkg/index.json</RestoreAdditionalProjectSources>
</PropertyGroup>
<ItemGroup>
    <PackageReference Include="Avalanche.Template"/>
</ItemGroup>
```

<b>TemplateText(text, format)</b> represents a template text.

```csharp
ITemplateText templateText = new TemplateText("Error code {0} (0x{0:X4}).", TemplateFormat.Brace).SetReadOnly();
WriteLine(templateText.Print(null, new object?[] { 0x100 })); // "Error code 256 (0x0100)."
```

<b>TemplateFormat.Brace</b> detects which parameter convention to use, numeric "{0}" or alphanumeric "{user}".

```csharp
ITemplateFormat templateFormat = TemplateFormat.Brace;
ITemplateBreakdown breakdown2 = templateFormat.Breakdown["Today is {time}. Welcome, {user}"];
```

<b>.Print(arguments)</b> prints template string by placing arguments into placeholders.

```csharp
// Create text without template format
ITemplatePrintable printable = new TemplateText("Time is {0}.", TemplateFormat.Brace).WithFormat(CultureInfo.CurrentCulture);
// Create arguments
object?[] arguments = { DateTime.Now };
// Print ok
string print = printable.Print(arguments);
// ""
WriteLine(print);
```