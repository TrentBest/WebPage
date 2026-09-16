# Singularity City Simulation

Singularity City is a persistent simulation substrate that happens to be explorable by a human.

The city is not merely a background rendered behind an Experience. Experiences are activities occurring inside the same persistent world.

## 1. The City Is a Texture

At a given simulation level, the city can be represented as a 2D cell surface.

Each cell answers a small set of questions:

- Is this cell walkable?
- Is it blocked?
- Which Digiten currently occupies it?
- What local movement flow is available from it?

The current CPU representation is `DigitenCityTexture`. A GPU representation may mirror it later. The texture is a storage and acceleration representation, not the authority for behavior.

One cell may contain at most one Digiten. The occupant value is the persistent Digiten ID.

This gives the simulation an unusually useful primitive:

```text
CITY LEVEL
┌───┬───┬───┬───┬───┐
│   │   │ # │   │   │
├───┼───┼───┼───┼───┤
│   │ D │ # │   │   │
├───┼───┼───┼───┼───┤
│   │   │ # │   │   │
└───┴───┴───┴───┴───┘

D = persistent Digiten ID
# = blocked
```

The same surface can drive occupancy, collision, local movement, crowd density, rendering, and later population analytics.

## 2. Waypoints Are Anchors, Not Rails

A waypoint describes where a Digiten intends to go, not exactly where its feet must land.

A route therefore becomes:

```text
semantic waypoint
      ↓
random sample around waypoint
      ↓
reject blocked / occupied cell
      ↓
shrink radius when necessary
      ↓
walk toward selected point
      ↓
arrive
      ↓
sample again
```

The sampling radius is part of the route definition. A small radius produces disciplined movement. A larger radius produces a looser population. Different Digitens may use different radii.

This deliberately prevents the city from looking like every actor is replaying one exact animation path.

## 3. Desired Direction Is a Heuristic

The Digiten knows the desired bird-flight direction toward its current objective.

The local vector field does not promise to reproduce that line. It scores available neighboring cells and chooses a locally useful direction.

When obstacles force repeated divergence, travel frustration accumulates. Aligned movement relieves frustration. At a configured threshold the Digiten can decide that continuing in the current local flow is no longer worthwhile and reverse its preferred movement, then slide back toward the desired line when the local surface permits it.

The important behavior is not a hard-coded maze solution. It is a small heuristic producing emergent-looking travel.

## 4. DigiCrowd

A `DigiCrowd` is a movement optimization boundary.

It exists because thousands of individual actors should not all require the same expensive movement reasoning simultaneously.

At low density:

```text
Digiten → waypoint → local cell choice
```

At sufficient density:

```text
many Digitens
      ↓
DigiCrowd
      ↓
vector field
      ↓
individuals inherit local flow
```

The population remains composed of persistent Digitens. The representation of movement changes.

A crowd can therefore produce useful emergent effects:

- one side of a flow becomes congested;
- flow bypasses through the other side;
- pressure moves through the field;
- the blocked side eventually becomes available;
- individual actors resume their own goals.

The simulation should not require every Digiten to solve the city independently.

## 5. DigiGroup

`DigiGroup` is intentionally different from `DigiCrowd`.

A crowd exists because actors are moving through the same space. A group exists because actors have arrived to do something together.

Examples:

- a construction crew cooperatively building a foundation;
- two teams competing in a game;
- a factory shift performing coordinated work;
- friends meeting after work;
- emergency responders forming a temporary response group;
- Digitens gathering for a city event.

A group owns an objective and a mode such as cooperative or competitive. Group membership can be temporary or persistent.

## 6. Digitens Have Lives

A Digiten is not merely a moving sprite.

A persistent city population needs:

```text
identity
  ↓
home
  ↓
role / work
  ↓
needs
  ↓
travel
  ↓
attendance
  ↓
commerce
  ↓
social groups
  ↓
return home
  ↓
repeat
```

The city therefore contains homes, workplaces, businesses, transit, public spaces, factories, construction sites, entertainment, and eventually entire districts with different rhythms.

The simulation can continue while nobody is looking at it. Human presence increases observation and rendering rather than creating the population from nothing.

## 7. Singularity Currency

The first economic substrate is deliberately small.

A persistent citizen owns a `SingularityWallet`. A business can charge for attendance. Attendance becomes business revenue and reduces the citizen's balance.

That creates the first closed loop:

```text
citizen earns currency
        ↓
citizen travels
        ↓
attends business
        ↓
business receives currency
        ↓
attendance becomes economic data
        ↓
world state changes
```

This is not yet the final economic model. It is the heartbeat that allows richer MicroBundles and providers to define jobs, wages, prices, production, ownership, markets, and services without replacing the world substrate.

## 8. Rendering Is an Observation

The authoritative world should remain renderer-neutral.

The same city state can eventually be observed as:

- the current 2D Explore map;
- a blueprint;
- a GPU texture;
- a dense population visualization;
- a transit view from the Space Elevator or tram;
- a future 3D city;
- analytics for population and economy;
- an AI/agent observation surface.

Rendering should therefore never become the owner of city truth.

## 9. Experience Is a Side Quest Inside the Universe

A user may enter the Workshop to create digital content, visit the Forge, inspect a factory, watch construction, take a tram, or follow a Digiten home.

Those activities are all Experiences occurring inside the same persistent universe.

The user's primary reason for visiting the site can therefore be anything. The city remains alive underneath it.

That distinction is fundamental:

> The content is the Experience. The universe is the substrate.

## 10. Architecture Direction

The current implementation intentionally grows from tiny deterministic primitives:

1. `DigitenWaypointTexture` — compact route storage.
2. `DigitenWaypointCursor` — deterministic to-and-fro progression.
3. `DigitenCityTexture` — walkability and one-Digiten-per-cell occupancy.
4. `DigitenCrowdField` — local vector-field movement heuristic.
5. `DigitenWaypointSampler` — imperfect obstacle-safe destination selection.
6. `DigitenTravelFrustration` — accumulated divergence from desired travel.
7. `DigiGroup` — intentional aggregation after arrival.
8. `SingularityCitizen` / `SingularityHome` — persistent population and residence.
9. `SingularityBusiness` / `SingularityWallet` — attendance and currency.

The next layers can be built on these without changing the central idea: **a persistent world made of compact state, with behavior supplied by contracts and FSM_API rather than hard-coded renderer logic.**
