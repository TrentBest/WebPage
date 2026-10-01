using Microsoft.AspNetCore.Components;
using TheSingularityWorkshop.Workshop.MicroBundles.AI;

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
        Func<Task> processClipboard,
        IReadOnlyList<WorkshopAiLedgerEntry> ledger,
        string statusText,
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
        avatars.Content(Avatar(receiver, null, "AI", "AI TERMINAL", Cyan, breathing: true).OnClick(toggle));

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

        panel.Content(WorkshopGui.Button(receiver).Label("PUT CONTEXT ON CLIPBOARD")
            .Style("width", "100%").Style("margin-top", ".55rem").Style("padding", ".5rem")
            .Style("border", $"1px solid {Yellow}77").Style("background", $"{Yellow}10")
            .Style("color", Yellow).Style("font-family", "inherit").Style("font-size", ".43rem")
            .Style("cursor", "pointer").OnClick(copyContext));

        panel.Content(WorkshopGui.Button(receiver).Label("TAKE AI DATA FROM CLIPBOARD")
            .Style("width", "100%").Style("margin-top", ".35rem").Style("padding", ".5rem")
            .Style("border", $"1px solid {Magenta}77").Style("background", $"{Magenta}10")
            .Style("color", Magenta).Style("font-family", "inherit").Style("font-size", ".43rem")
            .Style("cursor", "pointer").OnClick(processClipboard));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".45rem").Style("padding", ".4rem")
            .Style("border-left", $"2px solid {Cyan}")
            .Style("color", "#9db0b9").Style("font-size", ".37rem").Style("line-height", "1.45")
            .Text("The clipboard is the exchange boundary. ProtocolAi validates integer vocabulary; GrammarAi validates legal composition. Safe operations execute immediately; approval-dependent operations become ledger entries."));

        if (ledger.Count > 0)
        {
            var ledgerPanel = WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".55rem")
                .Style("padding", ".45rem")
                .Style("border", $"1px solid {Yellow}33")
                .Style("background", "rgba(255,211,77,.025)");

            ledgerPanel.Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Yellow).Style("font-size", ".38rem")
                .Style("letter-spacing", ".14em")
                .Text("AI ACTION LEDGER"));

            foreach (var entry in ledger.Reverse().Take(8))
            {
                ledgerPanel.Content(WorkshopGui.Element(receiver, "div")
                    .Style("display", "grid")
                    .Style("grid-template-columns", "auto auto minmax(0,1fr)")
                    .Style("gap", ".35rem")
                    .Style("margin-top", ".28rem")
                    .Style("font-size", ".34rem")
                    .Style("line-height", "1.35")
                    .Content(WorkshopGui.Element(receiver, "span").Style("color", "#708892").Text($"#{entry.Sequence:00}"))
                    .Content(WorkshopGui.Element(receiver, "span").Style("color", entry.Status == WorkshopAiLedgerStatus.AutoExecuted ? Cyan : entry.Status == WorkshopAiLedgerStatus.AwaitingHumanApproval ? Yellow : Magenta).Text(entry.Status.ToString().ToUpperInvariant()))
                    .Content(WorkshopGui.Element(receiver, "span").Style("color", "#c9d7dc").Text($"{entry.OperationId} // {entry.Description}")));
            }

            panel.Content(ledgerPanel);
        }

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".45rem")
            .Style("color", "#8fa7b2")
            .Style("font-size", ".35rem")
            .Style("line-height", "1.4")
            .Text(statusText));

        root.Content(panel);
        return root;
    }

    private static ElementBuilder Avatar(object receiver, string? imageUrl, string label, string title, string accent, bool breathing = false)
    {
        var avatar = WorkshopGui.Element(receiver, "div")
            .Style("width", "38px").Style("height", "38px")
            .Style("border", $"1px solid {accent}aa")
            .Style("background", "rgba(2,9,18,.94)")
            .Style("border-radius", "50%")
            .Style("box-shadow", $"0 0 18px {accent}22")
            .Style("cursor", title == "AI TERMINAL" ? "pointer" : "default")
            .Class(breathing ? "workshop-ai-breathing" : string.Empty)
            .Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
            .Title(title);

        if (breathing)
        {
            avatar.Content(WorkshopGui.Element(receiver, "style").Text(
                "@keyframes workshop-ai-breathe{0%,100%{filter:brightness(1);transform:scale(1)}50%{filter:brightness(1.22);transform:scale(1.06)}}.workshop-ai-breathing{animation:workshop-ai-breathe 2.8s ease-in-out infinite;transform-origin:center center}@media(prefers-reduced-motion:reduce){.workshop-ai-breathing{animation:none;filter:brightness(1.1)}}"));
        }

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
