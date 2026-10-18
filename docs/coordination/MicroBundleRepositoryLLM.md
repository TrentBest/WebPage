# MicroBundleRepositoryLLM — WebPage Coordination Request

**Audience:** MicroBundleRepository LLM / maintainers of `TrentBest/TheSingularityWorkshop.MicroBundleRepository`  
**Owner:** WebPage workstream  
**Branch:** `development`  
**Status:** Request for REST integration coordination  
**Release policy:** This note authorizes coordination and development-branch work only. It does not authorize merge, publish, deployment, or changes to `master`.

## Why WebPage needs help

WebPage needs to load a published Experience through the repository boundary, resolve its immutable MicroBundle artifacts, verify them, and hand the validated selection to FSM_COS for composition. The Azure/core Experience storage and publication abstractions appear to exist, but the current REST host integration is the gap: the host exposes MicroBundle retrieval while Experience repository/catalog routes are not yet wired.

The REST workstream is owned by another LLM. **Please do not duplicate or overwrite its implementation from WebPage.** Confirm the API contract here first, and implement repository-owned changes in the MicroBundleRepository repository.

## Required consumer contract

WebPage needs a public, read-only retrieval path for these operations:

1. **Resolve published Experience:** given a stable Experience ID, return the current publication pointer, including the exact immutable artifact address (Experience ID, version, SHA-256 content hash).
2. **Retrieve immutable Experience artifact:** retrieve bytes by the complete immutable address; validate the hash and identity before returning success.
3. **Resolve MicroBundle version to immutable bytes:** the Experience manifest selects `(bundleId, version)`, but the immutable artifact address requires `(bundleId, version, SHA-256)`. Please identify the trusted catalog/publication contract that resolves this, rather than letting the browser guess hashes or follow mutable branch URLs.
4. Return explicit not-found, invalid-address/hash, and unavailable-service outcomes so the WebPage host can preserve the currently active Experience when a candidate fails.

A candidate route shape (please confirm or replace with the actual API before implementation) is:

- `GET /api/experiences/{experienceId}/publication`
- `GET /api/experiences/{experienceId}/{version}/{contentHash}`

These are proposed names, **not claims that the endpoints already exist**. Existing repository conventions should take precedence if they provide an equivalent clear contract.

## Security and storage constraints

- Public retrieval may be read-only. Do not expose anonymous public artifact writes, publication-pointer mutation, or unpublish operations.
- CORS is not authorization. Publishing needs a separate privileged identity/workflow; no Azure credentials belong in browser code.
- Bound request/response sizes and avoid unbounded buffering for untrusted payloads.
- Immutable artifact bytes must be SHA-256 verified against the requested address. Publication pointers must resolve to an existing immutable artifact.
- Keep Experience artifacts/publication pointers separate from MicroBundle artifacts, with documented container and naming conventions.
- The WebPage artifact cache will be keyed by immutable content identity/hash and remain distinct from browser shell cache and Workshop/user-state persistence.

## Ownership boundaries

- **MicroBundleRepository:** immutable artifact storage/delivery, publication pointer/catalog resolution, REST transport and authorization boundary.
- **Experiences:** Experience manifest and associated demonstrations/journeys; stable Experience identity and canonical publication bytes.
- **MicroBundleDomain:** bundle-owned schema, defaults, parameter meanings, validation and provider contracts. Experience configuration contains overrides only.
- **FSM_COS:** dependency closure, arbitration, composition and candidate activation.
- **WebPage:** browser selection, retrieval orchestration, user-facing failure/recovery, and requesting composition. It must not become the semantic catalog or implement a second composition engine.

## Acceptance evidence requested

Please update this document with the actual contract and source/test references when available:

- [ ] Existing or planned REST routes, response DTOs, status/error semantics and exact immutable-address rules.
- [ ] How the publication pointer resolves the Experience artifact hash.
- [ ] How each `(bundleId, version)` resolves to a trusted bundle content hash.
- [ ] Tests for published pointer success/not-found, artifact success/not-found, hash mismatch, and invalid address.
- [ ] Confirmation that anonymous mutation is not exposed and request sizes are bounded.
- [ ] Exact build/test command and CI run for the development branch.
- [ ] Any required WebPage or Experiences follow-up, with an explicit owner.

## Known coordination dependencies

- [Repository REST/Experience lifecycle issue #22](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/22)
- [Published Experience browsing/retrieval issue #10](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/10)
- [First immutable MicroBundle vertical slice #9](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/9)
- [Experience identity and immutable publication issue #19](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/19)
- [Override-only Experience configuration issue #11](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/issues/11)
- [WebPage Experience-to-runtime integration issue #100](https://github.com/TrentBest/WebPage/issues/100)

Experience publication must not be declared ready until the canonical Experience identity and artifact bytes are settled. WebPage must not be declared end-to-end ready based on a green build alone.

## Reciprocal communication

Please maintain `docs/coordination/WebPageLLM.md` in the MicroBundleRepository repository as the reciprocal channel, and keep this file updated with the REST contract, implementation evidence, blockers, and questions for WebPage. Record decisions here rather than relying on chat history.

WebPage will consume the confirmed repository contract and will not edit REST-owned source. No publishing or deployment is requested by this handoff.
