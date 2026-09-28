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


---

## 🔗 The Singularity Workshop

This project is part of a deliberately troublesome ecosystem:

- **[FSM_API](https://github.com/TrentBest/FSM_API)** — behavior and state.
- **[FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS)** — composition and runtime assembly.
- **[FSM_Serialization](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization)** — representation and the byte boundary.
- **[WebPage](https://github.com/TrentBest/WebPage)** — browser manifestation and proving ground.
- **[FSM_API_Unity](https://github.com/TrentBest/FSM_API_Unity)** — Unity manifestation.

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
