namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Declarative first-visit garden that teaches the spatial Experience before the
/// visitor reaches the Workshop. The garden is a threshold, not a replacement
/// for the larger universe.
/// </summary>
public sealed class SpatialGardenManifest
{
    private SpatialGardenManifest(
        string id,
        string name,
        string purpose,
        IReadOnlyList<SpatialGardenGate> gates,
        SpatialGardenLandmark gazebo,
        SpatialGardenLandmark settingsShed)
    {
        Id = id;
        Name = name;
        Purpose = purpose;
        Gates = gates;
        Gazebo = gazebo;
        SettingsShed = settingsShed;
    }

    public string Id { get; }
    public string Name { get; }
    public string Purpose { get; }
    public IReadOnlyList<SpatialGardenGate> Gates { get; }
    public SpatialGardenLandmark Gazebo { get; }
    public SpatialGardenLandmark SettingsShed { get; }

    public static SpatialGardenManifest CreateDefault()
        => new(
            "welcome-garden",
            "THE GARDEN",
            "A small human-scale place to learn movement, interaction, and the idea that Experiences are places rather than pages.",
            [
                new("hangout", "HANGOUT", "Social space", SpatialGardenGateSide.Left, "hangout"),
                new("exit", "EXIT", "Leave the Workshop Experience", SpatialGardenGateSide.Center, "exit"),
                new("singularity-station", "SINGULARITY STATION", "Gateway to the larger universe", SpatialGardenGateSide.Right, "singularity-station")
            ],
            new("gazebo", "SOCIAL GAZEBO", "A quiet place for conversation, gathering, and future shared Experiences.", "left"),
            new("settings-shed", "SETTING SHED", "Personal settings and visitor controls live here.", "right"));

    public SpatialGardenGate? FindGate(string id)
        => Gates.FirstOrDefault(x => x.Id == id);
}

public enum SpatialGardenGateSide
{
    Left,
    Center,
    Right
}

public readonly record struct SpatialGardenGate(
    string Id,
    string Name,
    string Purpose,
    SpatialGardenGateSide Side,
    string DestinationId);

public readonly record struct SpatialGardenLandmark(
    string Id,
    string Name,
    string Purpose,
    string Side);
