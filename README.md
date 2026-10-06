# The Singularity Workshop — WebPage

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![FSM_API](https://img.shields.io/badge/FSM_API-1.0.13-00A98F?style=flat-square)](https://github.com/TrentBest/FSM_API)
[![Tests](https://img.shields.io/badge/tests-GitHub%20Actions-f39c12?style=flat-square&logo=githubactions&logoColor=white)](https://github.com/TrentBest/WebPage/actions)

> The page opens the door. The Experience gives you somewhere to stand. The machinery lets you look underneath it.

## Why WebPage exists now

WebPage started as a practical website around an earlier distribution requirement. It became much more useful than that.

It is now the browser proving ground and public learning resource for The Singularity Workshop.

There are two valid ways to enter:

- Visit the WebPage to witness the technology working.
- Visit the repository to understand how and why it works.

Those two paths should teach the same architecture.

> Show the behavior. Preserve the semantics. Explain the machinery.

## The first lesson: the host is not the machinery

WebPage is a host. It should consume the Workshop's reusable packages rather than quietly becoming a second implementation of them.

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

The current manifest requests LivingGuiExperienceMicroBundle 2102 as the primary root.

That MicroBundle declares MonikerMicroBundle 2110 as its dependency.

Therefore the correct FSM_COS request is:

~~~text
RuntimeManifest
    root = 2102
        ↓
FSM_COS
    ↓
dependency closure
    ↓
2110 + 2102
    ↓
RuntimeAssembly
~~~

WebPage must not manually request the Moniker as a second root merely because the browser presents it first.

That is the lesson.

## What to learn

Start with LEARNING_PATH.md, then use REPOSITORY_MAP.md to navigate the source.

| Question | Start here |
|---|---|
| Why does WebPage exist? | this README |
| How do I learn the architecture? | LEARNING_PATH.md |
| Where is everything? | REPOSITORY_MAP.md |
| Who owns what? | WORKSHOP_RUNTIME_ARCHITECTURE.md |
| How does WebPage call FSM_COS? | FSM_COS_USAGE.md |
| What is an Experience? | EXPERIENCE_THEORY.md |
| How does MicroBundle arbitration work? | MICROBUNDLE_ARBITRATION_MAP.md |
| What is currently being proven? | CURRENT_VERTICAL_SLICE.md |
| How do we keep docs honest? | DOCUMENTATION_STANDARD.md |

The full index remains DOCUMENTATION_INDEX.md.

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

Every document and major code path must distinguish:

- Current — implemented and observable.
- Direction — actively being migrated or established.
- Future — deliberately not part of today's contract.

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