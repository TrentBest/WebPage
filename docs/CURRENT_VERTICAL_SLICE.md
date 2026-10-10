# Current Vertical Slice — Manifest → Moniker → Primary Experience

Read this before changing the active WebPage startup.

## Branch discipline

- development is the integration branch.
- master is the stable promotion target.
- Short-lived exploratory branches start from development, merge after proof/CI, then get deleted.
- Do not accumulate long-lived feature branches.

## Current objective

Make the WebPage host manifest the startup authority and make the repository explain the same architecture it executes.

The current manifest contains:

- one startup Moniker presentation;
- one primary running Experience: Living GUI;
- Deep Dive restart/telemetry intent.

The model permits more than one startup Experience. The concrete sample currently uses one.

## Startup sequence

~~~text
WEBPAGE BOOT
   ↓
load host manifest
   ↓
compose primary root through FSM_COS
   ↓
FSM_COS resolves 2102
   ↓
2102 declares 2110 Moniker dependency
   ↓
RuntimeAssembly contains the closed composition
   ↓
present startup Moniker
   ↓
activate primary Experience
   ↓
RuntimeAssembly is handed to the host
   ↓
**Current gap:** browser manifestation still consumes the transitional WebPage PageFSM/LivingGuiFsm runtime
~~~

The critical architectural point is:

> Startup order is a presentation concern. Dependency order is an FSM_COS composition concern.

The host must not confuse the two by requesting the same dependency twice.

## Current primary Experience

The current primary root is 2102 — LivingGuiExperienceMicroBundle.

Its declared dependency is 2110 — MonikerMicroBundle.

The manifest names 2102. FSM_COS discovers 2110 through the MicroBundle dependency contract.

## Current browser responsibilities

WebPage owns:

- fetching/parsing the host manifest;
- providing the WebPage composition catalog;
- consuming the RuntimeAssembly;
- browser presentation;
- navigation/chrome;
- WebPage-only Deep Dive presentation.

WebPage does not own:

- FSM state-machine semantics;
- FSM_COS dependency traversal;
- FSM_COS arbitration;
- reusable package implementation.

## Current public navigation

The navigation is intentionally small while this host is being stabilized. The first inspectable hub is a checkpoint, not a claim that the final hub content or information architecture has been decided.

The host boundaries must remain visible:

- **WebPage** is the browser proving ground and public learning resource.
- **WebApp** is itself a web application/host, not merely a tab inside AnyApp or a WebPage-only feature.
- **AnyApp** is itself the native desktop host, with its own process, window, input, and presentation lifecycle. A WebPage route describing AnyApp is explanatory material; it must not imply that it launches the native application when no public installer/launcher exists.

Manifest hub entries may introduce or link to these distinct hosts. The hub is a discovery and entry surface, not a substitute implementation of those hosts. Keep the first hub small enough to inspect; grow its content only as the intended visitor journey becomes clear.

The repository is the deeper learning resource. Future educational Experiences should be added through the Experience/manifest model rather than by turning the navigation menu into a static encyclopedia.

## Transitional work

The repository still contains historical and transitional code, including older first-contact/page-FSM paths and older demonstration scaffolding.

Do not delete these by aesthetic judgment.

Use:

~~~text
locate behavior
  ↓
identify owner
  ↓
preserve with tests
  ↓
replace
  ↓
prove
  ↓
remove duplicate
~~~

## Proof obligations

Every architectural change must have executable evidence when practical.

The active proof for this slice must establish:

1. the manifest contains startup and running entries;
2. the primary root is 2102;
3. 2102 declares 2110 as an FSM_COS dependency;
4. the composed RuntimeAssembly contains both;
5. the browser transitions from Moniker to the primary Experience;
6. CI build and tests pass.
7. the functionality-preservation ledger remains green for every behavior being migrated.

## Important files

| File | Responsibility |
|---|---|
| LEARNING_PATH.md | Visitor/developer curriculum |
| REPOSITORY_MAP.md | Source ownership and dependency map |
| WORKSHOP_RUNTIME_ARCHITECTURE.md | Runtime boundary |
| FSM_COS_USAGE.md | Operational composition guide |
| TheSingularityWorkshop/Services/WorkshopExperienceService.cs | Manifest loading and primary composition |
| TheSingularityWorkshop/Workshop/Composition/WorkshopCompositionCatalog.cs | FSM_COS catalog |
| TheSingularityWorkshop/Workshop/MicroBundles/LivingGuiExperienceMicroBundle.cs | Primary root + Moniker dependency |
| TheSingularityWorkshop/Workshop/MicroBundles/MonikerMicroBundle.cs | Canonical startup capability |
| TheSingularityWorkshop/Pages/Home.razor | Browser manifestation; currently bridges to transitional runtime |
| TheSingularityWorkshop/Layout/MainLayout.razor | Browser chrome |
| SingularityHub.Tests/WebPageHostManifestTests.cs | Manifest/composition contract tests |
| SingularityHub.Tests/IncrementalVersionTests.cs | Incremental proof ledger |

## Mandatory completion loop

1. make the requested change;
2. add or update meaningful test proof;
3. build;
4. run relevant tests;
5. inspect visual behavior when presentation changes;
6. verify CI;
7. update authoritative documentation;
8. leave master untouched unless promotion is explicitly requested.

Do not publish packages or releases without explicit approval.

## Function-preservation gate

See [`FUNCTIONALITY_PRESERVATION.md`](FUNCTIONALITY_PRESERVATION.md). The active refactor must preserve the existing Living GUI behavior—100-slot population, independent organism FSMs on one processing group, travel/planting/growth/reproduction/reduction, deterministic target distribution, and gravity—while moving its authority out of WebPage.
