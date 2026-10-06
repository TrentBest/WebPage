# WebPage Usage

## Purpose

WebPage is a .NET 8 browser host and proving ground for The Singularity Workshop. It demonstrates how published Workshop capabilities are composed and manifested without moving their ownership into the host.

## Run locally

~~~bash
dotnet restore
dotnet run
~~~

Use the development branch for active engineering.

## Runtime shape

The host supplies the manifest and the host-side catalog/configuration boundary. FSM_COS composes the requested MicroBundles and returns a RuntimeAssembly. WebPage then manifests the assembled result through its browser presentation layer.

~~~text
manifest
   ↓
WebPage host
   ↓
FSM_COS
   ↓
RuntimeAssembly
   ↓
WebPage presentation
   ↓
browser
~~~

## External package usage

WebPage explains the integration contract for each external package it consumes, while the package's own repository remains authoritative for its complete API, domain documentation, and theory.

See [FSM_COS Usage](FSM_COS_USAGE.md), [Package Ecosystem](PACKAGE_ECOSYSTEM.md), and [Ecosystem Dependency Matrix](ECOSYSTEM_DEPENDENCY_MATRIX.md).

## Manifest-driven presentation

The host manifest determines which Experience/MicroBundle composition is active. Presentation should not silently invent a second architecture behind the manifest.

Current tab/Experience activation is documented in [Public Workshop Tabs](PUBLIC_WORKSHOP_TABS.md).

## Development discipline

- Keep reusable responsibility in its owning package.
- Keep browser-specific behavior in WebPage.
- Add tests for architectural claims.
- Update documentation when a boundary changes.
- Do not publish packages or releases without explicit approval.
