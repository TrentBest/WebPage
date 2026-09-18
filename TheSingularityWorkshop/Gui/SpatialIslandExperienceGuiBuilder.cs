namespace TheSingularityWorkshop.Gui;

using System;
using Microsoft.AspNetCore.Components.Web;

/// <summary>Renders Singularity Island as a living attraction map.</summary>
public static class SpatialIslandExperienceGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        SpatialIslandExperience island,
        string userName,
        Action<string> enterAttraction,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", "radial-gradient(circle at 50% 45%, #29115c 0, #080311 48%, #010104 100%)")
            .Style("color", "#fff").Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("background", "repeating-linear-gradient(0deg, transparent 0 38px, rgba(0,234,255,.035) 39px, transparent 40px)")
            .Style("pointer-events", "none"));

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "5vw").Style("top", "4vh").Style("z-index", "5")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", "clamp(1.2rem,4vw,3.5rem)").Style("letter-spacing", ".22em").Style("color", "#ffd34d").Text("SINGULARITY ISLAND"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".5rem").Style("font-size", ".55rem").Style("letter-spacing", ".2em").Style("color", "#00eaff").Text($"PLEASURE OF DISCOVERY // VISITOR {userName.ToUpperInvariant()}"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".7rem").Style("max-width", "720px").Style("font-size", ".58rem").Style("line-height", "1.5").Style("color", "#c7bdcf").Text(island.Description)));

        var map = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "5vw").Style("right", "5vw").Style("top", "28vh").Style("bottom", "10vh")
            .Style("display", "grid").Style("grid-template-columns", "repeat(auto-fit,minmax(190px,1fr))").Style("gap", "1rem")
            .Style("overflow", "auto");

        foreach (var attraction in island.Attractions)
        {
            var card = WorkshopGui.Panel(receiver)
                .Style("position", "relative").Style("min-height", "150px")
                .Style("border", "1px solid rgba(0,234,255,.3)")
                .Style("background", "rgba(5,4,18,.72)")
                .Style("box-shadow", "inset 0 0 35px rgba(0,234,255,.04)")
                .Style("padding", "1rem").Style("box-sizing", "border-box")
                .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".42rem").Style("letter-spacing", ".16em").Style("color", "#ff38d1").Text(attraction.Kind.ToString().ToUpperInvariant()))
                .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".55rem").Style("font-size", ".72rem").Style("font-weight", "700").Style("color", "#fff").Text(attraction.Name))
                .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".55rem").Style("font-size", ".5rem").Style("line-height", "1.45").Style("color", "#aeb7c0").Text(attraction.Description))
                .Content(WorkshopGui.Button(receiver).Label("ENTER EXPERIENCE →")
                    .Style("position", "absolute").Style("left", "1rem").Style("right", "1rem").Style("bottom", "1rem")
                    .Style("padding", ".5rem").Style("border", "1px solid #ffd34d66")
                    .Style("background", "rgba(255,211,77,.06)").Style("color", "#ffd34d")
                    .Style("font-family", "inherit").Style("font-size", ".42rem").Style("letter-spacing", ".1em")
                    .Style("cursor", "pointer").OnClick(() => enterAttraction(attraction.ExperienceId)));

            map.Content(card);
        }

        root.Content(map);
        root.Content(WorkshopGui.Button(receiver).Label("← RETURN TO STATION")
            .Style("position", "fixed").Style("left", "5vw").Style("bottom", "2vh").Style("z-index", "10")
            .Style("padding", ".55rem .8rem").Style("border", "1px solid #ff38d166")
            .Style("background", "rgba(20,4,18,.9)").Style("color", "#ff38d1")
            .Style("font-family", "inherit").Style("font-size", ".42rem").Style("letter-spacing", ".1em")
            .Style("cursor", "pointer").OnClick(exit));

        return root;
    }
}
