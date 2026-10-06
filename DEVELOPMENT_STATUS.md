# WebPage Development Status

## Current state

The repository is in a consolidation pass.

WebPage began as a website wrapper and has become the browser proving ground and public learning resource for The Singularity Workshop.

The current priority is not adding more site sections. It is making the existing host explain and execute the correct architecture.

## Active startup model

~~~text
WEBPAGE BOOT
   ↓
load manifest
   ↓
compose primary root through FSM_COS
   ↓
2102 Living GUI
   ↓
2110 Moniker dependency is resolved automatically
   ↓
present Moniker
   ↓
activate primary Experience
   ↓
FSM_API-driven Living GUI
~~~

## Current public surface

AnyApp is the active cross-host demonstration.

The repository documentation is the deeper learning surface while the runtime architecture is being stabilized.

## Known transitional code

The repository still contains:

- older first-contact/page-FSM code;
- older Idler/Flex catalogs;
- demo/bootstrap scaffolding;
- older exploratory pages;
- duplicated presentation/runtime assumptions.

These remain until their behavior has been located, tested, replaced, and proven elsewhere.

## Package boundary

The WebPage application consumes:

- TheSingularityWorkshop.FSM_API;
- TheSingularityWorkshop.FSM_COS;
- TheSingularityWorkshop.MicroBundleDomain;
- GUI packages;
- other Workshop packages where they own reusable capabilities.

Do not introduce a local copy when the package is the canonical owner.

## Current proof obligations

- manifest loads and parses;
- startup and running entries remain distinct;
- primary root is 2102;
- 2102 resolves 2110 through FSM_COS dependency closure;
- RuntimeAssembly contains the closed composition;
- browser transitions from Moniker to the primary Experience;
- CI is clean.

## Review point

The next human review should be visual: run the WebPage at multiple client sizes and confirm the manifest Moniker and Living GUI both fit the client rather than assuming fixed viewport geometry.

Master remains untouched.
