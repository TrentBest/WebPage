# Solution-native Experience tests

Projects under `Experiences/` are the executable architectural boundary for **core Experiences**.

A core Experience is recognized by being built into `WebPage.sln`; an externally configured Experience is not required to become solution-native merely to participate in the Hub.

Current core Experience test projects:

- `LivingGuiExperience.Tests`
- `PongExperience.Tests`
- `OntologyMicroBundle.Tests`

These projects intentionally begin as contract/catalog tests. As the Experience model becomes concrete, each project becomes the home for that Experience's focused lifecycle, sensory, ontology, and process-group tests.

## Knowledge MicroBundles

`PhysicsKnowledgeLut` establishes the first graduated knowledge payload: the same subject can expose **Essential, Intermediate, Advanced, and Expert** material without changing its integer MicroBundle identity.

`PhysicsKnowledgeMicroBundleCatalog` promotes those tables into independently addressable MicroBundles. This is the beginning of the larger knowledge strategy:

`ontology address -> MicroBundle -> detail level -> facts/equations -> compiled runtime representation`

The authored LUT is deliberately separate from the future runtime representation. That gives us room to compile equations, units, constants, relationships, provenance, and other structured knowledge into integer-addressed tables rather than forcing the heartbeat to interpret prose.

The same architecture is intended for mathematics, history, engineering, and other knowledge domains. A user should be able to request a useful level of detail without loading the entire knowledge graph.
