# The Singularity Workshop — Publisher WebPage

## What this repository is

This repository is the public-facing web surface for **The Singularity Workshop**.
It is not intended to be a conventional marketing site.

The Workshop is a place where the software itself is part of the message:
state becomes behavior, behavior becomes visible, and the boundary between a
web application and a native/engine runtime becomes something the visitor can
actually experience.

The guiding idea is simple:

> **Make the mundane magical. Then show the machinery that made it possible.**

The long-term direction is **Forging Software for the Singularity**: deterministic
software infrastructure, state machines, data/ontology systems, AI-facing command
boundaries, tooling, and runtime experiments that can cross manifestation domains.

---

## Current status — September 2026

### Working

- Blazor WebAssembly publisher site.
- FSM_API integration and FSM-oriented infrastructure.
- Unity WebGL build hosted by the Workshop site.
- Workshop/Forge assets are present under `wwwroot/Workshop`.
- MicroBundle infrastructure exists as a small, independently schedulable behavior
  abstraction.
- The landing experience has been restored to the intentionally theatrical
  **living software** direction.

### Landing experience

The home page is deliberately unusual.

1. A visitor encounters a large **ENTER THE WORKSHOP** control rather than a
   conventional hero/marketing section.
2. The control occupies roughly 50% of its containing surface in the desktop
   experience.
3. The Workshop environment breathes, scans, glows, and contains small ambient
   signals rather than sitting motionless on a blank page.
4. The entry control uses Trent's public GitHub avatar as the initial living
   identity surface.
5. Hovering the control increases its energy and scale and drives the animated
   border treatment.
6. Entering the Workshop releases the first recognizable GUI-like software node.
7. Nodes grow, reproduce, carry generation/lineage information, and multiply
   exponentially until critical mass.
8. The swarm freezes and falls away, exposing the runtime boundary.
9. The experience transitions toward the Unity WebGL proof-of-concept gateway.
10. The gateway leads into the Showcase while the Forge remains explicitly marked
    as infrastructure under construction.

The point is not decoration. The page is itself a demonstration that ordinary
software controls can become a living system.

### Important visual rule

Do **not** flatten the landing page into a generic hero section.

The cyan/magenta instrumentation, scan lines, orbit traces, warning panel,
animated border energy, breathing typography, sparks/signals, and recognizable
GUI nodes are intentional parts of the Workshop's identity.

If a future change makes the page look like a normal SaaS landing page, that is
probably a regression.

---

## Architectural direction

The Workshop is being built around a layered model:

```text
Human intent
     |
     v
AI / command boundary
     |
     v
Grammar / ontology
     |
     v
Deterministic command representation
     |
     v
FSM_API / runtime state
     |
     +--------------------+
     |                    |
     v                    v
 Blazor/Web            Unity/WebGL
 manifestation         manifestation
```

The important architectural distinction is between **what is happening** and
**how it is manifested**.

That is why MicroBundles do not own UI behavior. They carry lifecycle/state
information and delegate manifestation to providers. See
`TheSingularityWorkshop/Workshop/MicroBundles/README.md` for the current model.

The next major evolution is integer-backed identity and ontology metadata. Human
readable strings remain valuable at authoring/documentation boundaries, while
runtime and AI transport should increasingly operate on compact integer identities.

This repository should remain an experimental proving ground for that architecture.

---

## Development rules / guideposts

### Preserve the behavior before refactoring the implementation

The Workshop has accumulated visual and behavioral experiments through iteration.
Before replacing a component, inspect its history. A newer-looking implementation
is not automatically a better implementation if it removes behavior that was
already demonstrating the intended concept.

### Do not optimize the magic away

The theatrical effects are not disposable polish. They are part of the UX and the
technical story. Keep them understandable and reasonably cheap, but preserve the
sense that the interface is alive.

### Prefer deterministic state over animation spaghetti

Animation may be CSS, DOM, Blazor, or Unity, but meaningful lifecycle transitions
should have explicit states and clear ownership. The eventual goal is to let
`FSM_API` own those transitions rather than scattering behavioral decisions across
timers and event handlers.

### Keep manifestation separate from semantics

A behavior should be expressible without assuming that its output is HTML, CSS,
WPF, Unity, WebGL, audio, or speech.

### Document the strange parts

If something looks unnecessarily weird, assume there is a reason until the history
and architecture have been checked. Add a comment or README note when introducing
behavior that future developers could otherwise mistake for accidental complexity.

### Never use a successful build as proof that the architecture is correct

The Workshop exists partly to make architecture observable. A feature should be
judged by lifecycle clarity, determinism, portability, performance, and whether the
resulting behavior can be explained.

---

## Project architecture

- **Technology:** Blazor WebAssembly / .NET
- **Core state technology:** `FSM_API`
- **Web integration:** `FSMManagerService`, `BlazorFSMIntegration`, MicroBundles
- **Engine boundary:** Unity WebGL
- **Deployment target:** static hosting such as Azure Static Web Apps
- **Primary repository:** `TrentBest/WebPage`
- **Core FSM repository:** `TrentBest/FSM_API`

The site also contains demonstrations and infrastructure for the Workshop's FSM,
editor, runtime, and Unity integration work.

---

## Local development

```bash
git clone https://github.com/TrentBest/WebPage.git
cd WebPage
dotnet restore
dotnet run
```

The application will launch on the HTTPS/HTTP port selected by the project's
launch settings.

For active Workshop development, work from the feature branch being developed and
keep `master` untouched unless a change is intentionally being promoted.

---

## The larger Workshop

The web page is only one manifestation of a much larger body of work:

- FSM_API and deterministic state-driven runtime infrastructure
- integer-backed state/identity experiments
- SingularityWarehouse and indexed data infrastructure
- ontology and string-to-integer mapping
- MicroBundles and provider-based manifestation
- AI command/grammar experiments
- Unity integration and WebGL runtime boundaries
- developer tooling and visual architecture exploration
- performance measurement and benchmarking
- C# systems engineering across unusual runtime boundaries

The website is where these ideas become visible.

---

## Writing / research direction

The Workshop should document the journey as it is being built rather than waiting
for a mythical "finished" version.

Future technical writing should cover subjects such as:

- SingularityWarehouse architecture and why it exists
- integer-backed ontology and mapping
- deterministic AI command boundaries
- Grammar / ProtocolAI / CommandAI evolution
- MicroBundles and manifestation domains
- FSM_API performance and design decisions
- Unity/WebGL hosting experiments
- rebuilding lost systems better than the originals
- the engineering lessons hiding inside seemingly ridiculous experiments

The goal is technical writing that is rigorous without becoming sterile —
**Dr. Seuss for software developers**: playful enough to invite people in, precise
enough that the machinery underneath can survive inspection.

---

*This way leads to the Singularity.*

*Built by The Singularity Workshop.*
