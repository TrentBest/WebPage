# Digiten data model — first executable contract

## Purpose

A **Digiten** is a persistent entity in a Digiverse. It has stable identity, mutable state, relationships to shared world records, needs, activity, and a life course. It may be a resident, visitor, student, worker, job seeker, caregiver, retiree, or another kind of participant.

This document defines the first CPU reference record used by the WebPage host. It is a working starting point—not a claim that a city simulation, GPU execution, or the statistical calibration pipeline is already complete.

## Two representations, one identity

```text
Digiten (IStateContext adapter)
  ├── stable identity + FSM_API lifecycle
  └── DigitenState (68-byte primitive record)
        ├── numeric links to shared world tables
        ├── position + age + money
        ├── compact need meters
        └── life stage, education, activity, employment, schedule, flags
```

The FSM_API-facing `Digiten` is a reference-type adapter because it implements `IStateContext`. The hot data record is a separate value type. FSM context identity is not the GPU storage format.

The current `DigitenState` layout is 68 bytes with four-byte packing and explicit tail padding. The automated tests check its size and verify that it contains no managed references. This is a measured starting point, not a requirement that all future data fit in this record.

## What belongs in the hot record?

| Data | Representation | Reason |
|---|---|---|
| Stable entity identity | 64-bit ID | Survives movement and role changes |
| Household, home, workplace, school, employer, occupation | Numeric table IDs | Avoids embedding strings or object graphs |
| Position | Three floats | Spatial queries and representation |
| Age | Days | Derive age bands for schedules and demographic calibration |
| Money | Integer minor units | Avoid floating-point currency rounding |
| Hunger, rest, social, safety, purpose | Five byte meters | Compact needs, with 0 satisfied and 255 urgent |
| Health and aggregate skill signal | Byte each | Coarse simulation signals; detailed models can be separate |
| Education, activity, life stage, employment, schedule | Byte-sized codes | Finite state that can be processed in bulk |
| Flags | Byte bit field | Common yes/no conditions without managed booleans |

Detailed skills, relationships, personal history, inventory, preferences, job contracts, school records, and rich narrative should be keyed extension data or independently composed capabilities. They should not automatically inflate every hot record.

## Statistical inputs: keep provenance and vintage

The current Singularity City work has a New York City frame, so NYC is a useful first reference population. The official Census Bureau QuickFacts page reports **8,804,190 residents in the April 1, 2020 Census**. The same page currently reports **20.2% under age 18**, **16.6% age 65+**, **3,334,088 households**, and **2.48 persons per household** for its **2020–2024** estimate period. Those values are useful for locating the dimensions a profile must represent, but they are **not one consistent-year dataset** and must not be silently blended into a calibrated 2020 population.

Sources:
- [U.S. Census Bureau QuickFacts — New York City](https://www.census.gov/quickfacts/fact/table/newyorkcitynewyork/POP645221)
- [U.S. Census Bureau — Age and Sex Composition in the United States: 2020](https://www.census.gov/data/tables/2020/demo/age-and-sex/2020-age-sex-composition.html)
- [U.S. Census Bureau — American Community Survey subject tables](https://www.census.gov/acs/www/data/data-tables-and-tools/subject-tables/)

Before we generate a baseline, choose one reference year and geography, then record each input's source table, estimate period, universe, units, and uncertainty. Use consistent ACS/Census tables for age, household structure, labor-force participation, occupation/industry, education, and school enrollment. Keep **observed statistics**, **derived simulation parameters**, and **synthetic 100× scenario rules** as separate records so we can audit every generated outcome.

## The city is a coupled system, not a crowd counter

A Digiten's activity is the result of needs, age/life stage, education, available opportunities, household responsibilities, location, schedule, and world conditions. A person should not be marked employed merely because a percentage says they have a job: an actual job slot and employer/workplace must exist. Job seekers search available openings; students need a school and enrollment; commuting connects home, school, shops, and workplaces through the spatial/physics layer.

The first useful loop is:

1. Generate a baseline population from a named statistical profile and year.
2. Assign households, homes, life stages, education, and initial employment status using explicit distributions.
3. Generate actual housing, school, and job capacity so assignments cannot exceed supply.
4. Resolve daily activity and travel from schedules, needs, and available destinations.
5. Aggregate outcomes and compare them with the input profile.
6. Apply a separately named scale factor (for example, 100×) and rerun capacity and performance checks.

**The 100× factor is a simulation scenario, not a claim that source statistics become true at the new scale.** Keep observed inputs, derived parameters, and synthetic expansion rules separately identified. A city with 100× residents needs a defined answer for land area, housing density, infrastructure capacity, jobs, schools, and transport; multiplying only the headcount would create impossible ratios.

## GPU and texture direction

The record is primitive-only and fixed-layout to support later bulk-transfer experiments. It does not yet prove that a texture is the best storage primitive or that GPU simulation is faster. Benchmark structured buffers, texture-backed layouts, and CPU arrays against the same deterministic workload before choosing.

A 1 KiB record costs approximately 1 GiB per 1,048,576 Digitens before auxiliary buffers. The current 68-byte hot record costs about 68 MiB per 1,048,576 records before alignment and working copies. Those are raw payload estimates, not measured runtime memory. Keeping cold extension data out of the hot record leaves room to add richer behavior without forcing every simulation pass to move it.

## What this first slice proves

- The FSM_API context contract can wrap a Digiten.
- A stable numeric identity is separate from mutable state.
- The bulk state is a value type with no managed references.
- The state has a tested, fixed 68-byte layout.
- Activity, employment, education, life stage, needs, place links, and money have explicit initial fields.

## What it does not prove yet

- A calibrated population model or any specific statistical accuracy.
- A working job market, school enrollment, household formation, commute network, or city economy.
- GPU upload, texture storage, compute shaders, or GPU speedups.
- A production-ready public Digiten NuGet API.

Next milestones: add statistical profile inputs with provenance; model household, housing, school, and job capacities; build a deterministic population generator; then benchmark 1×/10×/100× workloads and compare CPU and GPU layouts.
