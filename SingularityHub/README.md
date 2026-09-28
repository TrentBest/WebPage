# Singularity Hub

`SingularityHub` is the first concrete implementation of the Hub abstraction.

The Hub owns:

1. MicroBundle registration.
2. Deterministic, maximum-ten-round arbitration.
3. Process Group lifecycle tracking.
4. The abstract liaison to warehouse identity.

It does **not** own hardware scheduling. Eligible work crosses `IExecutionProvider`, where the host decides how computational resources are acquired.

The project is intentionally inside WebPage for the first working deployment. It can later move to its own repository without changing the kernel contract.


---

## 🔗 The Singularity Workshop

This project is part of a deliberately troublesome ecosystem:

- **[FSM_API](https://github.com/TrentBest/FSM_API)** — behavior and state.
- **[FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS)** — composition and runtime assembly.
- **[FSM_Serialization](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization)** — representation and the byte boundary.
- **[WebPage](https://github.com/TrentBest/WebPage)** — browser manifestation and proving ground.
- **[FSM_API_Unity](https://github.com/TrentBest/FSM_API_Unity)** — Unity manifestation.

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
