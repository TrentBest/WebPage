# Agents

Agents are decision-makers operating persistent actors in Singularity World.

## Digitens

A Digiten has intentionally small universal knowledge. It can:

- maintain identity and persistent state;
- understand its current goal;
- choose among viable destinations/actions;
- navigate using the current world's wayfinding contracts;
- interact with MicroBundles;
- acquire specialized capabilities from the object it is operating.

A Digiten should **not** contain machine-specific knowledge such as how a backhoe digs or how a crane lifts.

## Capability discovery

Specialized objects expose capabilities through contracts such as `IVehicleCapabilitySource`.

The pattern is:

```text
Digitens
   │
   │ discover
   ▼
Target MicroBundle
   │
   ├── capabilities
   └── actions + requirements
          │
          ▼
      FSM_API agent
          │
          ▼
     target FSM behavior
```

The target remains authoritative. An agent may request `dig`, but it does not implement the bucket, boom, grounding, hydraulic, or engine behavior itself.

## Human equivalence

A human-controlled avatar and an autonomous Digiten must consume the same interaction contracts. This allows the autonomous Star Wars journey to become a real user journey without building a second universe for automation.

## FSM rule

Agent lifecycle and behavior are implemented with `TheSingularityWorkshop.FSM_API`. WebPage must not grow a competing state-machine runtime.

## Development heartbeat

Each new agent capability should receive an incremental unit test before the next architectural layer is added. The current vehicle capability-discovery increment is **Incremental Unit Test 10** in `SingularityHub.Tests/SpatialSimulationTests.cs`.
