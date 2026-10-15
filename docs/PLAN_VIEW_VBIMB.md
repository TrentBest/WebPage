# VBIMB — Virtual Building Information Model Builder

## Purpose

VBIMB is the Workshop's path from intent to a coherent, inspectable building plan. It is not a static floor-plan picture. A plan is structured data: rooms, walls, openings, fixtures, circulation, and the relationships that make the geometry meaningful.

The first implementation slice lives in WebPage as a host-side plan capability and the interactive Singularity Architecture studio at `/architecture`. The studio draws the renderer-neutral plan model, lets you select rooms, edit room rectangles, toggle opening state, and capture a structured design brief. Edits are currently in-memory only; room edits do not yet reshape related wall geometry.

## First plan vocabulary

- **BuildingPlan** — stable identity, coordinate extents and collections of plan elements.
- **PlanRoom** — bounded area with name and use.
- **PlanWall** — segment with thickness.
- **PlanOpening** — a semantic opening attached to a wall, with offset and width.
- **PlanFixture** — built-in or movable plan content.
- **PlanOpeningKind** — hinged door, sliding door, pocket door, revolving door, archway, or window.

Opening semantics are separate from drawing style. A renderer can draw a hinged door's leaf and swing arc, a sliding door's panel and travel direction, a pocket door disappearing into a wall, or a revolving door's radial leaves. A closed opening, open fraction, hinge, and swing direction are data; the visual glyph is a projection of those data.

The initial schema validates unique IDs, positive plan extents, in-bounds room and fixture rectangles, wall references, positive opening widths, and open fractions in the range 0–1. It is a sketching contract, not a structural, fire, accessibility, or permit validator.

## The organization and asset substrate

Singularity Architecture is a Workshop destination, but the organization model is intentionally generic. An architecture firm is one kind of organization; the same data shape can represent a school, manufacturer, retailer, studio, or civic service. Members reference stable Digiten IDs and role labels rather than cloning people into each company. Projects can produce typed artifacts such as a `BuildingPlan`, tool, service, or another reusable asset.

The initial `SingularityOrganization` contract supports identity, industry, optional building assignment, membership, and project/artifact references. It does not implement persistence, employment simulation, ownership policy, licensing, payment, or marketplace fees. Those policies belong above stable identities for people, organizations, projects, and reusable assets.

## The Architect and the ProtocolAi terminal

**The Workshop does not have to talk directly to an LLM.** The user chooses their own LLM and the Hub exposes the ProtocolAi terminal as the exchange boundary. The intended interaction is a deliberate clipboard round trip:

1. The host assembles the current task, relevant plan state, protocol vocabulary, and grammar/context.
2. The user presses **Copy to Clipboard** and pastes the exchange into the LLM of their choice.
3. The user copies the LLM's response.
4. The user presses **Paste from Clipboard** in the Workshop terminal.
5. The host parses the architecture response as a strict JSON proposal and validates its plan geometry through `BuildingPlan`.
6. The user explicitly applies a validated proposal; import alone never changes the working plan.

ProtocolAi defines deterministic application-owned meaning; it does not itself provide clipboard transport, contact a provider, or execute returned instructions. GrammarAi describes allowed structure; it is not a truth oracle or permission system. The terminal is the transport surface, and the host remains responsible for validation and authority. The current WebPage Architecture path includes the composed protocol and grammar descriptions in the copied context, then parses a returned JSON plan and applies the documented `BuildingPlan` checks. It does **not yet** resolve the incoming proposal through ProtocolAi symbols or validate it with a full GrammarAi rule engine; those semantic checks remain a next step.

HeadlessAi can optionally support a role such as **Architect** when a user or deployment explicitly configures a provider endpoint. In that mode, HeadlessAi is an invocation path for a bounded role task—not a requirement for the building schema or the clipboard workflow. It must remain optional, and its output must enter the same proposal validation and review boundary. The role can help clarify requirements, explain trade-offs, or draft a plan proposal; it cannot silently approve its own work.

The plan schema must not depend on ProtocolAi, GrammarAi, or HeadlessAi. They are optional collaborators around the same host-owned plan model.

## Proposal contract and safety boundary

A response should be treated as untrusted proposal data. Preserve stable IDs where they are supplied, reject malformed or out-of-bounds geometry, report unresolved references and conflicts, and retain the original response for inspection. Do not silently guess a missing dimension or execute an arbitrary response as code.

The current plan validator checks only its documented geometric constraints. Door clearance, circulation, occupancy, structure, egress, accessibility, and building-code requirements need separate explicit rules before a plan can be called buildable.

## Agent cards and inspect-on-select

The city plan should be a world view, not a wall of labels. Each visible Digiten gets a compact card containing stable ID, name, current role/activity, home/work/school relationship where known, and a few high-value status signals. Clicking a card selects a Digiten by stable ID and opens a detailed inspector that resolves richer state on demand.

Do not copy all personal history into every card. Use a compact summary for the overview and fetch/compose detailed capabilities only for the selected entity. Selection is an inspector state, not a second Digiten instance.

## One world, multiple projections

The plan view, aerial view, street/FPS view, and historical view must resolve the same building, room, door, street, district, and Digiten IDs. Different views are projections over shared world data, not separately invented scenes. Buildings should carry history and reasons for their shape—parcel constraints, additions, renovations, usage changes, infrastructure, and local culture—so neighborhoods can grow through understandable causes rather than a reusable sci-fi template.

## Iteration sequence

1. **Schema and example** — implemented as the renderer-neutral plan contract and compact apartment.
2. **Plan canvas first pass** — implemented at `/architecture`: selectable room rectangles, dimension editing, fixtures, walls, and semantic opening symbols. Edits are local to the page session and room edits do not yet reshape associated walls.
3. **Clipboard terminal + first proposal loop** — the WebPage exposes the two-button clipboard terminal in navigation and within Singularity Architecture. The architecture context includes the current plan and design brief; imported JSON is checked by the plan schema and requires an explicit Apply action. ProtocolAi symbol resolution and full GrammarAi validation are not yet connected.
4. **Persistent editing** — coordinated room/wall edits, undo/redo, saved projects, and validation feedback.
5. **Architect role** — first use the clipboard round trip; optionally add a HeadlessAi-backed role when explicitly configured. Both paths must feed the same validated proposal contract.
6. **Digiten cards** — compact data cards linked to persistent Digiten IDs; selection opens a detailed inspector.
7. **City placement** — place building footprints on actual parcels/blocks and expose plan/building/street projections.
8. **Calibration and simulation** — connect occupancy, jobs, schools, schedules and circulation to the population model only when shared identities and world data support it.

## Performance guardrails

- Do not rebuild every plan and every agent card when one selection changes.
- Separate immutable/shared geometry from per-frame actor state.
- Use stable IDs and incremental updates.
- Measure time spent in simulation, state resolution, SVG generation, Blazor rendering, and browser paint separately.
- Benchmark at 1×, 10×, and 100× city scale before claiming the architecture has scaled.

## Current boundary

The plan schema, example, interactive plan studio, generalized organization/project data contract, two-button clipboard terminal, and first strict-JSON proposal review loop are real code. Imported proposals are checked by the current BuildingPlan schema and require explicit application, but semantic ProtocolAi resolution and full GrammarAi validation are not yet connected. Persistence, coordinated room/wall editing, live employment simulation, interactive Digiten cards, city-wide multi-view projections, and an optional HeadlessAi Architect role remain future integration work. The plan bundle provider is a WebPage demonstration adapter, not yet a separately published MicroBundle package.
