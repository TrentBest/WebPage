# Singularity Hub Abstractions

This project is the kernel boundary of the Singularity Hub.

It contains only platform-neutral contracts and fixed-depth runtime value types:

- `ISingularityHub` — the public Hub surface.
- `IArbitrator` / `IMicroBundle` — MicroBundle registration and arbitration.
- `OntologySignature` — nine integer runtime layers.
- `IDataWarehouseLiaison` — identity-to-storage boundary.
- `IProcessGroupHost` — lifecycle without execution scheduling.
- `IExecutionProvider` — the opaque boundary where a host chooses Task, Thread, process, GPU queue, or another mechanism.

**Law:** the abstraction project must never acquire platform scheduling or UI dependencies.
