# WebPage Theory

## 1. WebPage is a proving ground, not the Workshop

A proving ground exists to make an architectural idea observable. WebPage should be rich enough to demonstrate the ecosystem, but disciplined enough that reusable behavior migrates downward into the package that owns it.

## 2. Manifest before implementation detail

The manifest is the host's declared composition request. It describes what the host wants to make available; it should not become a hidden second source of truth in individual pages.

~~~text
declared composition
       ↓
composition
       ↓
assembled capability
       ↓
host manifestation
       ↓
observable experience
~~~

## 3. Composition is not manifestation

FSM_COS assembles. WebPage manifests.

The browser, Blazor components, presentation CSS, and host lifecycle belong to WebPage. Dependency closure, arbitration, and RuntimeAssembly creation belong to FSM_COS. State-machine execution belongs to FSM_API.

Keeping those responsibilities distinct means the same composition can eventually be manifested by another host.

## 4. Experience is the human-facing consequence

An Experience is not merely a page. It is the encounter produced when composed capabilities are given a host, a presentation boundary, and a user.

~~~text
domain capability
      +
composition
      +
presentation
      =
Experience
~~~

## 5. Perception is a boundary

The Rendering work demonstrates an important Workshop principle: what an observer can perceive should be separable from what the complete simulation knows.

A renderer can expose a restricted observation. ProtocolAi and GrammarAi can then operate on that observation rather than receiving privileged hidden state.

That boundary matters because it allows an AI or other consumer to reason from evidence instead of omniscience.

## 6. External theory belongs to external packages

WebPage can explain why it uses FSM_API, FSM_COS, MicroBundleDomain, ProtocolAi, GrammarAi, GUI packages, and other dependencies. It should not duplicate their complete theories.

The owning repository remains authoritative. WebPage provides the bridge:

~~~text
external package theory
        ↓
WebPage integration contract
        ↓
WebPage behavior
~~~

## 7. Extraction is architectural hygiene

When a capability becomes independently reusable, extracting it is not merely code cleanup. It is evidence that the abstraction has found its proper home.

WebPage can prototype. The package ecosystem should own the durable abstraction.

---

*The purpose of the proving ground is to make the architecture visible enough that the next abstraction can be placed correctly.*
