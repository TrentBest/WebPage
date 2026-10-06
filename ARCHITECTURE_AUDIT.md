# Architecture Audit — October 2026

## Reference standard

FSM_API is the reference implementation for deterministic lifecycle behavior.

WebPage consumes FSM_API and the other Workshop packages through their published/package boundaries. It must not grow competing runtime kernels.

## Current architectural position

The repository is being rehabilitated from its original website-wrapper purpose into a browser proving ground and public learning resource.

The authoritative direction is:

~~~text
host manifest
    ↓
startup presentation Experience(s)
    ↓
primary running Experience
    ↓
FSM_COS RuntimeManifest
    ↓
dependency closure / arbitration / RuntimeAssembly
    ↓
FSM_API execution
    ↓
WebPage presentation
~~~

## Confirmed aligned boundaries

- FSM_API owns meaningful state-machine execution.
- FSM_COS owns composition, dependency closure, arbitration, convergence, and RuntimeAssembly creation.
- MicroBundleDomain owns the MicroBundle domain contract.
- WebPage owns browser presentation, manifest loading, host adapters, and proving-ground integration.
- Reusable capabilities are consumed through Workshop NuGet packages when a package already owns them.
- WebPage explicitly references `FSM_API`, `FSM_COS`, and `MicroBundleDomain`; package versions must remain on published versions unless a newer package is explicitly released and approved.
- The Living GUI primary MicroBundle declares the canonical Moniker dependency.

## Current composition proof

The current manifest requests primary root 2102.

~~~text
2102 LivingGuiExperienceMicroBundle
        ↓
declared dependency
        ↓
2110 MonikerMicroBundle
~~~

WebPage must not request 2110 separately just because it is presented before the primary Experience.

## Transitional areas

The repository still contains older page-FSM, first-contact, catalog, demo, and exploration implementations.

These are migration candidates, not automatically deletion candidates.

For each:

1. locate behavior;
2. identify canonical owner;
3. preserve with executable proof;
4. replace the host implementation;
5. verify package/runtime behavior;
6. remove duplicate code.

## Known cleanup targets

1. Old gateway/first-contact lifecycle documentation.
2. Static Idler/Flex selection scaffolding.
3. WorkshopDemoBundle bootstrap scaffolding.
4. Duplicate host lifecycle state where FSM_API already owns the behavior.
5. `Workshop/MicroBundles/MicroBundle.cs` and its local `IMicroBundle`/context/manifestation scaffolding: historical lifecycle machinery that must not become a second MicroBundle runtime. Migrate concrete demos to the published FSM_COS + MicroBundleDomain contracts as their capabilities become current.
6. Local catalogs/providers that duplicate package-owned MicroBundle discovery or composition responsibilities.
7. Stale platform-specific documentation.
8. Root-level documentation duplication.

## Documentation rule

A document is current only when its claims match code and tests.

Historical ideas may remain, but they must be labeled historical or future and must not be presented as the active runtime contract.

## Completion standard

The audit is satisfied when:

- package ownership is explicit;
- manifest startup and primary Experience semantics are unambiguous;
- FSM_COS is the only composition engine;
- FSM_API is the lifecycle authority;
- browser components are manifestations, not hidden runtime owners;
- current documentation agrees with tests and running behavior.
