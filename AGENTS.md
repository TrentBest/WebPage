# The Singularity Workshop — WebPage Agent Instructions

This repository is an active engineering proving ground.

## Branch safety

- development is active engineering.
- master is the stable promotion target.
- Do not modify master during ordinary development.
- Preserve valuable behavior before replacing it.
- Inspect history when unusual code may contain intentional behavior.

## What this repository is for

WebPage is the browser proving ground for The Singularity Workshop.

Its purpose is to demonstrate real behavior, expose architectural boundaries, move reusable functionality into independently owned packages, and document how those packages are used.

> **Use the technology to build the technology.**

WebPage is not the canonical owner of every capability it demonstrates.

## Architectural extraction rule

When a useful feature is discovered inside WebPage:

~~~text
experiment
   ↓
identify responsibility
   ↓
identify owner
   ↓
extract reusable implementation
   ↓
test package
   ↓
consume package here
   ↓
document the decision
~~~

Do not simply split a monolith into packages while keeping the old WebPage assumptions hidden between them.

Ask:

- Is this behavior reusable?
- Does another host need it?
- Which package or domain should own its contract?
- Does it qualify as a MicroBundle participant?
- What remains genuinely browser-specific?

A package is not automatically a MicroBundle.

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
Moniker dependency
   ↓
living GUI growth / reproduction
   ↓
population threshold
   ↓
gravity
   ↓
Moniker presentation
   ↓
Workshop navigation
~~~

Do not describe another host's startup or landing scene as WebPage behavior.

## FSM_API ownership

FSM_API is the authoritative state-machine implementation.

WebPage may own wrappers such as PageFSM, but it must not grow a competing FSM.

- An object wrapper advances its own owned handle.
- Group-wide ticking belongs to the host/integration loop.
- Disposed handles must be unregistered.
- Tests must prove lifecycle ownership.

## FSM_COS ownership

FSM_COS is the composition boundary.

WebPage should:

- select an Experience;
- create a RuntimeManifest;
- supply a MicroBundle catalog;
- execute the composition;
- consume RuntimeAssembly;
- perform browser presentation after handoff.

WebPage should not:

- duplicate dependency closure;
- implement arbitration;
- make the composition kernel understand browser concerns;
- make reusable packages depend upward on WebPage.

Read FSM_COS_USAGE.md before changing this boundary.

## Documentation standard

Documentation is engineering memory.

Every authoritative document should distinguish:

- Current — implemented behavior;
- Direction — architecture we are deliberately implementing;
- Future — not part of today's WebPage contract.

Do not put speculative world-building, abandoned platforms, old host experiments, or conversational side visions into current operational documentation.

If an idea belongs to another repository, document the boundary and link to that repository instead of importing its entire vision.

Prefer:

> why the boundary exists → what the code does → how another developer uses it → what proves it

over a chronological account of conversations.

## Visual standard

When a visual behavior matters:

- show it before explaining it;
- prefer diagrams over paragraphs when relationships are spatial;
- keep text truthful to implementation;
- never use visual effects to imply capabilities that do not exist.

## Tests are architectural evidence

A meaningful architectural change should have executable proof when practical.

Do not weaken a test merely to make an implementation pass. Fix the lifecycle or ownership problem the test exposes.

## Quality bar

Before calling work complete:

1. build;
2. run relevant tests;
3. inspect behavior;
4. update authoritative documentation;
5. verify GitHub Actions;
6. confirm dependency direction;
7. leave master untouched unless promotion was explicitly requested.

Zero warnings and zero avoidable errors is the target.

## Historical notes

Git history and issue history preserve chronology.

Do not turn the main documentation into a fossil record of every intermediate idea.

The repository should teach the machine we have, explain the decisions that produced it, and identify the next deliberate boundary.
