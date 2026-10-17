# WebApp Showcase Orchestration and Living Documentation

**Audience:** WebPageLLM and maintainers of the Workshop host  
**Branch:** `development`  
**Status:** architecture and integration contract; it does not claim the full runtime-redefinition proof is already complete.

## Mission

WebPage is the Workshop's WebApp-shaped showcase: one compiled browser host should be able to load a manifest, resolve the declared composition through the canonical stack, and present the Experiences that composition makes available. It should show what the technology actually does—not a parallel set of hard-coded pages that merely look like the architecture.

**The main task is orchestration, not inventing another framework.** The host loads and validates the manifest, asks FSM_COS to resolve its declared MicroBundle roots and dependency closure, then presents the resulting composition through the browser adapter. FSM_API remains the state-machine execution authority. Experience and MicroBundle owners supply their own behavior. GUI.SVG provides inspectable visual manifestations for concepts and evidence.

## Living proof: see the model become a picture

The repository already links a GUI.SVG-generated animated route sample:

[**Open the GUI.SVG connected-points sample**](../../TheSingularityWorkshop/wwwroot/Visualize/07-gui-svg-connected-points.svg)

The sample is evidence of SVG manifestation, not evidence that a WebApp manifest is dynamically reconfiguring the entire host. Keep those claims separate.

The Workshop's rule for documentation is:

> When a concept has a visible consequence, show that consequence. Prefer a living or generated visual that uses the real model and canonical package over a hand-drawn diagram that can drift away from the implementation.

For each substantive concept or demo, ask:
- Can a reader change a parameter and immediately see its effect?
- Can the image expose the state, geometry, scale, timing, route, dependency, or transformation being explained?
- Can the visual be generated from the actual model/API and verified in CI?
- Does the caption say what it proves and what it does not prove?

Use GUI.SVG for semantic GUI trees and inspectable 2D scenes. Use Visualize/Renderer when the concept needs their actual rendering boundary. A GUI.SVG animation is declarative SVG presentation; it does not itself execute FSM_API, provide a simulation clock, or prove GPU rendering. Do not create a decorative animation that implies those capabilities.

## Canonical loading and presentation flow

```text
COMPILED WEBAPP HOST
  |
  | load manifest at runtime
  v
VALIDATE MANIFEST + VERSION + REFERENCES
  |
  | startup Experience + primary Experience + active destinations
  v
RESOLVE DECLARED MICROBUNDLE ROOTS
  |  FSM_COS resolves compatible versions and dependency closure
  v
RuntimeAssembly / capability assemblies
  |
  | bind actual Experience providers and lifecycle
  v
FSM_API state machines + Experience-owned behaviors
  |
  | browser-specific manifestation
  +--------------------------+
  |                          |
  v                          v
INTERACTIVE WEB EXPERIENCE   GUI.SVG / VISUAL EVIDENCE
  |                          |
  +-------------+------------+
                v
       OBSERVABLE, TESTABLE CLAIMS
```

Ownership is intentional:

| Responsibility | Owner |
|---|---|
| State-machine semantics and execution primitives | FSM_API |
| MicroBundle identity and version contracts | MicroBundleDomain |
| Dependency closure and RuntimeAssembly composition | FSM_COS |
| User journey, scene, and reusable behavior | Experiences and owning MicroBundles |
| Browser lifecycle, URL integration, input, and host presentation | WebApp host |
| Semantic GUI-to-SVG serialization | GUI.SVG |
| Renderer-neutral rendering data and concrete rendering backend | Visualize / Renderer, according to their actual APIs |

Do not move a package's behavior into WebPage just because WebPage currently displays it. WebPage should orchestrate the composition and provide browser adapters.

## Current implementation facts

The current development source has real foundations:

- `WorkshopExperienceService.InitializeAsync` loads `Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json` at runtime.
- It parses the manifest, requires startup and running Experiences, and passes their declared roots to FSM_COS.
- FSM_COS returns a real `RuntimeAssembly`; separately declared hub MicroBundles and capabilities can also be composed into assemblies.
- The host's composition catalog currently registers known MicroBundle implementations. This is not yet proof of loading arbitrary new executable bundle artifacts at runtime.
- The current manifest has startup bundle 2101, primary bundle 2102, and capability roots for Rendering (4100) and ProtocolAi Terminal (4099).
- The manifest's hub destinations currently declare empty `microBundles` lists. Their routes and visible behavior are still partly implemented in compiled host pages/components.
- `SwitchManifestAsync(path)` now loads a candidate same-origin host-manifest path, composes a new primary assembly, optional hub/capability assemblies, and the candidate Moniker bundle, then activates it in the stable session-scoped Hub only after success; the Hub identity remains shared with PageFSM. `RestartFromManifestAsync` reloads the currently selected host-manifest path.
- The composition catalog now creates fresh MicroBundle instances for each resolve, which is required to avoid sharing runtime-owned objects across activations. Hub/HubRuntime are scoped to a browser session, and manifest reset preserves the Hub instance required by PageFSM.
- The new switch path is covered by tests for two host-manifest configurations, failed-candidate rollback, and external/traversal-path rejection. CI is the authority for the result; do not claim green until it completes.
- The Experience repository now has the `workshop-showcase` definition at [experiences/WorkshopShowcase/manifest.json](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/manifest.json), plus the complete current six-entry demonstration catalog at [demonstrations.json](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/demonstrations.json). These are migration artifacts; the WebPage host does not consume them yet, and the old in-host demonstration definitions must not be deleted until runtime resolution and visitor journeys are tested.

Therefore the host has **manifest loading and composition**, but the full application is not yet manifest-authoritative and hot-redefinition has not been proven. Do not describe the current implementation as arbitrary runtime bundle loading.

## The next orchestration slice

Implement this as a vertical slice with tests, not as a second page framework:

1. **One explicit manifest-loading boundary.** The first slice now switches legacy host-manifest asset paths. Next, select an Experience identity/version, resolve its manifest and referenced content through the Experience/MicroBundle repository boundary (with an explicit local bootstrap/cache path), then validate schema/version, required sections, root versions, digests, and permitted references before changing active state.
2. **Compose into a candidate runtime.** Resolve startup, primary Experience, and explicitly selected destination/capability roots through FSM_COS. Do not silently substitute hard-coded roots when a supplied manifest is invalid.
3. **Activate only after success.** Keep the currently active composition until the candidate has validated and composed successfully. Report missing, incompatible, or unauthorized roots clearly.
4. **Present the resolved composition.** The browser host must route to an Experience/provider selected by manifest data. A manifest entry that only changes a label or URL is not a meaningful redefinition proof.
5. **Support deliberate reload/reconfiguration.** Reload the manifest from its configured source and rebuild the candidate composition without recompiling the host. Dispose replaced runtime resources according to the owning contracts.
6. **Prove a visible difference.** Use one test host build and two valid manifests that select meaningfully different available Experience compositions. Assert both the resolved roots and the actual browser behavior change.
7. **Keep visual evidence connected to the model.** Where the Experience teaches a concept, generate the diagram/animation from its real model through GUI.SVG or use the owning renderer. Verify generated assets in CI when the repository has a generator.
8. **Preserve journeys.** Test welcome/Moniker startup, Living GUI, direct routes including `/about-us`, Forge, demonstrations, and the current browser smoke journeys before removing transitional code.

A runtime manifest can select only capabilities that the host can resolve. Installing genuinely new executable MicroBundle implementations requires a trusted, versioned artifact/provider mechanism and explicit compatibility/security policy; a JSON file alone cannot manufacture executable code.

## Acceptance criteria

- [ ] Manifest source and version are observable in diagnostics.
- [ ] Invalid manifest input leaves the active composition intact and produces a useful error.
- [ ] FSM_COS resolves declared roots and compatible dependency closure; tests inspect the resulting RuntimeAssembly.
- [ ] The selected Experience is actually presented by the host rather than shadowed by a hard-coded route/page branch.
- [ ] Two manifests cause a meaningful runtime behavior/presentation difference on the same host build.
- [ ] Missing/incompatible roots fail safely and predictably.
- [ ] GUI.SVG examples are generated from canonical GUI/model data where practical, have accessible labels/descriptions, and are not mislabeled as FSM/GPU execution.
- [ ] Existing journeys and CI checks remain green.

## Cross-repository ownership

The Experience-owned showcase manifest and first demonstration catalog now live in [TheSingularityWorkshop.Experiences](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/tree/development/experiences/WorkshopShowcase). Track the reciprocal host integration in [`docs/WebPageLLM.md`](WebPageLLM.md) and the Experience repository's [`WebPageLLM_HANDOFF.md`](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/WebPageLLM_HANDOFF.md). Do not treat these first files as a completed runtime artifact pipeline.

## Coordination contract

WebPageLLM coordinates the host integration and consumes canonical package APIs. It must not edit other repositories' owned implementation files without the owner's permission. Send package-specific API gaps to the owning repository through its reciprocal handoff document. Record exact files, commit, tests/commands, observed evidence, limitations, and the next seam in handoffs.

## Branch and release discipline

Work on `development`; preserve the two-branch Sith policy. Do not modify or promote `master`, publish packages, or deploy the public site without explicit approval.
