# Testing and Coverage

WebPage follows the testing discipline established by FSM_API.

## Local test run

The test projects are:

- SingularityHub.Tests
- Experiences/LivingGuiExperience.Tests
- Experiences/PongExperience.Tests

Run one suite:

    dotnet test SingularityHub.Tests/SingularityHub.Tests.csproj --collect:"XPlat Code Coverage" --results-directory ./TestResults/SingularityHub --logger "trx;LogFileName=SingularityHub.Tests.trx"

Run all three using the same pattern, with separate TestResults directories for each suite.

Each run produces machine-readable TRX results and Cobertura coverage under TestResults/.

## Reading a failed run

The TRX files are the failure ledger. They contain the complete test outcome without requiring individual failures to be copied into a chat.

The generated TestResults/ directory is intentionally ignored by Git. It should be uploaded as a CI artifact rather than committed to the repository.

## GitHub Actions

The .NET Tests workflow is intentionally manual while the repository is being consolidated. Run it from Actions → .NET Tests → Run workflow.

The workflow restores all test projects, builds Release, runs all three suites, writes TRX results, collects cross-platform coverage, uploads TRX and coverage as a 14-day artifact, and uploads to Codecov when CODECOV_TOKEN is configured.

## Architecture tests

Architecture tests are first-class tests. They enforce contracts such as FSM/API lifecycle ownership, MicroBundle identity and ontology, Experience composition, Hub arbitration, bounded ten-round convergence, dependency loading, installation order, and registry behavior.

## Coverage philosophy

Coverage is evidence about exercised code, not a substitute for architectural tests. The goal is to make important behavior executable and observable, then use coverage to identify untested surfaces.

No arbitrary coverage threshold is imposed yet. We will establish thresholds after the current architecture has been consolidated and the remaining transitional host code has been migrated.
