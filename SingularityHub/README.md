![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

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

<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop" aria-label="The Singularity Workshop living avatar">
    <img src="https://raw.githubusercontent.com/TrentBest/WebPage/development/TheSingularityWorkshop/wwwroot/Images/TheSingularityWorkshopLivingAvatar.svg" alt="The Singularity Workshop living avatar — a digital vortex converging on its central eye" width="112" />
  </a><br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em><br />
  <a href="https://github.com/TrentBest">GitHub</a> · <a href="https://www.patreon.com/TheSingularityWorkshop">Support the Workshop</a>
</p>
