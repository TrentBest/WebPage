# Soccer Arena — WebPageLLM Integration Brief

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Audience:** WebPageLLM and the WebPage / Singularity City experience team  
**Status:** integration direction plus a deterministic domain fixture on a separate review branch; not a playable soccer simulation  
**Do not merge, publish, or deploy from this brief without the owner's explicit approval.**

## The experience goal

Singularity City should feel inhabited. The soccer arena is one public, legible event in that city: players make purposeful runs, the ball and score remain understandable, spectators react to meaningful moments, and the venue feels active before, during, and after the match.

The goal is not a decorative sports screen or a set of unrelated player animations. The visible match should be driven by domain state and confirmed outcomes.

### Digitens are active arena participants

Digitens who arrive at the stadium to watch are not static background props. The arena supplies a spectator-gesture vocabulary that they can participate in: arriving and navigating to a stand, moving through aisles and finding a place, sitting in the bleachers, standing to get a better view where appropriate, turning attention toward the live play, reacting to near chances, cheering when meaningful events occur, and celebrating especially when a goal is authoritatively awarded. Their activity should be varied and individually situated, not a single crowd animation played by everyone at once.

The arena/venue provides the available gestures and environmental constraints; each Digiten's own state and context determine which gesture is appropriate and when. A Digiten can be arriving, finding a seat, seated and watching, moving temporarily, reacting, cheering, or settling again. Crowd reactions should be event-driven and staggered across people and sections. A shot attempt can prompt attention or anticipation; a save, miss, or confirmed goal should produce distinct responses. The score and goal celebration still follow match authority, not spectator animation. This handoff describes desired behavior, not a claim that spectator locomotion, seating, or crowd simulation is already implemented.

## Source work currently available for review

- [Soccer Arena Economy](https://github.com/TrentBest/WebPage/blob/work/soccer-arena-handoff/docs/integrations/SOCCER_ARENA_ECONOMY.md) — ticket inventory and admission, food and beverage concessions, merchandise, memorabilia, vendor operations, transaction invariants, and a staged simulated-economy milestone.

The soccer domain work is staged on a review branch, not yet merged or published:

- [Soccer Domain MicroBundle](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/blob/work/soccer-domain-microbundle/docs/SOCCER_DOMAIN_MICROBUNDLE.md) — field, match readiness, teams, zones, action vocabulary, and authority boundaries.
- [Soccer Match Experience](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/blob/work/soccer-domain-microbundle/docs/SOCCER_MATCH_EXPERIENCE.md) — staged path from readable match decisions to an arena experience.
- [First executable fixture](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/blob/work/soccer-domain-microbundle/docs/SOCCER_MATCH_FIXTURE.md) — scope and limitations of the first deterministic attack.
- [Runnable fixture project](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/tree/work/soccer-domain-microbundle/examples/SoccerMatchFixture) — a small scripted attack used to establish the domain seam.

The fixture currently scripts a pass, shot, and goal outcome. It does **not** implement ball physics, collision/contact, real player locomotion, goalkeeper saves, offside adjudication, crowd simulation, or Renderer/WebPage integration. Do not present it as a functioning match.

The fixture also now projects authoritative fixture events into individual spectator intents in [SpectatorReactionDirector.cs](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/blob/work/soccer-domain-microbundle/examples/SoccerMatchFixture/SpectatorReactionDirector.cs). Each reaction carries the Digiten identity, stand section, seat label, triggering event, gesture intent, and per-person presentation-delay metadata. The seat label is an anchor key for a future venue adapter—not a world-space position—and the delay is data, not a running timer. The fixture validates that all five identities and seats remain distinct and that celebration is triggered only by `GoalAwarded`.

**Next host seam:** resolve each seat label through the arena's explicit venue/seat model; route the resulting reaction intent to the Digiten's own gesture/pose pipeline; let Renderer/WebPage present the result. Keep the match event authoritative, keep each Digiten's task/state independent, and do not invent a public Renderer rig API to bridge the gap. Until a real host contract and adapter exist, this remains a deterministic domain-to-presentation projection—not live crowd animation or a scheduled reaction system.


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
7. **A participant gesture plan:** Digitens who come to watch should use arena-provided gestures to enter, navigate stands, sit in bleachers, watch the match, and react. Design individual variation and section-level waves rather than synchronized crowd puppetry. Gesture availability comes from the arena; individual timing comes from each Digiten's context; reactions to match events are triggered by authoritative outcomes.

## First experience acceptance checks

- A visitor can identify the two teams, goals, ball, attacking direction, score, and match phase.
- At least one support player has an independent objective away from the ball carrier.
- Digitens can be represented as active spectators with distinct arrival, seating, watching, and reaction states; crowd reactions vary across individuals and follow confirmed match events.
- A goal produces a stronger, clearly event-linked cheer and celebration than a routine pass, while misses and saves have distinct reactions.
- Spectator gestures do not change match state or manufacture a scoring outcome.
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
6. Add spectator gesture participation and crowd reactions, with individual/section variation driven by confirmed match state.
7. Add camera, scoreboard and restart reactions driven by confirmed match state.
8. Validate the full path in the browser before describing it as a live match.

This brief is a handoff, not authorization to publish or deploy. Keep the WebPage branch separate from the MicroBundleDomain review branch until the integration contract is agreed.

## Singularity City is the primary showcase

The arena is not a standalone sports page. The city itself is the main WebPage experience: people should see a connected future city, understand that Digitens live and move through it, and then discover the arena as one destination within that shared world. The water-world/physics showcase is adjacent work, not a reason to delay the city.

### Implementation order

1. **Make the existing city legible first.** Preserve the canonical structure IDs and footprints in `SingularityCityCatalog`; the Workshop remains the stable anchor. Selecting the soccer arena should lead into its venue experience without creating a second, unrelated world model.
2. **Connect citizens to real activity state.** Use the existing spatial Digiten/crowd model and movement contracts. Show a citizen's location and activity only when those values come from the model; do not animate decorative dots and label them autonomous citizens.
3. **Connect ground transit and aerial traffic as separate layers.** The current taxi and transit manifests are declarative definitions, not proof that vehicles are being simulated or rendered in the city. A flying-car layer needs stable vehicle identity, route/origin/destination, altitude or lane, and reproducible update rules. Keep air lanes distinct from ground movement, and expose the actual manifest/state to the renderer instead of hard-coding an unrelated stream of moving icons inside the GUI builder.
4. **Make the arena a city destination.** Arrival routes, transit/drop-off, entrances, concourses, stands, and field should share the same spatial identity. Crowd counts and seating states must come from an explicit venue/crowd model, not a guessed fill percentage.
5. **Compose match authority before showing live play.** The current MicroBundleDomain fixture scripts a pass, shot, and goal. Until a real match contract supplies authoritative snapshots/events, the venue may show a clearly labelled static layout or scripted fixture, but it must not claim live ball physics, player locomotion, a real score, or crowd reactions to authoritative events.
6. **Use Gestures for action vocabulary and Renderer for presentation.** Gesture requests are not match outcomes. A pose, animation, or crowd reaction must not independently award a goal or change possession.

### City-first acceptance checks

- The arena's stable ID resolves to the same structure when viewed from the city and when entering the venue.
- Ground transit, flying vehicles, and Digitens have explicit, inspectable model/state sources; the visual layer does not manufacture autonomous behavior.
- Air traffic is visibly separated in altitude/lane from street-level movement and is deterministic enough for repeatable tests.
- Arena visitors can be traced through arrival, entrance, stand/seat, watching, reaction, and departure states as those states become implemented.
- Scoreboard and goal celebration change only in response to an authoritative match result.
- Every experience labels the difference between catalogued/design-only, scripted fixture, integrated, and simulated behavior.

This order deliberately prioritizes a city that feels connected over a polished isolated arena mock-up. It is an integration plan, not a claim that these acceptance checks already pass.

