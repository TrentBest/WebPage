using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class DemonstrationRegistryTests
{
    [Fact]
    public void Registry_discovers_entries_by_stable_id_without_a_name_switch()
    {
        var first = Create("gpu-proof", requiredCapabilities: ["WebGPU"]);
        var second = Create("event-horizon");

        var registry = new DemonstrationRegistry([first, second], ["WebPage", "Browser"]);

        Assert.Equal(2, registry.Entries.Count);
        Assert.True(registry.TryGet("GPU-PROOF", out var discovered));
        Assert.Same(first, discovered);
        Assert.True(registry.TryGet("event-horizon", out _));
    }

    [Fact]
    public void Missing_host_capability_is_reported_without_hiding_the_source_entry()
    {
        var proof = Create("gpu-proof", requiredCapabilities: ["WebGPU"]);
        var registry = new DemonstrationRegistry([proof], ["WebPage", "Browser"]);

        Assert.Single(registry.Entries);
        Assert.False(registry.HasRequiredHostCapabilities(proof));
        Assert.False(registry.TryGet("not-declared", out _));
    }

    [Fact]
    public void Registry_rejects_duplicate_ids_case_insensitively()
    {
        var first = Create("gpu-proof");
        var duplicate = Create("GPU-PROOF");

        Assert.Throws<ArgumentException>(() =>
            new DemonstrationRegistry([first, duplicate], ["WebPage"]));
    }

    [Fact]
    public void Host_manifest_parses_demonstration_metadata_without_embedding_behavior()
    {
        const string json = """
        {
          "manifestId": "test-host",
          "version": "1.0.0",
          "startup": [
            { "name": "Moniker", "kind": "Moniker", "microBundles": [], "presentationSeconds": 3 }
          ],
          "hub": [],
          "running": [
            { "name": "Primary", "kind": "Primary", "microBundles": [{ "bundleId": 1, "version": "1.0.0" }], "presentationSeconds": 0 }
          ],
          "deepDive": { "experience": "Primary", "restartFromManifest": true, "telemetry": false },
          "demonstrations": [
            {
              "id": "gpu-proof",
              "title": "GPU proof",
              "summary": "A source-backed proof.",
              "whyItMatters": "It explores instancing.",
              "status": "Experimental",
              "whatItProves": "A browser experiment.",
              "whatItDoesNotProve": "Production performance.",
              "launchMode": "Run locally",
              "runInstructions": "Start a local static server.",
              "sourceUrl": "https://example.com/source",
              "requirements": "A WebGPU-capable browser.",
              "requiredHostCapabilities": ["WebGPU"]
            }
          ]
        }
        """;

        var manifest = WebPageHostManifestParser.Parse(json);
        var demonstration = Assert.Single(manifest.Demonstrations!);

        Assert.Equal("gpu-proof", demonstration.Id);
        Assert.Equal("Run locally", demonstration.LaunchMode);
        Assert.Equal("Production performance.", demonstration.WhatItDoesNotProve);
        Assert.Equal("https://example.com/source", demonstration.SourceUrl);
        Assert.Contains("WebGPU", demonstration.RequiredHostCapabilities!);
    }

    private static DemonstrationDefinition Create(
        string id,
        IReadOnlyList<string>? requiredCapabilities = null) =>
        new(
            id,
            $"Title {id}",
            "Summary",
            "Why it matters",
            "Experimental",
            "What it proves",
            "What it does not prove",
            "Read explanation",
            "Read the source",
            "https://example.com/source",
            "No special requirements",
            requiredCapabilities);
}
