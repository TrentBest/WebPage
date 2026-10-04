namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Describes what a provider wants manifested without coupling the bundle to
/// Blazor, WPF, CSS, or another presentation technology.
/// </summary>
public sealed class MicroBundleManifestation
{
    public MicroBundleManifestation(int id, string target, string effect)
    {
        Id = id;
        Target = target;
        Effect = effect;
    }

    /// <summary>Integer identity for compact transport and lookup.</summary>
    public int Id { get; }

    /// <summary>Ontology target to which the manifestation applies.</summary>
    public string Target { get; }

    /// <summary>Named manifestation capability, such as Trace, Extrude, or Breathe.</summary>
    public string Effect { get; }

    /// <summary>Optional integer parameters understood by the effect provider.</summary>
    public IReadOnlyDictionary<string, int> Parameters { get; init; }
        = new Dictionary<string, int>();
}
