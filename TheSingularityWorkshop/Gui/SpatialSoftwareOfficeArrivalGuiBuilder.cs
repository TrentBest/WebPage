namespace TheSingularityWorkshop.Gui;

using TheSingularityWorkshop.Workshop.Spatial;

/// <summary>Low-detail spatial presentation for the Singularity Software Inc. arrival sequence.</summary>
public static class SpatialSoftwareOfficeArrivalGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        SpatialSoftwareOfficeArrivalModel arrival,
        string avatarName,
        Action beginHelicopter,
        Action enterHelicopter,
        Action enterElevator,
        Action ding,
        Action completeBriefing,
        Action openBehavior,
        Action exitScene)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", "#02050a").Style("color", "#fff")
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text(
            "@keyframes office-lean{0%,100%{transform:rotate(-3deg)}50%{transform:rotate(2deg)}}" +
            "@keyframes rotor{to{transform:rotate(360deg)}}" +
            "@keyframes heli-in{0%{left:-20%;opacity:0}35%{opacity:1}100%{left:48%;opacity:1}}" +
            "@keyframes heli-out{0%{left:48%;opacity:1}100%{left:120%;opacity:0}}"));

        root.Content(Backdrop(receiver, arrival.Stage));
        root.Content(Status(receiver, arrival, avatarName));

        switch (arrival.Stage)
        {
            case SpatialSoftwareOfficeArrivalStage.Rampway:
                root.Content(ActionButton(receiver, "LAND HELICOPTER", beginHelicopter));
                break;
            case SpatialSoftwareOfficeArrivalStage.HelicopterApproach:
                root.Content(Helicopter(receiver, false));
                root.Content(ActionButton(receiver, "ENTER HELICOPTER", enterHelicopter));
                break;
            case SpatialSoftwareOfficeArrivalStage.HelicopterDeparting:
                root.Content(Helicopter(receiver, true));
                break;
            case SpatialSoftwareOfficeArrivalStage.TowerHelipad:
                root.Content(Helipad(receiver));
                root.Content(ActionButton(receiver, "ENTER ELEVATOR", enterElevator));
                break;
            case SpatialSoftwareOfficeArrivalStage.ElevatorDescending:
                root.Content(Elevator(receiver, arrival));
                root.Content(ActionButton(receiver, "DING", ding));
                root.Content(ActionButton(receiver, "CONTINUE BRIEFING", completeBriefing));
                break;
            case SpatialSoftwareOfficeArrivalStage.FacilityAccess:
                root.Content(Briefing(receiver));
                root.Content(ActionButton(receiver, "OPEN BEHAVIOR WORKBENCH", openBehavior));
                break;
        }

        if (arrival.Stage is SpatialSoftwareOfficeArrivalStage.Rampway or SpatialSoftwareOfficeArrivalStage.TowerHelipad)
            root.Content(Avatar(receiver, avatarName));

        if (arrival.Stage is not (SpatialSoftwareOfficeArrivalStage.TowerHelipad or SpatialSoftwareOfficeArrivalStage.FacilityAccess))
            root.Content(WorkshopGui.Button(receiver).Label("← LEAVE").Style("position", "fixed").Style("right", "1rem").Style("bottom", "1rem").Style("z-index", "30").OnClick(exitScene));

        return root;
    }

    private static ElementBuilder Backdrop(object r, SpatialSoftwareOfficeArrivalStage stage)
        => WorkshopGui.Element(r, "div").Style("position", "absolute").Style("inset", "0")
            .Style("background", stage == SpatialSoftwareOfficeArrivalStage.TowerHelipad
                ? "radial-gradient(circle at 50% 50%,#20342e 0,#07100f 35%,#010306 75%)"
                : "linear-gradient(180deg,#081522,#02050a 70%,#000 100%)");

    private static ElementBuilder Status(object r, SpatialSoftwareOfficeArrivalModel arrival, string avatarName)
        => WorkshopGui.Element(r, "div").Style("position", "fixed").Style("left", "1rem").Style("top", "1rem").Style("z-index", "40")
            .Style("padding", ".65rem .8rem").Style("border", "1px solid #00eaff66").Style("background", "rgba(1,4,10,.88)")
            .Content(WorkshopGui.Element(r, "div").Style("color", "#00eaff").Style("font-size", ".5rem").Style("letter-spacing", ".18em").Text("SINGULARITY SOFTWARE INC."))
            .Content(WorkshopGui.Element(r, "div").Style("margin-top", ".25rem").Style("color", "#d7e8dd").Style("font-size", ".55rem").Text($"VISITOR // {avatarName} // {arrival.Stage.ToString().ToUpperInvariant()}"));

    private static ElementBuilder Helipad(object r)
        => WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "50%").Style("top", "50%").Style("transform", "translate(-50%,-50%)")
            .Style("width", "min(52vw,520px)").Style("height", "min(52vw,520px)").Style("border-radius", "50%")
            .Style("border", "2px solid #ffd34d").Style("background", "radial-gradient(circle,#13221f 0,#07100e 58%,#020406 60%)")
            .Style("box-shadow", "0 0 70px #ffd34d22,inset 0 0 40px #00eaff16")
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "50%").Style("top", "50%").Style("transform", "translate(-50%,-50%)").Style("font-size", "clamp(2rem,8vw,6rem)").Style("color", "#ffd34d").Text("H"))
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "50%").Style("bottom", "8%").Style("transform", "translateX(-50%)").Style("color", "#8ba8b2").Style("font-size", ".55rem").Style("letter-spacing", ".18em").Text("TOWER HELIPAD // ACCESS ELEVATOR ONLY"));

    private static ElementBuilder Helicopter(object r, bool departing)
        => WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", departing ? "48%" : "-20%").Style("top", "45%").Style("width", "210px").Style("height", "110px")
            .Style("animation", departing ? "heli-out 2.6s ease-in forwards" : "heli-in 2.6s ease-out forwards")
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "35px").Style("top", "35px").Style("width", "110px").Style("height", "42px").Style("border", "2px solid #00eaff").Style("border-radius", "50% 42% 35% 45%").Style("background", "#07151c"))
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "135px").Style("top", "45px").Style("width", "70px").Style("height", "8px").Style("border", "2px solid #00eaff").Style("transform", "skewX(-18deg)"))
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "70px").Style("top", "8px").Style("width", "80px").Style("height", "8px").Style("border-top", "2px solid #ffd34d").Style("animation", "rotor .25s linear infinite"));

    private static ElementBuilder Elevator(object r, SpatialSoftwareOfficeArrivalModel arrival)
        => WorkshopGui.Element(r, "div").Style("position", "absolute").Style("inset", "0").Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
            .Content(WorkshopGui.Element(r, "div").Style("width", "min(620px,82vw)").Style("height", "min(620px,78vh)").Style("border", "1px solid #00eaff55").Style("background", "linear-gradient(180deg,#101a20,#020408)").Style("box-shadow", "0 0 50px #00eaff12,inset 0 0 30px #000")
                .Content(WorkshopGui.Element(r, "div").Style("padding", "1.2rem").Style("font-size", ".6rem").Style("color", "#ffd34d").Text("DING"))
                .Content(WorkshopGui.Element(r, "div").Style("height", "70%").Style("margin", "0 12%").Style("border", "1px solid #ffffff18").Style("background", "repeating-linear-gradient(180deg,transparent 0 36px,#00eaff0a 37px 38px)").Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
                    .Content(WorkshopGui.Element(r, "div").Style("text-align", "center").Content(WorkshopGui.Element(r, "div").Style("font-size", "1rem").Style("color", "#fff").Text("CEO // SINGULARITY SOFTWARE INC.")).Content(WorkshopGui.Element(r, "div").Style("margin-top", ".8rem").Style("color", "#8ba8b2").Style("font-family", "system-ui,sans-serif").Style("font-size", ".75rem").Text("We're going a long way down. This building is enormous. While we descend, we'll prepare your identity, access, and workspace. We will give you room and put our teams at your discretion while we work out what you're creating.")))
                .Content(WorkshopGui.Element(r, "div").Style("padding", "1rem 1.2rem").Style("color", "#00eaff").Style("font-size", ".55rem").Text($"ELEVATOR STOPS // {arrival.ElevatorStops:0000}"))));

    private static ElementBuilder Briefing(object r)
        => WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "50%").Style("top", "50%").Style("transform", "translate(-50%,-50%)").Style("width", "min(760px,84vw)").Style("padding", "2rem").Style("border", "1px solid #00eaff66").Style("background", "rgba(1,5,9,.94)")
            .Content(WorkshopGui.Element(r, "div").Style("color", "#00eaff").Style("font-size", ".6rem").Style("letter-spacing", ".18em").Text("FACILITY ACCESS GRANTED"))
            .Content(WorkshopGui.Element(r, "h1").Style("font-size", "clamp(1.5rem,3vw,2.4rem)").Text("We will figure out what you're creating."))
            .Content(WorkshopGui.Element(r, "p").Style("font-family", "system-ui,sans-serif").Style("color", "#b8c9ce").Text("Your identity badge is being prepared. Our architecture, structure, behavior, visualization, play-test, documentation, and publishing teams are available to you. If you have already visited the FSM Forge, the next step is to turn its lifecycle scaffold into behavior."));

    private static ElementBuilder Avatar(object r, string name)
        => WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "50%").Style("bottom", "14%").Style("transform", "translateX(-50%)").Style("z-index", "20")
            .Content(WorkshopGui.Element(r, "div").Style("width", "14px").Style("height", "14px").Style("border", "1px solid #fff").Style("border-radius", "50%").Style("box-shadow", "0 0 14px #fff8"))
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "50%").Style("top", "-1.1rem").Style("transform", "translateX(-50%)").Style("font-size", ".45rem").Text(name));

    private static ElementBuilder ActionButton(object r, string label, Action action)
        => WorkshopGui.Button(r).Label(label).Style("position", "fixed").Style("left", "50%").Style("bottom", "3rem").Style("transform", "translateX(-50%)").Style("z-index", "50").Style("padding", ".8rem 1.2rem").Style("border", "1px solid #ffd34d88").Style("background", "rgba(1,4,10,.9)").Style("color", "#ffd34d").Style("font-family", "inherit").Style("font-size", ".55rem").Style("letter-spacing", ".12em").OnClick(action);
}
