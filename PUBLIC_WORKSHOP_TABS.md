# Public Workshop Tabs

## Current manifest activation

The public host manifest currently activates **Rendering only**.

Each top-level tab is represented by a manifest MicroBundleIds collection. The host may expose many tab MicroBundles in the future, but the active manifest deliberately exposes only one while the renderer/AI perception vertical slice is being proven.

Current dependency direction:

WEBPAGE HOST MANIFEST
  -> Rendering tab
    -> RenderingMicroBundle (4100)
      -> AiExchange
        -> ProtocolAi
        -> GrammarAi

This is an intentional proving-ground configuration, not a claim that the future Workshop has only one tab.

## Navigation principle

The top-level tabs are not merely site sections. Each one represents a distinct relationship a visitor can have with the living Workshop.

| Tab | Question it answers | Primary actor |
| --- | --- | --- |
| Explore | What already exists here? | visitor / participant |
| Create | What can I make here? | creator / builder |
| Education | How can I teach with this? | teacher / institution |
| MadMen | How can attention and advertising exist inside the world? | advertiser / agency / publisher |
| About Us | Who is building this and why? | everyone |
| Consult | Can we work together? | prospective partner / customer |

## Explore and Create

These should absolutely lead into one another.

**Explore is what exists. Create is what could exist.**

The correct relationship is a loop:

```text
                 +------------------+
                 |                  |
                 v                  |
              EXPLORE              |
                 |                  |
          find something            |
          interesting               |
                 |                  |
                 v                  |
              CREATE                |
                 |                  |
          modify / compose          |
          / build something         |
                 |                  |
                 v                  |
              PREVIEW               |
                 |                  |
                 v                  |
               RUN                  |
                 |                  |
                 v                  |
              PUBLISH --------------+
```

An Experience discovered through Explore should be able to say **Create your own** or **Use this as a starting point**.

A newly created Experience should be able to say **Explore your creation** after it runs.

Creation therefore does not replace exploration. It feeds it.

## Explore

Explore is the public discovery surface for things that have already been made.

It should eventually expose:

- published Experiences;
- Workshop facilities and destinations;
- creator-authored environments;
- educational Experiences;
- advertisements and advertising inventory;
- renderer demonstrations;
- public experiments;
- other creators' work;
- spatial/map entry points.

Explore should feel like entering a world, not browsing a documentation index.

## Create

Create is the Workshop's authoring surface.

It should eventually allow a creator to:

- compose MicroBundles;
- build an Experience manifest;
- preview the resulting RuntimeAssembly;
- create environments, buildings, classrooms, simulations, displays, and other content;
- save versions;
- publish explicitly;
- share the resulting Experience.

Create is the place where **capabilities become content**.

## MadMen

MadMen is not simply an advertising information page.

It demonstrates a fundamental property of the Singularity: **advertising inventory is a first-class digital object.**

Examples include:

- an advertisement on the side of a bus moving through the world;
- a large billboard or screen overlooking a square;
- a television in a storefront playing an advertisement as a visitor passes;
- displays inside a virtual business;
- sponsored locations or experiences;
- dynamically selected advertising placements.

The important architectural idea is that an advertisement has identity, content, placement, audience/context, lifecycle, and potentially a transaction — just like other world objects.

The MadMen experience should therefore eventually demonstrate both sides:

```text
ADVERTISER
    |
    v
create / acquire advertisement
    |
    v
DISCOVER INVENTORY
    |
    v
SELECT PLACEMENT
    |
    v
RUN IN WORLD
    |
    v
MEASURE / MANAGE
```

## Education

Education is a creator program, not merely an educational article.

A teacher should eventually be able to create an Educational account and receive an authoring environment for digital classrooms.

The classroom can be composed from the same Workshop primitives:

- historical simulations;
- physics laboratories;
- chemistry environments;
- mathematical visualizations;
- engineering demonstrations;
- interactive models;
- virtual field trips;
- teacher-controlled activities;
- student participation spaces;
- assessment or observation capabilities where appropriate.

The teacher should not have to become a software developer to create a lesson.

The long-term model is:

```text
TEACHER ACCOUNT
      |
      v
CREATE CLASSROOM
      |
      +--> choose / create Experiences
      +--> choose learning capabilities
      +--> configure teacher controls
      +--> invite students
      |
      v
RUN LESSON
      |
      v
STUDENT PARTICIPATION
```

Education should therefore use the same underlying Experience and MicroBundle architecture rather than becoming a separate product stack.

## Consultation

Consultation is the business conversion surface and should be first-class.

The goal is not a generic contact form. The visitor should be able to schedule time to discuss an opportunity.

Primary invitation:

> **Have an opportunity for the Workshop? Let's talk.**

Initial scheduling flow:

```text
CONSULT
   |
   v
choose opportunity type
   |
   v
choose available time
   |
   v
provide contact details
   |
   v
confirm consultation
```

Opportunity types can eventually include:

- technology / integration;
- education;
- virtual environment creation;
- advertising / sponsorship;
- custom Experience development;
- research / rendering;
- other partnership.

The first implementation does not need to build a complete scheduling platform. It needs a reliable public scheduling boundary that can later be backed by the Workshop's own scheduling MicroBundle/service.

## The resulting public story

```text
EXPLORE       CREATE       EDUCATION       MADMEN       ABOUT US       CONSULT
   |             |              |              |             |              |
 discover      build         teach          advertise      understand      talk
   |             |              |              |             |              |
   +-------------+--------------+--------------+-------------+--------------+
                                      |
                                      v
                              THE WORKSHOP
```

The tabs should feel like different doors into the same living system.

## Implementation priority

1. Resolve the Explore/Create relationship in the UX and navigation.
2. Make Explore → Create → Preview → Run → Publish → Explore an explicit product loop.
3. Establish Education as a creator/account program with classroom Experiences.
4. Establish MadMen as first-class advertising inventory and placement.
5. Add Consultation as a direct scheduling/business-conversion surface.
6. Keep all of these above the same MicroBundle → Experience → FSM_COS runtime architecture.

**The Workshop is the platform. The tabs are different ways to enter it.**