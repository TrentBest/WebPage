namespace TheSingularityWorkshop.Workshop.Spatial;

/// <summary>
/// Allocates human-friendly names without making persistence part of the authoring model.
/// Names are unique within the supplied in-memory scope and can be changed later without
/// colliding with another artifact.
/// </summary>
public static class SpatialUniqueNameAllocator
{
    /// <summary>Returns a unique name, preserving the requested name when it is available.</summary>
    public static string Allocate(string? requestedName, IEnumerable<string> existingNames, string fallback = "New Object")
    {
        ArgumentNullException.ThrowIfNull(existingNames);

        var existing = new HashSet<string>(
            existingNames.Where(name => !string.IsNullOrWhiteSpace(name)),
            StringComparer.OrdinalIgnoreCase);

        var baseName = string.IsNullOrWhiteSpace(requestedName)
            ? fallback.Trim()
            : requestedName.Trim();

        if (!existing.Contains(baseName))
            return baseName;

        for (var suffix = 2; ; suffix++)
        {
            var candidate = $"{baseName} {suffix}";
            if (!existing.Contains(candidate))
                return candidate;
        }
    }

    /// <summary>Validates a rename against the current in-memory namespace.</summary>
    public static string EnsureUnique(string requestedName, IEnumerable<string> existingNames, string currentName)
    {
        ArgumentNullException.ThrowIfNull(existingNames);

        var names = existingNames
            .Where(name => !string.Equals(name, currentName, StringComparison.OrdinalIgnoreCase));

        return Allocate(requestedName, names);
    }
}
