# Singularity Hub

`SingularityHub` is the first concrete implementation of the Hub abstraction.

The Hub owns:

1. MicroBundle registration.
2. Deterministic, maximum-ten-round arbitration.
3. Process Group lifecycle tracking.
4. The abstract liaison to warehouse identity.

It does **not** own hardware scheduling. Eligible work crosses `IExecutionProvider`, where the host decides how computational resources are acquired.

The project is intentionally inside WebPage for the first working deployment. It can later move to its own repository without changing the kernel contract.
