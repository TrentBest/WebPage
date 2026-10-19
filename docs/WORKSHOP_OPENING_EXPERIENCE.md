# Workshop Opening Experience

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


The public landing page is the Workshop's perception boundary.

It is also the first practical lesson in how the architecture works.

## Current opening contract

The host manifest is loaded first.

~~~text
WEBPAGE BOOT
   ↓
manifest + FSM_COS composition
   ↓
opening statement: “If an image is worth a thousand words…”
   ↓
opening question: “How many words is a living image worth?”
   ↓
visitor entry gateway
   ↓
Living GUI population
   ↓
Moniker reveal
   ↓
gravity + three-second presentation
   ↓
manifest-driven Workshop Hub
   ↓
living Moniker owns the primary panel by default
   ↓
visitor selects THE WORKSHOP, ANYAPP, or SUPPORT
   ↓
only the selected destination replaces the Moniker
~~~

The manifest declares one startup Moniker, the primary Living GUI Experience, and the visitor-facing hub destinations AnyApp and Support.

When the hub arrives, the Moniker remains the selected primary-panel experience. No destination is activated automatically. The upper-left tabs are explicit visitor choices; selecting AnyApp or Support replaces the Moniker panel until the visitor chooses another tab.

The model permits multiple startup Experiences. They are presentation surfaces. The primary running Experience remains a separate manifest concern.

## Why this matters

The Moniker is visible first, but WebPage does not manually compose the Moniker as an unrelated root.

The primary root is LivingGuiExperienceMicroBundle 2102.

That MicroBundle declares MonikerMicroBundle 2110 as a dependency.

Therefore:

~~~text
manifest
   ↓
2102
   ↓
FSM_COS dependency closure
   ↓
2110
   ↓
RuntimeAssembly
   ↓
Moniker presentation
   ↓
Living GUI execution
~~~

This is an architectural demonstration, not just a startup animation.

## FSM_API presentation rule

Meaningful lifecycle behavior belongs to FSM_API.

GUI builders describe semantic presentation.

CSS and browser APIs manifest that presentation.

~~~text
FSM_API
   ↓
state / timing / lifecycle

GUI builder
   ↓
semantic structure

browser / CSS
   ↓
visual manifestation
~~~

A timer in a Razor component is not an acceptable replacement for a runtime lifecycle state machine merely because the timer is convenient.

## Visitor-facing principle

> Show the behavior before asking the visitor to understand the machinery.

The page should demonstrate real Workshop technology first. The repository and Deep Dive surfaces then explain why the demonstration works.

## Evidence boundary

Every public claim should correspond to executable code, a test, a manifest, a RuntimeAssembly inspection, a benchmark, or an explicitly labeled future direction.

If a presentation beat is not implemented, call it a direction or experiment.

## Human agency

The opening should invite curiosity without silently making consequential decisions for the visitor.

The visitor decides what to enter, create, change, publish, or commit.

*The browser is the manifestation. The architecture is underneath it.*