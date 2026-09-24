using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class MonikerExperienceTests
{
    [Fact(DisplayName = "Moniker experience does not expose a Unity runtime handoff")]
    public void MonikerExperienceDoesNotExposeUnityRuntimeHandoff()
    {
        using var service = new WorkshopExperienceService();

        service.Initialize();
        for (var i = 0; i < 200 && service.FirstContact.CurrentState != "Gateway"; i++)
            service.Tick();

        Assert.Equal("Gateway", service.FirstContact.CurrentState);
        service.RequestEntry();
        service.Tick();

        Assert.Equal("Moniker", service.FirstContact.CurrentState);
        Assert.False(service.ShowUnity);
    }

    [Fact(DisplayName = "Moniker presentation is a three-second living gateway glyph")]
    public void MonikerPresentationUsesWorkshopPaletteAndPhasedGlyphContract()
    {
        var home = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Components", "FirstContactView.razor"));

        Assert.DoesNotContain(".moniker-foreground", home);
        Assert.Contains("THE", home);
        Assert.Contains("SINGULARITY", home);
        Assert.Contains("WORKSHOP", home);
        Assert.Contains("Moniker", home);
        Assert.DoesNotContain("MADE WITH UNITY", home);
        Assert.DoesNotContain("interop.initUnity", home);
    }

    [Fact(DisplayName = "Landing keeps the moniker as the sole primary-panel occupant")]
    public void LandingKeepsMonikerAsPrimaryPanel()
    {
        var view = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Components", "FirstContactView.razor"));

        Assert.Contains("State == \"Moniker\" || State == \"HubGrowth\" || State == \"Landing\"", view);
        Assert.Contains("State == \"Landing\" ? \"hub-moniker-panel\" : \"\"", view);
        Assert.Contains("@if (State == \"HubGrowth\")", view);
    }

    [Fact(DisplayName = "Landing leaves the moniker in place until the user navigates away")]
    public void LandingDoesNotAutoDismissMoniker()
    {
        using var service = new WorkshopExperienceService();
        service.Initialize();

        for (var i = 0; i < 200 && service.FirstContact.CurrentState != "Gateway"; i++)
            service.Tick();

        Assert.Equal("Gateway", service.FirstContact.CurrentState);

        service.RequestEntry();

        for (var i = 0; i < 200 && service.FirstContact.CurrentState != "Landing"; i++)
            service.Tick();

        Assert.Equal("Landing", service.FirstContact.CurrentState);
        Assert.Equal("Intro", service.CurrentState);

        for (var i = 0; i < 120; i++)
            service.Tick();

        Assert.Equal("Landing", service.FirstContact.CurrentState);
        Assert.Equal("Intro", service.CurrentState);
    }

    [Fact(DisplayName = "Gateway presentation uses the active first-contact gateway contract")]
    public void GatewayPresentationRemainsStable()
    {
        var gateway = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Components", "FirstContactView.razor"));

        Assert.Contains("enter-workshop", gateway);
        Assert.Contains("ENTER THE WORKSHOP", gateway);
        Assert.Contains("SYSTEM ADVISORY: MAXIMUM OVERDRIVE ACTIVE", gateway);
        Assert.Contains("WorkshopGui.Button(this)", gateway);
    }

    [Fact(DisplayName = "Gateway avatar fills the inscribed circle and pressure scales on hover")]
    public void GatewayAvatarUsesInscribedCircleAndPressureHover()
    {
        var gateway = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Components", "FirstContactView.razor"));

        Assert.Contains("WorkshopGui.Button(this)", gateway);
        Assert.Contains("WorkshopGui.Image(this)", gateway);
        Assert.Contains(".Style(\"left\", \"50%\")", gateway);
        Assert.Contains(".Style(\"top\", \"50%\")", gateway);
        Assert.Contains(".Style(\"width\", \"auto\")", gateway);
        Assert.Contains(".Style(\"height\", \"72%\")", gateway);
        Assert.Contains(".Style(\"aspect-ratio\", \"1\")", gateway);
        Assert.Contains(".Style(\"transform\", $\"translate(-50%,-50%) scale(", gateway);
        Assert.Contains("_gatewayHovered ? 1.55d : 1d", gateway);
        Assert.Contains("GatewayBreath", gateway);
        Assert.Contains(".Style(\"object-fit\", \"cover\")", gateway);
        Assert.Contains(".Style(\"object-position\", \"center\")", gateway);
        Assert.Contains(".OnMouseEnter(() => _gatewayHovered = true)", gateway);
        Assert.Contains(".OnMouseLeave(() => _gatewayHovered = false)", gateway);
        Assert.DoesNotContain("<button class=\"enter-workshop\"", gateway);

        var home = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Pages", "Home.razor"));

        Assert.DoesNotContain(".enter-workshop:hover img", home);
        Assert.DoesNotContain(".enter-workshop img", home);

    }

    [Fact(DisplayName = "Gateway breathing is driven by FSM ticks")]
    public void GatewayBreathingIsFsmDriven()
    {
        using var service = new WorkshopExperienceService();
        service.Initialize();

        for (var i = 0; i < 90; i++)
            service.Tick();

        Assert.Equal("Gateway", service.FirstContact.CurrentState);
        var first = service.FirstContact.GatewayBreath;

        for (var i = 0; i < 12; i++)
            service.Tick();

        Assert.Equal("Gateway", service.FirstContact.CurrentState);
        Assert.NotEqual(first, service.FirstContact.GatewayBreath);
        Assert.InRange(service.FirstContact.GatewayBreath, 0d, 1d);
    }

    [Fact(DisplayName = "First-contact presentation is driven by explicit FSM phases")]
    public void FirstContactPresentationUsesExplicitPhasesWithoutAFlash()
    {
        using var service = new WorkshopExperienceService();

        service.Initialize();

        Assert.Equal("FirstOnly", service.FirstContact.CurrentState);
        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(0d, service.FirstContact.QuestionOpacity, 6);

        // FirstOnly owns the entire fade-in. The transition is evaluated
        // immediately after the tick that reaches full visibility.
        for (var i = 0; i < 30; i++)
            service.Tick();

        Assert.Equal("FirstFadingSecondComingIn", service.FirstContact.CurrentState);
        Assert.Equal(1d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(0d, service.FirstContact.QuestionOpacity, 6);

        // FSM_API defers OnEnter for the newly selected state until the next
        // tick. Therefore the handoff frame remains exactly 1 / 0.
        service.Tick();

        Assert.Equal("FirstFadingSecondComingIn", service.FirstContact.CurrentState);
        Assert.True(service.FirstContact.StatementOpacity < 1d);
        Assert.True(service.FirstContact.QuestionOpacity > 0d);
        Assert.Equal(
            1d,
            service.FirstContact.StatementOpacity + service.FirstContact.QuestionOpacity,
            6);

        // Complete the crossfade. The transition to SecondOnly happens on
        // the same tick that the incoming label reaches exactly 1.
        for (var i = 0; i < 29; i++)
            service.Tick();

        Assert.Equal("SecondOnly", service.FirstContact.CurrentState);
        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(1d, service.FirstContact.QuestionOpacity, 6);

        // SecondOnly owns the final fade-out. There is no return to either
        // of the preceding presentation phases.
        service.Tick();

        Assert.Equal("SecondOnly", service.FirstContact.CurrentState);
        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.InRange(service.FirstContact.QuestionOpacity, 0d, 1d);
        Assert.True(service.FirstContact.QuestionOpacity < 1d);

        for (var i = 0; i < 29; i++)
            service.Tick();

        Assert.Equal("Gateway", service.FirstContact.CurrentState);
        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(0d, service.FirstContact.QuestionOpacity, 6);
    }

    [Fact(DisplayName = "Moniker scene contains only the colorful glyph field")]
    public void MonikerSceneContainsOnlyColorfulGlyphField()
    {
        var view = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Components", "FirstContactView.razor"));

        Assert.Contains("<section class=\"flex-hello", view);
        Assert.Contains("hub-moniker-panel", view);
        Assert.Contains("hello-moniker", view);
        Assert.Contains("hello-glyph", view);
        Assert.DoesNotContain("moniker-water", view);
        Assert.DoesNotContain("flex-hello-orbit", view);
        Assert.DoesNotContain("moniker-foreground", view);
        Assert.DoesNotContain("helloOrbit", view);
    }

    [Fact(DisplayName = "Moniker glyphs preserve their authored row origin during animation delay")]
    public void MonikerGlyphsPreserveAuthoredRowOrigin()
    {
        var view = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Components", "FirstContactView.razor"));

        var layout = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Layout", "MainLayout.razor.css"));

        Assert.Contains("RenderMonikerLine(\"THE\", 0, 1d)", view);
        Assert.Contains("RenderMonikerLine(\"SINGULARITY\", 3, 5d)", view);
        Assert.Contains("RenderMonikerLine(\"WORKSHOP\", 14, 3.5d)", view);
        Assert.Contains("--glyph-index:{glyphIndex}", view);
        Assert.Contains("--glyph-midpoint:{midpoint:0.##}", view);

        // Positive animation delays must retain the 0% transform. Otherwise the
        // delayed glyph has no transform and sits at its absolute left/top point
        // (the row center) until the animation starts.
        Assert.Contains("transform: translate(-50%, -50%)", layout);
        Assert.Contains("animation-fill-mode: backwards !important", layout);
        Assert.DoesNotContain(".hello-glyph:nth-child", layout);
    }

    [Fact(DisplayName = "Gateway does not render a duplicate first-contact question")]
    public void GatewayDoesNotReintroduceFirstContactQuestion()
    {
        var view = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Components", "FirstContactView.razor"));

        Assert.DoesNotContain("gateway-memory", view);

        const string phrase = "HOW MANY WORDS IS A LIVING IMAGE WORTH?";
        Assert.Equal(1, CountOccurrences(view, phrase));
    }

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        var offset = 0;

        while ((offset = text.IndexOf(value, offset, StringComparison.Ordinal)) >= 0)
        {
            count++;
            offset += value.Length;
        }

        return count;
    }
}
