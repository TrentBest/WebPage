# WebPage Theory

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Living GUI lens.** Trace this page’s claims to the responsible source, a visible manifestation where one exists, and executable evidence. This animated loop is an orientation illustration—not live runtime telemetry. The page must label implemented, illustrated, proposed, and unverified claims honestly.


## Source trace: the current browser proof

Use these source links to distinguish implemented behavior from the target architecture:

- **Declared request:** [`webpage-host.manifest.json`](../TheSingularityWorkshop/wwwroot/Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json) — declares the startup Moniker, primary Living GUI Experience, and capabilities.
- **Composition boundary:** [`WorkshopExperienceService.cs`](../TheSingularityWorkshop/Services/WorkshopExperienceService.cs) — requests FSM_COS composition and retains the resulting `RuntimeAssembly`. This is evidence of composition, not proof of end-to-end execution by that assembly.
- **Visible host:** [`Home.razor`](../TheSingularityWorkshop/Pages/Home.razor) — constructs the browser-facing GUI and currently coordinates both the composition service and transitional FSM service.
- **Actual progression:** [`FSMManagerService.cs`](../TheSingularityWorkshop/Services/FSMManagerService.cs) and [`PageFSM.cs`](../TheSingularityWorkshop/Services/PageFSM.cs) — the current page lifecycle advances on a 33 ms heartbeat using FSM_API.
- **Living population:** [`LivingGuiFsm.cs`](../TheSingularityWorkshop/Services/LivingGuiFsm.cs) — manages organism FSMs and population updates.
- **Behavioral proof:** [`LivingGuiRuntimeTests.cs`](../SingularityHub.Tests/LivingGuiRuntimeTests.cs) and [`LivingGuiReproductionTests.cs`](../SingularityHub.Tests/LivingGuiReproductionTests.cs) — exercise lifecycle progression, independent organism phases, growth, reproduction, and inherited position.

### Evidence status

| Element | Status | What the evidence means |
|---|---|---|
| Manifest input | Implemented | A concrete versioned host request exists |
| FSM_COS composition | Implemented | A `RuntimeAssembly` is produced and exposed |
| Transitional page FSM / Living GUI runtime | Implemented | Current behavior is driven by the FSM_API-backed host path |
| Single canonical path from composed assembly to observed browser behavior | **Not yet proven** | Composition and execution still have distinct paths; this is a migration/release gate |
| Shared living-GUI SVG | Illustrative | It explains the evidence loop; it does not read runtime state |
| Runtime-backed inspection panel | Proposed | Should bind to real service/runtime values and link to their tests; do not imply it exists yet |

A useful next visual is not another decorative diagram. It is a small evidence panel that reports real values from the current host: manifest identity/version, selected primary Experience, composition result, active page-FSM state, Living GUI population/phase, and a test/source trail. Keep each value visibly labelled as runtime, source-derived, test-derived, or proposed.


## 1. WebPage is a proving ground, not the Workshop

A proving ground exists to make an architectural idea observable. WebPage should be rich enough to demonstrate the ecosystem, but disciplined enough that reusable behavior migrates downward into the package that owns it.

## 2. Manifest before implementation detail

The manifest is the host's declared composition request. It describes what the host wants to make available; it should not become a hidden second source of truth in individual pages.

~~~text
declared composition
       ↓
composition
       ↓
assembled capability
       ↓
host manifestation
       ↓
observable experience
~~~

## 3. Composition is not manifestation

FSM_COS assembles. WebPage manifests.

The browser, Blazor components, presentation CSS, and host lifecycle belong to WebPage. Dependency closure, arbitration, and RuntimeAssembly creation belong to FSM_COS. State-machine execution belongs to FSM_API.

Keeping those responsibilities distinct means the same composition can eventually be manifested by another host.

## 4. Experience is the human-facing consequence

An Experience is not merely a page. It is the encounter produced when composed capabilities are given a host, a presentation boundary, and a user.

~~~text
domain capability
      +
composition
      +
presentation
      =
Experience
~~~

## 5. Perception is a boundary

The Rendering work demonstrates an important Workshop principle: what an observer can perceive should be separable from what the complete simulation knows.

A renderer can expose a restricted observation. ProtocolAi and GrammarAi can then operate on that observation rather than receiving privileged hidden state.

That boundary matters because it allows an AI or other consumer to reason from evidence instead of omniscience.

## 6. External theory belongs to external packages

WebPage can explain why it uses FSM_API, FSM_COS, MicroBundleDomain, ProtocolAi, GrammarAi, GUI packages, and other dependencies. It should not duplicate their complete theories.

The owning repository remains authoritative. WebPage provides the bridge:

~~~text
external package theory
        ↓
WebPage integration contract
        ↓
WebPage behavior
~~~

## 7. Extraction is architectural hygiene

When a capability becomes independently reusable, extracting it is not merely code cleanup. It is evidence that the abstraction has found its proper home.

WebPage can prototype. The package ecosystem should own the durable abstraction.

---

*The purpose of the proving ground is to make the architecture visible enough that the next abstraction can be placed correctly.*
