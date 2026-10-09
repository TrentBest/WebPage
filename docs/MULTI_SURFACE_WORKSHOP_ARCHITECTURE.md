# Multi-Surface Workshop Architecture

> WebPage is a host for Experiences, not one page that accumulates every Workshop capability.

## Purpose

This document defines the architectural direction for turning WebPage into a navigable Workshop while preserving the current startup proof and package boundaries. It is a host/presentation contract, not a second implementation of FSM_COS, the MicroBundle domain, or the Experience runtime.

## Current / Direction / Future

### Current

- WebPage loads a host manifest and composes the primary Living GUI root through FSM_COS.
- The current root is MicroBundle 2102; its Moniker dependency is 2110.
- The startup sequence is a presentation sequence; dependency closure belongs to FSM_COS.
- The browser still bridges into transitional WebPage-owned PageFSM/LivingGuiFsm execution. Runtime composition is real, but it is not yet the sole authority for Living GUI execution.
- The repository contains architectural direction for Understand, Experiences, Create, and Publish, but not every destination is an implemented, manifest-backed Experience.

### Direction

- The manifest and capability/Experience registry become the source of truth for available destinations.
- The Hub becomes the navigable home/orchestration surface after the opening Experience. It is not fabricated as an additional FSM_COS Experience simply to provide a navigation target.
- Each destination has an explicit human purpose and a capability/Experience identity. Navigation labels are presentation; they do not define runtime behavior.
- A destination is advertised as available only when its backing capability is discoverable and its state is honestly represented (available, experimental, unavailable, or planned).
- The same semantic Experience should remain host-independent where practical; WebPage owns browser presentation and WebPage-only educational Deep Dives.
- The user can move between experiencing a capability, observing its behavior, inspecting its composition, and learning how it works without treating all content as one long page.

### Future

- The Workshop may manifest navigation as doors, terminals, portals, workbenches, or spatial landmarks rather than only tabs.
- Ontology can organize discovery and eventually drive a shared 2D/3D spatial navigation grammar.
- Create/Capture/Publish can turn a visitor's composition into attributable, versioned artifacts.
- These future surfaces must not be presented as functional until backed by real contracts and implementation.

## Human-facing surfaces

The primary surfaces express user intent, not an exhaustive list of packages:

| Surface | Visitor question | Responsibility |
|---|---|---|
| Understand | What am I seeing, and how does it work? | Explanations linked to executable evidence and owning packages. |
| Experiences | What can I enter? | Discover, enter, resume, and leave composed Experiences. |
| Create | Can I make or change something? | Compose capabilities and observe the consequences. |
| Publish | Can I keep, share, or distribute what I made? | Eventually capture, attribute, package, and publish creations. |

Rendering, Education, the Warehouse, and other specialized tools are capabilities or destinations within this model, not reasons to keep adding unrelated top-level navigation items.

## Startup and navigation lifecycle

The opening sequence remains a distinct, intentional first-contact Experience:

```text
WebPage boot
  -> load host manifest
  -> compose the primary root through FSM_COS
  -> present WebPage-only entry labels and advisory
  -> obtain explicit visitor entry
  -> manifest the Living GUI startup behavior
  -> reveal the Workshop Moniker
  -> transition into the Hub
  -> discover and enter an available destination
```

The exact visual choreography must continue to follow the active startup contract and its tests. This diagram states responsibility and intent; it does not authorize Razor components to invent parallel timers, counters, or FSM behavior.

The Hub must not cover or prematurely replace the Moniker reveal. Once the Hub is available, navigation should appear as a consequence of the startup state transition, not as a second independent page lifecycle.

## Manifest-driven destination contract

A destination record should identify, at minimum:

- stable destination/Experience identity;
- visitor-facing name and concise purpose;
- the manifest or capability root required to enter it;
- availability/lifecycle state;
- the host presentation/entry strategy;
- relevant deep-dive or documentation references, when they exist.

Do not duplicate the full package contract into WebPage's destination record. Package/domain owners remain authoritative for their APIs and theory.

Do not invent a second competing manifest schema if an existing canonical Experience or host-manifest contract already carries these semantics. Extend the owner of the existing contract only when a proven gap requires it.

## Runtime and package boundaries

- **FSM_API** owns meaningful state-machine behavior and execution semantics.
- **MicroBundleDomain** owns the MicroBundle contract and its domain meaning.
- **FSM_COS** owns composition, dependency closure, arbitration/convergence, and RuntimeAssembly creation.
- **Experience/MicroBundles** define the composition and capabilities being requested.
- **WebPage** owns host manifest loading, browser-specific presentation, navigation/chrome, host integration, and WebPage-only Deep Dive presentation.
- **The Hub** coordinates discovery and entry; it must not become a giant manager that absorbs unrelated capability implementations.
- **The Forge** may help create or modify compositions, but building and storing/publishing are distinct responsibilities. Persistent content belongs to the appropriate hosting/storage capability.

If a capability is reusable beyond WebPage, implement and document it at its canonical owner, then consume and prove it here. Avoid introducing a dependency from a lower-level package back into WebPage or FSM_COS.

## Agent-to-agent repository requests

When an agent needs a capability or direct usage example from another repository, deposit the request in the repository that owns the requested capability. Name the request using the requesting repository and a unique request identifier, for example `WebPage-0001-usage-example.md`. The containing repository already identifies the recipient; the request body must not redundantly state that repository as its location.

A request records the need, evidence, expected outcome, and disposition. It is not an automatic dependency, merge, release, or implementation authorization. Prefer a concrete usage example and executable proof when the request concerns integration.

This convention is a coordination mechanism, not a new runtime dependency or a reason to centralize all requests in one repository.

## Implementation sequence

1. Preserve and verify the current opening sequence and functionality-preservation ledger.
2. Inventory the existing manifest, registry/catalog, Hub, and route/presentation mechanisms before adding new abstractions.
3. Define the smallest destination contract using existing canonical types wherever possible.
4. Make the Hub enumerate only destinations backed by the manifest/registry and clearly represent their lifecycle state.
5. Connect one additional real destination end-to-end as the vertical proof; do not seed a static menu of fictional features.
6. Ensure entering/leaving a destination preserves host identity and does not duplicate FSM_COS composition or FSM execution.
7. Add contract tests for discovery, unavailable destinations, identity, and entry/exit; add browser-level visual evidence for the transition.
8. Update the smallest authoritative documents and prove the build/tests/CI.
9. Consolidate temporary branches after proof. Keep `master` untouched unless promotion is explicitly requested.

## Acceptance criteria

- The startup Experience remains manifest-led and its current behavior is preserved.
- The Hub is distinct from the opening Experience and from any destination Experience.
- Available destinations are derived from real manifest/registry data, not duplicated hard-coded catalogs.
- A visitor can enter at least one additional real destination and return to the Hub without losing the host/runtime contract.
- Page/component code does not duplicate runtime FSM behavior.
- Deep Dives explain actual implementation and link to owning package documentation.
- Current, transitional, and future behavior are labeled honestly.
- Tests and visual evidence support architectural claims.
- No NuGet package or release is published without explicit approval.

---

*The page opens the door. The Experience gives the visitor a place to stand. The Hub helps them choose where to go next. The architecture lets them look underneath it.*
