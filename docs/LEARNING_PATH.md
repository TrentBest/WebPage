# WebPage Learning Path

The Workshop should not greet a new visitor with the entire ecosystem at once. It should let curiosity pull the next layer into view.

The intended experience is a progression, not a conventional website menu:

~~~text
MONIKER
   ↓ one clear invitation
WELCOME — what problem are we solving?
   ↓ show me
PROOF — let me see something real
   ↓ how does it work?
UNDERSTAND — FSM_API + FSM_COS + MicroBundles
   ↓ let me try
CREATE — use the ideas, not just read about them
   ↓ what can this become?
SINGULARITY CITY — Digitens and a living-world direction
   ↓ reveal the wider system
ECOSYSTEM — Gestures, Physics, Renderer, Visualize, ProtocolAi, and more
~~~

One WebPage host. One Workshop identity. One navigable experience whose depth increases as the visitor chooses to continue.

## Non-negotiable compatibility

- **Keep `/about-us` stable.** Existing external links, including the Unity package, depend on this URL. Do not rename, redirect, remove, or make the page dependent on hub progression.
- Keep the existing About Us page and its content intact while the landing experience evolves.
- About Us is an always-available escape hatch, not a progression gate. The hub can present it as a quiet utility link rather than one of the primary discovery tabs.
- A direct URL remains directly addressable. Progressive disclosure controls what the hub advertises next; it is not access control.
- The animated Workshop moniker remains the persistent home/return affordance.
- Keep one host and one composition boundary. Do not create a second hub for each new level of the story.

## Visitor track

### 0 — Witness the arrival

The animated moniker is the first impression. Give the visitor room to notice it. Once the hub arrives, offer one obvious primary action: **Welcome**.

Do not display every destination at once. Keep About Us reachable by its stable URL and through a low-emphasis utility link.

### 1 — Welcome: enter the Workshop

Welcome is a place, not another text-heavy page. Selecting it collapses the hub navigation and opens a non-scrolling, top-down blueprint of the dragon-warrior-inspired Workshop. The visitor explores the Forge, assembly benches, observatory, world globe, and cartographers’ drafting wing by hovering, focusing, or selecting rooms.

Room explanations should distinguish a working facility from a conceptual workflow. The Forge opens the existing live FSM_API lab. The observatory-to-globe-to-drafting path is currently a design direction, not a completed celestial or terrain editor. Optional browser speech introduces the Workshop and explains inspected rooms; authored opening SFX must be coordinated with the sound owner and Physics rather than faked in WebPage.

The floor plan is the first step toward a composition-driven Workshop: installed MicroBundles may contribute facilities when an actual provider contract exists. About Us remains outside this progression at its stable /about-us URL.

### 2 — Proof: show, do not merely promise

Introduce a small number of source-backed demonstrations. Prefer one compelling, working vertical slice over a large catalogue of disconnected links.

Every proof surface labels what is **working**, **experimental**, or **planned**. A screenshot, SVG, conceptual diagram, CPU projection, or GPU result must be identified accurately. Link to source and tests where they substantiate the claim.

After a visitor inspects proof, reveal the deeper explanation.

### 3 — Understand: reveal the architecture

Teach the responsibility boundaries in the order a newcomer needs them:

1. **FSM_API** — behavior and lifecycle.
2. **MicroBundles** — focused capabilities.
3. **FSM_COS** — composition and dependency closure.
4. **WebPage** — the host that presents the assembled Experience.

Explain the reason for each boundary before introducing its vocabulary. The existing [About Us page](/about-us) remains the stable, directly addressable overview for people who want the broader explanation immediately.

### 4 — Create: let the visitor participate

Once the visitor understands the premise, expose hands-on creation and inspection surfaces. Make the next step an action: change a parameter, inspect a state, compose a capability, or run a verified demonstration.

Do not imply a tool is runnable when it is only a source explanation.

### 5 — Singularity City: make the consequences visible

Only after the visitor has context should the hub introduce the larger world: places, buildings, Digitens, services, movement, and economy.

Start with one convincing, inspectable slice. Expand the world by composition rather than presenting every future system as already implemented.

### 6 — The ecosystem: let curiosity fan outward

Deeper exploration can reveal Gestures, Physics, Renderer, Visualize, ProtocolAi, GrammarAi, additional hosts, and more. These are connected branches of the same Workshop, not unrelated marketing tabs.

Each deeper surface should answer: what does it add, which package owns it, what can I witness today, and what remains unfinished?

## How navigation should behave

- At each stage, show one primary next step. Keep previously discovered destinations available without restoring the full menu explosion.
- Reveal destinations after an explicit, meaningful visitor choice—not after a timer, scroll position, or arbitrary click.
- Use transitions that feed or fade the next layer into view. Avoid forcing visitors through a tall document or a maze of pages.
- The moniker returns to the Workshop home. About Us remains available independently.
- Do not invent artificial locked content. The progression is a way to pace discovery, not a punishment for skipping ahead.
- A visitor who already knows the system should still be able to use direct routes and links.

## Developer track

### Lesson 1 — FSM_API

Start with the FSM_API repository and its published package. Learn FSM definitions, instances, processing groups, contexts, lifecycle, and deferred execution. WebPage should consume this behavior rather than reimplement it.

### Lesson 2 — FSM_COS

Read [FSM_COS_USAGE.md](FSM_COS_USAGE.md). The host supplies a root RuntimeManifest. FSM_COS resolves dependency closure, loads and arbitrates MicroBundles, and produces a RuntimeAssembly. WebPage presents the result; it must not duplicate dependency traversal.

### Lesson 3 — MicroBundles

Read [EXPERIENCE_THEORY.md](EXPERIENCE_THEORY.md) and [MICROBUNDLE_ARBITRATION_MAP.md](MICROBUNDLE_ARBITRATION_MAP.md). A MicroBundle is a focused composition participant. It declares dependencies and participates in loading and arbitration. A package and a MicroBundle are related concepts, but they are not interchangeable labels.

### Lesson 4 — Experience

An Experience describes the environment being requested. The host manifest distinguishes startup presentation Experiences from the primary running Experience. A moniker is not automatically the primary Experience.

### Lesson 5 — WebPage

Study `WorkshopExperienceService`, `WorkshopCompositionCatalog`, `Pages/Home.razor`, `Layout/MainLayout.razor`, and the concrete Living GUI manifestation only after the runtime boundaries are understood.

## Repository archaeology

There is old code in this repository. Do not use chronology as architecture.

When you encounter a suspicious local implementation:

1. find the behavior;
2. find its test;
3. identify the canonical package/domain owner;
4. determine whether WebPage still needs an adapter;
5. migrate only after equivalent proof exists;
6. remove the duplicate implementation.

## The central lesson

~~~text
WHAT
  Experience

WHAT CAPABILITY
  MicroBundle

HOW TO COMPOSE
  FSM_COS

HOW TO BEHAVE
  FSM_API

WHERE TO PRESENT
  WebPage / another host

WHERE TO STORE OR DISCOVER
  Repository / provider boundary
~~~

WebPage is valuable because it demonstrates these boundaries in one working host. It is not valuable because every piece of its current source code should become a pattern for other applications.

*WITNESS → WONDER → WELCOME → PROOF → UNDERSTAND → CREATE → SINGULARITY CITY → ECOSYSTEM*
