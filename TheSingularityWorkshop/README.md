![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

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

<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop" aria-label="The Singularity Workshop living avatar">
    <img src="https://raw.githubusercontent.com/TrentBest/WebPage/development/TheSingularityWorkshop/wwwroot/Images/TheSingularityWorkshopLivingAvatar.svg" alt="The Singularity Workshop living avatar — a digital vortex converging on its central eye" width="112" />
  </a><br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em><br />
  <a href="https://github.com/TrentBest">GitHub</a> · <a href="https://www.patreon.com/TheSingularityWorkshop">Support the Workshop</a>
</p>
