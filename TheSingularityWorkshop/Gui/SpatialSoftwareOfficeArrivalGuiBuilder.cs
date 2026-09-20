namespace TheSingularityWorkshop.Gui;

using TheSingularityWorkshop.Workshop.Spatial;

/// <summary>
/// Cinematic presentation for the Workshop helipad transfer to Singularity Software Inc.
/// Motion is driven by the pure-data LUT physics microbundle rather than CSS pretending to be physics.
/// </summary>
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
            "@keyframes rotor{to{transform:rotate(360deg)}}" +
            "@keyframes ramp-breathe{0%,100%{opacity:.62}50%{opacity:1}}" +
            "@keyframes warning-pulse{0%,100%{opacity:.72}50%{opacity:1}}" +
            "@keyframes wind-lines{0%{transform:translateX(-12%)}100%{transform:translateX(112%)}}"));

        root.Content(Backdrop(receiver, arrival));
        root.Content(Status(receiver, arrival, avatarName));

        switch (arrival.Stage)
        {
            case SpatialSoftwareOfficeArrivalStage.Rampway:
                root.Content(Rampway(receiver));
                root.Content(ActionButton(receiver, "WALK UP TO THE HELIPAD", beginHelicopter));
                break;

            case SpatialSoftwareOfficeArrivalStage.HelipadReady:
                root.Content(Rampway(receiver, true));
                root.Content(WaitingHelicopter(receiver));
                root.Content(ActionButton(receiver, "BOARD HELICOPTER", enterHelicopter));
                break;

            case SpatialSoftwareOfficeArrivalStage.HelicopterTakeoff:
                root.Content(WorkshopHelipad(receiver));
                root.Content(Helicopter(receiver, arrival.FlightPose, false));
                break;

            case SpatialSoftwareOfficeArrivalStage.TowerReveal:
                root.Content(RicketyTower(receiver, arrival.TowerSway, arrival.CinematicProgress));
                break;

            case SpatialSoftwareOfficeArrivalStage.HelicopterLanding:
                root.Content(RicketyTower(receiver, arrival.TowerSway, arrival.CinematicProgress));
                root.Content(Helicopter(receiver, arrival.FlightPose, true));
                break;

            case SpatialSoftwareOfficeArrivalStage.TowerFloorPlan:
                root.Content(TowerFloorPlan(receiver));
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

        if (arrival.Stage is SpatialSoftwareOfficeArrivalStage.Rampway or SpatialSoftwareOfficeArrivalStage.HelipadReady)
            root.Content(Avatar(receiver, avatarName));

        if (arrival.Stage is SpatialSoftwareOfficeArrivalStage.Rampway or SpatialSoftwareOfficeArrivalStage.HelipadReady)
            root.Content(WorkshopGui.Button(receiver).Label("← LEAVE").Style("position", "fixed").Style("right", "1rem").Style("bottom", "1rem").Style("z-index", "30").OnClick(exitScene));

        return root;
    }

    private static ElementBuilder Backdrop(object r, SpatialSoftwareOfficeArrivalModel arrival)
    {
        var background = arrival.Stage switch
        {
            SpatialSoftwareOfficeArrivalStage.Rampway or SpatialSoftwareOfficeArrivalStage.HelipadReady
                => "linear-gradient(165deg,#081522 0%,#0d2530 42%,#02050a 100%)",
            SpatialSoftwareOfficeArrivalStage.TowerReveal or SpatialSoftwareOfficeArrivalStage.HelicopterLanding
                => "radial-gradient(circle at 50% 38%,#182e38 0,#061016 34%,#010306 78%)",
            SpatialSoftwareOfficeArrivalStage.TowerFloorPlan
                => "radial-gradient(circle at 50% 50%,#172d2b 0,#07100f 40%,#010306 76%)",
            _ => "linear-gradient(180deg,#081522,#02050a 70%,#000 100%)"
        };

        return WorkshopGui.Element(r, "div").Style("position", "absolute").Style("inset", "0")
            .Style("background", background);
    }

    private static ElementBuilder Status(object r, SpatialSoftwareOfficeArrivalModel arrival, string avatarName)
        => WorkshopGui.Element(r, "div").Style("position", "fixed").Style("left", "1rem").Style("top", "1rem").Style("z-index", "40")
            .Style("padding", ".65rem .8rem").Style("border", "1px solid #00eaff66").Style("background", "rgba(1,4,10,.88)")
            .Content(WorkshopGui.Element(r, "div").Style("color", "#00eaff").Style("font-size", ".5rem").Style("letter-spacing", ".18em").Text("SINGULARITY SOFTWARE INC."))
            .Content(WorkshopGui.Element(r, "div").Style("margin-top", ".25rem").Style("color", "#d7e8dd").Style("font-size", ".55rem").Text($"VISITOR // {avatarName} // {arrival.Stage.ToString().ToUpperInvariant()}"));

    private static ElementBuilder Rampway(object r, bool reachedHelipad = false)
    {
        var root = WorkshopGui.Element(r, "div").Style("position", "absolute").Style("inset", "0").Style("overflow", "hidden");

        root.Content(WorkshopGui.Element(r, "div")
            .Style("position", "absolute").Style("left", "-10%").Style("bottom", "-6%")
            .Style("width", "120%").Style("height", reachedHelipad ? "72%" : "54%")
            .Style("transform", "perspective(700px) rotateX(52deg) rotateZ(-5deg)")
            .Style("transform-origin", "center bottom")
            .Style("background", "repeating-linear-gradient(90deg,#0a161c 0 22px,#10262f 23px 25px)")
            .Style("border", "1px solid #ffd34d55")
            .Style("box-shadow", "0 -20px 60px #00eaff10")
            .Style("animation", "ramp-breathe 2.8s ease-in-out infinite"));

        for (var i = 0; i < 13; i++)
        {
            root.Content(WorkshopGui.Element(r, "div")
                .Style("position", "absolute").Style("left", $"{8 + i * 7}%")
                .Style("bottom", $"{8 + i * 4.4}%").Style("width", "4px").Style("height", "16%")
                .Style("background", "#ffd34d55").Style("transform", "rotate(-5deg)").Style("pointer-events", "none"));
        }

        root.Content(WorkshopGui.Element(r, "div")
            .Style("position", "absolute").Style("left", "50%").Style("top", "12%")
            .Style("transform", "translateX(-50%)").Style("color", "#ffd34d")
            .Style("font-size", ".55rem").Style("letter-spacing", ".2em")
            .Text(reachedHelipad ? "WORKSHOP HELIPAD // HELICOPTER READY" : "WORKSHOP // ACCESS RAMP // CLIMB"));

        if (reachedHelipad)
            root.Content(HelipadCircle(r, 50, 25, "WORKSHOP HELIPAD"));

        return root;
    }

    private static ElementBuilder WorkshopHelipad(object r)
        => WorkshopGui.Element(r, "div").Style("position", "absolute").Style("inset", "0")
            .Content(HelipadCircle(r, 50, 56, "DEPARTURE // WORKSHOP HELIPAD"));

    private static ElementBuilder HelipadCircle(object r, double left, double top, string label)
        => WorkshopGui.Element(r, "div")
            .Style("position", "absolute").Style("left", $"{left}%").Style("top", $"{top}%")
            .Style("transform", "translate(-50%,-50%)")
            .Style("width", "min(38vw,390px)").Style("height", "min(38vw,390px)")
            .Style("border-radius", "50%").Style("border", "2px solid #ffd34d")
            .Style("background", "radial-gradient(circle,#13221f 0,#07100e 57%,#020406 60%)")
            .Style("box-shadow", "0 0 70px #ffd34d22,inset 0 0 40px #00eaff16")
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "50%").Style("top", "50%").Style("transform", "translate(-50%,-50%)").Style("font-size", "clamp(2rem,7vw,5rem)").Style("color", "#ffd34d").Text("H"))
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "50%").Style("bottom", "7%").Style("transform", "translateX(-50%)").Style("color", "#8ba8b2").Style("font-size", ".5rem").Style("letter-spacing", ".15em").Text(label));

    private static ElementBuilder WaitingHelicopter(object r)
        => Helicopter(r, new SpatialHelicopterPhysicsSample(0, .5, 0, 0, 0, .7, .5, 0), false)
            .Style("left", "calc(50% - 105px)").Style("top", "18%");

    private static ElementBuilder Helicopter(object r, SpatialHelicopterPhysicsSample pose, bool landing)
    {
        var left = 12d + pose.X * 76d;
        var top = 64d - pose.Y * 24d;
        var scale = .72d + pose.Z * .42d;
        var pitch = pose.Pitch * 18d;

        return WorkshopGui.Element(r, "div")
            .Style("position", "absolute")
            .Style("left", $"{left:0.###}%").Style("top", $"{top:0.###}%")
            .Style("width", "210px").Style("height", "110px")
            .Style("transform", $"translate(-50%,-50%) scale({scale:0.###}) rotate({pitch:0.###}deg)")
            .Style("z-index", "25")
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "35px").Style("top", "35px").Style("width", "110px").Style("height", "42px").Style("border", "2px solid #00eaff").Style("border-radius", "50% 42% 35% 45%").Style("background", "#07151c"))
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "135px").Style("top", "45px").Style("width", "70px").Style("height", "8px").Style("border", "2px solid #00eaff").Style("transform", "skewX(-18deg)"))
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "70px").Style("top", "8px").Style("width", "80px").Style("height", "8px").Style("border-top", "2px solid #ffd34d").Style("animation", "rotor .18s linear infinite"))
            .Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "55px").Style("top", "75px").Style("width", "78px").Style("height", "5px").Style("border-bottom", $"1px solid {(landing ? "#ffd34d" : "#00eaff")}88").Text(""));
    }

    private static ElementBuilder RicketyTower(object r, SpatialTowerSwaySample sway, double progress)
    {
        var roll = sway.Roll;
        var pitch = sway.Pitch;
        var yaw = sway.Yaw;

        var tower = WorkshopGui.Element(r, "div")
            .Style("position", "absolute").Style("left", "50%").Style("bottom", "-8%")
            .Style("width", "min(32vw,360px)").Style("height", "108%")
            .Style("transform", $"translateX(-50%) perspective(800px) rotate({roll:0.###}deg) rotateX({pitch:0.###}deg) rotateY({yaw:0.###}deg)")
            .Style("transform-origin", "bottom center")
            .Style("background", "repeating-linear-gradient(165deg,#101b23 0 10px,#172b35 11px 18px,#0a1118 19px 28px)")
            .Style("border", "1px solid #00eaff88")
            .Style("box-shadow", "0 0 50px #00eaff1a,inset 0 0 35px #00eaff0b")
            .Style("z-index", "10");

        for (var i = 0; i < 19; i++)
        {
            var left = 7 + (i % 4) * 29;
            var top = 3 + i * 5.1;
            tower.Content(WorkshopGui.Element(r, "div")
                .Style("position", "absolute").Style("left", $"{left}%").Style("top", $"{top}%")
                .Style("width", "22%").Style("height", "3px")
                .Style("background", i % 2 == 0 ? "#00eaff77" : "#ffd34d66"));
        }

        tower.Content(WorkshopGui.Element(r, "div")
            .Style("position", "absolute").Style("left", "50%").Style("top", "7%")
            .Style("transform", "translateX(-50%) rotate(-3deg)")
            .Style("padding", ".3rem .5rem").Style("border", "1px solid #fff4")
            .Style("background", "#01050bd9").Style("color", "#fff").Style("font-size", ".48rem")
            .Style("letter-spacing", ".1em").Style("white-space", "nowrap")
            .Text("SINGULARITY SOFTWARE INC. // MASS IN MOTION"));

        for (var i = 0; i < 6; i++)
            tower.Content(WorkshopGui.Element(r, "div")
                .Style("position", "absolute").Style("left", "-30%").Style("top", $"{12 + i * 14}%")
                .Style("width", "160%").Style("height", "1px")
                .Style("background", "#d7f9ff12").Style("transform", $"translateX({(i % 2 == 0 ? -1 : 1) * progress * 40}%)")
                .Style("pointer-events", "none"));

        return tower;
    }

    private static ElementBuilder TowerFloorPlan(object r)
    {
        var plan = WorkshopGui.Element(r, "div").Style("position", "absolute").Style("inset", "0")
            .Style("padding", "7vh 7vw").Style("box-sizing", "border-box");

        plan.Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "8%").Style("top", "8%").Style("color", "#00eaff").Style("font-size", ".55rem").Style("letter-spacing", ".2em").Text("SINGULARITY SOFTWARE INC. // TOWER FLOOR PLAN"));

        var floor = WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "14%").Style("top", "18%").Style("width", "72%").Style("height", "68%")
            .Style("border", "1px solid #00eaff66").Style("background", "#071017").Style("box-shadow", "inset 0 0 70px #00eaff08");

        floor.Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "8%").Style("top", "12%").Style("width", "28%").Style("height", "30%").Style("border", "1px solid #ffd34d55").Style("background", "#ffd34d06").Text("ARCHITECTURE"));
        floor.Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "40%").Style("top", "12%").Style("width", "28%").Style("height", "30%").Style("border", "1px solid #00eaff55").Style("background", "#00eaff06").Text("STRUCTURE"));
        floor.Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "72%").Style("top", "12%").Style("width", "20%").Style("height", "30%").Style("border", "1px solid #ff2cff55").Style("background", "#ff2cff06").Text("BEHAVIOR"));
        floor.Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "8%").Style("top", "50%").Style("width", "40%").Style("height", "34%").Style("border", "1px solid #00eaff44").Style("background", "#00eaff04").Text("VISUALIZATION"));
        floor.Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "52%").Style("top", "50%").Style("width", "40%").Style("height", "34%").Style("border", "1px solid #ffd34d44").Style("background", "#ffd34d04").Text("PLAY TEST"));
        floor.Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "45%").Style("top", "42%").Style("width", "10%").Style("height", "16%").Style("border", "2px solid #ffd34d").Style("background", "#ffd34d12").Style("display", "flex").Style("align-items", "center").Style("justify-content", "center").Style("color", "#ffd34d").Text("ELEVATOR"));

        plan.Content(floor);
        plan.Content(WorkshopGui.Element(r, "div").Style("position", "absolute").Style("left", "8%").Style("bottom", "7%").Style("color", "#8ba8b2").Style("font-size", ".5rem").Style("letter-spacing", ".12em").Text("ARRIVAL COMPLETE // ONLY PATH: ELEVATOR DESCENT"));
        return plan;
    }

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
