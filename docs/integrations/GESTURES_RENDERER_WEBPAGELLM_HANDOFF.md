# Gestures / Renderer — WebPageLLM Integration Brief

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Audience:** WebPageLLM and anyone implementing the Workshop's public-facing experience  
**Status:** integration plan; no Gestures-to-Renderer runtime adapter or browser demonstration is claimed complete  
**Source:** Gestures PR #10, branch `work/planar-ik-reference`; Renderer `master`; WebPage `development`  
**Do not merge, publish, or deploy from this brief.**

## Purpose

Bring the Workshop's Gestures work into view without presenting a mock as working technology. Gestures is an engine-neutral .NET model and CPU reference evaluator for physical procedures. Renderer is the observer-aware presentation system. WebPage is the host and learning surface where people should be able to inspect what is implemented, what is experimental, and what remains unconnected.

The visitor should learn one central distinction:

**A Gesture describes and evaluates an intended procedure. It does not prove that the physical action succeeded.**

A pose that reaches a handhold is not evidence of contact; contact is not proof of load-bearing support; and a completed time-based evaluation does not prove that a soldier boarded a vehicle. Physics and the owning domain must provide those outcomes.

## What exists in Gestures now

Repository: [TheSingularityWorkshop.Gestures](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/tree/work/planar-ik-reference)  
Current integration review: [PR #10](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/pull/10)

The branch contains an engine-neutral `GestureDefinition`, normalized keyframes, named local `PoseTransform` values, deterministic bounded variation, and a CPU `GestureEvaluator` that returns an `EvaluatedPose`. The pose exposes normalized progress, completion of the requested time interval, name-keyed local transforms, and a root presentation offset.

It also includes test-backed examples for soldier movement and provider-owned vehicle boarding, a provisional playback policy, initial linear-translation golden vectors, planar two-link IK research, and movement-course model work. These are separate levels of maturity; do not present all of them as one finished locomotion or physics system.

The current CI validation recorded for PR #10 passed build, tests, and package-artifact creation. This does not establish GPU parity, physical validity, or a published NuGet package. Gestures is not to be published as part of this task.

## Renderer boundary: do not invent a skeletal API

Renderer currently accepts a `RenderFrame` containing a `RenderView` and draw commands. Each draw command has resource IDs, an already-resolved world transform, and draw parameters. The current public contract does not expose a rig, named skeleton nodes, or a skeletal-pose input slot.

Therefore, do not simply pass `EvaluatedPose` to Renderer or convert its named local transforms into arbitrary draw commands. A real host/rig adapter must first define the rig-node mapping, parent composition, coordinate convention, root-offset policy, and missing-name behavior. The adapter must then target a representation Renderer actually supports. If that representation does not yet exist, surface the missing seam as a planned integration—not as a working feature.

Keep the dependency direction clean: Gestures must not depend on Renderer, a graphics API, Unity, or WebPage.

## What WebPage should do now

1. **Make the work discoverable from the existing Workshop hub.** Prefer an entry in the existing hub/showcase convention; do not create a competing navigation system.
2. **Provide a truthful Gestures / Movement Lab page or panel.** Explain the data flow: provider-authored procedure → CPU evaluation → evaluated local pose → consumer/rig adapter → observer-aware presentation.
3. **Separate maturity states.** Mark CPU evaluation and the specific tests/examples as implemented on the referenced branch; mark host rig mapping, Renderer pose interchange, GPU evaluation/parity, and physics-backed outcomes as not yet integrated or planned.
4. **Do not fake a live Gesture.** Do not hand-code a visually convincing motion and label it as output from `GestureEvaluator`. Until WebPage can call the actual .NET evaluator through a real project/runtime boundary, show the architecture, source links, and status. A separately approved static diagram is fine when labeled as a diagram.
5. **If implementing a live sample later, use the real evaluator.** Start with a tiny fixed-time translation fixture and show its actual normalized progress and evaluated transform values. Keep the fixture clearly identified as a numerical reference test, not a physical simulation.
6. **Keep physical outcomes external.** Do not add UI implying contact, grip, support, collision, successful boarding, or course completion unless the relevant domain/physics source supplies and validates that evidence.
7. **Keep the page light and accessible.** Load any future interactive sample only when opened; provide keyboard access, readable status labels, and a source/test link beside the claim.
8. **Respect the existing WebPage interaction boundary.** Do not introduce direct LLM-provider calls. The Workshop's ProtocolAi terminal uses its explicit clipboard handoff; this brief is documentation for implementation, not permission to bypass that boundary.

## Proposed first visible experience

A compact panel is sufficient:

- **Procedure:** a named example such as vehicle boarding, with provider-owned anchors called out.
- **Evaluation:** a progress control only when backed by the actual evaluator; otherwise show a static data-flow diagram.
- **Pose output:** named local transforms and root offset, labeled as presentation data.
- **Evidence boundary:** a visible statement that evaluation completion is not physical success.
- **Integration status:** `CPU reference: implemented`; `host rig mapping: pending`; `Renderer pose contract: pending`; `GPU parity: unmeasured`; `physics/domain outcome: external`.
- **Source trail:** links to Gestures, its open PR, the [Renderer pose-handoff proposal](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/work/planar-ik-reference/docs/RENDERER_POSE_HANDOFF_PROPOSAL.md), and the relevant tests.

## Acceptance checks

- The visitor can reach the panel from the existing hub.
- No simulated motion is misrepresented as evaluator output.
- No current Renderer rig/skeleton API is implied.
- Physical contact/support and domain success remain explicitly external.
- Source links and maturity labels agree with the referenced branch.
- Existing landing and living-GUI behavior remain unchanged.
- No package publication, merge, Azure publish, or deployment is performed.

## What WebPageLLM should report back

List the exact files changed, how to launch the page locally, what to click for review, and any missing runtime/project boundary that prevents a real evaluator-backed sample. Leave release and deployment untouched.
