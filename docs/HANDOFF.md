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


## Demonstrations gallery — implementation checkpoint (2026-10-12)

The Demonstrations tab and manifest-backed gallery are being introduced on `development`.

- `webpage-host.manifest.json` now declares a Demonstrations hub destination and three source-backed entries: WebGPU instancing, native WPF/Direct3D 12 surface, and Renderer Event Horizon buckets.
- `DemonstrationRegistry` resolves entries by stable ID and reports whether declared host capabilities are present.
- The gallery/detail view is hosted by the existing Home experience; detail links return to the gallery. No second router or standalone demo framework was added.
- No **Run demonstration** action is shown because none of the three proofs has a verified runnable destination inside WebPage. The WebGPU entry documents its local-server instructions instead.
- The current registry is a metadata-discovery increment, not yet an executable `IExperience` registry. Do not claim the external proofs are registered for Hub-owned execution or that their processing groups are integrated.

Next verification steps:
1. Build the solution and run `SingularityHub.Tests`.
2. Verify the landing/Moniker sequence is unchanged.
3. Open Demonstrations, each detail view, and the return-to-gallery link in an actual browser.
4. Check desktop and narrow layouts, and verify the navigation collapses as before.
5. Update this checkpoint with actual build/test/browser evidence; do not infer success from commits alone.
6. Do not merge or publish without explicit authorization.
