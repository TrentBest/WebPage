# WebPage Learning Path

WebPage has two entrances to the same lesson.

~~~text
GitHub repository                         Running WebPage
       |                                      |
       v                                      v
 understand the architecture          witness the behavior
       |                                      |
       +------------------+-------------------+
                          |
                          v
                 inspect the proof
                          |
                          v
                   read the theory
                          |
                          v
                    build with it
~~~

The goal is not to make a visitor memorize WebPage internals. The goal is to understand the boundaries well enough to use the underlying Workshop technology elsewhere.

## Visitor track

### 1. Witness

Start at the Workshop opening.

The browser is the manifestation. It should show that a real Experience is being composed and executed rather than presenting a static marketing page.

### 2. Identify

After the opening, ask:

- What Experience am I seeing?
- Which MicroBundles make it possible?
- Which behavior belongs to FSM_API?
- Which composition belongs to FSM_COS?
- Which pieces are only browser presentation?

When the hub appears, choose **SHOW DEEP DIVE** to open the educational view for the Experience named by the host manifest. From the running hub, the view uses the actual startup `RuntimeAssembly`: requested roots are shown separately from resolved MicroBundles, along with the runtime ID and arbitration-round count. If a visitor opens the route directly before startup has produced an assembly, the page labels its catalog-based fallback instead of presenting it as a live runtime snapshot.


- What Experience am I seeing?
- Which MicroBundles make it possible?
- Which behavior belongs to FSM_API?
- Which composition belongs to FSM_COS?
- Which pieces are only browser presentation?

### 3. Explore

The public navigation is intentionally narrow while the host architecture is being stabilized. Rendering is the current active proof surface for the renderer → perception → semantic-observation boundary.

Future educational surfaces can be added as Experiences rather than turning navigation into a pile of static documentation pages.

## Developer track

### Lesson 1 — FSM_API

Start with the FSM_API repository and its published package.

Learn the distinction between FSM definitions, FSM instances, processing groups, contexts, lifecycle, and deferred execution.

WebPage should consume this behavior rather than reimplementing it.

### Lesson 2 — FSM_COS

Read FSM_COS_USAGE.md.

The WebPage host supplies a root RuntimeManifest.

For the current Living GUI proof:

~~~text
manifest
   ↓
primary root: 2102 LivingGuiExperienceMicroBundle
   ↓
FSM_COS closes dependency graph
   ↓
2110 MonikerMicroBundle
   ↓
load + arbitrate + converge
   ↓
RuntimeAssembly
~~~

The critical lesson is that WebPage does not request the Moniker as a second root. The Living GUI MicroBundle declares that dependency.

### Lesson 3 — MicroBundles

Read EXPERIENCE_THEORY.md and MICROBUNDLE_ARBITRATION_MAP.md.

A MicroBundle is a focused composition participant. It declares dependencies and participates in loading and arbitration.

A package and a MicroBundle are related concepts, but they are not interchangeable labels.

### Lesson 4 — Experience

An Experience describes the environment being requested.

The current manifest separates:

- startup — one or more presentation Experiences such as the Workshop Moniker;
- running — the primary Experience that follows startup.

A splash/Moniker is not automatically the primary Experience.

### Lesson 5 — WebPage

Only after the runtime boundaries are understood should a developer study:

- WorkshopExperienceService;
- WorkshopCompositionCatalog;
- Pages/Home.razor;
- Layout/MainLayout.razor;
- the concrete Living GUI manifestation.

The host's job is to load the manifest, request composition, consume the RuntimeAssembly, and manifest the result in a browser.

## Repository archaeology

There is old code in this repository.

Do not use chronology as architecture.

When you encounter a suspicious local implementation:

1. find the behavior;
2. find its test;
3. identify the canonical package/domain owner;
4. determine whether WebPage still needs an adapter;
5. migrate only after equivalent proof exists;
6. remove the duplicate implementation.

This is how the repository gets smaller and more correct over time.

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

WebPage is valuable because it demonstrates all of these boundaries in one working host.

It is not valuable because every piece of its current source code should become a pattern for other applications.

## Proof before prose

A learning surface should point to executable evidence.

Use tests for lifecycle and composition claims, manifests for requested roots, RuntimeAssembly inspection for dependency closure, visual behavior for presentation claims, and benchmarks for performance claims.

If the code cannot prove the sentence, rewrite the sentence.

---

*WITNESS → WONDER → DEEP DIVE → UNDERSTAND → CREATE → RUN*