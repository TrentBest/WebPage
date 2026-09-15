# Workshop Runtime Architecture

This is the current contract for how the WebPage host becomes a Workshop runtime.

## Governing rule

> **The page is the presentation layer. The Hub is the runtime owner. Experiences and MicroBundles are the content and behavior units.**

Blazor connects the browser to the Hub and renders what the Hub says is currently present. It must not become a second application runtime.

## Bootstrap

```text
Browser
  |
  v
Blazor host
  |
  | load WebPage host manifest
  v
Hub boot
  |
  +--> connect MicroBundle repository / registry
  +--> discover available Experiences
  +--> discover Idler-capable Experiences
  +--> discover Flex-capable Experiences
  |
  v
select one available Idler
  |
  v
RUN IDLER
  |
  | visitor selects Enter Workshop
  v
select one available Flex
  |
  v
RUN FLEX
  |
  v
present Workshop moniker for the configured reveal interval
  |
  v
reveal navigation / page chrome / primary panel
  |
  v
moniker remains in primary panel until navigation selection
  |
  | visitor selects a tab
  v
clear moniker
  |
  v
present selected GUI / Experience
```

Pong and Living GUI are the first concrete implementations, not protocol keywords. The bootstrap must not require those names to exist.

## Manifest versus inventory

The WebPage manifest describes the **host**: registry locations, schema/policy versions, host capabilities, manifestation domains, lifecycle policy, and eventually trust/security requirements.

It must not become a growing list of concrete Experiences. Inventory belongs to the MicroBundle/Experience registry.

## Registry responsibility

The registry/repository is the discovery boundary. It supplies addressable bundle metadata such as:

- address and version;
- ontology;
- capabilities;
- sensory systems;
- dependencies;
- providers/manifests; and
- integrity/trust information as those layers mature.

The Hub selects eligible candidates from discovered inventory. The page does not maintain the inventory.

## Experience responsibility

An Experience is an environment composed of MicroBundles. It describes identity, version, ontology, sensory systems, capabilities, MicroBundle membership, and required process groups.

The Experience does not own the application heartbeat.

The Hub owns loading, dependency ordering, arbitration, runtime indexing, process-group activation, stepping, lifecycle transitions, and unloading/invalidation.

## Idler and Flex

**Idler** and **Flex** are capability/category concepts used by bootstrap policy, not special hardcoded application classes.

The first vertical slice is:

```text
Pong       -> Idler-capable
Living GUI -> Flex-capable
```

Future Experiences can provide either, both, or new capabilities without requiring the bootstrap to be rewritten around their names.

## Current transitional code

The branch still contains static `IdleExperienceCatalog` and `FlexExperienceCatalog` definitions and a small `WorkshopDemoBundle`. These are compatibility/composition scaffolding while the registry-driven path is completed.

**Do not delete them until their functionality has been located in the Experience/MicroBundle/registry architecture and covered by tests.**

The same rule applies to historical branches: preserve behavior first, relocate it second, delete obsolete implementation last.

## AI is later, not lost

AI, Grammar, Protocol, and command-pipeline work remains valuable and should be retained as future architecture. It is deliberately not required for this deterministic bootstrap.

The intended future relationship is:

```text
AI / Grammar / Protocol
          |
          v
 deterministic command / selection
          |
          v
 Experience + MicroBundle boundary
          |
          v
 Hub arbitration / runtime index
          |
          v
 FSM_API execution
```

The runtime must work without AI. AI becomes a future producer of deterministic inputs, not the owner of runtime semantics.

## Housekeeping rule

For every refactor toward this model:

1. locate the existing behavior;
2. identify its intended Experience/MicroBundle/provider boundary;
3. preserve it with a test;
4. move or adapt it;
5. update the explanatory documentation;
6. only then remove obsolete host-specific code.

The implementation may change dramatically. Functionality must not disappear accidentally.

*The page opens the door. The Hub runs the Workshop.*
