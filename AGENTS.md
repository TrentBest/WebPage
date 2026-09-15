# The Singularity Workshop — Agent Instructions

This repository is an active engineering experiment, not a conventional marketing site.

## Branch safety

- The active vertical-slice landing work is on `feature/pong-microbundle-vertical-slice`.
- **Do not modify `master`.**
- Keep changes narrowly scoped to the current experiment.
- Before replacing an existing behavior, inspect its Git history. Strange code may be carrying intentional behavior from an earlier experiment.

## What we are building

The WebPage is becoming a living front door to **The Singularity Workshop**.

The guiding rule is:

> **Use the technology to build the technology.**

The site should demonstrate the Workshop's architecture through its own behavior rather than merely describing it.

The larger architecture connects:

```text
FSM_API
   +
SingularityWarehouse
   +
Workshop developer tooling
   |
   v
Deterministic semantics
   |
   v
Protocol AI / Grammar AI / Command AI
   |
   v
Manifestation
   +---- Blazor / WebAssembly
   +---- Unity / WebGL
   +---- future hosts
```

## Current landing experience

The landing page is deliberately an experience rather than a static hero.

Current intended sequence:

```text
GATEWAY
  |
  | visitor clicks Enter Workshop
  v
GATEWAY_EXIT
  |
  v
LIVING_GUI_IGNITION
  |
  | create one centered seed
  v
LIVING_GUI_POPULATING
  |
  | reproduce / fly / grow
  | NO MONIKER
  v
100 GUI NODES / CRITICAL MASS
  |
  | freeze
  v
MONIKER_REVEAL
  |
  | sequencing boundary only
  | NO VISIBLE MONIKER FRAME
  v
GRAVITY
  |
  | visible "The Singularity Workshop"
  | 91 FSM heartbeats ~= 3 seconds
  v
NAVIGATION_ARRIVAL
  |
  v
RUNNING
```

The detailed executable-vision contract lives in `WORKSHOP_PRESENTATION_CONTRACT.md`.

### Moniker timing is a hard UX contract

The phrase **The Singularity Workshop** must not dominate first contact.

The gateway presents the visitor with the large avatar-backed `Enter Workshop` control. The living GUI then earns the reveal by reaching exactly 100 nodes and freezing. `MONIKER_REVEAL` is an FSM sequencing boundary; the visual moniker is rendered only once `GRAVITY` begins. Gravity owns the approximately three-second presentation window.

Do not add the moniker to the initial gateway simply because it appears in a page title, accessibility label, source filename, or browser metadata. Those are not the visual presentation contract.

## Important UX facts

- Hovering/mouse movement must not reset or pause the landing timer.
- Clicking the warning or Workshop button is an explicit interaction that dismisses the Pong experience.
- Pong must disappear **before** the living GUI is presented. The current `MainLayout.razor` therefore removes the `<PongField>` component instead of relying only on its internal FSM stop state.
- The landing surface is intended to use the **entire browser viewport**, not the old content column. The point of the chaos is to rapidly overflow the visitor's visual field with living GUI.
- Do not replace the existing chaos/living GUI with a generic animation. Preserve the behavior and improve its architecture.

## PageFSM / FSM_API ownership

`PageFSM` is a wrapper around one FSM_API live handle. FSM_API stores definitions and instances in process-global processing groups.

Therefore:

- `PageFSM.Update()` advances **its own handle**.
- It must not call `Interaction.Update(PageFSM.ProcessingGroup)` merely to advance one page instance.
- `PageFSM.Dispose()` must unregister its live handle.
- Do not rebuild a process-global definition with instance-bound delegates and assume unrelated handles are isolated.

This is an important architectural distinction: **group-wide ticking belongs to the host/integration loop; an object wrapper's `Update()` belongs to its owned handle.**

## Tests as breadcrumbs

`SingularityHub.Tests/IdleExperienceArchitectureTests.cs` contains the executable landing breadcrumbs.

- Incremental Unit Test 04 is the canonical exact presentation sequence.
- Incremental Unit Test 07 proves disposal unregisters the runtime handle.
- Incremental Unit Test 08 proves one PageFSM's update does not advance another PageFSM instance.
- `SingularityHub.Tests/IncrementalVersionTests.cs` provides the visible incremental proof marker.

**Do not weaken Unit Test 04 to make an implementation pass. Fix the machine/lifecycle that violates the sequence.**

## Pong

`TheSingularityWorkshop/Components/PongField.razor` is an FSM_API-backed Pong experiment.

Current behavior includes:

- `Dormant`, `Manifesting`, `Playing`, and `Stopped` lifecycle states.
- A 50 ms heartbeat.
- Cyan player paddle.
- Magenta autonomous AI paddle.
- Initial serve toward the AI so the visitor has time to understand the scene.
- Score tracking.
- Pure Blazor pointer interaction for paddle dragging; do not introduce JavaScript merely to implement dragging.
- A temporary full-field drag surface while the player is dragging, allowing the paddle to reach the useful vertical range.
- Underlying landing controls remain clickable while Pong is visible.

The important architectural lesson is that Pong is not merely a game bolted onto the page. It is a small demonstration that deterministic state machinery can drive an interactive manifestation.

## Living GUI / chaos

`TheSingularityWorkshop/Pages/LivingGui.razor` is the dedicated living GUI presentation surface used by the landing sequence.

The model includes nodes carrying:

- identity
- generation
- lineage
- position
- scale
- opacity
- rotation
- seed-flight state

The runtime behavior includes reproduction, exact critical mass at 100 nodes, freeze, and gravity/collapse.

The visual intent is deliberately excessive:

> **ordinary GUI -> GUI with a pulse -> GUI that reproduces -> swarm -> critical mass -> freeze -> gravity**

Do not replace this with a generic animation.

## Scrolling experience feed

The landing should eventually expose a scrolling list/feed whose text is derived from actual code behavior rather than generic marketing copy.

Truthful observations include:

- Pong is powered by FSM_API.
- The visitor can interact with the Pong paddle.
- The living interface can reproduce.
- Nodes carry generation and lineage.
- The population reaches critical mass.
- The machine freezes and then enters gravity.
- The Workshop moniker appears only after the critical-mass boundary.

Long-term, prefer a real observable/data model over a pile of hard-coded marketing strings. The GUI should be able to observe the machine it is demonstrating.

## AI tab

`TheSingularityWorkshop/Pages/AI.razor` is the `/ai` route.

The conceptual pipeline presented there is:

```text
LLM
 |
 v
PROTOCOL AI   -- What can the machine say?
 |
 v
GRAMMAR AI    -- How may it be assembled?
 |
 v
COMMAND AI    -- What does the machine actually do?
 |
 v
APP AI        -- What if the application itself is the artifact?
```

The AI page is currently an architectural presentation, not a claim that every layer is already implemented as a production system.

The next evolution is to make this tab **come alive using actual Workshop constructs**: real state, context, commands, mappings, execution, and observability instead of a static brochure.

The key research question is:

> **How do you allow probabilistic intelligence to operate a deterministic machine without letting ambiguity leak through the boundary?**

Do not make unsupported claims such as being the first person/project ever to implement an idea. Demonstrate the architecture and document its actual lineage instead.

## Integer-backed FSM / AI mapping direction

The FSM_API 2.0 work is adding integer-backed state/identity alongside the established string-backed behavior.

The important idea is:

```text
Human-readable concept
        |
        v
String identity / documentation
        |
        v
Mapping
        |
        v
Integer identity
        |
        v
Deterministic runtime / command stream
```

For AI-facing grammar, the intended trick is that the LLM can read the human-readable mapping and understand that a token is a keyword, while the generated machine representation can assemble integer identities rather than repeatedly emitting strings. String values still exist where the actual semantic value must be a string.

The next agent should inspect the actual FSM_API 2.0 branch/code before asserting exact API details.

## GUI Forge direction

The longer-term destination is a GUI Forge where the system demonstrates its own ability to construct applications/interfaces from reusable deterministic constructs.

A useful progression is:

```text
Field
 -> Pong
 -> Living GUI
 -> Unity
 -> Workshop
 -> FSM telemetry
 -> Simulation Lab
 -> GUI Forge
 -> Massive World
```

Examples discussed include thousands of lightweight GUI/tree actors, culling, towns as buttons, and a Dragon Warrior-like GUI/tile world. These are design targets, not necessarily implemented features.

## Visual language

Preserve the current Workshop personality:

- cyan/magenta instrumentation for Pong and technical state
- gold/orange warning energy
- breathing controls and borders
- subtle distortion and scan-line atmosphere
- glowing but restrained signal details
- playful, weird, technically literate tone

The visual goal is **mundane magical**, not generic sci-fi decoration.

## Navigation

The AI tab exists and should remain.

Be careful when editing `NavMenu.razor`: earlier navigation included important links such as Booking and Unity Asset Store/package destinations. Do not silently remove existing useful navigation while adding AI. Inspect the current file before rewriting it.

## Documentation style

Documentation should be:

- technically honest
- useful to a developer arriving cold
- explicit about what is implemented versus experimental
- playful without becoming vague
- written so the next agent can continue without reconstructing the entire conversation

The Workshop likes the phrase:

> **Dr. Seuss for software developers.**

## Operational rule

When a task is ambiguous, first inspect the current repository state and recent history. Then make the smallest coherent change that advances the current experiment.

Do not rebuild the site from scratch because a component looks unusual.

Do not optimize away the behavior that makes the Workshop interesting.

Do not modify `master` during this experimental phase.
