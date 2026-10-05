# WebPage Documentation Index

> **Documentation is engineering memory. It describes the machine we actually have, the architecture we are deliberately moving toward, and the experiments we have not finished.**

## Start here

| Document | Purpose | Authority |
|---|---|---|
| README.md | Public repository entry point and current product story | public orientation |
| DOCUMENTATION_STANDARD.md | Rules for writing and maintaining Workshop documentation | documentation policy |
| WORKSHOP_RUNTIME_ARCHITECTURE.md | Current WebPage runtime ownership and FSM_COS handoff | runtime contract |
| FSM_COS_USAGE.md | How WebPage consumes FSM_COS and how reusable functionality is extracted | operational usage |
| WEBPAGE_EXPERIENCE_ARCHITECTURE.md | What an Experience means in this browser proving ground | WebPage product contract |
| WORKSHOP_OPENING_EXPERIENCE.md | Current first-contact sequence | presentation contract |
| EXPERIENCE_THEORY.md | Experience vocabulary and composition model | domain theory |
| MICROBUNDLE_ARBITRATION_MAP.md | Installation and arbitration flow | composition explanation |
| DEEP_DIVE_PROVIDER_ARCHITECTURE.md | WebPage-only Deep Dive provider boundary | host capability contract |
| CURRENT_VERTICAL_SLICE.md | Current implementation state, transitional work, and proof obligations | engineering handoff |
| PUBLIC_WORKSHOP_TABS.md | Explore/Create/Education/MadMen/About/Consult information architecture | public UX direction |

## How to read the architecture

Use the documents in this order when investigating a feature:

```text
WHAT DOES THE VISITOR SEE?
          ↓
WHAT EXPERIENCE IS RUNNING?
          ↓
WHICH MICROBUNDLES ARE COMPOSED?
          ↓
HOW DOES THIS HOST USE FSM_COS?
          ↓
WHAT DOES THE WEBPAGE HOST OWN AFTER HANDOFF?
          ↓
WHAT PROVES THE CLAIM?
```

Do not infer runtime ownership from where a visual element happens to be rendered.

## Current versus future

Every document should clearly separate:

- **Current** — behavior or code that exists now.
- **Direction** — architecture we are deliberately moving toward.
- **Future** — useful ideas that are not yet implemented.

An implementation detail must not be presented as a permanent contract merely because it exists today.

## Host boundaries

WebPage is the browser proving ground. It is not the canonical owner of every Workshop concept.

Shared runtime/domain repositories should remain independent of WebPage. Host-specific educational or presentation behavior belongs here only when it is explicitly a WebPage capability. Read FSM_COS_USAGE.md for the concrete integration boundary.

## Maintenance

When code changes ownership or behavior, update the smallest set of authoritative documents that describe that contract. Do not copy the same architecture into five documents just to make it appear documented.

If two documents disagree, reconcile them rather than choosing whichever sounds newer.

*The documentation should let the machine explain itself.*
