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

## 🔗 The Singularity Workshop

This project is part of a deliberately troublesome ecosystem:

- **[FSM_API](https://github.com/TrentBest/FSM_API)** — behavior and state.
- **[FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS)** — composition and runtime assembly.
- **[FSM_Serialization](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization)** — representation and the byte boundary.
- **[WebPage](https://github.com/TrentBest/WebPage)** — browser manifestation and proving ground.
- **[FSM_API_Unity](https://github.com/TrentBest/FSM_API_Unity)** — Unity manifestation.

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
