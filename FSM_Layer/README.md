# FSM_Layer

The FSM_Layer is the behavioral layer of MicroBundles used to assemble an AnyApp.

**Architecture heartbeat: 0.0.25**

MicroBundles are recursively composable. A MicroBundle may contain lesser MicroBundles, and a lesser MicroBundle may itself contain further MicroBundles without a fixed depth limit. A top-level domain MicroBundle therefore acts as the master of the behavior it contains while remaining a normal MicroBundle that can itself be contained by a still larger bundle.

## Development-time recursion

Recursive composition is a development-time concern. `MicroBundleCompiler` resolves the recursive graph, rejects missing children and cycles, deduplicates shared children, and emits a deterministic flattened closure.

Ontology addresses are likewise compiled deliberately. The nine-layer `OntologySignature` is accessed through its integer indexer; authored ontology data must place semantic tokens at their intended layers rather than relying on a partial positional path. The initial physics inventory uses the ninth layer for distinct topic tokens while leaving intermediate layers available for future semantic refinement.

## Runtime

Runtime does not recursively discover the graph. It receives the compiled closure and executes the behavioral MicroBundles through FSM/API process groups. This keeps recursion, authoring, dependency discovery, and ontology expansion out of the heartbeat path.

The intended pipeline is:

`authored MicroBundle tree -> development compiler -> static integer closure -> Hub/FSM runtime -> AnyApp`

The recursive model is deliberately not a separate domain/container abstraction. **The domain is itself a MicroBundle.**
