# True O(9)

**Status:** Research framework / working theory

True O(9) is the hypothesis that a fixed-depth, nine-layer ontological address can provide a stable structural coordinate system for software entities across domains and implementations.

This directory treats the idea as a research subject, not as an unquestionable architectural fact. The implementation supplies evidence and constraints; the theory states the propositions that can be tested against them.

## Lineage

The idea emerged from practical work involving cross-domain tooling, data organization, micro-bundle arbitration, and the need to translate human-readable concepts into compact runtime representations. The recurring constraint was that semantic identity and relationships needed a structural coordinate system that could survive changes in presentation, host platform, and implementation detail.

The current Workshop architecture represents the address as nine integer layers. Strings belong at authoring and translation boundaries; runtime structures are intended to operate on numeric identities and fixed structural coordinates.

## Core hypothesis

> A fixed-depth ontological address can act as a universal structural coordinate system when its layers are deterministic, comparable in constant time, and independent of presentation technology.

"O(9)" is deliberately literal in this context: the architecture has nine layers. Because nine is a fixed constant, operations over all layers are constant-time with respect to the number of layers. This should not be confused with a claim that every operation involving an addressed object is globally O(1).

## What this theory attempts to explain

- Why a fixed number of semantic layers can be useful as an address space.
- Why editor-time names can be translated into integer identities before runtime.
- Why structural identity should not depend on UI, engine, framework, or serialization vocabulary.
- How ontological coordinates can become keys for data and capability resolution.
- How the same address space can support tooling, micro-bundles, arbitration, and future AI-assisted composition.

## What remains unproven

The theory does not yet establish that nine layers are universally optimal, that every domain maps cleanly to nine layers, or that a particular integer assignment is canonical. Those are empirical and architectural questions.

## Documents

- [Propositions](PROPOSITIONS.md) — explicit claims that can be challenged or tested.
- [Evidence](EVIDENCE.md) — implementation observations currently supporting or constraining the theory.
- [Open Questions](OPEN-QUESTIONS.md) — unresolved questions and proposed investigations.

## Academic use

A professor, researcher, or author may treat this directory as source material for independent analysis. Claims should be cited as Workshop research and separated from established external theory unless independent literature establishes the same proposition.
