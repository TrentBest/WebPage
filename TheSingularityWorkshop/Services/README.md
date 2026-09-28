# Page Experience Services

This folder contains runtime services behind the active web experience.

## Landing ownership

`PageStateContext`
: Owns mutable experience data: the living node population, exact critical mass, freeze state, moniker gate, and gravity data.

`PageFSM`
: Owns the deterministic state sequence using FSM_API. Each instance owns a private processing group so updating one page FSM cannot advance another.

`FSMManagerService`
: Bridges the application/component lifetime to the page FSM and exposes current context to presentation components.

## Hard rule

The reveal is data-driven:

```text
LivingNodes.Count < 100  => MonikerReady == false
LivingNodes.Count == 100 => freeze + MonikerReady == true
```

Do not move that gate into CSS, JavaScript, or a Razor counter. The UI may decide how the reveal looks, not when it happens.

See `../../WORKSHOP_PRESENTATION_CONTRACT.md` for the full sequence.


---

## 🔗 The Singularity Workshop

This project is part of a deliberately troublesome ecosystem:

- **[FSM_API](https://github.com/TrentBest/FSM_API)** — behavior and state.
- **[FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS)** — composition and runtime assembly.
- **[FSM_Serialization](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization)** — representation and the byte boundary.
- **[WebPage](https://github.com/TrentBest/WebPage)** — browser manifestation and proving ground.
- **[FSM_API_Unity](https://github.com/TrentBest/FSM_API_Unity)** — Unity manifestation.

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
