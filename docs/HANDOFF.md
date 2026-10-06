# WebPage handoff

Active branch: `development`. Do not modify `master`.

The landing experience is not finished. Tests may be green while the browser manifestation is broken.

Known failures:
- Living GUI is visibly dead.
- The Living GUI root still appears in the upper-left instead of the center of its panel.
- The intended six-color panel breathing animation was added to `LivingGui.razor`, but `wwwroot/css/app.css` still has hard-coded `border` and `box-shadow` declarations with `!important` that can defeat it.
- Idle is activating when the deliberate Living GUI Flex path should be active.

Intended sequence:

`PAGE_INITIALIZING -> GATEWAY -> GATEWAY_EXIT -> LIVING_GUI_IGNITION -> LIVING_GUI_POPULATING -> 100 nodes -> freeze -> MONIKER_REVEAL -> GRAVITY -> LIVING_GUI_DISSIPATING -> NAVIGATION_ARRIVAL -> RUNNING`

The moniker is earned at exactly 100 nodes and freeze. Do not move it into first contact.

Pong is the first Idler-capable Experience. Living GUI is the first Flex-capable Experience. Idler and Flex are capabilities, not hardcoded Experience identities.

Important files:
- `CURRENT_VERTICAL_SLICE.md`
- `WORKSHOP_PRESENTATION_CONTRACT.md`
- `EXPERIENCE_THEORY.md`
- `TheSingularityWorkshop/Pages/LivingGui.razor`
- `TheSingularityWorkshop/Services/PageStateContext.cs`
- `TheSingularityWorkshop/Services/PageFSM.cs`
- `TheSingularityWorkshop/Services/LivingGuiFsm.cs`
- `TheSingularityWorkshop/Services/FSMManagerService.cs`
- `TheSingularityWorkshop/wwwroot/css/app.css`
- `SingularityHub.Tests/IdleExperienceArchitectureTests.cs`
- `SingularityHub.Tests/LivingGuiRuntimeTests.cs`

Recent commits:
- `9303146eaab0236f785c7cb67f065d9d6f9f8a76` — Living GUI runtime tests.
- `0ee2ba8234c7533d5611369f4ea7d9093ee32055` — six-color panel breathing animation.
- `f4e02a96643d5735f13bf56a8c4ef19fdb86f859` — Hub-driven landing lifecycle merged into development.

Historical working implementations are worth inspecting before rewriting anything. The recurring lesson is: locate behavior, identify its owner, preserve it with a test, then move/adapt it.

Do not declare success from unit tests alone. For visual work, verify the browser manifestation.

The Workshop is deliberately weird. That is a feature. The software is supposed to demonstrate itself.
