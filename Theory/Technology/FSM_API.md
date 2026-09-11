# FSM_API: Architectural Reference

## Purpose

FSM_API is a software-agnostic Finite State Machine system for C# applications. Its central architectural concern is deterministic state-driven behavior while keeping state logic decoupled from application data.

## Architectural layers

The public model is intentionally layered:

- **FSMBuilder** — declaratively defines states, transitions, and lifecycle actions.
- **FSMHandle** — represents a runtime FSM instance operating on a context.
- **IStateContext** — the application-owned context contract.
- **Processing Groups** — organize and control update cycles for FSM instances.
- **Interaction/update cycle** — advances the selected processing group.

## Deterministic execution

The documented tick cycle is deliberately constrained. A tick may perform an Enter operation when required, an Update operation, and transition evaluation. When a transition occurs, the current state's Exit executes, the target state becomes current, and Enter for the new state is deferred until the next tick.

That separation is important: lifecycle work is not recursively cascaded through an arbitrary chain of transitions inside one update.

## Processing groups

Processing Groups provide a sequential boundary around a set of FSM updates. They allow independently controlled update domains while preserving a deterministic order inside each group.

This concept is significant to later Workshop architecture: a processing group describes **logical sequencing**, not the mechanism by which a machine obtains CPU or other hardware resources.

## Design rationale

FSM_API deliberately avoids coupling the state machine to a game engine, UI framework, or other host environment. Application data remains in ordinary C# context objects while the FSM provides behavior and lifecycle management around that data.

## Performance and verification

The project documentation treats lightweight execution, low allocation, unit testing, diagnostics, and mathematically analyzable state transitions as first-class concerns. Benchmark results and formal claims should be recorded separately from architectural intent when they are added to this library.

## Source

The implementation and primary public documentation live in `TrentBest/FSM_API`. This document is a theory-library reference rather than a replacement for that repository's API documentation.
