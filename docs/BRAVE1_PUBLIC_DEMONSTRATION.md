# Brave1 Public Demonstration — MVP Readiness

## Purpose

This is the minimum public-facing demonstration intended to accompany a Brave1 intake submission. The goal is not to present the entire Workshop vision as completed. The goal is to give a reviewer a credible, understandable entry point into the technology and direct evidence they can inspect.

## What the visitor should understand

Within the first minute, a reviewer should be able to answer:

1. **Who is building this?** Trent Best, founder and builder of The Singularity Workshop.
2. **What is it?** An independent software laboratory exploring composable software: small, reusable capabilities assembled into different experiences and hosts.
3. **What am I looking at?** A working browser demonstration. The hub identifies the active manifest and the current destination; the architecture page explains the parts.
4. **What is technically meaningful?** FSM_API provides state-machine execution; FSM_COS composes declared MicroBundles and dependency closure into a RuntimeAssembly; the host presents the result.
5. **What is proven versus still in progress?** Manifest-driven composition and a living graphical demonstration exist. Some browser lifecycle behavior remains transitional, and the wider authoring-to-WebApp bridge is not yet complete.
6. **Where is the evidence?** The page links to the architecture explanation and source repositories.

## MVP visitor path

```text
PUBLIC WEBPAGE
    ↓
Workshop identity / opening Experience
    ↓
Hub's public technology brief
    ├── How the technology works
    ├── Source code
    └── Current manifest destination (AnyApp)
             ↓
       Current proof and limitations
```

The opening should demonstrate first, explain immediately afterward, and never require a reviewer to understand the repository before understanding the proposition.

## Claim boundary

### Safe to say today

- The Workshop is actively developing a composable software architecture.
- FSM_API, FSM_COS, and MicroBundles have distinct responsibilities.
- WebPage loads a host manifest and composes declared MicroBundles through FSM_COS.
- The current public page demonstrates a living graphical Experience.
- Source code and implementation status are available for inspection.

### Do not imply

- That the entire Workshop vision is already implemented.
- That all browser lifecycle behavior is already owned by the composed RuntimeAssembly.
- That WebApp already turns arbitrary authored page artifacts into complete websites.
- That AnyApp has a public installer or is a finished commercial desktop product.
- That a public branch commit is already live at the production URL.

## Submission gate

Before pasting the public URL into Brave1, verify each item against the deployed site—not just the development branch:

- [ ] The exact public URL loads in a clean browser session.
- [ ] The opening sequence reaches the hub without an error or blank surface.
- [ ] The technology brief is readable on desktop and mobile.
- [ ] The “How the technology works” link and source links open correctly.
- [ ] The AnyApp destination explains what is demonstrated and what is not yet available.
- [ ] No broken assets, console-breaking runtime errors, exposed secrets, or misleading completion claims are present.
- [ ] The latest development build and tests pass.
- [ ] The deployed site corresponds to the reviewed commit.
- [ ] A concise founder/project contact path is visible to a reviewer.

Deployment and package publication remain separate, explicitly approved actions. A green development build alone does not mean the public URL has been updated.

## Completion definition

The MVP is ready when an unfamiliar technical reviewer can understand the proposition, see a working demonstration, distinguish current proof from future direction, and follow links to the implementation—without needing a guided conversation.

*The goal is clarity and inspectable evidence, not the appearance of completion.*
