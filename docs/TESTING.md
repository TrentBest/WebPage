# Testing and Coverage

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


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

The .NET Tests workflow runs automatically for pull requests targeting `development` and pushes to `development`. It can also be started manually from Actions → .NET Tests → Run workflow.

The workflow installs Chromium for the browser journey, restores and builds the WebPage browser host, runs the browser smoke test, builds and runs all three .NET test suites, writes TRX results, collects cross-platform coverage, and uploads test results and coverage as a 14-day artifact. Coverage is uploaded to Codecov when `CODECOV_TOKEN` is configured.

The source-integration job also checks the proposed WebPage pull-request revision against sibling Workshop development sources; on development pushes and manual runs, it checks the current `development` branch.

## Architecture tests

Architecture tests are first-class tests. They enforce contracts such as FSM/API lifecycle ownership, MicroBundle identity and ontology, Experience composition, Hub arbitration, bounded ten-round convergence, dependency loading, installation order, and registry behavior.

## Coverage philosophy

Coverage is evidence about exercised code, not a substitute for architectural tests. The goal is to make important behavior executable and observable, then use coverage to identify untested surfaces.

Browser smoke coverage verifies user-visible behavior but does not automatically count as .NET line coverage for Razor markup. Where component markup is flagged as uncovered, treat that as an explicit testing gap rather than claiming browser checks have resolved it.

No arbitrary coverage threshold is imposed yet. We will establish thresholds after the current architecture has been consolidated and the remaining transitional host code has been migrated.
