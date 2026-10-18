![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# Singularity Hub Abstractions

This project is the kernel boundary of the Singularity Hub.

It contains only platform-neutral contracts and fixed-depth runtime value types:

- `ISingularityHub` — the public Hub surface.
- `IArbitrator` / `IMicroBundle` — MicroBundle registration and arbitration.
- `OntologySignature` — nine integer runtime layers.
- `IDataWarehouseLiaison` — identity-to-storage boundary.
- `IProcessGroupHost` — lifecycle without execution scheduling.
- `IExecutionProvider` — the opaque boundary where a host chooses Task, Thread, process, GPU queue, or another mechanism.

**Law:** the abstraction project must never acquire platform scheduling or UI dependencies.

---

<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop" aria-label="The Singularity Workshop living avatar">
    <img src="https://raw.githubusercontent.com/TrentBest/WebPage/development/TheSingularityWorkshop/wwwroot/Images/TheSingularityWorkshopLivingAvatar.svg" alt="The Singularity Workshop living avatar — a digital vortex converging on its central eye" width="112" />
  </a><br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em><br />
  <a href="https://github.com/TrentBest">GitHub</a> · <a href="https://www.patreon.com/TheSingularityWorkshop">Support the Workshop</a>
</p>
