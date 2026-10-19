# Azure Blob Experience Delivery Contract

![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

> **Status: design contract / integration not yet verified.** This guide defines the seam for delivering versioned WebPage Experiences from Azure Blob Storage. It does not claim that WebPage currently fetches its runtime composition from Azure.

## Goal

A visitor should enter Singularity City as an Experience selected by the WebPage host, while its versioned manifests, declarative world data, SVG documentation, and media can be delivered independently from a public artifact repository. The host remains the browser environment; FSM_COS remains the composition authority.

## Ownership and data path

```text
WebPage host manifest / selected Experience
                  |
                  v
     Experience source resolver
        (browser-safe adapter)
                  |
                  v
       versioned artifact store
          Azure Blob Storage
                  |
                  v
 canonical MicroBundle provider/catalog
       resolves known ID + version
                  |
                  v
                FSM_COS
      dependency closure / arbitration
                  |
                  v
            RuntimeAssembly
                  |
                  v
     WebPage + GUI.Blazor presentation
       SVG / city / citizen / maps
```

- **WebPage** selects the Experience, performs browser-safe retrieval, reports loading/errors, and presents the result.
- **Azure Blob Storage** stores versioned artifacts. It does not define lifecycle or composition semantics.
- **MicroBundleDomain** owns the MicroBundle contract; the catalog/provider resolves exact supported IDs and versions.
- **FSM_COS** resolves dependency closure, loads/arbitrates the selected composition, and produces RuntimeAssembly.
- **Visualize / Renderer / GUI.SVG** own reusable visual and drawing capabilities. WebPage supplies data and browser input rather than creating a parallel renderer.

A Blob URL is a storage location, not a MicroBundle identity and not proof that a capability is executable. A JSON file may describe data, configuration, a manifest, or a known provider's parameters; it must not be treated as arbitrary .NET code.

## Artifact layout

Use stable, versioned, immutable paths. The exact container and route names are configuration decisions, not hard-coded assumptions:

```text
experiences/{experience-id}/{version}/manifest.json
microbundles/{bundle-id}/{version}/descriptor.json
worlds/{world-id}/{version}/world.json
worlds/{world-id}/{version}/svg/{view-id}.svg
assets/{content-hash}/{asset-name}
```

The Experience manifest references exact bundle IDs and versions. A descriptor can declare metadata, dependencies, asset references, and parameters for a provider already registered by the application. It cannot introduce an unknown executable implementation merely by naming a .NET type in JSON.

Prefer immutable version paths and content hashes over overwriting a live artifact in place. A mutable discovery index may point to a released version, but the selected Experience must record the resolved version/hash so it can be reproduced. Validate IDs, versions, lengths, media types, references, and supported schema versions before accepting data.

## Browser security and delivery

- **Never ship an Azure account key, connection string, or storage write credential in Blazor WebAssembly, JavaScript, a public manifest, or an SVG.** Anything sent to the browser is public to that visitor.
- For the public showcase, prefer a read-only public artifact container or CDN for non-sensitive released assets. Configure Blob CORS for the exact deployed origins and only the methods/headers required.
- If access must be private, put authorization and storage credentials behind a trusted server-side broker, or issue short-lived, narrowly scoped, read-only SAS URLs. Do not put a long-lived SAS token in source control or a permanent manifest.
- Treat every downloaded file as untrusted input. The current manifest reader rejects declared or streamed bodies larger than **1 MiB**, uses response-header streaming to avoid eagerly buffering an unbounded body, and propagates caller cancellation through the HTTP request and bounded stream read. HTTP timeout behavior remains an explicit release gate. The reader rejects missing or non-JSON response content types (accepting `application/json` and structured `+json` media types), and the host-manifest parser rejects malformed versions and unsupported major versions (currently major version `1`). Never load downloaded assemblies or run downloaded scripts.
- Make failure visible: distinguish unavailable source, invalid manifest, unsupported version, missing dependency, and composition failure. Do not silently substitute an unrelated Experience and claim the requested one loaded.
- Cache immutable artifacts by exact identity/version/hash. Cache policy must not allow a stale mutable index to silently select an old or different composition.

## SVG and living documentation

SVG is a useful artifact format for diagrams, plan views, visual examples, and generated evidence. Reuse the Workshop moniker and existing Visualize / GUI.SVG work rather than inventing a second SVG stack.

Each SVG or visual document must label its evidence class:

- **Live/runtime-derived:** generated from the actual selected manifest, resolved composition, world snapshot, or tested runtime state.
- **Fixture/sample:** generated from named, versioned sample data.
- **Architecture illustration:** explains intended relationships but does not claim live telemetry.
- **Proposed:** future work without an implementation proof.

For generated SVG, keep a reproducible generator and source fixture in the repository. Record the generator/input version and test the output's key semantic elements. Do not use a static diagram to imply that Azure retrieval, city persistence, simulation, or RuntimeAssembly integration is already live.

## Implemented source seam

The browser host reads the optional public base URI from `ExperienceArtifacts:BaseUri` in standard .NET configuration (for example, `https://example.blob.core.windows.net/public-experiences/`) and the optional relative startup manifest path from `ExperienceArtifacts:ManifestPath` (for example, `experiences/singularity-city/1.0.0/manifest.json`). The base URI chooses the origin/prefix; the manifest path chooses the artifact beneath it. Leaving either setting absent preserves the existing local default path behavior. Because this configuration is delivered to the browser, it must contain only a public endpoint—not account keys, write-capable SAS tokens, or other secrets.


`WorkshopExperienceService` now accepts an optional `experienceArtifactBaseUri`. When omitted, the existing relative manifest path continues to resolve through the application's configured `HttpClient` base address. When supplied, the base must be an absolute HTTPS URI with no embedded credentials, query, or fragment; relative manifest paths are resolved under that base. The resolver rejects absolute URLs, traversal segments, encoded path segments, backslashes, empty path segments, queries, and fragments.

This is the first source seam, not proof of a deployed Azure integration. The host still needs a real configured Blob endpoint, both `ExperienceArtifacts:BaseUri` and `ExperienceArtifacts:ManifestPath` configured for that artifact, container access/CORS settings, and a successful browser test against an actual versioned Experience manifest. It resolves manifest JSON only; MicroBundle execution remains restricted to the registered in-process catalog.

## Current implementation checkpoint

The WebPage host already has manifest-driven composition and package-version boundaries, and its repository includes Singularity City composition documentation plus generated SVG plan examples. The documented City contract explicitly distinguishes architecture direction from currently composed roots.

**Not yet certified here:** Azure Blob retrieval is not proven as the active source for the selected Experience; no deployment URL, container policy, CORS policy, authentication path, or end-to-end remote-load test has been verified in this change. Until those are implemented and tested, local manifest/catalog paths remain the known baseline.

## Integration sequence

1. Inventory the current host manifest loader, catalog/provider, canonical MicroBundle contract, and FSM_COS composition tests before changing code.
2. Define a small source interface that resolves an immutable artifact by logical ID, version, and optional hash. Keep storage-specific SDK details behind its adapter.
3. Implement a local-file/in-memory provider first so the same contract is testable without cloud credentials; preserve existing composition behavior.
4. Add the Azure Blob adapter at the correct trust boundary: public read-only client for public assets, or server-side broker for private data.
5. Prove success, not-found, invalid content, unsupported version, timeout, cancellation, and cache/version behavior with deterministic tests.
6. Compose a real, registered Experience from resolved versioned roots through FSM_COS. Do not put fake IDs in the host manifest or let a URL bypass dependency closure.
7. Add a public showcase deployment configuration with no secrets, verify CORS and cache headers, and test the actual deployed URL in a browser.
8. Generate or display one SVG view from the same resolved versioned source and label whether it is live-derived or fixture-based.
9. Update the current vertical-slice guide and release checklist only after end-to-end evidence exists.

## Release gate for a public Singularity City showcase

- [ ] Public visitor can load the published host without secrets or local developer files.
- [ ] Host resolves a versioned Experience artifact from the configured source.
- [ ] Every root resolves to a registered MicroBundle ID/version; FSM_COS produces the expected RuntimeAssembly.
- [ ] Invalid, missing, or unsupported artifacts fail visibly and safely.
- [ ] No downloaded executable assemblies or scripts are run.
- [ ] SVG/plan views identify their source and evidence class.
- [ ] Browser smoke test passes against the deployed endpoint.
- [ ] Cache, CORS, access policy, and rollback/version selection are verified.
- [ ] README and demo language distinguish current behavior from future plans.

*The goal is not to put the whole Workshop in a storage container. The goal is to make a released Experience reproducible, resolvable, and honestly observable.*
