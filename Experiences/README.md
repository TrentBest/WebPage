# Solution-native Experience tests

Projects under `Experiences/` are the executable architectural boundary for **core Experiences**.

A core Experience is recognized by being built into `WebPage.sln`; an externally configured Experience is not required to become solution-native merely to participate in the Hub.

## Current core Experience test projects

- `LivingGuiExperience.Tests`
- `PongExperience.Tests`

These projects intentionally begin as contract/catalog tests. As the Experience model becomes concrete, each project becomes the home for that Experience's focused lifecycle, sensory, ontology, capability, dependency, and process-group tests.

## Runtime role

An Experience is not a Blazor page and it is not an idle/screen-saver implementation. It is an environment composed of MicroBundles.

The host discovers Experiences through the Hub/registry path. The host may classify an Experience by capabilities such as:

- **Idler** — eligible to run while the landing page is idle;
- **Flex** — eligible to run as the first deliberate showcase after `Enter Workshop`;
- future capabilities — additional policies without changing the bootstrap protocol.

The first vertical slice uses Pong as the first Idler-capable Experience and Living GUI as the first Flex-capable Experience. Those names are implementation examples, not bootstrap requirements.

## Test boundary

Tests should prove behavior at the Experience boundary rather than teaching the page which concrete Experience must exist.

In particular, preserve tests for:

- Experience identity and version;
- ontology coordinates;
- MicroBundle membership;
- capability classification;
- distinct sensory systems;
- dependency/arbitration behavior;
- required FSM_API process groups; and
- Hub-owned stepping/lifecycle semantics.

See `EXPERIENCE_THEORY.md` and `WORKSHOP_RUNTIME_ARCHITECTURE.md` for the larger model.
