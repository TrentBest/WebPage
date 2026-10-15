# Soccer Arena — WebPageLLM Integration Brief

**Audience:** WebPageLLM and the WebPage / Singularity City experience team  
**Status:** integration direction plus a deterministic domain fixture on a separate review branch; not a playable soccer simulation  
**Do not merge, publish, or deploy from this brief without the owner's explicit approval.**

## The experience goal

Singularity City should feel inhabited. The soccer arena is one public, legible event in that city: players make purposeful runs, the ball and score remain understandable, spectators react to meaningful moments, and the venue feels active before, during, and after the match.

The goal is not a decorative sports screen or a set of unrelated player animations. The visible match should be driven by domain state and confirmed outcomes.

## Source work currently available for review

The soccer domain work is staged on a review branch, not yet merged or published:

- [Soccer Domain MicroBundle](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/blob/work/soccer-domain-microbundle/docs/SOCCER_DOMAIN_MICROBUNDLE.md) — field, match readiness, teams, zones, action vocabulary, and authority boundaries.
- [Soccer Match Experience](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/blob/work/soccer-domain-microbundle/docs/SOCCER_MATCH_EXPERIENCE.md) — staged path from readable match decisions to an arena experience.
- [First executable fixture](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/blob/work/soccer-domain-microbundle/docs/SOCCER_MATCH_FIXTURE.md) — scope and limitations of the first deterministic attack.
- [Runnable fixture project](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/tree/work/soccer-domain-microbundle/examples/SoccerMatchFixture) — a small scripted attack used to establish the domain seam.

The fixture currently scripts a pass, shot, and goal outcome. It does **not** implement ball physics, collision/contact, real player locomotion, goalkeeper saves, offside adjudication, crowd simulation, or Renderer/WebPage integration. Do not present it as a functioning match.

## Ownership boundaries

| Concern | Owner |
|---|---|
| Field availability, teams, roles, zones, tactical context | Soccer domain MicroBundle |
| Candidate actions and off-ball objectives | Tactical decision layer |
| Ball movement, contact, possession, rules, score and restarts | Authoritative match simulation |
| Pose evaluation | Gestures |
| Rig mapping, scene rendering and actual visual presentation | Renderer / WebPage host integration |
| Camera, scoreboard, crowd cues and commentary timing | Presentation director responding to confirmed match events |

A completed pose is not proof of a pass, possession, foul, offside, or goal. WebPage presentation must not independently invent those outcomes.

## What WebPageLLM should prepare

1. **A clear arena destination** in the city experience, with a camera/scene boundary that can later host the live pitch.
2. **An experience shell, not fake gameplay:** venue identity, pitch/goal/stand layout, match HUD concept, and clear placeholder status until real match state is connected.
3. **An event-driven presentation seam:** prepare the UI to react to a future typed match-event stream or snapshot adapter. Event names in the domain documents are proposals, not a published API contract.
4. **A readable first attack:** show carrier, support player, defenders, ball, attack direction, clock and score. Make off-ball intent legible; support players should create options instead of all chasing the ball.
5. **Honest status labeling:** distinguish designed, scripted fixture, integrated, and simulated behavior.
6. **A crowd/venue reaction plan:** crowd intensity, camera changes, celebration, scoreboard changes, and restart staging should follow confirmed match outcomes. A goal celebration follows a confirmed goal, not a shot animation.

## First experience acceptance checks

- A visitor can identify the two teams, goals, ball, attacking direction, score, and match phase.
- At least one support player has an independent objective away from the ball carrier.
- Defenders respond to the attack rather than merely following a canned animation.
- The score only changes after the authoritative match result says a goal was awarded.
- An attempted pass/shot is distinguishable from a completed pass or confirmed goal.
- If the match data is unavailable, the UI says so rather than manufacturing state.
- The experience is explicit about which elements are placeholders.

## Suggested sequence

1. Review and align with the domain fixture.
2. Create the arena shell and presentation vocabulary without claiming live simulation.
3. Agree on a host-facing snapshot/event contract before binding UI to invented types.
4. Replace scripted outcomes with authoritative simulation outcomes as the match engine becomes available.
5. Connect Gestures and a real rig adapter only after the host contract is agreed.
6. Add crowd, camera, scoreboard and restart reactions driven by confirmed match state.
7. Validate the full path in the browser before describing it as a live match.

This brief is a handoff, not authorization to publish or deploy. Keep the WebPage branch separate from the MicroBundleDomain review branch until the integration contract is agreed.
