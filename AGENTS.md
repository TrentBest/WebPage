# The Singularity Workshop — Agent Instructions

This repository is an active engineering experiment, not a conventional marketing site.

## Branch safety

- The active landing-page work is on `feature/living-workshop-landing`.
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
LANDING
  |
  | wait ~10 seconds without clicking
  v
PONG
  |
  | FSM_API-driven game state
  | cyan player paddle / magenta AI paddle
  | initial serve toward AI
  | drag player paddle with pointer
  |
  | click Warning or Enter Workshop
  v
PONG COMPONENT IS REMOVED
  |
  v
LIVING GUI / CHAOS
  |
  | GUI actors reproduce
  | generations and lineage exist
  | population reaches critical mass
  | system freezes / collapses
  v
UNITY WEBGL / NEXT MANIFESTATION
  |
  v
WORKSHOP
```

Important UX facts:

- Hovering/mouse movement must not reset or pause the landing timer.
- Clicking the warning or Workshop button is an explicit interaction that dismisses the Pong experience.
- Pong must disappear **before** the living GUI is presented. The current `MainLayout.razor` therefore removes the `<PongField>` component instead of relying only on its internal FSM stop state.
- The landing surface is intended to use the **entire browser viewport**, not the old content column. The point of the chaos is to rapidly overflow the visitor's visual field with living GUI.
- Do not replace the existing chaos/living GUI with a generic animation. Preserve the behavior and improve its architecture.

## Pong

`TheSingularityWorkshop/Components/PongField.razor` is an FSM_API-backed Pong experiment.

Current behavior includes:

- `Dormant`, `Manifesting`, `Playing`, and `Stopped` lifecycle states.
- A 50 ms heartbeat driving the FSM.
- Cyan player paddle.
- Magenta autonomous AI paddle.
- Initial serve toward the AI so the visitor has time to understand the scene.
- Score tracking.
- Pure Blazor pointer interaction for paddle dragging; do not introduce JavaScript merely to implement dragging.
- A temporary full-field drag surface while the player is dragging, allowing the paddle to reach the useful vertical range.
- Underlying landing controls remain clickable while Pong is visible.

The important architectural lesson is that Pong is not merely a game bolted onto the page. It is a small demonstration that deterministic state machinery can drive an interactive manifestation.

## Living GUI / chaos

`TheSingularityWorkshop/Pages/Home.razor` contains the living landing experiment.

The existing model includes node lifecycle states such as:

```text
Seed -> Living -> Reproducing -> Falling -> Dead
```

and experience phases including the introductory/critical-mass/collapse progression.

Nodes carry identity, generation, parent/lineage information, position, scale, opacity, and rotation.

The visual intent is deliberately excessive:

> **ordinary GUI -> GUI with a pulse -> GUI that reproduces -> swarm -> critical mass -> collapse**

The current layout has been moved toward full viewport use. If positioning still appears confined to the old panel/content column, fix the coordinate system rather than shrinking the experiment back into the panel.

## Scrolling experience feed

The next immediate landing enhancement is a scrolling list/feed whose text is derived from actual code behavior rather than generic marketing copy.

The feed should indirectly/directly teach the visitor what is about to happen. Examples of truthful observations include:

- Wait 10 seconds without clicking a button: Pong starts.
- Pong is powered by FSM_API.
- The cyan paddle is controlled by the visitor.
- The magenta paddle is autonomous.
- The first serve is toward the AI.
- The machine maintains a score.
- Clicking the warning or Workshop button ends the Pong phase.
- The living interface can reproduce.
- Nodes carry generation and lineage.
- The population reaches critical mass.
- The machine freezes and collapses.
- Another manifestation can take over.

Long-term, prefer a real observable/data model over a pile of hard-coded marketing strings. The GUI should be able to observe the machine it is demonstrating.

## AI tab

`TheSingularityWorkshop/Pages/AI.razor` is now the `/ai` route.

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
