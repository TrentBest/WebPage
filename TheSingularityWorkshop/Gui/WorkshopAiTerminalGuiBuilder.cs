using Microsoft.AspNetCore.Components;

namespace TheSingularityWorkshop.Gui;

public static class WorkshopAiTerminalGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Magenta = "#ff38d1";

    public static ElementBuilder Build(
        object receiver,
        string displayName,
        string? avatarUrl,
        string contextText,
        bool open,
        Action toggle,
        Func<Task> copyContext,
        string responseText,
        Action<string> responseChanged,
        bool docked = false)
    {
        var root = WorkshopGui.Element(receiver, "div")
            .Style("position", docked ? "relative" : "fixed")
            .Style("right", docked ? "auto" : "1rem")
            .Style("top", docked ? "auto" : "1rem")
            .Style("z-index", docked ? "auto" : "260")
            .Style("width", docked ? "100%" : "auto")
            .Style("box-sizing", "border-box")
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Style("display", "flex")
            .Style("flex-direction", "column")
            .Style("align-items", "flex-end")
            .Style("padding", docked ? ".35rem 0 .1rem" : "0")
            .Style("border-top", docked ? "1px solid rgba(0,234,255,.16)" : "none")
            .Style("margin-top", docked ? ".15rem" : "0");

        var avatars = WorkshopGui.Element(receiver, "div")
            .Style("display", "flex")
            .Style("align-items", "center")
            .Style("gap", ".55rem");

        if (docked)
        {
            avatars.Content(WorkshopGui.Element(receiver, "span")
                .Style("margin-right", ".15rem")
                .Style("color", "#708892")
                .Style("font-size", ".36rem")
                .Style("letter-spacing", ".16em")
                .Text("SESSION LINK"));
        }

        avatars.Content(Avatar(receiver, avatarUrl, displayName, "YOUR AVATAR", Yellow));
        avatars.Content(Avatar(receiver, null, "AI", "AI TERMINAL", Cyan).OnClick(toggle));

        root.Content(avatars);

        if (!open)
            return root;

        var panel = WorkshopGui.Element(receiver, "div")
            .Style("width", "min(430px,82vw)")
            .Style("margin-top", ".45rem")
            .Style("padding", ".75rem")
            .Style("border", $"1px solid {Cyan}88")
            .Style("background", "rgba(1,5,11,.97)")
            .Style("box-shadow", $"0 0 35px {Cyan}12")
            .Style("color", White);

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Cyan).Style("font-size", ".48rem").Style("letter-spacing", ".16em")
            .Text("AI TERMINAL // HUB CONTROL"));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".35rem").Style("color", "#9db0b9").Style("font-size", ".4rem").Style("line-height", "1.45")
            .Text("This terminal belongs to the Workshop Hub. Configure the connected LLM here; the current spatial context is always available for copying."));

        panel.Content(WorkshopGui.Button(receiver).Label("COPY CURRENT AI CONTEXT")
            .Style("width", "100%").Style("margin-top", ".55rem").Style("padding", ".5rem")
            .Style("border", $"1px solid {Yellow}77").Style("background", $"{Yellow}10")
            .Style("color", Yellow).Style("font-family", "inherit").Style("font-size", ".43rem")
            .Style("cursor", "pointer").OnClick(copyContext));

        panel.Content(WorkshopGui.Element(receiver, "textarea")
            .Attribute("value", responseText)
            .Attribute("placeholder", "Paste the LLM response here...")
            .Attribute("rows", "7")
            .Style("width", "100%").Style("margin-top", ".55rem").Style("box-sizing", "border-box")
            .Style("resize", "vertical").Style("border", $"1px solid {Magenta}55")
            .Style("background", "rgba(255,255,255,.025)").Style("color", White)
            .Style("font-family", "inherit").Style("font-size", ".42rem").Style("padding", ".45rem")
            .Attribute("oninput", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.ChangeEventArgs>(
                receiver,
                (Microsoft.AspNetCore.Components.ChangeEventArgs args) => responseChanged(args.Value?.ToString() ?? string.Empty))));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".45rem").Style("padding", ".4rem")
            .Style("border-left", $"2px solid {Cyan}")
            .Style("color", "#9db0b9").Style("font-size", ".37rem").Style("line-height", "1.45")
            .Text("PASTE ONLY THE RESPONSE. When executable behavior is returned, the Workshop will accept Protocol integer IDs rather than string command names."));

        root.Content(panel);
        return root;
    }

    private static ElementBuilder Avatar(object receiver, string? imageUrl, string label, string title, string accent)
    {
        var avatar = WorkshopGui.Element(receiver, "div")
            .Style("width", "38px").Style("height", "38px")
            .Style("border", $"1px solid {accent}aa")
            .Style("background", "rgba(2,9,18,.94)")
            .Style("border-radius", "50%")
            .Style("box-shadow", $"0 0 18px {accent}22")
            .Style("cursor", title == "AI TERMINAL" ? "pointer" : "default")
            .Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
            .Title(title);

        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            avatar.Content(WorkshopGui.Image(receiver)
                .Attribute("src", imageUrl)
                .Attribute("alt", label)
                .Style("width", "100%").Style("height", "100%").Style("object-fit", "cover").Style("border-radius", "50%"));
        }
        else
        {
            avatar.Content(WorkshopGui.Element(receiver, "span")
                .Style("font-size", ".46rem").Style("font-weight", "800")
                .Style("color", accent).Text(label == "AI" ? "AI" : "YOU"));
        }

        return avatar;
    }
}
