namespace TheSingularityWorkshop.Services;

/// <summary>
/// Describes a GUI-native experience that may be manifested while the application is idle.
/// Idle is a presentation condition, not the identity of the experience itself.
/// </summary>
public sealed record IdleExperienceDefinition(string Id, string DisplayName, string Description);

/// <summary>
/// Registry boundary for experiences that may be selected by the screen-saver layer.
/// The registry deliberately knows nothing about Unity, graphics engines, or navigation.
/// </summary>
public static class IdleExperienceCatalog
{
    private static readonly IdleExperienceDefinition[] Experiences =
    [
        new("pong", "PONG", "A tiny arcade machine hiding inside the Workshop.")
    ];

    /// <summary>Gets the currently available idle experiences.</summary>
    public static IReadOnlyList<IdleExperienceDefinition> Available => Experiences;

    /// <summary>Returns an experience using the supplied random source.</summary>
    public static IdleExperienceDefinition Select(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);
        return Experiences[random.Next(Experiences.Length)];
    }
}
