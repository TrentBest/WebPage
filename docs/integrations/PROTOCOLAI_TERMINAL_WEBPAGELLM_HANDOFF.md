# ProtocolAi Terminal — WebPageLLM Handoff

**Branch:** `development`  
**Purpose:** Keep the browser terminal tiny while rebuilding the manual LLM exchange and the safe path from returned text to intent actuation.

## User experience

The terminal is exactly two actions:

1. **Copy to Clipboard** — breathes when ProtocolAi and GrammarAi context is composed. Clicking copies the composed protocol and grammar descriptions plus optional host-provided task/context. The user pastes that into an LLM of their choice.
2. **Paste from Clipboard** — begins breathing after the copy succeeds. The user copies the LLM response and returns to click this action.

The page never calls an LLM provider. The terminal should not display separate protocol/grammar textareas, a response textbox, provider selectors, or a multi-step dashboard. A compact status line is acceptable. Breathing respects `prefers-reduced-motion`.

## Current implementation

- `TheSingularityWorkshop/Components/AiExchangePanel.razor` provides the two clipboard actions and their breathing state.
- `TheSingularityWorkshop/Workshop/Composition/AiExchangeCompositionBundle.cs` composes the same two-action terminal surface from FSM_COS.
- `TheSingularityWorkshop/Pages/AI.razor` receives the returned text through `OnResponseImported` and stages it in page memory as a proposal. The page explicitly reports that validation and actuation are not implemented.
- The clipboard interaction uses the browser Clipboard API and therefore requires browser permission / a supported secure context.

## Intent actuation boundary

Do **not** execute arbitrary model output. The next implementation stages should be separate and observable:

1. Define a concrete response envelope from the actual ProtocolAi/GrammarAi types and intended host context; do not invent a parallel protocol or infer unsupported grammar behavior.
2. Parse and validate imported text against that envelope and the composed grammar, with explicit rejection reasons.
3. Resolve each accepted intent to a registered host capability/command, checking identifiers, argument types, scope, and current state.
4. Produce a reviewable action plan before any mutation. Make authorization and any confirmation policy explicit.
5. Execute only approved, validated commands through a host-owned dispatcher; record results and failures. Never let the LLM claim that an action succeeded.
6. Test malformed input, unknown commands/IDs, invalid arguments, stale state, partial failure, and safe rejection.

The current code only **stages** the response. It does not yet parse, validate, dispatch, or actuate intent. Do not present the breathing animation as evidence of validation or execution.

## Verification

After the changes, run the WebPage build and test workflow on `development`. In particular, retain the composition contract that exposes `ai-copy-to-clipboard` and `ai-paste-from-clipboard`. Do not publish a package, merge a PR, or deploy from this handoff.
