using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Semantic annotation attached to a MicroBundle without changing the capability itself.
/// This is the extra information layer used for discovery, teaching, tooling, and future
/// ontology projection.
/// </summary>
public sealed record SoftwarePatternAnnotation(
    ulong BundleId,
    string Name,
    string Group,
    string Role,
    IReadOnlyList<string> Tags,
    string Description,
    ulong ParentBundleId);

/// <summary>
/// Annotation MicroBundle for the complete Software Patterns hierarchy.
/// It references the top bundle, every grouping bundle, and every focused pattern bundle.
/// </summary>
public sealed class SoftwarePatternsAnnotationMicroBundle : IDisposable
{
    public const int BundleId = 2214;

    private readonly MicroBundle _lifecycle;
    private readonly SoftwarePatternsMicroBundle _patterns;
    private bool _disposed;

    public SoftwarePatternsAnnotationMicroBundle(SoftwarePatternsMicroBundle patterns)
    {
        ArgumentNullException.ThrowIfNull(patterns);
        _patterns = patterns;
        _lifecycle = new MicroBundle(
            BundleId,
            "SOFTWARE PATTERNS // ANNOTATION",
            new WebMicroBundleProvider());

        Annotations = BuildAnnotations(patterns);
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;

    /// <summary>Direct reference to the composed pattern hierarchy.</summary>
    public SoftwarePatternsMicroBundle Patterns => _patterns;

    /// <summary>All bundles in the hierarchy, including the top-level and grouping bundles.</summary>
    public IReadOnlyList<object> ReferencedBundles =>
        [
            _patterns,
            .. _patterns.Groups,
            .. _patterns.Patterns
        ];

    /// <summary>Additional semantic data layered over the referenced capability bundles.</summary>
    public IReadOnlyList<SoftwarePatternAnnotation> Annotations { get; }

    public void Update()
    {
        if (_disposed) return;
        _lifecycle.Update();
    }

    public void Invalidate()
    {
        if (_disposed) return;
        _lifecycle.Invalidate();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _lifecycle.Dispose();
        _disposed = true;
    }

    private static IReadOnlyList<SoftwarePatternAnnotation> BuildAnnotations(SoftwarePatternsMicroBundle patterns)
    {
        var annotations = new List<SoftwarePatternAnnotation>
        {
            new(
                patterns.Id,
                "Software Patterns",
                "ROOT",
                "PATTERN FAMILY",
                ["software", "patterns", "workshop", "architecture"],
                "Top-level composition of the Workshop software-pattern capability family.",
                0)
        };

        foreach (var group in patterns.Groups)
        {
            annotations.Add(new SoftwarePatternAnnotation(
                group.Id,
                group.Name,
                group.Presentation.Title,
                "PATTERN GROUP",
                group.Presentation.Capabilities,
                group.Presentation.Description,
                patterns.Id));

            foreach (var pattern in group.Patterns)
            {
                annotations.Add(new SoftwarePatternAnnotation(
                    pattern.Id,
                    pattern.Name,
                    group.Presentation.Title,
                    "FOCUSED PATTERN",
                    pattern.Presentation.Capabilities,
                    pattern.Presentation.Description,
                    group.Id));
            }
        }

        return annotations;
    }
}
