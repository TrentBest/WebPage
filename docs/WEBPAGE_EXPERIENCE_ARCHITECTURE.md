# WebPage Experience Architecture

The WebPage is not a conventional website with a collection of pages.

It is the **public manifestation of The Singularity Workshop**: a place where a
visitor can encounter the system, inhabit an Experience, inspect the machinery,
compose something, and eventually publish or share what they made.

The WebPage therefore has two simultaneous jobs:

1. **make the technology felt**; and
2. **make the technology understandable after it has been felt**.

The first is perception. The second is proof.

## The governing idea

> **Do not ask the visitor to believe the architecture. Give them something they can touch.**

A diagram can explain composition.

A living GUI can demonstrate state.

A spatial Experience can demonstrate world composition.

The Forge can demonstrate creation.

A rendering laboratory can demonstrate computational representation.

The source and architecture can then explain why those things behave as they do.

The visitor should be able to move between all three:

```text
              EXPERIENCE
                  |
                  v
             OBSERVATION
                  |
                  v
              EXPLANATION
                  |
                  v
              EXPERIMENT
                  |
                  v
              CREATION
```

The architecture is successful when the visitor can cross those boundaries without
leaving the Workshop.

---

## The Workshop is an entrance, not a homepage

Conventional web design often assumes:

```text
logo -> headline -> feature list -> call to action
```

That pattern is useful when the primary product is a document.

The Workshop is not primarily a document.

Its first-contact sequence is therefore allowed to be experiential:

```text
ARRIVAL
  |
  v
WITNESS
  |
  v
CURIOSITY
  |
  v
ENTER
  |
  v
INHABIT
  |
  v
UNDERSTAND
  |
  v
CREATE
  |
  v
PUBLISH / SHARE
```

The visitor is not required to understand FSM_API, MicroBundles, FSM_COS, Event
Horizons, or the repository architecture before entering.

They encounter the consequence first.

Then we show them the machinery.

---

## Four primary Workshop surfaces

The public Workshop has four primary surfaces.

They are not merely navigation labels. They describe the four relationships a
person can have with the Workshop.

| Surface | Human question | Architectural purpose |
|---|---|---|
| **Understand** | "What am I looking at?" | Explain the architecture using the real system as evidence. |
| **Experiences** | "What can I enter?" | Let the visitor inhabit working compositions. |
| **Create** | "Can I make something?" | Expose the Forge and composition vocabulary. |
| **Publish** | "Can this become mine / be shared?" | Eventually expose packaging, distribution, trust, and publishing. |

The implementation may contain additional instruments—Rendering, Education,
Madmen, laboratories, experiments, and domain-specific destinations.

Those are **content and instruments**, not competing definitions of what the
Workshop is.

This distinction prevents the navigation from becoming a list of everything we
have ever built.

---

## Experiences are destinations, not pages

An Experience is an environment composed of MicroBundles.

The visitor should not primarily think:

> "I am opening the Explore page."

They should think:

> "I am entering this place."

That distinction matters because the same Experience may eventually manifest through
WebPage, WebApp, AnyApp, MyVR, a future renderer, or another host.

The WebPage is one doorway.

```text
                    Experience
                        |
          +-------------+-------------+
          |             |             |
          v             v             v
        WebPage      WebApp      AnyApp / MyVR
        /Browser      /Desktop
```

The route is an implementation detail.

The Experience is the thing being experienced.

---

## Current WebPage behavior

The current public proving-ground sequence is deliberately narrower than the larger Workshop vision:

```text
LABEL 1
  ↓
LABEL 2
  ↓
ENTER THE WORKSHOP + advisory
  ↓
explicit visitor entry
  ↓
FSM_COS composes LIVING GUI
  ↓
Moniker dependency resolves as part of composition
  ↓
living GUI grows and reproduces
  ↓
population threshold
  ↓
gravity
  ↓
Moniker presentation
  ↓
Workshop hub / navigation
```

This repository documents that sequence because it is what the WebPage currently proves. Other world models, social spaces, default environments, or host-specific landing scenes belong to their respective products and are not part of the WebPage opening contract.

The WebPage is also **not the canonical application implementation**. It is the place where working concepts are proven in the browser before the resulting contracts and capabilities are carried into the appropriate canonical or sibling host.

## The living GUI is architectural evidence

The Workshop should deliberately contain moments where the interface behaves unlike
ordinary interface furniture.

A control may breathe.

A GUI node may reproduce.

A population may reach a threshold.

A cohort may freeze.

A representation may change.

A distant object may become cheaper without disappearing.

A hidden capability may be discovered rather than exposed as a menu item.

These are not decorative tricks.

They are **executable arguments**.

When a visitor watches a living GUI reproduce, the Workshop has demonstrated state,
identity, lifecycle, population, scheduling, and manifestation without first asking
the visitor to read a white paper.

The explanation can follow:

```text
WHAT YOU SAW
     |
     v
WHAT STATE CHANGED
     |
     v
WHAT COMPUTATION RAN
     |
     v
WHAT COMPOSED IT
     |
     v
WHAT YOU CAN BUILD WITH IT
```

---

## GUI is alive, but presentation remains a boundary

"Alive" does not mean "architecture becomes magic."

The boundary remains explicit:

```text
FSM_API
  = behavior / state

MicroBundle
  = focused capability

Experience
  = environment / composition

FSM_COS
  = runtime composition

GUI semantic model
  = what the presentation means

Blazor / Renderer / future host
  = how the presentation manifests
```

The WebPage should therefore avoid hiding meaningful behavior inside arbitrary
component event handlers, timer piles, or page-local state when that behavior
belongs to the runtime model.

The visitor may see something magical.

The code underneath should remain explainable.

---

## Event Horizons become a WebPage teaching instrument

The Renderer work gives the Workshop a particularly useful opportunity.

Instead of merely displaying a beautiful scene, the WebPage can let a visitor change
observer context and see representation policy respond.

For example:

```text
OBSERVER
   |
   +-- move closer
   |       |
   |       v
   |   representation becomes richer
   |
   +-- move farther
           |
           v
       representation becomes cheaper
```

The important lesson is not "we have level of detail."

It is:

> **The world does not have to become cheaper by ceasing to exist.**

The Workshop can demonstrate that an observable consequence remains represented while
the computational detail changes.

That is a direct bridge from Renderer theory to an interactive experience.

---

## Creation is not a form

The Create surface should not become a conventional CRUD screen.

The Forge exists to let visitors manipulate the same vocabulary used to build the
Workshop:

```text
STATE
  +
CAPABILITY
  +
COMPOSITION
  +
ARBITRATION
  +
MANIFEST
  =
EXPERIENCE
```

A visitor should be able to take a small capability, observe it, combine it with
another capability, and watch the resulting composition change.

The strongest Create experience therefore has a **live consequence**.

If the visitor changes the composition, something should visibly change.

The page should not merely report that a configuration was saved.

---

## Understand means "show me"

The Understand surface is not a glossary.

Every architectural explanation should prefer a path to executable evidence:

```text
CONCEPT
  |
  +--> SEE IT
  |
  +--> CHANGE IT
  |
  +--> INSPECT IT
  |
  v
UNDERSTAND IT
```

For example:

- FSM_API -> manipulate a stateful behavior.
- FSM_COS -> change a MicroBundle composition.
- MicroBundle -> install or remove a capability.
- ProtocolAI / GrammarAi -> inspect a deterministic command boundary.
- Renderer -> change observer context and representation.
- Computation -> observe the difference between workload and execution provider.
- SingularityWarehouse -> inspect identity, ontology, and persistence.
- AnyApp -> see the same composition manifest in another host.

The architecture page should become an **index into experiments**, not a wall of
text.

---

## Navigation is allowed to become diegetic

The Workshop does not need to imitate a corporate dashboard.

Once a visitor enters an Experience, navigation may be represented by:

- doors;
- terminals;
- transit;
- workbenches;
- maps;
- instruments;
- physical-looking controls;
- spatial landmarks;
- characters;
- signals;
- discovered capabilities.

A conventional navigation rail remains acceptable where it improves accessibility
or orientation.

It is not sacred.

The correct question is:

> **Does this navigation help the visitor understand where they are and what they
> can do?**

If a physical doorway communicates that better than a tab, use the doorway.

If a tab communicates it better, use the tab.

Standards are tools, not laws.

---

## Human agency is part of the architecture

The Workshop may surprise the visitor.

It should not quietly make consequential decisions for them.

The system may:

- propose;
- demonstrate;
- animate;
- discover;
- compose previews;
- reveal capabilities;
- suggest a path.

The visitor decides what to enter, alter, create, publish, or share.

This matters especially as AI enters the architecture.

```text
AI / intelligence
       |
       v
proposal / command
       |
       v
deterministic boundary
       |
       v
Workshop runtime
       |
       v
human-visible consequence
```

The deterministic boundary is what makes the system inspectable.

---

## The public proof loop

Every major Workshop capability should eventually participate in this loop:

```text
        +--------------------+
        |                    |
        v                    |
      ENTER                CREATE
        |                    ^
        v                    |
    EXPERIENCE          INSPECT
        |                    ^
        v                    |
      OBSERVE ------------ PROVE
        |
        v
    UNDERSTAND
```

"Prove" means the visitor can do something that would be difficult to fake with a
static marketing page.

Examples include:

- change a state and watch behavior change;
- add/remove a capability;
- compose MicroBundles;
- alter observer position;
- watch representation policy respond;
- inspect the manifest that produced an Experience;
- compare two execution strategies;
- export a deterministic description of a composition.

The WebPage becomes compelling when the visitor can say:

> "I just watched it do that."

---

## What we deliberately reject

The Workshop should not adopt a standard merely because it is common.

We should reject or bypass conventions when they fight the architecture:

- **hero-first marketing** when a demonstration is more meaningful;
- **feature grids** when the visitor should enter a place;
- **static screenshots** when the system can run;
- **documentation walls** when an executable experiment can teach the concept;
- **navigation taxonomies** that expose implementation names instead of human intent;
- **page-local orchestration** that duplicates runtime behavior;
- **framework identity** as product identity;
- **renderer/engine lock-in** as an architectural boundary;
- **"coming soon" promises** presented as existing capability;
- **complexity for its own sake** merely because the Workshop can do it.

We should keep conventional web mechanisms when they improve accessibility, discoverability,
performance, security, or human agency.

The goal is not to be different.

The goal is to make the architecture serve the person.

---

## Host symmetry

The WebPage is one host and one proving ground.

The same Experience composition should be able to become:

```text
                 Manifest
                    |
                    v
               MicroBundles
                    |
                    v
                 FSM_COS
                    |
                    v
             RuntimeAssembly
              /      |      \
             /       |       \
        WebPage     AnyApp   future hosts
```

The browser is therefore not the definition of the Workshop.

It is our most public laboratory.

That makes it the right place to prove the architecture.

---

## Engineering rule

When a new WebPage feature is proposed, ask:

1. **What does the visitor experience?**
2. **What architectural capability does that experience prove?**
3. **Which runtime boundary owns the behavior?**
4. **Which MicroBundle / Experience composes it?**
5. **Can the visitor change something and observe a consequence?**
6. **Can the source/documentation explain what they just saw?**
7. **Can another host eventually consume the same semantic composition?**

If the answer to all seven is yes, the feature belongs naturally in the Workshop.

If the feature only adds another page, another card, or another paragraph, it may
be useful—but it is not yet taking advantage of what the Workshop actually is.

---

## Closing principle

> **The Workshop should not convince people that the future is possible.**
>
> **It should let them play with enough of the machinery that they begin to wonder
> what else is possible.**

That is the WebPage.

The page opens the door.

The Experience gives the visitor a place to stand.

The architecture lets them look underneath it.

And the Forge lets them start building.


---

## Workshop tooling, Singularity City, and Software Inc.

The Workshop is intended to **present tooling and let visitors play with it**. It is the sandbox in which the ecosystem's capabilities become places, instruments, experiments, and persistent creations.

Singularity City is deliberately different. It is persistent background proof: a living digital environment populated by Digitens that continues to demonstrate persistence, behavior, spatial composition, and scale while the visitor works elsewhere in the Workshop. The City is also the long-term substrate for digital real estate, user-created shops, and an Ontology Mall whose floors and departments spatialize the ontology itself.

The eventual commercial model should let a visitor encounter a user-created shop while playing an Experience and purchase digital or paired real-world goods without leaving the Experience. The underlying shop identity and product semantics should remain reusable across City and Experience manifestations.

**Singularity Software Inc. is separate from the Workshop.** It is the future visualized, agent-backed code-creation technology. The Workshop may expose a helipad as its eventual doorway to a rooftop helipad at Software Inc., but it must not present that destination as functional until the product is actually ready.

See WORKSHOP_CITY_AND_SOFTWARE_VISION.md for the fuller model.


## Singularity City as the living recipe library

Singularity City is intended to become a persistent simulated world in which visitors can inspect not only what they see, but **how what they see was composed**.

A City artifact should eventually expose a distinction between:

- **recipe** — Manifest, MicroBundles, configuration, initial data, arbitration, and composition choices;
- **runtime state** — what the composed system has done since it began running;
- **representation** — how that state is currently manifested for this observer.

This makes the City an executable library of Workshop examples:

```text
SEE
  |
INSPECT
  |
SHOW RECIPE
  |
COPY
  |
MODIFY
  |
COMPOSE
  |
OBSERVE CONSEQUENCE
```

The goal is not to make City a conventional digital-twin product. It is to create a persistent simulated reality that can teach the construction of simulated realities by exposing its own compositional lineage.

The City can therefore contain reference examples for companies, buildings, Digitens, transit, shops, neighborhoods, physics, rendering, and other capabilities. Each new example can become another reusable lesson and another starting point for creation.

This also gives the City a natural proving ground for Event Horizons and Computation: distant populations can remain represented while computational attention follows observable consequence. That is a benchmarkable hypothesis, not a claim that the architecture has already achieved a particular efficiency.

See WORKSHOP_CITY_AND_SOFTWARE_VISION.md for the deeper City model.
