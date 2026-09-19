using System;
using Microsoft.AspNetCore.Components;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds the semantic opening gateway for the Workshop.
/// The gateway is intentionally expressed as recursive GUI vocabulary rather than
/// hand-authored Razor markup. CSS remains a manifestation layer for atmosphere,
/// motion, and responsive presentation.
/// </summary>
public static class WorkshopGatewayGuiBuilder
{
    /// <summary>
    /// Builds the first-visit gateway: identity, invitation, and a concise
    /// explanation of what the visitor is about to enter.
    /// </summary>
    public static RenderFragment Build(
        object receiver,
        Func<Task> enterWorkshop,
        string warningHeader,
        string warningBody,
        string warningFinal)
    {
        ArgumentNullException.ThrowIfNull(receiver);
        ArgumentNullException.ThrowIfNull(enterWorkshop);

        var stage = WorkshopGui.Element(receiver, "section")
            .Class("monolith-stage")
            .Attribute("aria-label", "The Singularity Workshop opening");

        stage.Child(WorkshopGui.Element(receiver, "div").Class("ambient-grid").Attribute("aria-hidden", "true"));
        stage.Child(WorkshopGui.Element(receiver, "div").Class("ambient-scan").Attribute("aria-hidden", "true"));
        stage.Child(WorkshopGui.Element(receiver, "div").Class("ambient-orbit ambient-orbit-one").Attribute("aria-hidden", "true"));
        stage.Child(WorkshopGui.Element(receiver, "div").Class("ambient-orbit ambient-orbit-two").Attribute("aria-hidden", "true"));
        stage.Child(WorkshopGui.Element(receiver, "div").Class("ambient-signal signal-one").Attribute("aria-hidden", "true"));
        stage.Child(WorkshopGui.Element(receiver, "div").Class("ambient-signal signal-two").Attribute("aria-hidden", "true"));
        stage.Child(WorkshopGui.Element(receiver, "div").Class("ambient-signal signal-three").Attribute("aria-hidden", "true"));

        var gateway = WorkshopGui.Button(receiver)
            .Class("monolith-btn")
            .AriaLabel("Enter The Singularity Workshop")
            .OnClick(enterWorkshop);

        gateway.Content(WorkshopGui.Element(receiver, "span").Class("hero-corner hero-corner-tl").Attribute("aria-hidden", "true"));
        gateway.Content(WorkshopGui.Element(receiver, "span").Class("hero-corner hero-corner-tr").Attribute("aria-hidden", "true"));
        gateway.Content(WorkshopGui.Element(receiver, "span").Class("hero-corner hero-corner-bl").Attribute("aria-hidden", "true"));
        gateway.Content(WorkshopGui.Element(receiver, "span").Class("hero-corner hero-corner-br").Attribute("aria-hidden", "true"));
        gateway.Content(WorkshopGui.Element(receiver, "span").Class("hero-energy").Attribute("aria-hidden", "true"));
        gateway.Content(
            WorkshopGui.Element(receiver, "span")
                .Class("avatar-portal")
                .Attribute("aria-hidden", "true")
                .Child(WorkshopGui.Image(receiver)
                    .Class("monolith-image")
                    .Attribute("src", "/Images/TheSingularityWorkshopLogo.png")
                    .Attribute("alt", "The Singularity Workshop")));
        gateway.Content(WorkshopGui.Element(receiver, "span").Class("monolith-text").Text("ENTER THE WORKSHOP"));
        stage.Content(gateway);

        var warning = WorkshopGui.Panel(receiver)
            .Class("workshop-warning")
            .Attribute("role", "note")
            .Attribute("aria-label", "What to expect inside the Workshop");
        warning.Content(
            WorkshopGui.Element(receiver, "div")
                .Class("warning-header")
                .Content(WorkshopGui.Element(receiver, "span").Class("warning-mark").Text("⚠"))
                .Content(WorkshopGui.Element(receiver, "span").Text(warningHeader))
                .Content(WorkshopGui.Element(receiver, "span").Class("warning-mark").Text("⚠")));
        warning.Content(
            WorkshopGui.Element(receiver, "div")
                .Class("warning-body")
                .Text(warningBody)
                .Content(WorkshopGui.Element(receiver, "span").Class("warning-final").Text(warningFinal)));
        stage.Content(warning);

        return stage.Build();
    }
}
