# Development status

Branch: `development`

The current landing slice is an engineering handoff, not a finished feature.

## Known browser failures

- Living GUI is visibly dead.
- Living GUI root remains in the upper-left instead of the center of its panel.
- The six-color panel breathing animation exists in `TheSingularityWorkshop/Pages/LivingGui.razor`, but host CSS in `TheSingularityWorkshop/wwwroot/css/app.css` has important static border and shadow declarations that can override it.
- Idle activates during the deliberate Living GUI Flex path; selection/lifecycle ownership still needs tracing.

## Intended lifecycle

`PAGE_INITIALIZING -> GATEWAY -> GATEWAY_EXIT -> LIVING_GUI_IGNITION -> LIVING_GUI_POPULATING -> 100 -> FREEZE -> MONIKER_REVEAL -> GRAVITY -> DISSIPATING -> NAVIGATION_ARRIVAL -> RUNNING`

## Architectural truths

- FSM_API is the meaningful lifecycle execution mechanism.
- Hub owns runtime lifecycle/heartbeat responsibilities.
- Razor renders authoritative state; it does not own the lifecycle.
- Pong is the first Idler-capable Experience.
- Living GUI is the first Flex-capable Experience.
- Idler and Flex are capabilities, not concrete bootstrap identities.
- The moniker is earned at exactly 100 nodes and freeze.

## Recovery rule

Before simplifying anything, locate the behavior, identify its owner, preserve it with a focused test, then move or adapt it. Historical implementations are evidence.

## Recent checkpoints

- `f4e02a96643d5735f13bf56a8c4ef19fdb86f859` — Hub-driven landing lifecycle merged into `development`.
- `9303146eaab0236f785c7cb67f065d9d6f9f8a76` — Living GUI runtime tests.
- `0ee2ba8234c7533d5611369f4ea7d9093ee32055` — six-color panel breathing animation.
- `06a97a22751ab7cca0faa4fcb770259548e23f60` — handoff notes.

Master remains at `4cf613a19126548264e4f9e610f17528030532e6` and must not be modified for this work.
