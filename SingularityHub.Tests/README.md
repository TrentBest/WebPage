# Architecture and Incremental Tests

These tests are not disposable cleanup targets. They are breadcrumbs for the landing architecture.

## Key tests

- `IdleExperienceArchitectureTests.Unit 04` — exact landing presentation sequence.
- `Incremental Unit Test 07` — disposing a PageFSM unregisters its handle.
- `Incremental Unit Test 08` — one PageFSM update does not advance another.
- `Incremental Unit Test 09` — the moniker remains hidden until exactly 100 nodes, then persists through gravity.
- `IncrementalVersionTests` — visible proof-of-work heartbeat for repository changes.

## Agent rule

For every meaningful change, advance `CurrentVersion` and add/update the corresponding meaningful version assertion. See `../AGENT_INCREMENTAL_RULE.md`.

Do not weaken sequence tests merely to get green. If the contract fails, repair the implementation or explicitly update the canonical presentation contract with the user's approval.
