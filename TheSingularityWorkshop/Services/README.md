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
