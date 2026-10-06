# Workshop Presentation Contract

This document is the presentation contract for the current manifest-driven WebPage opening.

The browser is the manifestation. FSM_API and FSM_COS remain the runtime authorities.

## Current sequence

~~~text
PAGE BOOT
   |
   v
LOAD HOST MANIFEST
   |
   v
COMPOSE PRIMARY ROOT THROUGH FSM_COS
   |
   +--> 2102 LivingGuiExperienceMicroBundle
   |        |
   |        +--> 2110 MonikerMicroBundle
   |
   v
RUNTIME ASSEMBLY
   |
   v
STARTUP MONIKER PRESENTATION
   |
   v
PRIMARY EXPERIENCE
   |
   v
LIVING GUI
~~~

The concrete manifest has one startup Moniker and one primary running Experience. The contract permits more than one startup Experience.

## Ownership

### FSM_API

Owns meaningful state-machine behavior, processing groups, lifecycle, and execution.

### FSM_COS

Owns composition:

- root requests;
- dependency closure;
- loading;
- arbitration;
- convergence;
- RuntimeAssembly.

### WebPage

Owns:

- manifest retrieval/parsing;
- WebPage composition catalog;
- browser presentation;
- navigation/chrome;
- WebPage-only Deep Dive presentation.

### GUI builders / browser

Own semantic presentation and visual manifestation. They do not become a second lifecycle engine.

## Moniker rule

The Moniker is presented first because it is a startup Experience.

It is not composed as a second root.

The primary root is 2102. FSM_COS obtains 2110 because the primary MicroBundle declares that dependency.

This distinction demonstrates the difference between presentation order and dependency order.

## Responsive presentation rule

The startup scene must fit the actual client.

Do not rely on a fixed desktop geometry.

Presentation should derive usable dimensions from the available browser client and preserve the composition's semantic structure across:

- desktop;
- resized desktop windows;
- narrow browser widths;
- tall/narrow clients.

A screenshot at one resolution is not proof of responsive correctness.

## Evidence

Presentation claims should be supported by:

- FSM_API state/process-group tests;
- FSM_COS composition tests;
- manifest tests;
- RuntimeAssembly inspection;
- visual inspection at multiple client sizes.

If a visual behavior is not implemented, document it as direction or future work.

## Migration rule

When presentation behavior is still owned by an older local FSM or timer:

1. locate the behavior;
2. preserve it with a focused test;
3. move authority to FSM_API or the appropriate package;
4. keep WebPage as the manifestation;
5. remove duplicate ownership.

> The page should show the machinery, not secretly replace it.
