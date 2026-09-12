# FSM_Layer

The FSM_Layer is the behavioral layer of MicroBundles used to assemble an AnyApp.

MicroBundles are recursively composable. A MicroBundle may contain lesser MicroBundles, and a lesser MicroBundle may itself contain further MicroBundles without a fixed depth limit. A top-level domain MicroBundle therefore acts as the master of the behavior it contains while remaining a normal MicroBundle that can itself be contained by a still larger bundle.

## Development-time recursion

Recursive composition is a development-time concern. `MicroBundleCompiler` resolves the recursive graph, rejects missing children and cycles, deduplicates shared children, and emits a deterministic flattened closure.

## Runtime

Runtime does not recursively discover the graph. It receives the compiled closure and executes the behavioral MicroBundles through FSM/API process groups. This keeps recursion, authoring, dependency discovery, and ontology expansion out of the heartbeat path.

The intended pipeline is:

`authored MicroBundle tree -> development compiler -> static integer closure -> Hub/FSM runtime -> AnyApp`

The recursive model is deliberately not a separate domain/container abstraction. **The domain is itself a MicroBundle.**
