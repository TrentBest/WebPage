# Ecosystem Dependency Matrix

This document records the dependency boundary used while the Workshop moves from package-coupled hosts toward manifest-selected, repository-backed MicroBundles.

## Current published package baseline

| Package | Current verified consumer target | Source status | Role |
|---|---:|---|---|
| TheSingularityWorkshop.FSM_API | 1.0.13 | Current release | FSM execution authority |
| TheSingularityWorkshop.MicroBundleDomain | 1.0.1 | Current release | Neutral MicroBundle contract |
| TheSingularityWorkshop.FSM_COS | 0.1.0-alpha.5 | Latest verified consumer boundary; source is alpha.6 | Minimal composition/runtime substrate |
| TheSingularityWorkshop.FSM_UserIO | 0.1.0-alpha.1 | Current consumer target | User-input capability boundary |
| TheSingularityWorkshop.GUI.Core | 0.1.0-alpha.4 | Current Experience consumer target | Host-independent GUI primitives |
| TheSingularityWorkshop.GUI.Blazor | 0.1.0-alpha.8 | Current WebPage consumer target | Browser presentation adapter |
| TheSingularityWorkshop.GUI.WPF | 0.1.0-alpha.6 | Current AnyApp consumer target | Native Windows presentation adapter |
| TheSingularityWorkshop.ProtocolAi | 0.1.0-alpha.2 | Current consumer target | Protocol/AI capability |
| TheSingularityWorkshop.GrammarAi | 0.1.0-alpha.2 | Current consumer target | Grammar capability |
| TheSingularityWorkshop.FSM_Serialization | 1.0.0 | Current release | Serialization infrastructure |

**Important:** FSM_COS source currently declares 0.1.0-alpha.6, but consumers must not be moved to alpha.6 until that package is actually published and independently verified.

## Direct dependency graph

### Runtime substrate

FSM_COS -> FSM_API -> FSM_UserIO -> MicroBundleDomain

FSM_COS is the composition boundary. It should remain deliberately small. It does not become an application framework and it does not own presentation.

### Browser host

WebPage -> FSM_API, FSM_COS, MicroBundleDomain, GUI.Blazor, optional capability packages such as ProtocolAi and GrammarAi.

WebPage owns browser concerns: manifest acquisition, authentication/session handoff, viewport sizing, browser events, host navigation, persistence adapters, and presentation. Reusable runtime behavior belongs below this boundary.

### Desktop host

AnyApp -> FSM_API, FSM_COS, GUI.WPF, FSM_UserIO, optional capability packages such as ProtocolAi and GrammarAi.

AnyApp is a native host, not a second composition system.

### Experience MicroBundles

The current Workshop Moniker Experience is already being converted into a repository-publishable MicroBundle:

Experiences.Moniker -> MicroBundleDomain + GUI.Core

The next major extraction is the Living GUI behavior currently resident in WebPage. Its autonomous behavior, state contexts, deterministic population rules, and presentation contract must move into canonical Experience/MicroBundle ownership without changing observable behavior.

## What should become a MicroBundle?

A useful test is: if a capability can be selected by an Experience manifest, loaded independently, configured, cached, and composed with other capabilities, it is a strong MicroBundle candidate.

Likely candidates:
- Workshop Moniker — already converted
- Living GUI Experience — next extraction
- Forge/editor capabilities — Experience + MicroBundle composition
- future multiplayer capability — on-demand bundle; not installed until an Experience requests it
- future rendering/scene capabilities — bundle according to actual runtime composition needs
- visitor-created behavior/content — normal immutable MicroBundles, not WebPage-specific plugins

Likely infrastructure that should remain packages rather than MicroBundles:
- FSM_API — execution substrate
- FSM_COS — composition substrate
- MicroBundleDomain — contract
- FSM_Serialization — serialization infrastructure
- GUI adapters — host/presentation infrastructure
- FSM_REST — transport infrastructure

A package can still be used by a MicroBundle without itself becoming a MicroBundle.

## Target runtime lifecycle

User identity/session
  -> Experience manifest + configuration
  -> Immutable MicroBundle addresses
  -> MicroBundleRepository
  -> browser/device cache
  -> FSM_COS
  -> dependency closure + configuration + arbitration + RuntimeAssembly
  -> FSM_API execution
  -> host presentation (WebPage / AnyApp / MyVR / future hosts)

The important optimization is that the host does not carry every capability. If a visitor never enters a multiplayer Experience, there is no reason for multiplayer MicroBundles to be resident on the device. The Experience manifest requests them; the runtime retrieves, verifies, caches, loads, and later updates them.

## Two Workshop modes

### Runtime mode

WebPage is the browser runtime host. It presents Experiences selected by manifests and configuration. It should not accumulate application-specific implementations merely because a particular Experience needs them.

AnyApp demonstrates the same principle on the desktop. Its browser bridge is host interoperability infrastructure, not an alternative runtime architecture.

### Editor mode

Forge is the editor-time Experience. Forge creates MicroBundles and Experience manifests/configuration. The long-term goal is for Forge to appear inside the Workshop itself, making the Workshop both the place where Experiences are consumed and the place where they are authored.

That creates the intended recursive relationship:

Workshop -> Forge -> MicroBundles + Experiences -> Workshop

## Browser <-> desktop interoperability

The current AnyApp bridge establishes the first proof point:
- WebPage can identify the desktop host endpoint.
- AnyApp exposes a loopback command/state bridge.
- The Moniker Experience can be used as the shared visual identity.
- Future work should make ownership/focus transferable rather than duplicating the Experience.
- Splitting the Moniker exactly across browser and desktop is a deliberate interoperability demonstration, not merely a UI trick.

The production deployment path must use a user-approved, signed Windows package. A browser must not search arbitrary local directories or silently execute an .exe.

## Current migration order

1. Keep FSM_API at 1.0.13.
2. Keep MicroBundleDomain at 1.0.1.
3. Align all verified FSM_COS consumers to published alpha.5.
4. Publish and verify FSM_COS alpha.6 before any alpha.6 consumer migration.
5. Publish Moniker and subsequent MicroBundles to MicroBundleRepository.
6. Extract Living GUI from WebPage while preserving its behavioral tests.
7. Replace WebPage's local catalog with bootstrap Moniker + repository-backed MicroBundle resolution.
8. Make the WebPage hub entirely manifest-driven.
9. Establish signed AnyApp deployment and browser<->desktop Experience handoff.
10. Move Forge toward the editor-time runtime so visitors can author their own MicroBundles and Experiences.
11. Add capability bundles only when requested by an Experience.

## Non-negotiable architectural rule

**Do not move application functionality upward merely to make a host work.**

If a capability belongs to an Experience, extract it downward into the Experience/MicroBundle boundary.
If it belongs to execution, keep it in FSM_API.
If it belongs to composition, keep it in FSM_COS.
If it belongs to storage/retrieval, keep it in MicroBundleRepository.
If it belongs to browser/native presentation, keep it in the host or the appropriate GUI adapter.

The host should become thinner as the ecosystem becomes more capable.