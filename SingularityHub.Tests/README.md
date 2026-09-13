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
