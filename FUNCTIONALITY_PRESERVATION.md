# Functionality Preservation Ledger — WebPage Host Refactor

## Purpose

This ledger is a guardrail for the WebPage gutting/refactor.

The objective is **not** to make WebPage smaller by deleting behavior. The objective is to move behavior to its canonical Workshop runtime/package owner while preserving externally observable behavior.

A migration is complete only when:

1. the behavior is identified;
2. its canonical owner exists;
3. executable proof preserves the behavior;
4. WebPage consumes that owner;
5. the old implementation is no longer required;
6. documentation describes the actual boundary.

If the canonical owner does not yet exist, the WebPage implementation is **migration debt and must remain**.

## Current composition versus current execution

The manifest → FSM_COS composition path is real and proven:

```text
host manifest
    ↓
Living GUI root 2102
    ↓
declared Moniker dependency 2110
    ↓
FSM_COS
    ↓
RuntimeAssembly
```

However, the browser manifestation is **not yet fully driven by that RuntimeAssembly**.

The current `Home.razor` and `LivingGui.razor` still consume `FSMManagerService → PageFSM → LivingGuiFsm → PageStateContext`.

That distinction is intentional and important:

> Composition has been migrated. Execution/presentation ownership has not yet been fully migrated.

Do not describe the current browser as if this second migration has already happened.

## Preserved Living GUI behavior

The existing runtime currently proves the following behavior and therefore establishes the migration contract:

| Capability | Existing behavior | Existing proof | Destination |
|---|---|---|---|
| Root creation | One Gen0 root starts at 50%, 50%, size 50 | `PageStateContextTests`, `LivingGuiRuntimeTests` | Living GUI Experience package |
| Population pool | 100 preallocated slots | `LivingGuiRuntimeTests` | Living GUI Experience package |
| Seed size | Offspring starts at size 10 | `PageStateContextTests`, `LivingGuiReproductionTests` | Living GUI Experience package |
| Target distribution | Squirrel-based deterministic stratification across 10–90% viewport bounds | `LivingGuiRuntimeTests` | Living GUI Experience package |
| Independent organisms | Each organism owns an FSM instance while sharing one processing group | `LivingGuiRuntimeTests` | Living GUI Experience package + FSM_API |
| Travel | Organisms lerp toward individual targets rather than jumping | `PageStateContextTests`, `LivingGuiRuntimeTests` | Living GUI Experience package |
| Planting | Seed grows from 10 to default size 50 before becoming existing | `PageStateContextTests`, `LivingGuiReproductionTests` | Living GUI Experience package |
| Growth | Existing organism lerps toward maximum size 100 | `PageStateContextTests` | Living GUI Experience package |
| Reproduction | Maximum-size organism creates a new independent organism | `LivingGuiReproductionTests` | Living GUI Experience package |
| Reduction/recovery | Parent reduces back to size 50 and resumes existing state | `LivingGuiReproductionTests` | Living GUI Experience package |
| Population observation | Population threshold is detected without freezing the runtime prematurely | `PageStateContextTests` | Living GUI Experience package |
| Gravity | Frozen population receives gravity acceleration and rotation until fallen | `PageFSM` + runtime tests | Living GUI Experience package |
| Moniker reveal | Moniker presentation is a distinct lifecycle beat before gravity/navigation | `PageFSM` lifecycle | Experience/package runtime |
| Navigation arrival | Workshop chrome appears after the experience's arrival condition | `PageFSM` + layout behavior | Host presentation contract |
| Browser manifestation | Actors are absolutely positioned relative to the supplied panel | `LivingGui.razor` | GUI/browser host |
| Responsive coordinate space | 50%,50% is the supplied panel center; no fixed viewport coordinate system | `PageStateContextTests` + CSS contract | GUI/browser host |

## Migration status

### Complete

- Host manifest loading/validation.
- Manifest-selected primary root.
- FSM_COS dependency closure.
- 2102 → 2110 dependency declaration.
- RuntimeAssembly composition proof.
- WebPage documentation of package boundaries.
- Removal of obsolete manifest presentation stopwatch/heartbeat code.

### In progress

- Replace WebPage `PageFSM` as the authority for the active browser lifecycle.
- Move Living GUI domain/runtime behavior out of `PageStateContext` and `LivingGuiFsm` into a canonical reusable Experience package.
- Make the browser render the package/runtime state rather than a parallel WebPage-owned runtime.
- Preserve the existing 100-slot, independent-FSM, growth/reproduction/gravity behavior during that move.
- Move resource/compute requirements into the MicroBundle declaration path once the neutral contract is established in MicroBundleDomain/FSM_COS.

### Not safe to delete yet

- `Services/PageFSM.cs`
- `Services/LivingGuiFsm.cs`
- `Services/PageStateContext.cs`
- `Services/FSMManagerService.cs`

Deleting these before their replacement exists would be functionality loss disguised as architectural cleanup.

## Resource declaration requirement

The desired final path is:

```text
MicroBundle
   ↓ declares requirements
FSM_COS
   ↓ resolves dependency + compute/resource plan
resource/memory warehouse
   ↓ provides allocations
FSM_API
   ↓ executes declared FSMs/process groups
WebPage
   ↓ supplies browser environment and presents state
```

The current published MicroBundle contract used by this WebPage branch does not yet expose a verified resource/compute declaration surface sufficient to claim that this path exists.

Therefore WebPage must not invent a fake resource API merely to make the architecture diagram look complete.

## Rule for future cleanup

Before deleting any runtime class, attach its behavior to:

- a canonical package;
- an executable regression/architectural test;
- a RuntimeAssembly composition proof where applicable;
- a browser manifestation proof where applicable.

Then delete the duplicate.

**The measure of success is not lines removed. It is functionality retained under the correct owner.**
