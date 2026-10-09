# The Singularity Workshop — WebPage

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![FSM_API](https://img.shields.io/badge/FSM_API-1.0.13-00A98F?style=flat-square)](https://github.com/TrentBest/FSM_API)
[![Tests](https://img.shields.io/badge/tests-GitHub%20Actions-f39c12?style=flat-square&logo=githubactions&logoColor=white)](https://github.com/TrentBest/WebPage/actions)

> The page opens the door. The Experience gives you somewhere to stand. The machinery lets you look underneath it.

## Why WebPage exists now

WebPage is the browser proving ground and public learning resource for The Singularity Workshop.

There are two valid ways to enter:

- Visit the WebPage to witness the technology working.
- Visit the repository to understand how and why it works.

Those two paths should teach the same architecture.

> Show the behavior. Preserve the semantics. Explain the machinery.

## The first lesson: the host is not the machinery

WebPage is a host. It consumes the Workshop's reusable packages rather than becoming a second implementation of them.

~~~text
FSM_API
   ↓
state execution

MicroBundleDomain
   ↓
MicroBundle contract

FSM_COS
   ↓
composition / dependency closure / arbitration

WebPage
   ↓
manifest + browser host + presentation

browser / renderer
   ↓
visible Experience
~~~

When a capability becomes reusable, the implementation belongs in the package or domain that owns it. WebPage consumes that result and proves it in a real browser.

## The current startup proof

The host manifest is the startup authority.

~~~text
WebPage loads manifest
        ↓
startup Experience(s)
        ↓
Workshop Moniker
        ↓
primary running Experience
        ↓
Living GUI
        ↓
FSM_API-driven behavior
        ↓
browser manifestation
~~~

The current manifest and active Experience composition are documented in [Current Vertical Slice](docs/CURRENT_VERTICAL_SLICE.md).

## Public demonstration / Brave1 MVP

The public landing experience should explain the technology before asking visitors to inspect the repository. The [Brave1 Public Demonstration guide](docs/BRAVE1_PUBLIC_DEMONSTRATION.md) defines the evaluator journey, current-versus-future claim boundary, and the checks required before submitting a deployed URL.

## Documentation and theory

The repository deliberately separates the public orientation from the engineering documentation.

- [Documentation Index](docs/DOCUMENTATION_INDEX.md) — complete map.
- [Learning Path](docs/LEARNING_PATH.md) — progressive route through the repository.
- [Repository Map](docs/REPOSITORY_MAP.md) — where responsibilities live.
- [Usage](docs/USAGE.md) — how to run and consume WebPage.
- [Theory](docs/THEORY.md) — concepts and architectural reasoning behind WebPage.
- [Architecture](docs/WEBPAGE_EXPERIENCE_ARCHITECTURE.md) — host and Experience model.
- [Runtime Architecture](docs/WORKSHOP_RUNTIME_ARCHITECTURE.md) — runtime responsibility boundaries.
- [FSM_COS Usage](docs/FSM_COS_USAGE.md) — how WebPage consumes the external composition package.
- [Package Ecosystem](docs/PACKAGE_ECOSYSTEM.md) — what WebPage consumes and why.
- [Documentation Standard](docs/DOCUMENTATION_STANDARD.md) — how this repository keeps documentation useful and honest.
- [FSM_COS Proving-Ground Migration](docs/PROVING_GROUND_MIGRATION.md) — extraction rules, responsibility boundaries, startup flow, and branch discipline.

### External abstractions

WebPage documents **how it uses** an external abstraction, not the external abstraction's entire domain.

For example, WebPage explains its use of FSM_API, FSM_COS, GUI, ProtocolAi, GrammarAi, and MicroBundleDomain at the integration boundary. The authoritative theory and API documentation for those packages remains in their own repositories.

~~~text
WebPage
  │
  ├── documents its own domain
  │
  ├── documents how external packages are configured/consumed
  │
  └── points to the owning package for that package's
      complete documentation and theory
~~~

## The extraction rule

Useful functionality will naturally appear in a proving ground first.

That is fine.

The permanent path is:

~~~text
experiment in WebPage
        ↓
identify reusable responsibility
        ↓
move implementation to owning package/domain
        ↓
test and document the package
        ↓
consume the package here
        ↓
prove the integration
~~~

This keeps WebPage from becoming a giant application that merely happens to contain the Workshop's technology.

## Current / Direction / Future

Every document and major code path should distinguish:

- **Current** — implemented and observable.
- **Direction** — actively being migrated or established.
- **Future** — deliberately not part of today's contract.

Old experiments remain useful evidence, but they are not automatically architecture.

## Build it

~~~bash
git clone https://github.com/TrentBest/WebPage.git
cd WebPage
dotnet restore
dotnet run
~~~

Active engineering occurs on development. master is the stable promotion target.

Do not publish packages or releases without explicit approval.

## Quality bar

- FSM_API owns meaningful state-machine execution.
- FSM_COS owns composition, dependency closure, arbitration, and RuntimeAssembly creation.
- NuGet packages own reusable capabilities.
- WebPage owns browser-specific presentation and integration.
- Tests prove architectural claims.
- Visual behavior has visual evidence.
- Documentation explains both how and why.
- Zero warnings and zero avoidable errors is the target.

> Can a visitor experience it, can a developer understand it, and can the architecture tell us who owns it?

---

*This way leads to the Singularity.*

*Built by The Singularity Workshop.*
