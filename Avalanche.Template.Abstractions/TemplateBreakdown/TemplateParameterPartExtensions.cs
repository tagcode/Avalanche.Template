// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template;

/// <summary>Extension methods for <see cref="ITemplateParameterPart"/>.</summary>
public static class TemplateParameterPartExtensions
{
    /// <summary>Assign <paramref name="parameterIndex"/>, correlates with arguments and <see cref="ITemplateBreakdown.Parameters"/>.</summary>    
    public static T SetParameterIndex<T>(this T part, int parameterIndex) where T : ITemplateParameterPart
    {
        // Assign 
        part.ParameterIndex = parameterIndex;
        // Return 
        return part;
    }


}
