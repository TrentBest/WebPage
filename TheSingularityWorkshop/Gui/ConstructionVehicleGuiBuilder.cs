namespace TheSingularityWorkshop.Gui;

/// <summary>Builds a deliberately crude, data-shaped manifestation of a construction machine.</summary>
public static class ConstructionVehicleGuiBuilder
{
    private const string Ink = "#01040a";
    private const string Yellow = "#ffd34d";
    private const string Cyan = "#00eaff";
    private const string White = "#ffffff";

    /// <summary>Returns the semantic parts that make up the machine's first visual manifestation.</summary>
    public static IReadOnlyList<ConstructionVehiclePart> Parts(ConstructionVehicleKind kind)
        => kind switch
        {
            ConstructionVehicleKind.Backhoe =>
            [new("CAB", "operator cab"), new("SCOOP", "front scoop"), new("BOOM", "rear boom"), new("STICK", "rear stick"), new("BUCKET", "digging bucket"), new("TIRE", "wheel", 4)],
            ConstructionVehicleKind.Crane =>
            [new("CAB", "operator cab"), new("BOOM", "crane boom"), new("HOOK", "lifting hook"), new("OUTRIGGER", "grounding outrigger", 4), new("TIRE", "wheel", 4)],
            ConstructionVehicleKind.Rover =>
            [new("CAB", "operator enclosure"), new("SENSOR", "sensor mast"), new("BED", "utility bed"), new("TIRE", "wheel", 4)],
            ConstructionVehicleKind.Lifter =>
            [new("CAB", "operator cab"), new("MAST", "lift mast"), new("FORK", "fork", 2), new("TIRE", "wheel", 4)],
            _ =>
            [new("CAB", "operator cab"), new("BED", "dump bed"), new("TAILGATE", "tailgate"), new("TIRE", "wheel", 4)]
        };

    /// <summary>Creates the compact recursive GUI manifestation.</summary>
    public static ElementBuilder Build(object receiver, ConstructionVehicle vehicle)
    {
        var root = WorkshopGui.Element(receiver, "div")
            .Style("position", "relative").Style("width", "150px").Style("height", "92px")
            .Style("box-sizing", "border-box").Style("border", $"1px solid {Yellow}66")
            .Style("background", "rgba(1,4,10,.9)").Style("font-family", "Consolas, 'Courier New', monospace")
            .Style("pointer-events", "none").AriaLabel($"{vehicle.Name} // {vehicle.Kind}");

        root.Content(Body(receiver, vehicle.Kind));
        foreach (var part in Parts(vehicle.Kind)) root.Content(PartLabel(receiver, part));
        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "4px").Style("bottom", "3px")
            .Style("font-size", ".4rem").Style("letter-spacing", ".08em").Style("color", $"{White}aa")
            .Text($"{vehicle.Kind.ToString().ToUpperInvariant()} // {vehicle.Name}"));
        return root;
    }

    private static ElementBuilder Body(object receiver, ConstructionVehicleKind kind)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "16px").Style("right", "16px")
            .Style("top", "28px").Style("height", "30px").Style("border", $"1px solid {Cyan}77")
            .Style("background", $"{Cyan}10")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute")
                .Style("left", kind == ConstructionVehicleKind.Crane ? "40px" : "72px")
                .Style("top", "4px").Style("width", "28px").Style("height", "22px")
                .Style("border", $"1px solid {White}88").Style("background", $"{White}08"));

    private static ElementBuilder PartLabel(object receiver, ConstructionVehiclePart part)
    {
        var count = part.Count > 1 ? $" ×{part.Count}" : string.Empty;
        var position = part.Code switch
        {
            "CAB" => ("47px", "7px"), "SCOOP" => ("0px", "34px"), "BOOM" => ("88px", "7px"),
            "STICK" => ("108px", "25px"), "BUCKET" => ("118px", "47px"), "BED" => ("72px", "15px"),
            "TAILGATE" => ("104px", "15px"), "HOOK" => ("124px", "10px"), "OUTRIGGER" => ("4px", "68px"),
            "SENSOR" => ("64px", "0px"), "MAST" => ("80px", "4px"), "FORK" => ("112px", "58px"),
            "TIRE" => ("16px", "62px"), _ => ("4px", "4px")
        };

        return WorkshopGui.Element(receiver, "span")
            .Style("position", "absolute").Style("left", position.Item1).Style("top", position.Item2)
            .Style("font-size", ".36rem").Style("line-height", "1").Style("letter-spacing", ".05em")
            .Style("white-space", "nowrap").Style("padding", "1px 2px")
            .Style("border", $"1px solid {Yellow}55").Style("background", $"{Ink}dd")
            .Style("color", Yellow).Text($"{part.Code}{count}");
    }
}

/// <summary>A semantic machine part used by the crude GUI manifestation.</summary>
public readonly record struct ConstructionVehiclePart(string Code, string Description, int Count = 1);
