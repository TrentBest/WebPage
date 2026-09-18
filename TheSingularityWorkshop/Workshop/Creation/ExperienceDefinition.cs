using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Creation;

/// <summary>
/// Declarative definition of an Experience: an environment composed from named
/// presentation surfaces and their associated behavior definitions.
/// </summary>
/// <remarks>
/// An Experience owns the context in which GUI definitions are presented.
/// A GUI remains a reusable recursive definition; it does not know which
/// Experience hosts it. Runtime services may later attach spatial placement,
/// resources, MicroBundles, and compiled FSM instances without changing this boundary.
/// </remarks>
public sealed class ExperienceDefinition
{
    public ExperienceDefinition(
        string name,
        IEnumerable<ExperienceSurfaceDefinition> surfaces)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Experience name is required.", nameof(name));

        Name = name;
        Surfaces = new ReadOnlyCollection<ExperienceSurfaceDefinition>(
            (surfaces ?? throw new ArgumentNullException(nameof(surfaces))).ToList());
    }

    /// <summary>Stable human-facing name of the Experience.</summary>
    public string Name { get; }

    /// <summary>Named presentation surfaces exposed by the Experience.</summary>
    public IReadOnlyList<ExperienceSurfaceDefinition> Surfaces { get; }

    /// <summary>Finds a surface by its stable name.</summary>
    public ExperienceSurfaceDefinition FindSurface(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Surface name is required.", nameof(name));

        return Surfaces.FirstOrDefault(
                   x => string.Equals(x.Name, name, StringComparison.Ordinal))
               ?? throw new InvalidOperationException(
                   $"Experience surface '{name}' was not found in '{Name}'.");
    }
}

/// <summary>
/// A named place where an Experience presents a GUI definition and optionally
/// associates an FSM definition with that presentation.
/// </summary>
/// <remarks>
/// The surface is intentionally semantic. It does not contain Blazor, WPF,
/// Unity, or other platform objects. A renderer manifests the GUI; an execution
/// boundary manifests the behavior.
/// </remarks>
public sealed class ExperienceSurfaceDefinition
{
    public ExperienceSurfaceDefinition(
        string name,
        GuiNode gui,
        FsmBlueprint? fsm = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Experience surface name is required.", nameof(name));

        Name = name;
        Gui = gui ?? throw new ArgumentNullException(nameof(gui));
        Fsm = fsm;
    }

    /// <summary>Stable name used to address this surface inside its Experience.</summary>
    public string Name { get; }

    /// <summary>The recursive GUI definition presented by this surface.</summary>
    public GuiNode Gui { get; }

    /// <summary>Optional declarative behavior associated with the surface.</summary>
    public FsmBlueprint? Fsm { get; }
}
