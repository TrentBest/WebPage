# Workshop Opening Experience

The public landing page is the Workshop's **perception boundary**.

The current gateway is functional: it presents the visitor with a breathing identity control and an advisory before entering the runtime. That is not yet the intended opening experience.

The next presentation layer is deliberately **show, do not explain**.

## The problem

A visitor arriving at the URL does not yet know why they should care.

A large `ENTER THE WORKSHOP` button is an invitation, but it is not a proposition. A warning card that explains implementation details is interesting to an engineer but does not answer the visitor's immediate question:

> **What is this, and why should I enter?**

The opening must create curiosity before it asks for commitment.

## Perception sequence

The intended opening progression is:

```text
ARRIVAL
   |
   v
IDENTITY
   |
   v
TENSION
   |
   v
SHOW
   |
   v
INVITATION
   |
   v
ENTER WORKSHOP
```

The visitor should encounter a short, authored sequence of visual and auditory evidence rather than a paragraph of product explanation.

The design principle is:

> **Do not spend the visitor's attention explaining the Workshop when the Workshop can demonstrate itself.**

A picture communicates more than prose in the right context. A moving, timed, spatial, and auditory manifestation can communicate more than a static picture.

## Shock and awe, responsibly

"Shock and awe" here means **presentation intensity**, not manipulation.

The opening may use:

- scale changes;
- synchronized motion;
- strong typography;
- spatial depth;
- rapidly changing manifestations;
- controlled silence followed by impact;
- original sound design;
- environmental audio;
- a short sequence of concrete Workshop capabilities.

It must not:

- hide consequential behavior;
- imply capabilities that do not exist;
- pressure a visitor into a consequential action;
- silently make decisions on the visitor's behalf.

The visitor remains the human decision-maker.

## Audio

The opening should eventually have an original audio identity with the dramatic weight of a major game opening.

The reference point is **energy and orchestration**, not reproduction of Warcraft music or another copyrighted work.

The audio architecture should be:

```text
Opening state
    |
    +--> visual cue
    |
    +--> audio cue
    |
    v
presentation beat
    |
    v
next state
```

Audio is therefore part of the presentation state, not an unrelated page effect.

Browser autoplay restrictions must be respected. The system should treat audio as an enhancement that begins after an explicit user gesture when required by the browser.

## Architecture

The opening gateway is represented by:

`TheSingularityWorkshop/Gui/WorkshopGatewayGuiBuilder.cs`

The builder owns the semantic structure:

- gateway
- identity image
- invitation
- advisory
- opening context

CSS remains responsible for manifestation:

- color
- glow
- breathing
- scan lines
- motion
- responsive layout

This is an intentional boundary:

```text
GUI BUILDER
    |
    | semantic recursive structure
    v
RENDERER / BLAZOR
    |
    | manifestation
    v
CSS + browser primitives
```

The goal is **not** to eliminate CSS. The goal is to stop CSS and raw Razor markup from becoming the architecture.

## FSM_API alignment

The opening sequence should eventually have explicit lifecycle states owned by FSM_API rather than a collection of unrelated timers and boolean flags.

A future presentation state machine may resemble:

```text
Arrival
  |
  v
Awaken
  |
  v
Reveal
  |
  v
Demonstrate
  |
  v
Invite
  |
  v
Workshop
```

The state machine owns *when* the presentation is in each phase.

The GUI builder owns *what* each phase manifests.

CSS and audio are manifestations of those states.

This preserves the project's existing architectural direction:

> **FSM_API owns behavior. GUI Builders own semantic presentation. CSS and browser APIs manifest it.**

## Evidence boundary

The opening is a showcase, not a claim generator.

Every dramatic visual should ultimately correspond to something the Workshop actually contains:

- FSM_API-driven behavior;
- recursive GUI builders;
- MicroBundles;
- Experiences;
- spatial exploration;
- procedural composition;
- Unity/WebGL boundaries;
- the persistent-world model.

If a future presentation beat is not implemented, it should be clearly treated as a teaser or experiment rather than presented as an existing capability.

## Human agency

The opening exists to reduce cognitive friction and make exploration inviting.

It does not replace human judgment.

The Workshop may propose, demonstrate, compose, or guide.

The person decides what to enter, create, change, publish, or commit.

That distinction remains a runtime boundary throughout the system.
