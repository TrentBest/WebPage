namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;

/// <summary>Inspectable, explicitly fixture-backed Acoustic Studio Lab presentation.</summary>
public static class SpatialAcousticStudioLabGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        SpatialAcousticStudioManifest studio,
        string userName,
        string? selectedSourceId,
        string? selectedListenerId,
        Action<string> selectSource,
        Action<string> selectListener,
        Action exit)
    {
        ArgumentNullException.ThrowIfNull(studio);
        selectedSourceId ??= studio.Sources.FirstOrDefault().Id;
        selectedListenerId ??= studio.Listeners.FirstOrDefault().Id;
        var selectedSource = studio.Sources.FirstOrDefault(x => x.Id == selectedSourceId);
        var selectedListener = studio.Listeners.FirstOrDefault(x => x.Id == selectedListenerId);
        var observation = studio.FindFixtureObservation(selectedSourceId ?? "", selectedListenerId ?? "");

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", "radial-gradient(circle at 50% 35%, #142a3a 0, #03070c 48%, #010204 100%)")
            .Style("color", "#e7f7ff").Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "4vw").Style("right", "4vw").Style("top", "3vh")
            .Style("display", "flex").Style("justify-content", "space-between").Style("align-items", "flex-start").Style("gap", "1rem")
            .Content(WorkshopGui.Element(receiver, "div")
                .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".6rem").Style("letter-spacing", ".2em").Style("color", "#00eaff").Text("THE SINGULARITY WORKSHOP // ACOUSTICS"))
                .Content(WorkshopGui.Element(receiver, "h1").Style("font-size", "clamp(1rem,2vw,1.7rem)").Style("margin", ".4rem 0").Style("color", "#fff").Text(studio.Name))
                .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".52rem").Style("color", "#9db5c4").Text($"{studio.OwnershipLabel} // VISITOR {userName}")))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("padding", ".5rem .65rem").Style("border", "1px solid #ffcf5a66").Style("color", "#ffcf5a").Style("font-size", ".48rem").Style("letter-spacing", ".1em")
                .Text(studio.StatusLabel)));

        var layout = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "4vw").Style("right", "4vw").Style("top", "18vh").Style("bottom", "12vh")
            .Style("display", "grid").Style("grid-template-columns", "minmax(0,1.5fr) minmax(260px,.8fr)").Style("gap", "1rem").Style("min-height", "0");

        var plan = WorkshopGui.Panel(receiver)
            .Style("position", "relative").Style("overflow", "hidden").Style("min-height", "0")
            .Style("border", "1px solid rgba(0,234,255,.28)")
            .Style("background", "linear-gradient(rgba(0,234,255,.055) 1px, transparent 1px), linear-gradient(90deg, rgba(0,234,255,.055) 1px, transparent 1px), rgba(1,8,14,.84)")
            .Style("background-size", "24px 24px");

        foreach (var zone in studio.Zones)
        {
            plan.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", $"{zone.X / studio.WidthMeters * 100:0.##}%").Style("top", $"{zone.Y / studio.HeightMeters * 100:0.##}%")
                .Style("width", $"{zone.Width / studio.WidthMeters * 100:0.##}%").Style("height", $"{zone.Height / studio.HeightMeters * 100:0.##}%")
                .Style("box-sizing", "border-box").Style("border", "1px solid rgba(0,234,255,.36)")
                .Style("background", "rgba(0,234,255,.035)").Style("padding", ".4rem")
                .Style("font-size", ".45rem").Style("letter-spacing", ".1em").Style("color", "#70d9e8")
                .Text(zone.Name));
        }

        foreach (var source in studio.Sources)
        {
            var selected = source.Id == selectedSourceId;
            var button = WorkshopGui.Button(receiver).Label($"◉ {source.Name}")
                .Style("position", "absolute")
                .Style("left", $"{source.PositionMeters.X / studio.WidthMeters * 100:0.##}%")
                .Style("top", $"{source.PositionMeters.Y / studio.HeightMeters * 100:0.##}%")
                .Style("transform", "translate(-50%,-50%)").Style("z-index", "3")
                .Style("padding", ".3rem .4rem").Style("max-width", "42%")
                .Style("border", $"1px solid {(selected ? "#ffcf5a" : "#ff38d1")}")
                .Style("background", selected ? "rgba(60,42,4,.96)" : "rgba(24,4,22,.92)")
                .Style("color", selected ? "#ffcf5a" : "#ff9be8").Style("font-family", "inherit").Style("font-size", ".42rem")
                .Style("cursor", "pointer").OnClick(() => selectSource(source.Id));
            plan.Content(button);
        }

        foreach (var listener in studio.Listeners)
        {
            var selected = listener.Id == selectedListenerId;
            var button = WorkshopGui.Button(receiver).Label($"⬟ {listener.Name}")
                .Style("position", "absolute")
                .Style("left", $"{listener.PositionMeters.X / studio.WidthMeters * 100:0.##}%")
                .Style("top", $"{listener.PositionMeters.Y / studio.HeightMeters * 100:0.##}%")
                .Style("transform", "translate(-50%,-50%)").Style("z-index", "4")
                .Style("padding", ".3rem .4rem").Style("max-width", "42%")
                .Style("border", $"1px solid {(selected ? "#8aff9b" : "#4b9fff")}")
                .Style("background", selected ? "rgba(4,44,22,.96)" : "rgba(2,17,39,.92)")
                .Style("color", selected ? "#8aff9b" : "#9bcaff").Style("font-family", "inherit").Style("font-size", ".42rem")
                .Style("cursor", "pointer").OnClick(() => selectListener(listener.Id));
            plan.Content(button);
        }

        plan.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", ".7rem").Style("bottom", ".6rem")
            .Style("font-size", ".42rem").Style("line-height", "1.6").Style("color", "#9db5c4")
            .Text($"PLAN // {studio.WidthMeters:0.#} m × {studio.HeightMeters:0.#} m // ◉ SOURCE   ⬟ LISTENER"));

        var inspector = WorkshopGui.Panel(receiver)
            .Style("overflow-y", "auto").Style("padding", ".8rem").Style("border", "1px solid rgba(255,207,90,.28)")
            .Style("background", "rgba(4,8,14,.9)");

        inspector.Content(Section(receiver, "SELECTED SOURCE", selectedSource.Name, selectedSource.Description,
            $"POSITION // {selectedSource.PositionMeters.X:0.##}, {selectedSource.PositionMeters.Y:0.##}, {selectedSource.PositionMeters.Z:0.##} m"));
        inspector.Content(Section(receiver, "SELECTED LISTENER", selectedListener.Name, selectedListener.Description,
            $"POSITION // {selectedListener.PositionMeters.X:0.##}, {selectedListener.PositionMeters.Y:0.##}, {selectedListener.PositionMeters.Z:0.##} m"));

        inspector.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".7rem").Style("padding", ".7rem")
            .Style("border", "1px solid rgba(255,207,90,.25)").Style("background", "rgba(255,207,90,.035)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".5rem").Style("letter-spacing", ".14em").Style("color", "#ffcf5a").Text("OBSERVATION // ILLUSTRATIVE FIXTURE"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("font-size", ".43rem").Style("line-height", "1.6").Style("color", "#b8c9d4")
                .Text(observation is null
                    ? "No authored fixture exists for this source/listener pair. Physics adapter is not connected; no acoustic values are inferred."
                    : $"Distance {observation.DistanceMeters:0.##} m // Delay {observation.PropagationDelayMilliseconds:0.##} ms // {observation.DirectionDescription} // Relative level {observation.RelativeLevelDb:0.#} dB"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".45rem").Style("font-size", ".4rem").Style("line-height", "1.5").Style("color", "#718896")
                .Text("These authored demonstration values are not calculated by the Physics solver. The WebPage adapter seam is intentionally not connected to the unpublished Physics PR.")));

        layout.Content(plan).Content(inspector);
        root.Content(layout);
        root.Content(WorkshopGui.Button(receiver).Label("← RETURN TO WORKSHOP")
            .Style("position", "fixed").Style("left", "4vw").Style("bottom", "3vh").Style("z-index", "20")
            .Style("padding", ".5rem .7rem").Style("border", "1px solid #00eaff66")
            .Style("background", "rgba(0,15,22,.95)").Style("color", "#00eaff")
            .Style("font-family", "inherit").Style("font-size", ".45rem").Style("letter-spacing", ".1em")
            .Style("cursor", "pointer").OnClick(exit));

        return root;
    }

    private static ElementBuilder Section(object receiver, string heading, string name, string description, string position)
        => WorkshopGui.Panel(receiver)
            .Style("margin-bottom", ".6rem").Style("padding", ".65rem")
            .Style("border", "1px solid rgba(0,234,255,.18)").Style("background", "rgba(0,234,255,.025)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".45rem").Style("letter-spacing", ".12em").Style("color", "#00eaff").Text(heading))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("font-size", ".6rem").Style("color", "#fff").Text(name))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".3rem").Style("font-size", ".43rem").Style("line-height", "1.5").Style("color", "#aabdc7").Text(description))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("font-size", ".43rem").Style("color", "#8aff9b").Text(position));
}
