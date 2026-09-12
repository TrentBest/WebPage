namespace TheSingularityWorkshop.Services;

/// <summary>
/// Defines the presentation category for Workshop experiences intended to demonstrate
/// something deliberately difficult, surprising, or visually excessive.
/// A Flex is an experience category, not a rendering technology.
/// </summary>
public sealed record FlexExperienceDefinition(string Id, string DisplayName, string Description);

/// <summary>
/// Runtime catalog for Flex experiences. The first Flex is the Living GUI.
/// </summary>
public static class FlexExperienceCatalog
{
    private static readonly FlexExperienceDefinition[] Experiences =
    [
        new(
            "living-gui",
            "LIVING GUI",
            "A GUI that recursively grows, doubles, and reproduces its lineage in real time.")
    ];

    public static IReadOnlyList<FlexExperienceDefinition> Available => Experiences;

    public static FlexExperienceDefinition Select(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);
        return Experiences[random.Next(Experiences.Length)];
    }

    public static bool IsAvailable(string id) => Experiences.Any(x => x.Id == id);
}