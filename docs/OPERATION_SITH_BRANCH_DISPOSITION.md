# Operation Sith — WebPage Branch Disposition

Audit date: 2026-10-16

## Canonical branch policy

WebPage has exactly two intended long-lived branches:

- `master`: stable release/promotion line.
- `development`: sole integration line.

Do not create additional persistent feature/work branches. Integrate work into `development`; promote to `master` deliberately. This audit does not authorize publication or branch deletion by an agent.

## Live branch inventory

The GitHub branch search returned these 11 branches:

- `master`
- `development`
- `feature/visual-fit-and-audience`
- `work/acoustic-studio-lab`
- `work/city-aerial-traffic-slice`
- `work/development-consolidation`
- `work/digiten-world-slice`
- `work/gestures-movement-lab`
- `work/moniker-experience-finalization`
- `work/movement-lab-current-development`
- `work/soldier-field-manifest-experience`

## Disposition of extra branches

The disposition below is based on comparing branch tips to `development`, checking representative files by blob SHA on both refs, and reviewing the relevant PR descriptions. It is not a claim that a full local build was run during this audit.

| Extra branch | Disposition | Evidence / remaining caution |
|---|---|---|
| `work/acoustic-studio-lab` | Candidate for deletion after final review | `SpatialAcousticStudioManifest.cs` has the same blob SHA on `development` and this branch. PR #114 says the manifest, GUI builder, tests, catalog/scene registration, and Explore navigation were integrated directly into `development`. |
| `work/city-aerial-traffic-slice` | Candidate for deletion after final review | `SpatialAirTrafficManifest.cs` has the same blob SHA on both refs. PR #112 says the deterministic manifest, tests, renderers, and Explore host integration were consolidated into `development`. |
| `work/digiten-world-slice` | Candidate for deletion after final review | `CityLab.razor` and `Digitens.razor` have identical blob SHAs on both refs. PR #107 says useful implementation, tests, assets, documentation, and integration seams were ported to `development`. |
| `work/moniker-experience-finalization` | Candidate for deletion after final review | `MonikerExperienceMicroBundle.cs` has the same blob SHA on both refs. PR #106 says the moniker/sway work was consolidated into `development`; keep the functionality-preservation ledger as the migration guardrail. |
| `work/development-consolidation` | Candidate for deletion after final review | `MovementLab.razor` and `GesturesRenderer.razor` have identical blob SHAs on both refs. Related PRs #109/#110 describe the movement/replay/ProtocolAi consolidation into `development`. |
| `work/movement-lab-current-development` | Candidate for deletion after final review | Same movement-lab consolidation family as above. Confirm all branch-only test/handoff changes are represented before deleting. |
| `work/gestures-movement-lab` | Candidate for deletion after final review | Same movement-lab consolidation family; branch tip is substantially behind and diverged from current `development`. Confirm its Physics replay and Gestures handoff documents are retained. |
| `work/soldier-field-manifest-experience` | Candidate for deletion after final review | `soldier-part-records.json` and `tests/soldier-field-smoke.mjs` have identical blob SHAs on both refs. PR #105 says Soldier Field assets, manifest wiring, and integration seams were consolidated into `development`. |
| `feature/visual-fit-and-audience` | Retire only after reviewing the landing diff | Its `WelcomeExperience.razor` differs from `development`. The current development version includes additional lifecycle, per-visitor processing-group isolation, cleanup, and control-gating logic. The branch appears stale relative to current development, but this is the one branch for which a representative file is not blob-identical; do not overwrite development with it. |

## Guardrails

- No blind merge of old, divergent branches into current `development`.
- Preserve functionality, tests, assets, handoffs, and documentation—not obsolete branch tips.
- Keep the current development version when it contains newer lifecycle or correctness fixes.
- The repository's `docs/DOCUMENTATION_INDEX.md` and `docs/FUNCTIONALITY_PRESERVATION.md` are the authoritative starting points for architecture and migration context.
- This inventory records evidence and a proposed disposition. The repository owner performs the final branch deletion.


## Follow-up preservation checks (2026-10-16)

A second pass compared additional branch-head files directly with `development` by blob SHA and inspected the content where hashes differ.

- Acoustic Studio: the manifest, GUI builder, integration guide, and focused tests checked in this pass are byte-identical to `development`.
- Aerial Traffic: the manifest, focused tests, and integration guide checked in this pass are byte-identical to `development`.
- Movement/Gestures: `MovementLab.razor`, `GesturesRenderer.razor`, `AiExchangeTextBuilder.cs`, its focused tests, and `docs/integrations/GESTURES_MOVEMENT_LAB.md` checked in this pass are byte-identical to `development`.
- Digiten World: `CityLab.razor`, `Digitens.razor`, `docs/EXPERIENCE_CATALOG.md`, and `docs/SINGULARITY_CITY_LIVING_WORLD.md` checked in this pass are byte-identical to `development`. The branch's `SingularityCitySliceTests.cs` differs only in the spelling of three xUnit predicate assertions; the development version uses the equivalent predicate overload. No branch-only behavior was found in those lines.
- Moniker: both `MonikerExperienceMicroBundle.cs` and `MonikerSwayBehaviorMicroBundle.cs` are byte-identical to `development`. The branch's older `docs/FUNCTIONALITY_PRESERVATION.md` had a more explicit description of the stationary Moniker plus independently selectable kelp-sway behavior. That architecture detail has now been restored to the canonical development ledger in commit `59d53b9`; the older branch version should not replace the newer ledger wholesale.
- Soldier Field: the dossier, research-demonstrations data, research-trial schema, soldier-part records, and smoke test sampled in the earlier pass are byte-identical to `development`.
- Replay Lab: the branch's `ReplayLab.razor` and Physics handoff guide are older than the development versions. Development's page is the current canonical fixture and its guide has a more accurate current integration/navigation description. Do not merge the old page or doc over development.
- Welcome/audience branch: its `WelcomeExperience.razor.cs` disposal implementation is older; development uses guarded disposal and `DestroyMachine()`. The branch's host-manifest test also predates the current destinations, Replay Lab and ProtocolAi Terminal assertions, and unsafe-embed URL validation. Preserve development's version; do not merge the stale test or component code over it.

These checks strengthen the evidence that several work branches are historical staging refs, but they are still a targeted sample, not a complete audit of every file unique to each branch. The final branch-deletion clearance must wait until all branch-only paths and relevant commit history have been reviewed, and any valuable branch-only documentation has been migrated.
