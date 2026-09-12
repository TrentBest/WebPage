# Solution-native Experience tests

Projects under `Experiences/` are the executable architectural boundary for **core Experiences**.

A core Experience is recognized by being built into `WebPage.sln`; an externally configured Experience is not required to become solution-native merely to participate in the Hub.

Current core Experience test projects:

- `LivingGuiExperience.Tests`
- `PongExperience.Tests`

These projects intentionally begin as contract/catalog tests. As the Experience model becomes concrete, each project becomes the home for that Experience's focused lifecycle, sensory, ontology, and process-group tests.
