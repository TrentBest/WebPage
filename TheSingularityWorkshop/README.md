# TheSingularityWorkshop Application

This is the Blazor/WebAssembly manifestation of the current Workshop web experience.

## Current responsibilities

- render the gateway and landing experience;
- host the FSM_API-driven page state machine;
- manifest the living GUI swarm;
- transition through critical mass, moniker reveal, gravity, and later navigation;
- host the current Pong and AI experiments without turning the landing into a generic static marketing page.

## Read before editing

- `../CURRENT_VERTICAL_SLICE.md`
- `../WORKSHOP_PRESENTATION_CONTRACT.md`
- `Services/README.md`

The application does not own the landing sequence in Razor. `PageStateContext` and `PageFSM` do. Presentation code should observe and manifest state rather than invent timers or duplicate population counters.
