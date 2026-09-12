namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Identifies where content entered the Workshop pipeline.
/// External content is data only until explicitly wrapped by a MicroBundle.
/// </summary>
public enum ContentOrigin
{
    External = 0,
    MicroBundle = 1
}

/// <summary>
/// Describes the onion depth of a MicroBundle's authored tooling surface.
/// Tooling is progressively shed as compilation moves toward runtime leaves.
/// </summary>
public enum MicroBundleLayer
{
    RuntimeLeaf = 0,
    RuntimeBundle = 1,
    AuthoringBundle = 2,
    DevelopmentBundle = 3,
    ConceptualRoot = 4
}

/// <summary>
/// The unwrapped external-data boundary. It intentionally has no environment,
/// ontology behavior, process-group participation, or tooling of its own.
/// </summary>
public sealed record ExternalContent(ulong ContentId, string MediaType, string PayloadReference)
{
    public ContentOrigin Origin => ContentOrigin.External;
}

/// <summary>
/// The MicroBundle shell placed around content during Forge/development time.
/// The shell exposes the providers and metadata needed to edit, inspect,
/// diagnose, compose, and ultimately compile the wrapped content.
/// </summary>
public sealed record MicroBundleEnvelope(
    ulong BundleId,
    ulong ContentId,
    MicroBundleLayer Layer,
    IReadOnlyList<ulong> ProviderIds,
    IReadOnlyList<ulong> ChildBundleIds)
{
    public ContentOrigin Origin => ContentOrigin.MicroBundle;

    public bool IsWrappedExternalContent => ContentId != 0;

    public bool HasTooling => ProviderIds.Count > 0;

    public bool IsConceptualRoot => Layer == MicroBundleLayer.ConceptualRoot;
}
