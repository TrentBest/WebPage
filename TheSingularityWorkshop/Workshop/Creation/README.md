# Workshop Creation

Creation is the persistence boundary between an editable Workshop definition and a runtime manifestation.

A **WorkshopAsset** has:

- a stable user-facing name,
- a recursive platform-neutral `GuiNode` tree,
- an optional declarative `FsmBlueprint`.

The important distinction is that we persist **definitions**, not runtime objects.

```text
                 Workshop Creation
                        |
              +---------+---------+
              |                   |
         GUI definition      FSM blueprint
              |                   |
           GuiNode          states/transitions
              |                   |
              +---------+---------+
                        |
                 WorkshopAsset
                        |
                 binary codec
                        |
                  stored artifact
                        |
                    rehydrate
                        |
              +---------+---------+
              |                   |
          render GUI         compile FSM
```

## Why the FSM is a blueprint

GUI trees are already declarative, so their structure can be serialized directly.

FSM runtime definitions can contain executable delegates (`OnEnter`, `OnUpdate`, `OnExit`, transition predicates). Those delegates are not treated as binary data. `FsmBlueprint` stores the durable graph and symbolic conditions; a later compiler/runtime adapter turns that blueprint plus host-provided behavior into a live FSM_API instance.

This gives the Workshop a clean separation:

1. **Define** — edit the artifact.
2. **Persist** — encode the artifact as binary.
3. **Rehydrate** — recover the definition without a renderer.
4. **Compile** — create platform/runtime objects.
5. **Run** — let FSM_API own execution.

## Browser persistence

`WorkshopAssetLibrary` stores the binary artifact as Base64 through `IWorkshopStorage`. Base64 is only the browser-storage transport representation; the artifact itself is produced and consumed by `WorkshopAssetBinaryCodec`.

The storage abstraction remains platform-neutral, so the same artifact can later live in a filesystem, server repository, or AnyApp storage implementation.

## Next vertical slice

The intended Workshop creation surface is a real editor rather than a form:

- **Library** — named artifacts and rehydration.
- **GUI** — recursive builder tree and live Web manifestation.
- **FSM** — state graph and transition recipe.
- **Manifest** — the serialized artifact boundary.
- **Run** — compile the selected definition and execute it.

That editor should be another recursive GUI-builder composition, making the Workshop itself the first substantial consumer of the architecture it exposes.
