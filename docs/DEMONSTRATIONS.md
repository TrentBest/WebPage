# Demonstrations — discovery, evidence, and extension

The Demonstrations gallery is a WebPage-hosted Experience, not a separate demo site. Its descriptive entries are loaded from the host manifest and resolved through `DemonstrationRegistry`; the existing Home presentation renders the gallery and its detail view inside the current Workshop host.

## Current entries

| Demonstration | Current status | What the visitor can do |
|---|---|---|
| GPU Instancing — Browser WebGPU Proof | Runnable locally; not hosted by WebPage | Read the scope and limitations, inspect the proof branch, and follow the local server instructions |
| Native WPF / Direct3D 12 Surface Proof | Experimental, source-only | Read the native-surface scope and inspect the sample source |
| Event Horizon Buckets — Renderer Architecture | Experimental architecture model | Read the bucket/cache/invalidation explanation and inspect the renderer pull request |
| Soldier Field — Exoskeleton and Event Horizon Lab | Live WebPage experience; exploratory WebGPU lab | View the 3D soldier, inspect 38 stable part records, download preliminary cut sheets, and scale per-horizon stress workloads |

Soldier Field is the first entry with an embedded destination inside WebPage. It is a browser adapter hosting the Visualize repository's exploratory WebGPU lab, not a production .NET Visualize backend. The other entries remain source-only or locally runnable as described. A source link is not a run link. Build success is not evidence of a native GPU run, and the Event Horizon entry does not invent simulated performance measurements.

## Runtime boundary

Current path:

```text
webpage-host.manifest.json
        |
        v
WebPageHostManifestParser
        |
        v
WorkshopExperienceService
        |
        v
DemonstrationRegistry (manifest-backed metadata discovery)
        |
        v
Home's existing Hub presentation adapter
```

The manifest describes entries, optional same-origin embedded presentation URLs, and their limitations; it does not embed executable behavior. The registry keeps unavailable/source-only entries discoverable so the detail view can explain why they cannot run here. Soldier Field is rendered in an iframe declared by its manifest entry and served from WebPage's static assets. The host currently declares WebPage and Browser as host capabilities. This does not claim a production .NET Visualize backend, native WPF/Direct3D 12 execution, or Renderer-driven bucket scheduling.

**Presentation boundary to tighten:** the gallery currently extends the Home page's existing RenderTreeBuilder-based Hub presentation path, nested inside the Workshop GUI root. It does not introduce a new UI framework or router, but the gallery's cards and diagrams are not yet composed as a fully platform-neutral GUI builder tree. Before claiming that boundary complete, inspect the current GUI builder/rendering contract and add a reusable link/action primitive if the existing vocabulary cannot express navigation safely.

**Architectural boundary still to advance:** this first gallery increment is a manifest-backed *demonstration metadata registry*. It does not yet turn the three external proofs into executable `IExperience` implementations or register their processing groups with the Hub. That is intentional for these source-only proofs; do not imply that metadata discovery equals runtime integration. As proofs acquire reusable behavior, move that behavior into the appropriate MicroBundle/Experience and let the Hub own execution lifecycle and stepping.

## Adding a demonstration

1. Verify the source, maturity, launch method, requirements, and evidence. Do not infer a runnable URL from a source repository or pull request.
2. Add a `demonstrations` entry to `TheSingularityWorkshop/wwwroot/Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json`.
3. Supply a stable `id`, title, short summary, engineering motivation, status, `whatItProves`, `whatItDoesNotProve`, launch mode, run instructions, source URL, requirements, and optional `requiredHostCapabilities`. An optional `embeddedExperienceUrl` is for an existing same-origin browser surface that the host can present; it is not a license to put reusable behavior in the gallery.
4. Keep descriptive metadata separate from behavior. The optional embedded URL is presentation routing only. If the demonstration becomes a real runtime Experience, use the existing Experience/MicroBundle contracts and Hub-owned process groups rather than adding a name-based switch to the gallery.
5. Add tests for metadata parsing, stable-ID lookup, duplicate IDs, and capability compatibility.
6. Build and run tests, then inspect the actual browser at desktop and narrow widths. Tests alone do not verify the visual manifestation.

`requiredHostCapabilities` describes what is needed to execute a proof in this host. An unmet requirement must be surfaced as a limitation, not silently converted into a runnable action. Source-only entries should still offer source and explanation links.

## Evidence links

- [GPU Instancing WebGPU proof pull request](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/pull/5)
- [Native WPF / Direct3D 12 sample source](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/tree/development/samples/WpfD3D12Proof)
- [Renderer Event Horizon buckets pull request](https://github.com/TrentBest/TheSingularityWorkshop.Renderer/pull/8)
- [Visualize Soldier Field and engineering cut-sheet lab](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/pull/7)

## Verification checkpoint

The implementation changes are committed on `development`. Automated build/test results and browser verification have not yet been confirmed in this handoff. Treat the visual experience as **pending verification** until those checks are performed.
