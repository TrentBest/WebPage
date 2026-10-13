# WebPage Dependency Workflow

This document is the operational source of truth for how WebPage resolves Workshop dependencies during development and release.

## The two modes

- **Repository-source mode (development):** WebPage builds against checked-out project source from each dependency repository's `development` branch. This makes dependency changes immediately testable without publishing a NuGet package for every iteration.
- **NuGet mode (release and independent verification):** WebPage restores explicit, versioned packages from nuget.org. This proves the dependency graph that other consumers will actually receive.

These modes must remain opt-in and mutually exclusive for each dependency. A project must never receive both a `ProjectReference` and a `PackageReference` for the same library.

A `ProjectReference` does **not** clone a repository or select its branch. The developer or CI setup must check out the expected repositories and branches before building. Missing source must fail clearly; it must not silently use an old package or a different branch.

## Dependency hierarchy

The arrows below point from a consumer toward what it may consume. Lower-level packages must not depend on WebPage.

```text
WebPage browser host
  ├── SingularityHub (WebPage-local host boundary)
  ├── FSM_COS ───────────────┐
  │    ├── MicroBundleDomain │
  │    └── FSM_API           │
  ├── MicroBundleDomain      │
  ├── FSM_API                │
  ├── GUI.Blazor             │
  ├── ProtocolAi             │
  └── GrammarAi              │
                             ▼
                Browser presentation / runtime
```

This is a dependency-orientation map, not a claim that every package depends directly on every other package. The authoritative package contracts and transitive dependencies belong to their owning repositories.

## Direct WebPage package baseline

The active browser host project is `TheSingularityWorkshop/TheSingularityWorkshop.csproj`. Its current direct Workshop package versions are:

| Package | Current NuGet version | Development source repository |
|---|---:|---|
| `TheSingularityWorkshop.FSM_API` | `1.0.13` | [FSM_API](https://github.com/TrentBest/FSM_API) |
| `TheSingularityWorkshop.MicroBundleDomain` | `1.0.1` | [MicroBundleDomain](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain) |
| `TheSingularityWorkshop.FSM_COS` | `0.1.0-alpha.6` | [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) |
| `TheSingularityWorkshop.GUI.Blazor` | `0.1.0-alpha.8` | [GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI) |
| `TheSingularityWorkshop.ProtocolAi` | `0.1.0-alpha.2` | [ProtocolAi](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi) |
| `TheSingularityWorkshop.GrammarAi` | `0.1.0-alpha.2` | [GrammarAi](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi) |

Each listed source repository has a `development` branch. Before adding or changing a source reference, verify the exact project path, target framework, and any transitive source dependencies in that repository. Do not infer a project path from the repository name.

`SingularityHub` is a WebPage-local project and is referenced by project path; it is not a NuGet dependency.

## Workspace expectations

A source-mode workspace must contain WebPage and each selected source dependency as separate checked-out repositories. Keep sibling locations predictable and document the expected relative paths next to the build switch. Source mode must validate all required project files before restore/build.

Do not make source mode the default. A normal checkout must continue to restore the pinned NuGet packages without requiring sibling repositories.

## Release and CI checks

Before a release or promotion:

1. Run the normal NuGet-only restore/build/test path from a clean workspace.
2. Confirm restore uses nuget.org and no locally packed replacement can satisfy a Workshop package.
3. Build the browser host and run the relevant integration and browser smoke tests.
4. Verify the pinned package versions and the dependency order against the current package owners.
5. Keep package publication and deployment disabled unless explicitly authorized.

Source mode and NuGet mode are separate proofs. A successful source build does not prove that published packages are compatible; a successful package build does not prove that current dependency source integrates.

## Current implementation status

This document establishes the hierarchy and required workflow. The existing browser-host project currently uses versioned NuGet references for its Workshop libraries and a local project reference to `SingularityHub`. Do not assume source-mode switching is already implemented: the next code change must add an explicit opt-in switch, validate every source path, and prove both modes in CI before the workflow can be considered complete.

## Related guides

- [Repository Map](REPOSITORY_MAP.md) — local source ownership and dependency direction.
- [Package Ecosystem](PACKAGE_ECOSYSTEM.md) — why each Workshop package is present.
- [Current Vertical Slice](CURRENT_VERTICAL_SLICE.md) — active WebPage startup proof.
- [Renderer source-development guide](https://github.com/TrentBest/TheSingularityWorkshop.Renderer/blob/development/docs/REPOSITORY-SOURCE-DEVELOPMENT.md) — established Workshop precedent for source/NuGet mode separation.
