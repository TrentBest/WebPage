# Singularity Laboratory Architecture

## Purpose

The Singularity Laboratory is a vertically extensible scientific environment. Its visible upper levels are the public-facing science center; its depth becomes progressively more specialized, experimental, hypothetical, and computational.

The laboratory does not need a finite rendered floor count. The authoritative system describes laboratory depth independently from the current spatial manifestation.

```text
                    SURFACE
                       |
          PUBLIC SCIENCE / EXHIBITION
                       |
          FOUNDATIONAL SCIENCE
                       |
             RESEARCH LEVELS
                       |
          ADVANCED / EXTREME PHYSICS
                       |
          HYPOTHETICAL / PRIVATE
                       |
             ... DEPTH CONTINUES ...
```

## Three responsibilities

### 1. Public science

The upper/external-facing laboratory teaches and demonstrates established concepts:

- physics;
- mechanics;
- thermodynamics;
- chemistry;
- materials;
- waves;
- hydrodynamics;
- electronics;
- geology/geophysics;
- large physical simulators.

These are not merely articles. They are spatial demonstrations backed by the same simulation substrate that will later support research.

### 2. Research

Deeper levels provide increasingly specialized simulation environments. A researcher can prepare experiments, manipulate declared parameters, record observations, and preserve the experiment as versioned data.

Research workspaces may contain:

- desks;
- digital/infinite whiteboards;
- equations and notes;
- equipment inventories;
- experiment preparation surfaces;
- simulation controls;
- observation/measurement surfaces;
- writeup generation from the actual prepared run.

### 3. Controlled exhibition

A researcher can publish a controlled presentation of an experiment without exposing the private research workspace.

```text
PRIVATE RESEARCH SPACE
        |
        v
PREPARED EXPERIMENT
        |
        v
AUTHORIZED EXHIBITION
        |
        +--> visitor observation
        +--> bounded interaction
        +--> same authoritative simulator
        +--> protected research state
```

The exhibition is therefore another provider/manifestation of the experiment, not a duplicated simulation.

## Simulator families

Simulator families are capability boundaries. Rooms and floors manifest those capabilities; they do not define them.

Initial families include:

- mechanics;
- gravity/orbits;
- thermodynamics;
- phase transitions;
- chemistry/reactions;
- materials;
- hydrodynamics;
- wave pools and wave propagation;
- hydraulics, dams, and spillways;
- acoustics;
- electronics/circuits;
- electromagnetism;
- tectonics;
- volcanoes;
- geology/geophysics;
- plasma/ionization;
- particle/collider systems;
- coupled planetary phenomena.

Each family should be decomposable into MicroBundles and reusable outside the laboratory.

## MicroBundle composition

```text
Element / Material
       |
       +--> physics/process MicroBundles
       |       mechanics
       |       thermodynamics
       |       hydrodynamics
       |       chemistry
       |       electromagnetism
       |       phase transitions
       |
       v
Experiment MicroBundle
       |
       v
Simulator Providers
       |
       +--> WebPage
       +--> WebGL / WebGPU
       +--> Unity
       +--> future hosts
```

The GUI is never the authority for the scientific state.

The same underlying bundles should be usable by:

1. public demonstrations;
2. laboratory experiments;
3. private researcher spaces;
4. controlled exhibitions;
5. eventual world simulation.

## Lazy depth

Depth is represented as data.

A renderer materializes only the relevant region around the current participant. The system should be able to describe vastly more laboratory depth than is currently rendered.

Required properties:

- lazy level materialization;
- renderer-independent authoritative experiment state;
- safe unloading of distant manifestations;
- preservation of experiment/research state after unloading;
- plan/elevation/ISO/3D-compatible spatial coordinates;
- no requirement to instantiate every possible level.

This is the laboratory application of:

**DATA -> SURFACE -> GPU**

## Research tenancy

Future research allocation should be represented as data rather than a collection of special-case pages.

A research tenancy needs, at minimum:

- tenant/researcher identity;
- allocated laboratory address/depth;
- capability permissions;
- MicroBundle dependencies;
- equipment allocation;
- simulation limits/resources;
- private/public data policy;
- exhibition policy;
- experiment versions;
- provenance and publication state.

The physical-looking office, laboratory, whiteboards, and presentation room are manifestations of that allocation.

## Access boundaries

The existing diegetic security system remains the entry boundary.

```text
CAMPUS
  -> SECURITY
  -> BADGE / ACCESS
  -> ADMINISTRATIVE AUTHORITY
  -> ELEVATOR / DEPTH
  -> RESEARCH SPACE
  -> EXPERIMENT
  -> CONTROLLED EXHIBITION
```

Security establishes physical entry. Administrative/research authority determines what a participant may access or publish after entry.

## Implementation order

1. Add a declarative laboratory depth model.
2. Define public, research, advanced, and hypothetical depth bands.
3. Give simulator families stable capability identities.
4. Represent research tenancy independently of presentation.
5. Represent controlled exhibition policy independently of the simulator.
6. Connect experiments to MicroBundle addresses.
7. Materialize only the current/required depth.
8. Connect the existing Physics 101/research experience to the first declared levels.

Do not build the infinite facility as thousands of SVG rooms.

Build the **authority and data model first**, then let spatial manifestations emerge from it.

## Relationship to existing work

This architecture extends the current:

- `SingularityCampusCatalog`;
- `SpatialWorkshopScene`;
- `SpatialLaboratoryPlanGuiBuilder`;
- `SpatialLaboratoryResearchGuiBuilder`;
- FSM_API-backed security/access;
- MicroBundle registry/addressing;
- physics laboratory work tracked in issues #48 and #49.

Master remains untouched. This architecture belongs to `development`.