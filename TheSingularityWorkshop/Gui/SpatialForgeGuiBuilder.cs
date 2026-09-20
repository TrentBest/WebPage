using System.Runtime.CompilerServices;

namespace TheSingularityWorkshop.Gui;

/// <summary>Builds the Forge as a diegetic FSM fabrication floor.</summary>
public static class SpatialForgeGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff38d1";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private static readonly ConditionalWeakTable<object, WelcomeState> WelcomeStates = new();
    private static readonly ConditionalWeakTable<object, ForgeAssemblyState> AssemblyStates = new();

    /// <summary>Builds the Forge interior, centered on the FSM creation workbench.</summary>
    public static ElementBuilder Build(object receiver, string avatarName, double avatarX, double avatarY, Action<Microsoft.AspNetCore.Components.Web.KeyboardEventArgs> onKeyDown, Action<double, double> moveAvatarTo, Action exitForge)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden").Style("background", "#020812").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Attribute("tabindex", "0")
            .AriaLabel("FSM Forge floor plan. Click to walk. Use WASD or arrow keys.")
            .OnKeyDown(onKeyDown)
            .PreventDefault("onkeydown");

        root.Content(WorkshopGui.Element(receiver, "style").Text("@keyframes forge-breathe{0%,100%{filter:brightness(1)}50%{filter:brightness(1.35)}}@keyframes forge-unlock-pulse{0%,100%{box-shadow:0 0 10px rgba(0,234,255,.12)}50%{box-shadow:0 0 24px rgba(0,234,255,.42),0 0 42px rgba(255,56,209,.12);transform:scale(1.025)}}@keyframes ingot-pulse{0%,100%{opacity:.65}50%{opacity:1}}"));

        var floor = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("inset", "0")
            .Style("background", "radial-gradient(circle at 50% 48%,#0a1c2a 0%,#030911 48%,#010308 100%)")
            .Style("overflow", "visible")
            .Style("transform", CameraTransform(avatarX, avatarY))
            .Style("transition", "transform .28s linear")
            .Style("will-change", "transform");

        floor.Content(Grid(receiver));
        // Readable RPG landmark: stone/iron shell, furnace, anvil, racks, and a work yard.
        // The FSM assembly remains the semantic purpose of the building inside that silhouette.
        floor.Content(ForgeYard(receiver));
        floor.Content(Ingots(receiver));
        floor.Content(StateShelf(receiver));
        floor.Content(Workbench(receiver));
        floor.Content(TransitionBay(receiver));

        root.Content(floor);
        root.Content(WalkSurface(receiver, avatarX, avatarY, moveAvatarTo));
        root.Content(Avatar(receiver, avatarName));
        root.Content(Title(receiver));
        root.Content(WorkshopGui.Button(receiver).Label("← EXIT FORGE")
            .Style("position", "fixed").Style("left", "50%").Style("bottom", "1rem").Style("transform", "translateX(-50%)").Style("z-index", "30")
            .Style("padding", ".65rem .9rem").Style("border", $"2px solid {Magenta}aa").Style("background", "rgba(20,2,16,.94)").Style("box-shadow", $"0 0 18px {Magenta}22, inset 0 0 12px {Magenta}10").Style("text-shadow", $"0 0 8px {Magenta}88")
            .Style("color", Magenta).Style("font-family", "inherit").Style("font-size", ".48rem")
            .Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(exitForge));

        var welcomeState = WelcomeStates.GetOrCreateValue(receiver);
        if (!welcomeState.Dismissed)
            root.Content(Welcome(receiver, () => { welcomeState.Dismissed = true; }));

        return root;
    }

    /// <summary>Temporary first-visit orientation explaining the Forge abstraction.</summary>
    private static ElementBuilder Welcome(object receiver, Action dismiss)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "50%").Style("top", "50%").Style("transform", "translate(-50%,-50%)")
            .Style("z-index", "100").Style("width", "min(620px,82vw)").Style("padding", "1.4rem")
            .Style("box-sizing", "border-box").Style("border", $"1px solid {Cyan}aa")
            .Style("background", "rgba(2,8,18,.96)").Style("box-shadow", $"0 0 60px {Cyan}22,inset 0 0 30px {Cyan}08")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".7rem").Style("letter-spacing", ".28em").Text("WELCOME TO THE FSM FORGE"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", "1rem").Style("color", White).Style("font-family", "system-ui,sans-serif").Style("font-size", ".9rem").Text("A blacksmith-like fabrication floor for assembling the structure of a finite state machine."))
            .Content(WorkshopGui.Element(receiver, "ol").Style("margin", "1rem 0").Style("padding-left", "1.3rem").Style("color", "#b8c9d0").Style("font-family", "system-ui,sans-serif").Style("font-size", ".7rem").Style("line-height", "1.7")
                .Content(WorkshopGui.Element(receiver, "li").Text("Walk the forge floor and read the building as a place, not a menu."))
                .Content(WorkshopGui.Element(receiver, "li").Text("States are raw material; transitions are the joins between pieces."))
                .Content(WorkshopGui.Element(receiver, "li").Text("The Forge shows structure; behavior is scaffolded elsewhere.")))
            .Content(WorkshopGui.Button(receiver).Label("UNDERSTOOD — ENTER THE FORGE").Attribute("data-forge-unlock", "true")
                .Style("padding", ".65rem 1rem").Style("animation", "forge-unlock-pulse 1.8s ease-in-out infinite").Style("border", $"1px solid {Cyan}88").Style("background", $"{Cyan}10")
                .Style("color", Cyan).Style("font-family", "inherit").Style("font-size", ".5rem").Style("letter-spacing", ".14em").Style("cursor", "pointer").OnClick(dismiss));

    private sealed class WelcomeState { public bool Dismissed { get; set; } }

    private sealed class ForgeAssemblyState
    {
        public int StateCount { get; set; }
    }

    private static string CameraTransform(double avatarX, double avatarY)
        => $"translate(calc(50vw - {avatarX:0.##}vw), calc(50vh - {avatarY:0.##}vh))";

    private static ElementBuilder WalkSurface(object receiver, double avatarX, double avatarY, Action<double, double> moveAvatarTo)
    {
        const int cells = 24;
        var surface = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0").Style("z-index", "5")
            .Style("display", "grid")
            .Style("grid-template-columns", $"repeat({cells},1fr)")
            .Style("grid-template-rows", $"repeat({cells},1fr)")
            .Style("pointer-events", "none");

        for (var row = 0; row < cells; row++)
        for (var column = 0; column < cells; column++)
        {
            var screenX = (column + .5) / cells * 100;
            var screenY = (row + .5) / cells * 100;
            var worldX = avatarX + screenX - 50;
            var worldY = avatarY + screenY - 50;

            surface.Content(WorkshopGui.Button(receiver)
                .Style("display", "block").Style("width", "100%").Style("height", "100%")
                .Style("min-width", "0").Style("min-height", "0").Style("margin", "0").Style("padding", "0")
                .Style("border", "0").Style("outline", "none").Style("background", "transparent")
                .Style("pointer-events", "auto").Style("cursor", "crosshair")
                .AriaLabel($"Walk to {worldX:0.#}, {worldY:0.#}")
                .OnClick(() => moveAvatarTo(worldX, worldY)));
        }

        return surface;
    }

    private static ElementBuilder Workbench(object receiver)
    {
        var assembly = AssemblyStates.GetOrCreateValue(receiver);
        var panel = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "5%").Style("top", "30%").Style("width", "140vw").Style("height", "43vh")
            .Style("box-sizing", "border-box").Style("z-index", "25").Style("padding", ".8rem")
            .Style("border", $"1px solid {Magenta}88")
            .Style("background", "rgba(3,7,13,.72)")
            .Style("box-shadow", $"inset 0 0 35px {Magenta}08");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("color", Magenta).Style("font-size", ".6rem").Style("letter-spacing", ".2em").Text("FSM ASSEMBLY PLAN"));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("color", "#7895a1").Style("font-size", ".4rem").Text($"{assembly.StateCount:00} STATES // STRUCTURE ONLY"));

        var states = WorkshopGui.MultiPanel(receiver).Style("margin-top", ".8rem").Style("display", "flex").Style("flex-wrap", "wrap").Style("align-content", "flex-start").Style("gap", "1rem").Style("overflow", "visible").Style("height", "calc(100% - 2rem)");
        if (assembly.StateCount == 0)
            states.Content(WorkshopGui.Element(receiver, "div").Style("min-width", "12rem").Style("min-height", "5rem").Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
                .Style("border", $"1px dashed {Cyan}55").Style("background", $"{Cyan}06").Style("color", "#58747e").Style("font-size", ".42rem").Style("letter-spacing", ".08em").Text("ADD STATES FROM THE MATERIAL SHELF"));
        else
            for (var index = 0; index < assembly.StateCount; index++) states.Content(StateIngot(receiver, index + 1));

        panel.Content(states);
        return panel;
    }

    private static ElementBuilder Avatar(object receiver, string name)
        => WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 40 40").Attribute("aria-label", "Your position")
            .Style("position", "fixed").Style("left", "50%").Style("top", "50%").Style("width", "34px").Style("height", "34px")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "45").Style("pointer-events", "none").Style("overflow", "visible")
            .Content(WorkshopGui.Element(receiver, "circle").Attribute("cx", "20").Attribute("cy", "20").Attribute("r", "8")
                .Attribute("fill", "none").Attribute("stroke", White).Attribute("stroke-opacity", ".4")
                .Child(WorkshopGui.Element(receiver, "animate").Attribute("attributeName", "r").Attribute("values", "6;13;6").Attribute("dur", "1.8s").Attribute("repeatCount", "indefinite")))
            .Content(WorkshopGui.Element(receiver, "circle").Attribute("cx", "20").Attribute("cy", "20").Attribute("r", "4").Attribute("fill", White))
            .Content(WorkshopGui.Element(receiver, "text").Attribute("x", "20").Attribute("y", "36").Attribute("text-anchor", "middle").Attribute("fill", White).Attribute("font-size", "4").Text(name));

    private static ElementBuilder StateIngot(object receiver, int number)
        => WorkshopGui.Element(receiver, "div")
            .Attribute("draggable", "true")
            .Attribute("data-state-id", $"STATE_{number}")
            .Attribute("ondragstart", "event.dataTransfer.setData('text/plain',this.dataset.stateId)")
            .Style("position", "relative").Style("width", "12rem").Style("height", "6rem").Style("padding", ".65rem")
            .Style("box-sizing", "border-box")
            .Style("border", $"1px solid {Cyan}88").Style("background", "rgba(0,20,30,.88)")
            .Style("box-shadow", $"inset 0 0 18px {Cyan}08,0 0 14px {Cyan}10").Style("cursor", "grab")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", White).Style("font-size", ".48rem").Style("letter-spacing", ".12em").Text($"STATE_{number}"))
            .Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", ".65rem").Style("right", ".65rem").Style("bottom", ".65rem")
                .Style("height", "1px").Style("background", $"{Cyan}44").Text(""));

    private static ElementBuilder StateShelf(object receiver)
    {
        var assembly = AssemblyStates.GetOrCreateValue(receiver);
        return WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "5%").Style("top", "9%").Style("width", "140vw").Style("height", "12%")
            .Style("z-index", "24").Style("box-sizing", "border-box").Style("padding", ".65rem")
            .Style("border", $"1px solid {Cyan}66").Style("background", "rgba(2,8,18,.92)")
            .Style("box-shadow", $"0 0 24px {Cyan}12,inset 0 0 18px {Cyan}08")
            .Content(WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("align-items", "center").Style("justify-content", "space-between")
                .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".55rem").Style("letter-spacing", ".18em").Text("STATE SHELF"))
                .Content(WorkshopGui.Element(receiver, "div").Style("color", "#7895a1").Style("font-size", ".38rem").Text($"{assembly.StateCount:00} ON WORKBENCH")))
            .Content(WorkshopGui.Button(receiver).Label("STATE +")
                .Style("position", "absolute").Style("right", ".65rem").Style("bottom", ".65rem").Style("padding", ".35rem .65rem")
                .Style("border", $"1px solid {Cyan}66").Style("background", $"{Cyan}0c").Style("color", Cyan)
                .Style("font-family", "inherit").Style("font-size", ".4rem").Style("letter-spacing", ".1em").Style("cursor", "pointer")
                .OnClick(() => assembly.StateCount++));
    }

    private static ElementBuilder TransitionBay(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "5%").Style("top", "78%").Style("width", "140vw").Style("height", "14%")
            .Style("z-index", "24").Style("box-sizing", "border-box").Style("padding", ".7rem")
            .Style("border", $"1px solid {Yellow}77").Style("background", "rgba(18,14,2,.94)")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", ".55rem").Style("letter-spacing", ".18em").Text("TRANSITIONS // FROM → TO"))
            .Content(WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("align-items", "center").Style("justify-content", "center").Style("gap", ".65rem").Style("height", "65%")
                .Content(TransitionSlot(receiver, "FROM STATE")).Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", ".7rem").Text("→")).Content(TransitionSlot(receiver, "TO STATE")));

    private static ElementBuilder TransitionSlot(object receiver, string label)
        => WorkshopGui.Element(receiver, "div")
            .Attribute("ondragover", "event.preventDefault()")
            .Attribute("ondrop", "event.preventDefault();this.textContent=event.dataTransfer.getData('text/plain')")
            .Style("width", "32%").Style("height", "2.2rem").Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
            .Style("border", $"1px dashed {Yellow}88").Style("background", $"{Yellow}08").Style("color", Yellow).Style("font-size", ".4rem").Style("letter-spacing", ".08em").Text(label);

    private static ElementBuilder ForgeYard(object receiver)
    {
        var yard = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "2%").Style("top", "1%").Style("width", "170vw").Style("height", "150vh")
            .Style("z-index", "3").Style("pointer-events", "none");

        yard.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "3%").Style("top", "4%").Style("width", "150vw").Style("height", "142vh")
            .Style("border", "2px solid rgba(112,88,66,.75)")
            .Style("box-shadow", "inset 0 0 70px rgba(255,104,32,.06),0 0 30px rgba(0,0,0,.5)")
            .Style("background", "linear-gradient(135deg,rgba(42,28,20,.18),rgba(5,8,12,.04) 45%,rgba(255,104,32,.04))"));

        yard.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "9%").Style("top", "9%").Style("width", "13rem").Style("height", "10rem")
            .Style("border", "2px solid #8d6045")
            .Style("background", "radial-gradient(circle at 50% 68%,#ff8a3d 0,#9b3515 18%,#1a0e09 44%,#07090b 72%)")
            .Style("box-shadow", "0 0 28px rgba(255,86,24,.24),inset 0 0 28px rgba(255,138,61,.12)")
            .Content(WorkshopGui.Element(receiver, "div").Style("position","absolute").Style("left","38%").Style("top","-3.8rem").Style("width","24%").Style("height","4rem").Style("border","2px solid #6f5142").Style("background","#090b0d"))
            .Content(WorkshopGui.Element(receiver, "div").Style("position","absolute").Style("left","18%").Style("right","18%").Style("bottom","12%").Style("height","18%").Style("border","1px solid #ff8a3d99").Style("background","#ff8a3d22")));

        yard.Content(ForgeProp(receiver, 28, 23, "ANVIL", "#c7d0d5"));
        yard.Content(ForgeProp(receiver, 38, 23, "TROUGH", "#00eaff"));
        yard.Content(ForgeProp(receiver, 54, 10, "IRON RACK", "#ffd34d"));
        yard.Content(ForgeProp(receiver, 68, 18, "COAL", "#ff8a3d"));
        yard.Content(ForgeProp(receiver, 78, 8, "TOOLS", "#c7d0d5"));

        yard.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "25%").Style("top", "28%").Style("width", "105vw").Style("height", "42vh")
            .Style("border", "1px dashed rgba(255,138,61,.24)")
            .Style("background", "radial-gradient(ellipse at center,rgba(255,138,61,.04),transparent 68%)"));

        return yard;
    }

    private static ElementBuilder ForgeProp(object receiver, double left, double top, string label, string accent)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", left + "%").Style("top", top + "%").Style("width", "8rem").Style("height", "4rem")
            .Style("border", $"1px solid {accent}66").Style("background", "rgba(8,10,13,.82)")
            .Style("box-shadow", $"inset 0 0 14px {accent}08").Style("display", "flex")
            .Style("align-items", "center").Style("justify-content", "center")
            .Style("color", accent).Style("font-size", ".38rem").Style("letter-spacing", ".12em")
            .Text(label);

    private static ElementBuilder Ingots(object receiver)
    {
        var rack = WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "5%").Style("top", "2%").Style("width", "28rem").Style("height", "5rem").Style("z-index", "26")
            .Style("display", "flex").Style("align-items", "center").Style("gap", "2rem");

        foreach (var item in new[] { ("ENTER", Cyan), ("UPDATE", Yellow), ("EXIT", Magenta) })
        {
            var barrel = WorkshopGui.Element(receiver, "div").Attribute("title", item.Item1)
                .Style("width", "4rem").Style("height", "4rem").Style("border-radius", "50%")
                .Style("border", $"1px solid {item.Item2}88").Style("background", $"radial-gradient(circle,{item.Item2}18,#03070d 70%)")
                .Style("box-shadow", $"0 0 14px {item.Item2}12,inset 0 0 12px {item.Item2}08")
                .Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
                .Style("color", item.Item2).Style("font-size", ".4rem").Style("letter-spacing", ".12em").Style("cursor", "default").Text(item.Item1);
            rack.Content(barrel);
        }
        return rack;
    }

    private static ElementBuilder Title(object receiver)
        => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("left", "50%").Style("top", "1.2rem").Style("transform", "translateX(-50%)")
            .Style("z-index", "40").Style("text-align", "center").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".65rem").Style("letter-spacing", ".3em").Text("FSM FORGE"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".2rem").Style("color", "#7895a1").Style("font-size", ".38rem").Style("letter-spacing", ".16em").Text("BLACKSMITH FLOOR // FSM ASSEMBLY"));

    private static ElementBuilder Grid(object receiver)
        => WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "0").Style("top", "0").Style("width", "180vw").Style("height", "160vh").Style("z-index", "0").Style("background-size", "5vw 5vh")
            .Style("background-image", "linear-gradient(rgba(0,234,255,.07) 1px,transparent 1px),linear-gradient(90deg,rgba(0,234,255,.07) 1px,transparent 1px)").Style("opacity", ".7").Style("pointer-events", "none");

    private static ElementBuilder Structure(object receiver, IReadOnlyList<SpatialRenderedLine> rendered)
    {
        var svg = WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("left", "0").Style("top", "0").Style("width", "180vw").Style("height", "160vh").Style("z-index", "1").Style("pointer-events", "none");
        foreach (var line in rendered)
        {
            var points = string.Join(" ", line.Points.Select(point => $"{point.X:0.###},{point.Y:0.###}"));
            svg.Child(WorkshopGui.Element(receiver, "polyline").Attribute("points", points).Attribute("fill", "none").Attribute("stroke", line.Style.Stroke)
                .Attribute("stroke-width", line.Style.Width).Attribute("stroke-opacity", line.Style.Opacity).Attribute("stroke-linecap", "round").Attribute("stroke-linejoin", "round"));
        }
        return svg;
    }

    private static ElementBuilder Station(object receiver, double left, double top, double width, double height, string label, string accent, string detail, Action? activate = null)
    {
        var station = WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", left + "%").Style("top", top + "%").Style("width", width + "%").Style("height", height + "%").Style("z-index", "20")
            .Style("box-sizing", "border-box").Style("border", $"1px solid {accent}aa").Style("background", $"linear-gradient(180deg,{accent}22,rgba(3,7,13,.96) 78%)")
            .Style("box-shadow", $"0 0 22px {accent}22,inset 0 0 20px {accent}10").Style("animation", "forge-breathe 3.2s ease-in-out infinite").Style("cursor", activate is null ? "default" : "pointer");
        station.Content(WorkshopGui.Element(receiver, "div").Style("padding", ".6rem").Style("color", accent).Style("font-size", ".55rem").Style("letter-spacing", ".18em").Text(label));
        station.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "8%").Style("right", "8%").Style("top", "38%").Style("height", "30%").Style("border", $"1px solid {accent}44").Style("background", $"{accent}08")
            .Content(WorkshopGui.Element(receiver, "div").Style("padding", ".5rem").Style("color", "#7895a1").Style("font-family", "system-ui,sans-serif").Style("font-size", ".55rem").Text(detail)));
        if (activate is not null)
            station.Content(WorkshopGui.Button(receiver).Label("ENTER STATION →").Style("position", "absolute").Style("left", "8%").Style("right", "8%").Style("bottom", "8%").Style("padding", ".4rem")
                .Style("border", $"1px solid {accent}66").Style("background", "transparent").Style("color", accent).Style("font-family", "inherit").Style("font-size", ".42rem").Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(activate));
        return station;
    }


}
