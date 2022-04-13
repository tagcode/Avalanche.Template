# Introduction
Avalanche.Template contains classes for text templating.


```csharp
ITemplateText templateText = new TemplateText("Error code {0} (0x{0:X4}).", TemplateFormat.Brace).SetReadOnly();
WriteLine(templateText.Print(null, new object?[] { 0x100 })); // "Error code 256 (0x0100)."
```

The following <em>global_includes.cs</em> can be used to include all extension methods.

```cs
global using Avalanche.Template;
global using Avalanche.Utilities;
```

<details>
<summary>Class libraries:</summary>
<ul>
<li>Avalanche.Template.dll contains implementations.</li>
<li>Avalanche.Template.Abstractions.dll contains interfaces.</li>
</ul>
<p>Dependency libraries:</p>
<ul>
<li>Avalanche.Tokenizer.dll</li>
<li>Avalanche.Tokenizer.Abstractions.dll</li>
<li>Avalanche.Utilities.dll</li>
<li>Avalanche.Utilities.Abstractions.dll</li>
</ul>
</details>



