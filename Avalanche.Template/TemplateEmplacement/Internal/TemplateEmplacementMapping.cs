// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Template.Internal;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Mapping of parent text's parameters to emplacements' parameters.</summary>
public class TemplateEmplacementMapping
{
    /// <summary>Parent text</summary>
    public ITemplateText Text = null!;
    /// <summary>Emplacement assignments</summary>
    public ICollection<ITemplateParameterEmplacement> Emplacements = null!;

    /// <summary>Parameters in order of original <see cref="Text"/> parameters</summary>
    public IList<ParameterMapping> OriginalParameters = null!;
    /// <summary>Parameters in the new emplaced version</summary>
    public IList<ParameterMapping> NewParameters = null!;

    /// <summary>Emplacement mapping</summary>
    public class ParameterMapping
    {
        /// <summary>Parameter index on parent text. -1 if parameter is not in use in parent text</summary>
        public int OrigParameterIndex = -1;
        /// <summary>Parameter index on the new text. -1 if parameter is not in use in the new text</summary>
        public int NewParameterIndex = -1;
        /// <summary>Parameter name on parent text. Null if parameter is in use in parent text.</summary>
        public string? OrigParameterName = null;
        /// <summary>Parameter name on the new text. Null if parameter is in use in the new text.</summary>
        public string? NewParameterName = null;
        /// <summary>Associated emplacement assignment. Null if parent parameter is not mapped.</summary>
        public ITemplateParameterEmplacement? EmplacementAssignment;
        /// <summary>Associated emplacement assignment. Null if parent parameter is not mapped.</summary>
        public ITemplateText? EmplacementText;
        /// <summary>Parameter index in the <see cref="EmplacementText"/></summary>
        public int EmplacementParameterIndex = -1;
        /// <summary>Parameter name in the <see cref="EmplacementText"/></summary>
        public string? EmplacementParameterName = null!;
        /// <summary>Corresponding emplacement parameters</summary>
        public IList<ParameterMapping> CorrespondingEmplacementParameters = Array.Empty<ParameterMapping>();
        /// <summary>Print name</summary>
        public override string ToString() => NewParameterName ?? OrigParameterName ?? EmplacementParameterName ?? "";
    }

    /// <summary>Get corresponding mappings with <paramref name="origParameterName"/>.</summary>
    public bool TryGetOrigParameterByOrigParameterName(string origParameterName, out ParameterMapping mapping)
    {
        // Search
        foreach (var parameterMapping in OriginalParameters) 
            if (parameterMapping.OrigParameterName == origParameterName) 
            { mapping = parameterMapping; return true; }
        // Return
        mapping = null!;
        return false;
    }

    /// <summary>Get corresponding mappings with <paramref name="origParameterIndex"/>.</summary>
    public bool TryGetNewParametersByOrigParameterIndex(int origParameterIndex, out ParameterMapping[] mappings)
    {
        // Index out of range
        if (origParameterIndex < 0 || origParameterIndex >= OriginalParameters.Count) { mappings = Array.Empty<ParameterMapping>(); return false; }
        // Place result here
        StructList4<ParameterMapping> _mappings = new();
        foreach (var parameterMapping in NewParameters) 
            if (parameterMapping.OrigParameterIndex == origParameterIndex) 
                _mappings.Add(parameterMapping);
        // Return
        mappings = _mappings.ToArray();
        return true;
    }

    /// <summary>Get corresponding mappings with <paramref name="origParameterName"/>.</summary>
    public bool TryGetNewParametersByOrigParameterName(string origParameterName, out ParameterMapping[] mappings)
    {
        // Check if parameter name exists
        if (!TryGetOrigParameterByOrigParameterName(origParameterName, out ParameterMapping __)) { mappings = Array.Empty<ParameterMapping>(); return false; }
        // Place result here
        StructList4<ParameterMapping> _mappings = new();
        foreach (var parameterMapping in NewParameters) 
            if (parameterMapping.OrigParameterName == origParameterName) 
                _mappings.Add(parameterMapping);
        // Return
        mappings = _mappings.ToArray();
        return true;
    }

    /// <summary>Create parameter names array</summary>
    public string?[] CreateNewParameterNames()
    {
        // Create result
        var result = new string?[NewParameters.Count];
        // Process
        for (var i = 0; i < NewParameters.Count; i++) result[i] = NewParameters[i].NewParameterName;
        // Return
        return result;
    }

    /// <summary>Print parameters</summary>
    public override string ToString() => string.Join(", ", NewParameters);

    /// <summary>Create emplacement mapping.</summary>
    public static TemplateEmplacementMapping CreateEmplacementMapping(ITemplateText text, ICollection<ITemplateParameterEmplacement> emplacements)
    {
        // Assert parameter names exist in parent text
        foreach (var emplacement in emplacements)
            if (!text.ParameterNames.Contains(emplacement.ParameterName))
                throw new ArgumentException($"ParameterName \"{emplacement.ParameterName}\" not found in \"{text}\"");

        // Initialize mapping
        var mapping = new TemplateEmplacementMapping
        {
            Text = text ?? throw new ArgumentNullException(nameof(text)),
            Emplacements = emplacements ?? throw new ArgumentNullException(nameof(emplacements)),
            OriginalParameters = new ParameterMapping[text.ParameterNames.Length],
            NewParameters = new List<ParameterMapping>(text.ParameterNames.Length + 3)
        };

        // Manage new parameter index here
        var newParameterIndex = -1;
        // Process each parameter
        for (var i = 0; i < text.ParameterNames.Length; i++)
        {
            // Get parameter name
            var parameterName = text.ParameterNames[i];
            // No associated emplacement
            if (!emplacements.TryGetEmplacement(parameterName!, out var emplacement))
            {
                // Create parameter mapping description
                var parameterMapping = new ParameterMapping { OrigParameterIndex = i, OrigParameterName = parameterName, NewParameterIndex = ++newParameterIndex, NewParameterName = parameterName };
                // Add
                mapping.NewParameters.Add(mapping.OriginalParameters[i] = parameterMapping);
                // Next
                continue;
            }
            // Get emplacement's parameter names
            var emplacementParameterNames = emplacement.Emplacement.ParameterNames;
            // List of emplacement parameters
            List<ParameterMapping> correspondingEmplacementParameters = new();
            // Add to original parameters list
            mapping.OriginalParameters[i] = new ParameterMapping { OrigParameterIndex = i, OrigParameterName = parameterName, NewParameterIndex = -1, NewParameterName = null, CorrespondingEmplacementParameters = correspondingEmplacementParameters, EmplacementAssignment = emplacement };
            // Process emplacement parameter names
            for (var ei = 0; ei < emplacementParameterNames.Length; ei++)
            {
                // Get emplacement parameter name
                var emplacementParameterName = emplacementParameterNames[ei];
                // Create parameter mapping description
                var parameterMapping = new ParameterMapping { OrigParameterIndex = i, OrigParameterName = parameterName, NewParameterIndex = ++newParameterIndex, NewParameterName = parameterName, EmplacementAssignment = emplacement, EmplacementParameterIndex = ei, EmplacementParameterName = emplacementParameterName, EmplacementText = emplacement.Emplacement };
                // Derive parameter name
                if (emplacementParameterName != null) parameterMapping.NewParameterName = $"{parameterName}{TemplateEmplacementExtensions.ParameterNameSeparator}{emplacementParameterName}";
                // Add
                mapping.NewParameters.Add(parameterMapping);
                correspondingEmplacementParameters.Add(parameterMapping);
            }
        }

        // Return mapping
        return mapping;
    }
}

/// <summary></summary>
public static class TemplateEmplacementMappingExtensions
{
    /// <summary>Search by <see cref="TemplateEmplacementMapping.ParameterMapping.EmplacementParameterName"/>.</summary>
    public static bool TryGetByEmplacementName(this ICollection<TemplateEmplacementMapping.ParameterMapping>? parameters, string emplacementName, out TemplateEmplacementMapping.ParameterMapping parameterMapping)
    {
        // Try each
        if (parameters != null)
            foreach (var _parameterMapping in parameters)
                if (_parameterMapping.EmplacementParameterName == emplacementName) { parameterMapping = _parameterMapping; return true; }
        // Not found
        parameterMapping = null!;
        return false;
    }

    /// <summary>Search by <see cref="TemplateEmplacementMapping.ParameterMapping.EmplacementParameterIndex"/>.</summary>
    public static bool TryGetByEmplacementIndex(this ICollection<TemplateEmplacementMapping.ParameterMapping>? parameters, int emplacementIndex, out TemplateEmplacementMapping.ParameterMapping parameterMapping)
    {
        // Try each
        if (parameters != null)
            foreach (var _parameterMapping in parameters)
                if (_parameterMapping.EmplacementParameterIndex == emplacementIndex) { parameterMapping = _parameterMapping; return true; }
        // Not found
        parameterMapping = null!;
        return false;
    }

}
