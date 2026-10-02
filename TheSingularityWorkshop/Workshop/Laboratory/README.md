# Singularity Laboratory

The Singularity Laboratory is the Workshop's diegetic scientific test facility.

**The lab bench is a unit test you can walk into.**

A physical relationship is implemented once in the domain. A laboratory experiment supplies known inputs, runs that relationship, and records an observable result. The same experiment can later be manifested as a console, instrument, apparatus, research station, or interactive room.

The first facility exposes benches for mechanics, electromagnetism, thermodynamics, fluid dynamics, materials, fracture mechanics, nuclear physics, chemistry, and research.

## Why this is a MicroBundle

SingularityLaboratoryMicroBundle owns the facility lifecycle through FSM_API and participates in the Hub contract. It does not depend on Blazor or another renderer.

The MicroBundle exposes experiment definitions and deterministic execution. A future GUI can turn those definitions into diegetic instruments without duplicating the science.

## Scientific growth

The first relationships cover:

- mechanics: F = ma, work, energy, momentum
- electromagnetism: V = IR, P = IV, resistance, charge, energy
- thermodynamics: Q = mcΔT, conduction, thermal expansion
- fluid dynamics: hydrostatic pressure, dynamic pressure, flow, Reynolds number, buoyancy
- materials: stress, strain, elasticity, bulk modulus
- fracture: Mode-I stress intensity, critical crack length, Paris crack-growth relationship
- nuclear: half-life, decay, activity, mass-energy
- chemistry: density and the existing elemental/property data boundary

This is a foundation, not a claim that a handful of equations constitutes a complete scientific simulator. More equations, constitutive models, boundary conditions, material data, numerical methods, uncertainty, and experimental provenance can be added behind the same diegetic lab boundary.

Fictional elements remain valid participants in the Chemistry domain. The Laboratory does not require every research object to correspond to a real-world element or material.
