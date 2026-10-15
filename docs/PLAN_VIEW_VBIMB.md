# VBIMB — Virtual Building Information Model Builder

## Purpose

VBIMB is the Workshop's proposed fast path from intent to a coherent, inspectable building plan. It is not a static floor-plan picture. A plan is structured data: rooms, walls, openings, fixtures, circulation, and the relationships that make the geometry meaningful.

The first implementation slice lives in WebPage as a host-side MicroBundle capability and an interactive Singularity Architecture plan studio at `/architecture`. The studio draws the renderer-neutral plan model, lets you select rooms, edit room rectangles, toggle opening state, and capture a structured design brief. The current edits are in-memory only; it does **not** yet persist projects, register a new bundle in FSM_COS, call an LLM, coordinate room edits with wall geometry, provide undo/redo, or claim compliance with building codes.

## First plan vocabulary

- **BuildingPlan** — stable identity, coordinate extents and collections of plan elements.
- **PlanRoom** — bounded area with name and use.
- **PlanWall** — segment with thickness.
- **PlanOpening** — a semantic opening attached to a wall, with offset and width.
- **PlanFixture** — a piece of built-in or movable plan content.
- **PlanOpeningKind** — hinged door, sliding door, pocket door, revolving door, archway, or window.

Opening semantics are separate from drawing style. A renderer can draw a hinged door's leaf and swing arc, a sliding door's panel and travel direction, a pocket door disappearing into a wall, or a revolving door's radial leaves. A closed opening, open fraction, hinge, and swing direction are data; the visual glyph is a projection of those data.

The initial schema validates unique IDs, positive plan extents, in-bounds room and fixture rectangles, wall references, positive opening widths, and open fractions in the range 0–1. It is a sketching contract, not a structural, fire, accessibility, or permit validator.

## The organization and asset substrate

Singularity Architecture is a Workshop destination, but the organization model is intentionally generic. An architecture firm is an organization profile; the same data shape can represent a school, manufacturer, retailer, studio, or civic service. Members reference stable Digiten IDs and role labels rather than cloning people into each company. Projects can produce typed artifacts such as a `BuildingPlan`, tool, service, or other reusable asset.

The initial `SingularityOrganization` contract supports identity, industry, optional building assignment, membership, and project/artifact references. It does not implement persistence, employment simulation, ownership policy, licensing, payment, or marketplace fees. Those policies should be layered on only after people, organizations, projects, and reusable assets have stable identities.

## How an AI-assisted sketch should work

ProtocolAi and GrammarAi should be optional authoring tools around the same plan schema, not dependencies of the plan model itself.

1. A person or agent proposes a small semantic brief: "a narrow townhouse, entry vestibule, stair, kitchen facing a courtyard, two bedrooms upstairs."
2. GrammarAi maps the vocabulary to a constrained plan grammar and typed operations.
3. ProtocolAi transports a structured request and result through the chosen provider/protocol.
4. The plan builder applies operations to a candidate plan.
5. Validation reports geometric and semantic conflicts instead of silently guessing.
6. The user accepts, edits, or rejects the result.
7. The selected plan can be projected into 2D plan, 3D building, street-level navigation, and city-scale massing using shared IDs and coordinate systems.

An AI suggestion is not truth. The schema and validation results are the authority for the properties they actually check. Door clearance, circulation, occupancy, structure, egress, and code requirements need separate explicit rules before a plan can be called buildable.

## Agent cards and inspect-on-select

The city plan should be a world view, not a wall of labels. Each visible Digiten gets a compact card containing stable ID, name, current role/activity, home/work/school relationship where known, and a few high-value status signals. Clicking a card selects a Digiten by stable ID and opens a detailed inspector that resolves richer state on demand.

Do not copy all personal history into every card. Use a compact summary for the overview and fetch/compose detailed capabilities only for the selected entity. Selection is an inspector state, not a second Digiten instance.

## One world, multiple projections

The plan view, aerial view, street/FPS view, and historical view must resolve the same building, room, door, street, district, and Digiten IDs. Different views are projections over shared world data, not separately invented scenes. Buildings should carry history and reasons for their shape—parcel constraints, additions, renovations, usage changes, infrastructure, and local culture—so neighborhoods can grow through understandable causes rather than a reusable sci-fi template.

## Iteration sequence

1. **Schema and example** — implemented as the renderer-neutral plan contract and compact apartment.
2. **Plan canvas first pass** — implemented at `/architecture`: selectable room rectangles, dimension editing, fixtures, walls, and semantic opening symbols. Edits are local to the page session and room edits do not yet reshape associated walls.
3. **Door behavior refinement** — hinged, sliding, pocket, revolving and window symbols are present; exact drafting conventions and clearance geometry still need refinement.
4. **Persistent editing** — next: coordinated room/wall edits, undo/redo, saved projects, and validation feedback.
5. **Digiten cards** — compact data cards linked to persistent Digiten IDs; selection opens a detailed inspector.
6. **City placement** — place building footprints on actual parcels/blocks and expose plan/building/street projections.
7. **AI authoring adapter** — structured, validated proposals from ProtocolAi/GrammarAi, with review and error reports.
8. **Calibration and simulation** — only then connect occupancy, jobs, schools, schedules and circulation to the population model.

## Performance guardrails

- Do not rebuild every plan and every agent card when one selection changes.
- Separate immutable/shared geometry from per-frame actor state.
- Use stable IDs and incremental updates.
- Measure time spent in simulation, state resolution, SVG generation, Blazor rendering, and browser paint separately.
- Benchmark at 1×, 10×, and 100× city scale before claiming the architecture has scaled.

## Current boundary

The schema, example, first interactive plan studio, and generalized organization/project data contract are real code. Persistence, coordinated room/wall editing, live employment simulation, interactive Digiten cards, AI authoring, and multi-view projections remain future work. The bundle provider is a WebPage demonstration adapter, not yet a separately published MicroBundle package.
