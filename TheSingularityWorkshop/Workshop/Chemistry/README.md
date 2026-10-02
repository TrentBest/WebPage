# Chemistry

The Chemistry domain is the Workshop's first deliberate playground for separating **data, construction, arbitration, and presentation**.

## Lineage

```text
Atom
  ↓
AtomBuilder
  ↓
AtomGuiBuilder / ChemistryLabGuiBuilder
  ↓
Periodic Table
  ↓
Chemistry MicroBundle
  ↓
Experience
```

The original Atom/AtomBuilder work in TheForge established the useful distinction between the domain object and its fluent construction boundary. This WebPage implementation brings that distinction into the platform-neutral Workshop domain.

## Arbitration

`ChemistryMicroBundle` exposes an elemental arbitration capability. Installed Hub bundles may opt into the `IElementalMaterialSource` contract and provide `MaterialComposition` records.

The chemistry arbitrator evaluates **what a material is made from and how it is being applied**, not what the containing bundle calls itself. This allows a future weapons bundle, armor bundle, vehicle bundle, building bundle, or fictional bundle to participate through the same boundary.

The first implementation produces normalized default physics heuristics. These are deliberately not laboratory measurements; authoritative material-property data belongs in the data domain and can replace the heuristics without changing the arbitration contract.\n\n`ElementalPropertySet` is the structured handoff for that future data domain. It provides nullable slots for phase behavior, thermodynamics, transport, electromagnetic behavior, mechanical response (including fracture and fatigue), fluid/material behavior, and nuclear/radiological properties, plus source/version/uncertainty metadata. `Atom` remains the elemental identity; the property set is optional and replaceable. This deliberately allows the eventual warehouse to carry much richer data without forcing every element to pretend that every measurement is known.

## Fiction

The same Atom/Builder pipeline is intended to support fictional elements such as Unobtanium. Fiction extends the data domain; it does not require a new GUI architecture.

## Presentation

`ChemistryLabGuiBuilder` consumes the domain catalog and produces the recursive `GuiNode` tree. It does not own elemental data, and the chemistry domain does not depend on Blazor, CSS, Unity, WPF, or another host renderer.


### Physics relationships

The chemistry domain also exposes fundamental relationships as calculations rather than duplicated data: electrical power `P = I × V`, force `F = m × a`, mechanical power `P = F × v`, work `W = F × d`, density `ρ = m / V`, and pressure `p = F / A`. This keeps the model composable: measured or sourced quantities can participate in derived physical behavior without baking derived values into elemental identity.
