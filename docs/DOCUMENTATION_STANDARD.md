# WebPage Documentation Standard

> **Shared standard:** The cross-repository README and visual-writing standard is maintained in [FSM_COS — Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/DOCUMENTATION_STANDARD.md). This page records how WebPage applies that standard at the browser-host and proving-ground boundary; when wording overlaps, keep the shared rule and local application aligned.

## Purpose

The Singularity Workshop does not use documentation as decoration. Documentation is part of the engineering system.

A good document lets a reader answer three questions:

1. What exists?
2. Why does it exist?
3. Who owns it?

## The progressive-invitation rule

A README is not a manual compressed into one page. It is the front door to a learning journey. Order its content so that each section answers the next question a curious reader is likely to ask.

The standard reader journey is:

1. **Recognize — What is this?** Give the package a plain-language definition and a memorable, specific promise. A reader should not need to know Workshop vocabulary to understand the opening.
2. **Care — Why use it?** Name the problem, the cost or friction it removes, the people or systems it helps, and the situations where it is a good fit. Explain outcomes rather than presenting an unexplained feature inventory.
3. **Believe — Can I see it work?** Offer a runnable “first proof” that a reader can complete in about one minute after prerequisites are available. Show expected output or visible behavior. If the package cannot sensibly run alone, link to a real working demonstration and explain exactly what it proves.
4. **Understand — How does it work?** Introduce the smallest useful mental model, a readable diagram, the normal workflow, and the key contracts. Explain unfamiliar terms when first used.
5. **Evaluate — What do I get and what are the limits?** Describe capabilities, boundaries, configuration, compatibility, performance evidence, limitations, and trade-offs.
6. **Continue — Where do I go next?** Link to focused usage guides, API reference, architecture/theory, tutorials for non-coders, examples, contribution guidance, and support.

This sequence is progressive disclosure, not a ban on depth. A skimming reader should get the promise; a practical reader should reach a first success quickly; an engineer should be able to evaluate the contract; a learner should have a route into the underlying ideas. Let readers choose their depth after the value is clear.

### The first-minute proof

The quick start must be near the top of a package README—after the definition and reason to care, not buried after extensive architecture material.

A good first-minute proof:

- uses the smallest meaningful scenario, not a contrived example that hides the package's real purpose;
- states prerequisites and supported framework/version honestly;
- contains code that matches the current API and can actually be compiled or run;
- explains what the code does in plain language;
- shows the expected result, output, or visual behavior;
- links to a fuller walkthrough without forcing the full walkthrough on everyone.

Never label an unverified snippet “working.” Validate it against the current source/API, and use CI or a reproducible command when feasible. If a true one-minute proof is impossible, explain the reason and provide the shortest honest path.

### README and documentation have different jobs

The README is the **orientation, value proposition, first proof, and map**. It should answer “what, why, show me, how does it fit, and where next?”

Focused documentation bodies carry the depth:

- **Getting started / usage:** setup, common tasks, runnable examples, expected results, and troubleshooting.
- **Concepts / theory:** mental models, adjacent concepts, assumptions, trade-offs, and why the design exists.
- **Architecture:** responsibility ownership, dependency direction, contracts, lifecycle, and diagrams.
- **API reference:** exact public types, members, constraints, and examples.
- **Performance / verification:** reproducible benchmarks, test evidence, environment, and limitations.
- **Tutorials for non-coders:** vocabulary, analogies with explicit limits, step-by-step reasoning, and exercises that build a correct mental model.
- **Integration guides:** how this package is consumed by another package or host, without claiming ownership of the other domain.

The README should link to these documents with descriptive labels and a brief statement of what each helps the reader accomplish. Do not duplicate whole manuals into the README.

### Teach the reader how to think

Non-coder education is not a simplified glossary pasted onto the end. It is a deliberate route from familiar experience to useful software concepts.

When introducing a difficult idea:

1. Begin with a concrete problem or observable behavior.
2. Offer a familiar analogy, then state where that analogy stops being accurate.
3. Name the software concept and define it in ordinary language.
4. Show how the concept changes the design decision.
5. Connect the concept to a small example or diagram.
6. Offer a deeper explanation for readers who want to continue.

Teach relationships and decision-making, not just vocabulary. Explain why an abstraction exists, what it protects, what it costs, and when it is not the right choice. Never imply that a reader must already be a programmer to understand the problem.

### Visual hierarchy and color

Visuals are part of the explanation. Use them to reduce cognitive load and help the reader recognize structure before reading every sentence.

- Establish a consistent hierarchy: one clear title, short opening, scannable section headings, concise paragraphs, and lists for parallel items.
- Put the most important distinction first. Use whitespace and short sections to make the page navigable.
- Prefer a useful architecture or sequence diagram over several paragraphs describing the same relationships. Prefer an actual screenshot or animation when the product's behavior is the evidence.
- Use tables for comparisons and compact reference facts, not as a substitute for explaining the meaning.
- Use color consistently to encode meaning—not as decoration. Define a small semantic palette for roles such as definition, benefit, procedure, caution, and deep theory; keep the same meaning across diagrams and related documentation.
- Do not rely on color alone: pair it with labels, icons, line styles, or text so diagrams remain understandable for color-blind readers and in monochrome.
- Check contrast, text size, mobile rendering, and GitHub's light/dark presentation. Keep SVGs legible at ordinary screen width and give meaningful images descriptive alt text.
- Use badges sparingly for genuine status or compatibility facts. Badge walls, emoji walls, and ornamental dividers must not compete with the package's actual promise.

Color-theory, literary, and visual-design techniques should serve comprehension: contrast establishes priority, repetition creates recognition, proximity communicates relationship, and a well-chosen metaphor gives the reader a mental handle. Never let atmosphere promise a capability the code does not provide.

### Claims, persuasion, and trust

The purpose is to make useful software compelling, not to pressure readers into believing unsupported claims.

- Lead with the strongest **specific, supportable** benefit.
- Prefer measured results, demonstrated behavior, and concrete before/after examples over superlatives.
- Separate current capability from direction and future intent.
- Explain costs, limitations, prerequisites, and poor-fit scenarios alongside benefits.
- Never fabricate benchmarks, adoption, guarantees, compatibility, or completeness.
- Make the next step obvious without hiding the source, API, or evidence.

Confidence comes from clarity and proof. The reader should feel invited by the value and equipped to verify the claim.

### Package README review checklist

Before considering a README ready for public use, ask:

- [ ] Can a newcomer explain what the package does after reading only the opening?
- [ ] Is the problem and reason to use it clear before the feature list?
- [ ] Is there a first proof near the top, with expected results and verified current API usage?
- [ ] Does the README distinguish package responsibilities from neighboring packages and hosts?
- [ ] Are the most important benefits concrete and supportable?
- [ ] Can readers skim the page and find the next useful section?
- [ ] Do diagrams, screenshots, and color explain actual concepts or behavior?
- [ ] Are limitations, maturity, framework support, and publication status honest?
- [ ] Are usage, theory, architecture, API reference, and deeper learning linked rather than duplicated?
- [ ] Can a non-coder follow a separate path that builds understanding instead of merely simplifying terminology?
- [ ] Do the examples, links, badges, and claims match the current repository state?
- [ ] Is the README readable on a narrow screen and without relying on color alone?

## The three-state rule

Every architectural document must distinguish:

### Current

State facts that can be verified in the repository or running product.

### Direction

Design intent that is actively guiding implementation but is not necessarily complete.

### Future

Ideas, research, or capabilities that are explicitly not yet implemented.

Never use future language to imply a feature already works.

## The ownership rule

For every significant behavior, documentation should identify its owner.

```text
Experience       = what is being composed
MicroBundle      = focused capability
FSM_COS          = composition and RuntimeAssembly
FSM_API          = deterministic state execution
Renderer         = rendering computation / representation policy
WebPage          = browser presentation and proving-ground UX
Repository       = artifact discovery / persistence when assigned
```

Do not move ownership upward merely because the current host happens to call the code.

## The evidence rule

A claim should have an observable proof:

- a test;
- a running behavior;
- a benchmark;
- a repository contract;
- or an explicitly labeled research hypothesis.

Visual behavior deserves visual evidence. Architecture deserves tests and diagrams. Performance claims deserve measurements.

## The stale-reference rule

Documentation must not carry obsolete platform assumptions forward simply because they existed in an earlier experiment.

If a historical integration remains relevant, point to its dedicated repository rather than rebuilding its architecture inside WebPage documentation.

WebPage is browser-first. Platform-specific integrations are separate concerns.

## The language rule

Prefer precise Workshop vocabulary:

- Experience
- MicroBundle
- RuntimeAssembly
- composition
- arbitration
- ontology
- manifestation
- provider
- Deep Dive
- proving ground

Do not invent synonyms that make ownership ambiguous.

## The visual rule

Use a diagram when a diagram is clearer than prose.

Use a screenshot when the reader needs to see the actual product.

Use a table when comparing contracts or ownership.

Do not turn every README into a wall of text.

## The honesty rule

Say when something is transitional.

Say when a path is experimental.

Say when another repository owns the canonical implementation.

Say when a capability does not exist yet.

That is not weakness. It is how a workshop earns trust.

## The public-writing rule

Public documentation should be technically honest while still sounding like The Singularity Workshop.

Be precise. Be playful when useful. Be strange when the software is strange.

> **Dr. Seuss for software developers — but with the tests passing.**

## Completion rule

A meaningful implementation change is not complete until:

1. the code is correct;
2. the tests prove the intended behavior;
3. the relevant documentation agrees with the code;
4. obsolete claims are removed;
5. CI is checked;
6. the commit can be understood by the next engineer.

*If the documentation lies, the Workshop has already begun to drift.*


## Package extraction

WebPage is allowed to be the fastest place to prove a capability. It is not automatically the permanent owner.

When behavior moves into a reusable package, documentation should move with it:

~~~text
WebPage proof
   ↓
package contract
   ↓
package usage
   ↓
theory / decision
   ↓
WebPage integration proof
~~~

The WebPage repository should explain how it consumes the package and why the boundary exists. The package repository should explain how developers consume the reusable capability.

Do not copy an entire package's theory into WebPage. Link to the package's authoritative documentation and document only the WebPage integration.

## FSM_COS usage

If a WebPage feature crosses the composition boundary, the authoritative local guide is FSM_COS_USAGE.md.

That document should answer:

- what Experience is selected;
- which MicroBundles are requested;
- how the catalog resolves them;
- how the RuntimeManifest is constructed;
- what FSM_COS owns;
- what RuntimeAssembly means;
- what WebPage does after handoff.

This is the operational companion to the deeper FSM_COS theory.
