# Current Vertical Slice — Pong + Living Workshop Landing

Read this before changing the active experience.

## Working branch

`feature/pong-microbundle-vertical-slice`

Do not modify `master`.

## Current objective

Publish a deterministic, FSM_API-driven landing experience that demonstrates the technology through the experience itself.

The immediate visual path is:

1. Gateway: oversized `Enter Workshop` control.
2. Control is 50vw × 50vh on desktop.
3. Avatar is a true inscribed circle, never a square image inside an oval.
4. Click enters the living GUI experience.
5. One GUI seed appears at center.
6. GUI nodes reproduce, travel, and grow.
7. At exactly 100 nodes the population freezes.
8. The exact 100-node mutation sets `MonikerReady`.
9. The Workshop moniker first appears at that boundary and persists for the rest of the experience page lifetime.
10. `MONIKER_REVEAL` and `GRAVITY` occur after the reveal; gravity must not be the reveal gate.
11. Navigation arrives later.

## Important files

| File | Responsibility |
|---|---|
| `WORKSHOP_PRESENTATION_CONTRACT.md` | Canonical behavior contract |
| `TheSingularityWorkshop/Services/PageStateContext.cs` | Living population, exact critical mass, reveal gate |
| `TheSingularityWorkshop/Services/PageFSM.cs` | FSM_API state progression and private scheduler ownership |
| `TheSingularityWorkshop/Pages/Home.razor` | Gateway presentation |
| `TheSingularityWorkshop/Pages/LivingGui.razor` | Living GUI manifestation |
| `TheSingularityWorkshop/wwwroot/css/workshop-landing.css` | Global landing overrides; inspect this when CSS seems to override component styles |
| `SingularityHub.Tests/IdleExperienceArchitectureTests.cs` | Exact executable sequence |
| `SingularityHub.Tests/IncrementalVersionTests.cs` | Visible incremental proof marker |
| `AGENT_INCREMENTAL_RULE.md` | Mandatory agent proof rule |

## Known trap: CSS precedence

The landing has both component-local styles and `wwwroot/css/workshop-landing.css`. The global stylesheet uses `!important` overrides. If a component appears to ignore its own geometry, inspect the global landing stylesheet before changing markup repeatedly.

## Mandatory agent behavior

Every meaningful repository change must:

1. make the actual requested change;
2. add/update an incremental visible unit-test/version proof;
3. advance `IncrementalVersionTests.CurrentVersion`;
4. keep the assertion meaningful;
5. report the commit SHA and version.

Do not claim a test is green without checking the current result.
Do not change `master`.
