# Workshop Handoff

This is the current engineering handoff for the WebPage proving ground. Git history and issues preserve older experiments.

## Current model

- development is active engineering.
- master is the stable promotion target.
- WebPage is a browser proving ground, not the owner of every reusable capability.
- reusable functionality should move into the package or domain that owns it.
- FSM_COS is the composition boundary.
- FSM_API is the deterministic behavior/state substrate.

## Current opening

~~~text
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
Moniker dependency resolves
  ↓
living GUI grows / reproduces
  ↓
population threshold
  ↓
gravity
  ↓
Moniker presentation
  ↓
Workshop navigation
~~~

This is current WebPage behavior. Do not import another host's scene or startup concept into this sequence.

## Current FSM_COS proof

The browser selects LivingGuiExperience.

The Experience requests LivingGuiExperienceMicroBundle.

That MicroBundle declares Moniker as a dependency.

WebPage turns the Experience into a RuntimeManifest.

FSM_COS resolves, loads, arbitrates, converges, and returns RuntimeAssembly.

Only then does WebPage enter its presentation/runtime phase.

## Current architectural extraction

The old WebPage accumulated useful functionality because it was the fastest place to experiment.

The active work is to move reusable responsibilities into independent NuGet/domain packages and leave WebPage as:

- consumer;
- browser adapter;
- visual proving ground;
- educational surface.

A capability may later be represented as a MicroBundle when its contract fits the composition model.

## Documentation obligation

Every extracted capability should leave:

1. package-level contract and usage documentation;
2. tests proving behavior;
3. a WebPage proving-ground example when useful;
4. theory explaining why the boundary exists.

## Do not reintroduce

Do not reintroduce:

- page-local composition algorithms;
- speculative host architecture;
- abandoned platform descriptions;
- hidden package responsibilities;
- documentation that treats future ideas as current behavior.

The question for every new feature is:

> **What does this prove, who owns it, and how does another developer use it?**
