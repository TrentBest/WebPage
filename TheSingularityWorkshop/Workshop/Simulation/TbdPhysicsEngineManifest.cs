namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Provisional physics-engine boundary for laboratory presentation code.
/// The laboratory can depend on this contract without pretending that a validated
/// numerical engine has already been selected or baked.
/// </summary>
public sealed record TbdPhysicsEngineManifest(
    string Id,
    string Name,
    string Status,
    string Description)
{
    public static TbdPhysicsEngineManifest Default
        => new(
            "physics-engine.tbd",
            "TBD PHYSICS ENGINE",
            "PROVISIONAL",
            "Laboratory experiments are authored against an explicit physics boundary. A validated SRPS-backed numerical provider will occupy this boundary before runtime results are treated as authoritative.");
}
