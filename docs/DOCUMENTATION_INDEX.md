# WebPage Documentation Index

> Documentation is engineering memory. It should teach the machine we have, the boundaries we are changing, and the proof behind both.

## Package ownership

- [PACKAGE_ECOSYSTEM.md](PACKAGE_ECOSYSTEM.md) — map each active capability to the Workshop package that owns it and identify WebPage-only responsibilities.
- [PROVING_GROUND_MIGRATION.md](PROVING_GROUND_MIGRATION.md) — explicit WebPage-to-package extraction contract, FSM_COS startup boundary, and branch consolidation rules.

## Start here

| Order | Document | Purpose |
|---:|---|---|
| 1 | ../README.md | Public purpose and architectural orientation |
| 2 | LEARNING_PATH.md | Visitor and developer curriculum |
| 3 | REPOSITORY_MAP.md | Source-tree ownership and dependency direction |
| 4 | WORKSHOP_RUNTIME_ARCHITECTURE.md | Runtime ownership and host boundary |
| 5 | FSM_COS_USAGE.md | Concrete WebPage → FSM_COS integration |
| 6 | EXPERIENCE_THEORY.md | Experience vocabulary and composition model |
| 7 | MICROBUNDLE_ARBITRATION_MAP.md | Loading and arbitration flow |
| 8 | WORKSHOP_OPENING_EXPERIENCE.md | Browser perception/startup contract |
| 9 | DEEP_DIVE_PROVIDER_ARCHITECTURE.md | WebPage-only educational provider boundary |
| 10 | CURRENT_VERTICAL_SLICE.md | Active implementation and proof obligations |
| 11 | PROVING_GROUND_MIGRATION.md | Extraction plan and architecture ownership contract |
| 12 | DOCUMENTATION_STANDARD.md | Documentation maintenance rules |

## Read architecture as a chain

~~~text
WHAT DOES THE VISITOR SEE?
          ↓
WHAT EXPERIENCE IS RUNNING?
          ↓
WHICH MICROBUNDLES ARE REQUESTED?
          ↓
WHAT DEPENDENCIES DOES FSM_COS RESOLVE?
          ↓
WHAT DOES RuntimeAssembly CONTAIN?
          ↓
WHAT DOES WEBPAGE PRESENT?
          ↓
WHAT PROVES THE CLAIM?
~~~

Do not infer ownership from the file that happens to render something.

## Current / Direction / Future

Every authoritative document must separate:

- Current — code or behavior that exists now.
- Direction — architecture actively being implemented.
- Future — not part of the current contract.

Historical experiments belong in history or their owning repository, not in current architecture claims.

## Package boundary

WebPage documentation should explain how the host consumes a package.

The package repository should explain the package's canonical API and theory.

This repository should link to FSM_API, FSM_COS, MicroBundleDomain, GUI packages, and other Workshop packages instead of copying their manuals.

## Maintenance rule

When code changes ownership or behavior, update the smallest authoritative set of documents that describes that contract.

If two documents disagree, reconcile them. Do not preserve contradictory architecture because both documents are old.

*The documentation should let the machine explain itself.*
