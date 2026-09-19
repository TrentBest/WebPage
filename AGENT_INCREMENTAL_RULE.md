# Mandatory Agent Incremental Proof Rule

## Every meaningful change MUST leave a versioned unit-test heartbeat

An AI agent working in this repository must not merely describe a proposed change and stop.
At a minimum, every meaningful code change must include a visible incremental unit-test/version marker that can be seen in the IDE and verified by the test runner.

### Required behavior

1. Make the requested code change.
2. Add or update a concrete incremental unit test that proves the change exists.
3. Append the next sequential versioned test to `SingularityHub.Tests/IncrementalVersionTests.cs`.
4. Make the corresponding incremental version test assert the new version value and describe the actual change rather than using a meaningless always-green assertion.
5. Report the resulting commit SHA and the incremental test/version in the response.

### Why this exists

The incremental marker is an operational proof-of-work signal. It lets the developer see that the agent actually changed the repository instead of returning prose, speculation, or an uncommitted suggestion.

**Do not remove, bypass, or replace this rule with a claim that the change was made. The repository must contain the proof.**
