namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;

/// <summary>
/// Presents Singularity City's first government as an explorable civic place.
/// The builder exposes the institutions and election rules without hard-coding
/// a political outcome; citizens supply candidates and votes through the future
/// persistent civic service.
/// </summary>
public static class SingularityGovernmentGuiBuilder
{
    private const string Ink = "#01040a";
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffe04a";
    private const string Magenta = "#ff3b8d";
    private const string White = "#ffffff";

    public static ElementBuilder Build(
        object receiver,
        string citizenName,
        DateTimeOffset now,
        Action exitRoom)
    {
        var rules = SingularityElectionCatalog.DefaultRules;
        var offices = SingularityGovernmentCatalog.Offices;
        var nextElection = NextElection(now, rules.ElectionInterval);

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "relative")
            .Style("width", "100vw")
            .Style("height", "100vh")
            .Style("overflow", "auto")
            .Style("background", Ink)
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Style("padding", "2rem");

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Yellow)
            .Style("font-size", "1rem")
            .Style("letter-spacing", ".25em")
            .Text("SINGULARITY GOVERNMENT"));

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".55rem")
            .Style("max-width", "760px")
            .Style("color", Cyan)
            .Style("font-size", ".55rem")
            .Style("line-height", "1.6")
            .Text($"CIVIC CORE // CITIZEN {citizenName} // NEXT ELECTION {nextElection:yyyy-MM-dd}"));

        root.Content(Section(receiver, "THE FIRST POWER", Magenta,
            "Singularity Mayor — elected city executive. The Mayor administers the city builder, proposes civic priorities, and coordinates the city's departments."));

        var grid = WorkshopGui.Element(receiver, "div")
            .Style("display", "grid")
            .Style("grid-template-columns", "repeat(auto-fit,minmax(260px,1fr))")
            .Style("gap", ".7rem")
            .Style("margin-top", "1.2rem");

        foreach (var office in offices)
        {
            var card = WorkshopGui.Panel(receiver)
                .Style("padding", ".8rem")
                .Style("border", $"1px solid {(office.Elected ? Magenta : Cyan)}66")
                .Style("background", "rgba(8,18,24,.72)");

            card.Content(WorkshopGui.Element(receiver, "div")
                .Style("color", office.Elected ? Magenta : Cyan)
                .Style("font-size", ".48rem")
                .Style("letter-spacing", ".13em")
                .Text(office.Title));

            card.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".4rem")
                .Style("color", White)
                .Style("font-size", ".38rem")
                .Style("line-height", "1.55")
                .Text(office.Purpose));

            card.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".5rem")
                .Style("color", "#91aeb8")
                .Style("font-size", ".34rem")
                .Text($"{office.Branch.ToString().ToUpperInvariant()} // {(office.Elected ? "ELECTED" : "ADMINISTRATIVE")}"));

            grid.Child(card);
        }

        root.Content(grid);

        root.Content(Section(receiver, "CITY BUILDER CIVIC LOOP", Yellow,
            $"Elections recur every {rules.ElectionInterval.TotalDays:0} days. Citizens may run for elected offices and citizens may vote. Campaign window: {rules.CampaignWindow.TotalDays:0} days."));

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", "1rem")
            .Style("padding", "1rem")
            .Style("border", $"1px dashed {Cyan}66")
            .Style("background", "rgba(0,234,255,.025)")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Cyan)
                .Style("font-size", ".42rem")
                .Style("letter-spacing", ".12em")
                .Text("WHAT GOVERNMENT CONTROLS"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".55rem")
                .Style("color", White)
                .Style("font-size", ".38rem")
                .Style("line-height", "1.7")
                .Text("DISTRICTS // FUNDING // BUILDING RULES // PUBLIC SERVICES // CIVIC PROJECTS // DIGITAL REAL ESTATE RULES"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".55rem")
                .Style("color", "#91aeb8")
                .Style("font-size", ".35rem")
                .Style("line-height", "1.6")
                .Text("The elected government becomes one of the city's simulation systems. Future civic actions will modify the city's rules and feed those rules into building and district specifications.")));

        root.Content(WorkshopGui.Button(receiver)
            .Label("RETURN TO SINGULARITY CITY")
            .Style("margin-top", "1.4rem")
            .Style("padding", ".65rem 1rem")
            .Style("border", $"1px solid {Cyan}99")
            .Style("border-radius", "0")
            .Style("background", Ink)
            .Style("color", Cyan)
            .Style("font-family", "inherit")
            .Style("font-size", ".45rem")
            .Style("letter-spacing", ".1em")
            .Style("cursor", "pointer")
            .OnClick(exitRoom));

        return root;
    }

    private static ElementBuilder Section(object receiver, string title, string accent, string body)
        => WorkshopGui.Panel(receiver)
            .Style("margin-top", "1.2rem")
            .Style("max-width", "900px")
            .Style("padding", ".85rem")
            .Style("border-left", $"3px solid {accent}")
            .Style("background", "rgba(255,255,255,.025)")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", accent)
                .Style("font-size", ".46rem")
                .Style("letter-spacing", ".15em")
                .Text(title))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".45rem")
                .Style("color", White)
                .Style("font-size", ".38rem")
                .Style("line-height", "1.7")
                .Text(body));

    private static DateTimeOffset NextElection(DateTimeOffset now, TimeSpan interval)
    {
        var epoch = DateTimeOffset.UnixEpoch;
        var elapsed = now - epoch;
        var periods = Math.Max(0, (long)Math.Ceiling(elapsed.TotalSeconds / interval.TotalSeconds));
        return epoch.AddTicks(interval.Ticks * periods);
    }
}
