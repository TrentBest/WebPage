# MicroBundle Arbitration Map

This is the working map for turning the current collection of MicroBundles into an interoperable capability system.

The goal is not to make every bundle depend on every other bundle. The goal is to identify semantic structure, missing coordination bundles, and places where arbitration can compose capabilities without creating hard references between implementations.

## Current inventory

### Runtime / platform

- MicroBundle — FSM_API-backed lifecycle substrate
- IMicroBundle / IMicroBundleProvider — runtime capability boundary
- UserMicroBundle — visitor identity and personalization
- ConferenceMicroBundle — shared collaboration/session capability
- ExploreExperienceMicroBundle — spatial experience host
- SpatialNavigationMicroBundle — spatial movement/navigation
- FsmForgeMicroBundle — FSM construction and preview
- WebMicroBundleProvider — web manifestation

### Demonstration / experience capabilities

- PongMicroBundle — FSM-driven demonstration
- MonikerMicroBundle — living visual identity/presentation
- EducationalProviderMicroBundle — educational/provider capability
- LeaderboardMicroBundle — competitive/social data
- TransitTrainMicroBundle — spatial transit demonstration
- ConstructionVehicleMicroBundle — construction vehicle capability

### Software pattern family

SoftwarePatternsMicroBundle is already hierarchical:

SoftwarePatternsMicroBundle → CreationalPatternsMicroBundle → Builder / Factory
SoftwarePatternsMicroBundle → StructuralPatternsMicroBundle → Composite / Facade
SoftwarePatternsMicroBundle → BehavioralPatternsMicroBundle → Strategy / Observer / Command / Provider

## Missing structural bundles

### Arbitration MicroBundle
Owns composition sessions: discovery, version comparison, dependency inspection, bounded rounds, conflict records, collaboration records, and undoable results.

### Capability / Ontology Discovery MicroBundle
Turns ontology and variant addressing into reusable discovery: capability filtering, experience compatibility, semantic tags, and content lookup. This should eventually replace hard-coded AEC content catalogs.

### Composition / Recipe MicroBundle
Arbitration decides whether capabilities can cooperate. Composition records what was produced and makes successful compositions reusable.

### Context MicroBundle
Provides the answer to: what is happening around this capability right now? Context can include Experience, visitor, spatial location, selected object, active floor, authorities, and active capabilities.

### Presentation MicroBundle
Declares what a capability wants to present without knowing whether the host is Blazor, Unity, WPF, BIM, VR, or another renderer.

### Audio / Living Signal MicroBundle
Reusable sound and signal capability for first contact, activation, state transitions, arbitration success/conflict, spatial presence, and ambient digitens.

### Presence MicroBundle
Owns users, anonymous visitors, digitens, destinations, activities, and visibility. This turns the Workshop campus from a diagram into a living place.

### Identity / Personalization MicroBundle
Extends UserMicroBundle toward account identity, avatar, preferences, connected LLM, AI terminal configuration, and persistent personalization.

## First interaction graph

Discovery → Arbitrator → Context / Composition → Providers → Presentation → Web / Unity / BIM / VR

The important point is that these are capability interactions, not direct implementation references.

## First concrete arbitration examples

### Singing sword

SWORD + WEAPON + AUDIO + VOICE/PERFORMANCE + CONTEXT → ARBITRATOR → SINGING SWORD COMPOSITION

The system should not need a special primitive SingingSwordMicroBundle. A successful composition can itself become an addressable reusable bundle.

### Construction vehicle

VEHICLE + TRACTOR + CAB + WHEELS + SCOOP + LABEL + CONTROL + SPATIAL PRESENCE

The same component capabilities should be reusable elsewhere without rebuilding the vehicle implementation.

### AEC room

ROOM + WALL + DOOR + SPACE + PROGRAM + SYSTEMS + ONTOLOGY + PRESENTATION

A duplicated room should remain a composition of reusable capability identities rather than becoming a one-off page object.

## Arbitration rules

1. Never arbitrate from display names. Use integer identity plus ontology coordinates.
2. Never make composition depend on presentation technology. Blazor/Unity/WPF/BIM/VR are providers or manifestations.
3. Do not create a new bundle when an existing bundle can satisfy the capability.
4. When two bundles can cooperate, record the relationship.
5. When a composition succeeds, the resulting composition becomes addressable.
6. Conflicts are data and should be visible in the arbitration record.
7. Arbitration is bounded; a composition attempt has a finite number of rounds.
8. Successful compositions must be reusable.

## Implementation sequence

Phase A — inventory: create a machine-readable capability manifest for every current MicroBundle.
Phase B — semantic structure: give every bundle a meaningful OntologySignature and variant identity.
Phase C — interaction declarations: required, optional, compatible, conflicting, and produced capabilities.
Phase D — arbitration: implement the bounded arbitration pipeline.
Phase E — composition: persist successful compositions as reusable bundle definitions.
Phase F — experiences: replace hard-coded catalogs with registry/discovery results.

This is the point where the Workshop stops merely containing examples of MicroBundles and starts demonstrating the interoperability model itself.