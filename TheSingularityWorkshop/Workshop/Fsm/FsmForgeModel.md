# FSM Forge abstraction

The Forge is a visual assembler, not the runtime FSM itself.

## Three control surfaces

### 1. State ingots
A state is a durable object placed on the workbench. It owns:

- name
- intent / description
- `OnEnter`
- `OnUpdate`
- `OnExit`

The lifecycle methods are **slots**, not code editors. An ingot can therefore be moved around without changing the semantic identity of the state.

### 2. Condition surface
Conditions live outside the state ingot because a condition is reusable logic, not state lifecycle behavior.

A condition has:

- name
- expression
- tokens

Transitions reference conditions by identity.

### 3. Connection surface
Connections are directed edges between state inputs. A transition is therefore modeled as:

`State.Input -> [Condition] -> State.Input`

This lets the UI draw wires between lifecycle/input ports without encoding the wire itself as GUI state.

## Tokens

Tokens should initially be deliberately small:

- operands: state/context values
- literals: numbers, strings, booleans
- comparison operators: `==`, `!=`, `<`, `>`, `<=`, `>=`
- boolean operators: `AND`, `OR`, `NOT`
- grouping: `(`, `)`

The first implementation should construct an expression tree rather than accept arbitrary source code. The eventual scaffold generator can translate that tree into target-language code.

## Why this separation matters

The workbench becomes a spatial editor for a graph. The condition surface becomes a spatial editor for predicates. The eventual code editor is downstream of both.

That gives us a clean pipeline:

`ingots -> slots -> connections -> conditions -> preview -> scaffolded behavior code`

The Forge should never need to know how the final C# behavior is written in order to preview the machine.
