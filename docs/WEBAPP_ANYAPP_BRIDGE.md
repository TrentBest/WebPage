# WebApp ↔ AnyApp Bridge

The Workshop WebApp can optionally connect to a local AnyApp host. This is a manifestation bridge, not a second runtime.

## Bootstrap

The WebApp can launch a published Experience in AnyApp with a registered desktop URI:

`anyapp://experience/{experienceId}/{version}/{contentHash}`

The browser supplies a short-lived launch token generated for that request. AnyApp uses the token to authorize the initial local bridge session.

## Local session

After launch, the WebApp connects to the loopback AnyApp bridge using WebSocket.

The browser sends a versioned `hello` message containing:

- protocol version
- WebApp origin
- browser/platform capability summary
- viewport dimensions
- device-pixel ratio
- visibility/focus state
- WebXR availability
- supported XR session modes
- launch/session token

AnyApp replies with `welcome` and a bridge session identifier.

## Co-entanglement

The browser may then observe:

- connection state
- requested Experience
- active Experience
- runtime lifecycle state
- MicroBundle identity summary when exposed by the host
- localization/loading state when exposed by the host
- heartbeat/latency
- explicit Experience events

The browser may send only explicit Experience/bridge events defined by the protocol. It does not become the authority for FSM execution.

## XR

WebXR capability detection belongs in the browser manifestation. If the browser/device supports an immersive VR session, the same Experience identity can be presented through XR while the desktop session remains a separate manifestation.

The first target is capability detection and a minimal immersive-vr entry point; richer spatial manifestation comes later through the semantic observer/view work in GUI.Core.

## Privacy boundary

The bridge should never collect credentials, cookies, browsing history, arbitrary DOM contents, or unrelated browser data. Capability telemetry exists to make the shared environment useful, not to fingerprint the visitor.
