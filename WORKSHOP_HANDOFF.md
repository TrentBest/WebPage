# Workshop Handoff — September 2026

This file exists so a new engineering/AI session can begin with the current state instead of reconstructing the recent conversation.

## Repository

`TrentBest/WebPage`

Active branch:

`feature/living-workshop-landing`

**Master is not the working branch and must remain untouched.**

## What just happened

The landing experience has been evolving from a normal webpage into a living demonstration of The Singularity Workshop.

The most recent work added an AI tab and tightened the landing/Pong transition.

Recent commits of interest:

- `35ccf5d...` — restored the original living/chaotic landing behavior.
- `bad8e62...` — allowed full-range paddle drag and continuous Pong ticks.
- `e11e119...` — made the landing use the full viewport and immediately dismiss/remove Pong when the landing is clicked.
- `e53d3d...` — added the Workshop AI navigation item.
- `5268079...` — created the `/ai` architecture page.
- This handoff was added after those changes so the next session has explicit context.

Use Git history to obtain the exact full SHAs/diffs when needed.

## Current landing intent

The landing is supposed to feel like the visitor has stumbled into an experiment.

The intended sequence is:

```text
NORMAL-LOOKING WORKSHOP
        |
        | wait 10 seconds without clicking
        v
      PONG
        |
        | visitor can drag cyan paddle
        | magenta paddle is autonomous
        | FSM_API drives the state
        |
        | click warning or Enter Workshop
        v
PONG DISAPPEARS
        |
        v
LIVING GUI
        |
        | GUI actors reproduce
        | generations / lineage become visible
        | population explodes
        | critical mass
        | freeze / collapse / gravity
        v
UNITY WEBGL
        |
        v
WORKSHOP / FORGE
```

The key transition bug recently addressed was that the living GUI could be visible behind Pong after clicking Enter Workshop. `MainLayout.razor` now owns `_pongActive` and conditionally renders `<PongField>` so the component is actually removed instead of merely being told to stop.

The other layout issue was that the living GUI looked shifted into the old content panel. The landing should occupy the **entire browser viewport**. That is intentional: the chaos is supposed to rapidly overflow the visitor's visual field.

## Current Pong implementation

File:

`TheSingularityWorkshop/Components/PongField.razor`

Important facts:

- FSM-backed lifecycle: Dormant / Manifesting / Playing / Stopped.
- 50 ms heartbeat.
- Cyan player paddle.
- Magenta AI paddle.
- Initial serve goes toward the AI.
- Score is maintained.
- Player input is pure Blazor pointer handling.
- No JavaScript is needed for paddle dragging.
- A drag surface temporarily captures pointer movement so the player paddle can reach the useful vertical range.
- Underlying landing controls must remain clickable.

Do not turn Pong into a separate unrelated game project. It exists as a demonstration of the deterministic machine driving an interactive manifestation.

## Current living GUI

File:

`TheSingularityWorkshop/Pages/Home.razor`

The existing chaos implementation is valuable and should be preserved.

It already has the concept of GUI nodes with:

- identity
- generation
- parent/lineage
- lifecycle state
- position
- scale
- opacity
- rotation

The behavior includes reproduction, critical mass, freeze, and collapse/falling.

The current task is not to replace this with a new animation. The goal is to expose and improve the architecture while keeping the weirdness.

## Immediate next landing task: scrolling code-derived feed

The visitor should be told what is happening without receiving a conventional instruction manual.

A scrolling feed/list should be populated from **truths derived from the codebase**, for example:

```text
WAIT 10 SECONDS WITHOUT CLICKING.
PONG WILL START.

PONG IS RUN BY FSM_API.

CYAN = VISITOR.
MAGENTA = MACHINE.

THE FIRST SERVE BELONGS TO THE MACHINE.

THE MACHINE KEEPS SCORE.

CLICK THE WARNING OR ENTER THE WORKSHOP.

PONG WILL DISAPPEAR.

THE INTERFACE CONTINUES.

THE GUI CAN REPRODUCE.

EVERY NODE KNOWS ITS GENERATION.

EVERY NODE CARRIES LINEAGE.

POPULATION REACHES CRITICAL MASS.

THE MACHINE FREEZES.

THE MACHINE FALLS APART.

THIS IS ONLY THE FRONT DOOR.
```

These should not become random marketing copy. The longer-term goal is a real observable `WorkshopExperience`/fact model that can expose state and behavior to the GUI itself.

## AI tab

Route:

`/ai`

File:

`TheSingularityWorkshop/Pages/AI.razor`

The page currently presents:

```text
LLM
 |
 v
PROTOCOL AI
 |
 v
GRAMMAR AI
 |
 v
COMMAND AI
 |
 v
APP AI
```

Concepts:

- **Protocol AI** — what can the machine say?
- **Grammar AI** — how may it be assembled?
- **Command AI** — what does the machine actually do?
- **App AI** — what if the application itself is the artifact?

App AI is intentionally described as an emerging construct, not as a falsely completed system.

The next AI task is to make this page **come alive with actual Workshop machinery** rather than leave it as a static architecture brochure.

The central research problem is:

> How do you allow probabilistic intelligence to operate a deterministic machine without letting ambiguity leak through the boundary?

## Integer-backed FSM / mapping work

The separate FSM_API 2.0 work is moving toward integer-backed state/identity alongside the existing string-backed behavior.

The important conceptual mapping is:

```text
LLM-visible vocabulary
        |
        v
keyword / semantic mapping
        |
        v
table / grammar mapping
        |
        v
integer machine representation
```

The LLM should be able to understand the human-readable vocabulary, but generated machine commands should use compact integer identities wherever the protocol permits. Actual string-valued data remains string data.

Do not invent exact APIs. Inspect the FSM_API 2.0 branch before implementing against it.

## Architecture philosophy

The Workshop is trying to demonstrate itself with itself.

Blazor is a manifestation boundary, not the architectural destination.

The larger direction is:

```text
SEMANTICS
   |
   v
STATE / IDENTITY
   |
   v
DETERMINISTIC RUNTIME
   |
   +--> Blazor/WebAssembly
   +--> Unity/WebGL
   +--> WPF / other hosts
```

MicroBundles, FSM_API, SingularityWarehouse, ontology, mappings, and AI command infrastructure are different experiments around the same larger question.

## GUI Forge direction

Eventually the Workshop should demonstrate software constructing software:

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

Possible future demonstrations include thousands of lightweight GUI actors, culling, towns as buttons, and a Dragon Warrior-like GUI/tile world.

These are targets, not claims of current implementation.

## Important style constraints

Keep:

- cyan/magenta technical energy
- gold/orange warning energy
- breathing controls
- restrained scan-line/distortion atmosphere
- mundane GUI becoming strange
- playful technical writing
- "Dr. Seuss for software developers" energy

Avoid:

- generic SaaS landing-page redesigns
- replacing the existing chaos because it looks odd
- gratuitous JavaScript for interactions Blazor can handle
- fake telemetry
- pretending future architecture is already implemented
- unverified "first ever" claims

## Navigation warning

`NavMenu.razor` currently contains Home, AI, Demos, Privacy Policy, Vote on Name, About, and LoginStatus.

Earlier versions also contained important Booking and Unity Asset Store/package links. Verify the history/current desired navigation before rewriting this file; do not accidentally delete useful existing destinations while working on AI.

## Files worth opening first

1. `AGENTS.md`
2. `TheSingularityWorkshop/Pages/Home.razor`
3. `TheSingularityWorkshop/Components/PongField.razor`
4. `TheSingularityWorkshop/Layout/MainLayout.razor`
5. `TheSingularityWorkshop/Layout/NavMenu.razor`
6. `TheSingularityWorkshop/Pages/AI.razor`
7. `TheSingularityWorkshop/Workshop/MicroBundles/README.md`
8. root `README.md`

Then inspect relevant Git history before making a structural change.

## The immediate mission

Do not start by rebuilding the site.

First finish the landing experience:

1. Verify Pong disappears completely on the intended landing interaction.
2. Verify the living GUI occupies the full browser viewport and is centered relative to that viewport.
3. Add the scrolling code-derived behavior feed.
4. Keep the existing chaos behavior.
5. Then make the AI tab interactive/observable using real Workshop constructs.

The point is not to make a pretty webpage.

The point is to make the webpage **evidence that the machinery works**.

> **Use the technology to build the technology.**

*This way leads to the Singularity.*
