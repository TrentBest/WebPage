# SingularityHub.Tests

## Digital test nomenclature

Every behavioral test has a stable digital address:

`L.GG.TTT`

- `L` = architectural layer.
- `GG` = test group / production contract.
- `TTT` = individual test within that contract.

The address is deliberately independent of source-file names and test-runner order. It is the number we use when discussing a specific test, for example `4.03.005`.

## One-to-one test ownership

Behavioral test classes follow the production type they test. A production type gets one corresponding test class, and that class owns the complete public surface of that type. Do not create `*DomainTests` aggregation classes that mix unrelated production types.

Interfaces and value types are production contracts too; they receive the same one-to-one treatment when they have public behavior or surface that must be verified.

## Version heartbeat

`IncrementalVersionTests.cs` is the **only** incremental-version test class in this project.

Each repository commit that advances the architecture adds exactly one new version heartbeat to that class:

```csharp
[ArchitectureTest(0, 0, 77)]
[Fact(DisplayName = "V0.0.77 — Incremental_Heartbeat")]
public void V0_0_77() => Assert.Equal("0.0.77", "0.0.77");
```

The version string is intentionally boring. It records the version claimed by the commit. It does not replace behavioral tests, and behavioral tests must not be moved into the version ledger.

There must not be another `IncrementalVersion###Tests.cs` class.


---

## 🔗 The Singularity Workshop

This project is part of a deliberately troublesome ecosystem:

- **[FSM_API](https://github.com/TrentBest/FSM_API)** — behavior and state.
- **[FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS)** — composition and runtime assembly.
- **[FSM_Serialization](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization)** — representation and the byte boundary.
- **[WebPage](https://github.com/TrentBest/WebPage)** — browser manifestation and proving ground.
- **[FSM_API_Unity](https://github.com/TrentBest/FSM_API_Unity)** — Unity manifestation.

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
