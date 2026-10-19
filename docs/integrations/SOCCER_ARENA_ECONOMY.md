# Soccer Arena Economy — Singularity City

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Audience:** WebPageLLM, Singularity City, venue, commerce, and MicroBundleDomain integration work  
**Status:** design contract; ticketing, payment, inventory, and settlement are not claimed to be implemented  
**Scope:** admission, concessions, merchandise, memorabilia, and vendor commerce  
**Boundary:** no merge, publication, deployment, or real payment connection without the owner's explicit approval.

## Purpose

The arena is part of a living city, not an isolated match screen. A match brings Digitens and visitors to the venue, gives them reasons to move through concourses and stands, and supports a local economy around the event. Tickets, food and beverages, team merchandise, and memorabilia should be represented as domain activities with inspectable state—not decorative shop icons or invented sales totals.

The goal is an event whose economic story can be followed: tickets are acquired before entry; arrivals are admitted against valid tickets; visitors can browse vendors, purchase refreshments or souvenirs, and return to their seats; sellers have stock, prices, and sales records; and the event has a transparent financial summary.

## Economy pillars

1. **Admission:** tickets, seat/section assignment, entry validation, capacity, and ticket status.
2. **Concessions:** food, non-alcoholic beverages, and other approved consumables sold at kiosks, carts, or concourse stalls.
3. **Merchandise:** team scarves, shirts, replica kits, caps, and other branded goods.
4. **Memorabilia:** match programs, commemorative items, and event-specific collectibles.
5. **Vendor operations:** vendor identity, stall location, catalog, prices, stock, transactions, and settlement.
6. **Event accounting:** gross sales, refunds, fees, taxes where applicable, vendor share, venue share, and net totals derived from recorded transactions.

## 1. Tickets and admission

A ticket is a domain record, not just a purchase button. It should have a stable identity and identify the event, ticket category, admission status, price/currency, and any assigned section/seat. General-admission tickets may omit a specific seat. Seat assignment must resolve through the venue's canonical seating model.

Configurable ticket categories may include general admission, reserved seating, premium seating with explicitly listed benefits, accessible seating, and non-public companion/staff/player credentials.

A ticket lifecycle should explicitly distinguish states such as Available, Reserved, Issued, CheckedIn, Cancelled, Refunded, and Expired. The implementation may choose different names, but transitions must be explicit and tested.

Rules:
- Never issue two active reserved tickets for the same seat and event.
- A reservation may expire; an issued ticket remains subject to the event's defined validity policy.
- Admission is granted by validating ticket state and event/venue eligibility, not by animation or proximity to an entrance.
- Check-in must be idempotent: replaying the same request must not create multiple admissions.
- Capacity comes from the venue and ticket inventory model. Never invent sell-outs, attendance, or occupancy.
- Purchase and entry are separate events. A valid ticket can remain unused.
- Transfers, resale, dynamic pricing, and scalping controls are future policy decisions; do not silently assume them.

## 2. Concessions: food and beverages

Concessions give visitors reasons to circulate through concourses before kickoff, at halftime, and after the match. Vendors can operate fixed kiosks, temporary stalls, or mobile carts, each tied to a real venue location or anchor.

Each offer should identify product, category, description, price/currency, stock or replenishment policy, vendor, serving location, availability window, and service status. Dietary/allergen information should be shown only when supplied by the product data. Alcohol is not assumed; any age-restricted or regulated product requires an explicit policy and eligibility checks.

An order should have a defined lifecycle, for example Created → Paid/Authorized → Preparing → Ready → Fulfilled, with explicit cancellation and refund paths. A simplified simulation may use fewer stages, but payment alone must not mark an order fulfilled.

Inventory rules:
- Stock changes only at the defined reservation or fulfilment point.
- Concurrent orders must not sell more stock than is available.
- Failed or cancelled orders release reserved stock as appropriate.
- A vendor may be closed or out of stock without losing its identity or sales history.
- A sale does not automatically imply that a Digiten consumed the item.

## 3. Merchandise and memorabilia

Merchandise connects the event to team identity. Memorabilia adds event-specific value without making the economy depend on arbitrary scarcity.

Product records should support stable product and variant identities, seller, team/event association where relevant, price/currency, inventory policy, and availability. Variants may include size or color. Memorabilia may have a serial or edition record if the system supports it; do not call an item limited or authentic without explicit product attributes and provenance rules.

Keep these concepts separate:
- **Catalog:** what may be offered.
- **Inventory:** what is available to sell.
- **Order:** what a buyer requested.
- **Payment/settlement:** the financial result.
- **Fulfilment:** whether the item was handed over, delivered, or otherwise completed.

Refunds and cancellations must have defined effects on inventory and ownership. A purchase must not grant an item if stock reservation or payment failed.

## 4. Vendors, locations, and settlement

A vendor is a persistent business identity with an explicit operating location, catalog, pricing, stock policy, and settlement terms. The same vendor may operate across several events, but sales must remain attributable to the correct venue and event.

An agreement may define a fixed stall fee, a percentage of eligible sales, or both; how discounts, refunds, taxes, and transaction fees are handled; settlement period and currency; inventory ownership; and who fulfils each order. Terms are configuration, not hard-coded UI assumptions.

A settlement summary should distinguish:
- Gross item sales.
- Discounts.
- Refunds and reversals.
- Applicable taxes and transaction fees, when modeled.
- Venue commission or stall fees.
- Vendor payable.
- Venue net revenue.

Every calculated amount must be traceable to transactions and the applicable agreement version. Never count a refund as a second sale, or infer revenue from foot traffic, inventory movement, or a purchase animation.

## 5. Transaction and accounting invariants

Economic state should be reproducible from recorded business events or an auditable transaction ledger.

- Every ticket, order, payment result, refund, and settlement has a stable identifier.
- Currency is explicit. Unlike currencies are not added without a defined conversion source and timestamp.
- Use fixed-precision decimal or integer minor units for money; avoid binary floating-point for financial totals.
- Snapshot price and terms at purchase time so later catalog edits do not rewrite history.
- Retrying the same idempotency key must not double-charge, double-issue, double-sell, or double-refund.
- Payment authorization, capture, refund, and settlement are distinct concepts where supported.
- Simulation mode uses clearly labeled simulated currency and transactions. It must not imply a real payment provider is connected.
- Failed payment does not create a completed sale. Successful payment alone does not prove fulfilment.
- Adjustments reference the original transaction and preserve the audit trail.

## 6. Visitor journey

1. A visitor discovers an event and chooses an available ticket.
2. The ticket is issued against event inventory and, where applicable, a canonical seat.
3. The visitor travels to the arena, enters through an admission point, and is checked in.
4. The visitor navigates the concourse and may browse food, beverage, merchandise, or memorabilia vendors.
5. An order is recorded and fulfilled through its own lifecycle.
6. The visitor returns to a seat or viewing area and participates in the match as a spectator.
7. After the match, the visitor may shop, leave, or continue into other city activities.

This is the target journey, not a claim that autonomous Digiten navigation, ticket scanning, queues, or checkout are integrated. Each implemented step must consume real state from the relevant domain.

## 7. Ownership and integration boundaries

| Concern | Owner / source of truth |
|---|---|
| Event identity, schedule, venue capacity, ticket inventory | Event and ticketing domain |
| Seat labels, sections, concourses, kiosks, entrances | Canonical venue/spatial model |
| Product catalog, stock, orders, fulfilment | Commerce/vendor domain |
| Payment result and refunds | Payment adapter or explicitly simulated service |
| Vendor terms and settlement ledger | Commerce/accounting domain |
| Visitor intent and movement | Digiten/agent decision and movement systems |
| Gestures and poses | Gestures plus host-specific adaptation |
| Scene presentation | Renderer / WebPage host |
| Kickoff, match events, score, final whistle | Authoritative match simulation |

The economy can enable visitor activities, but it does not own the match result. A goal may influence demand or trigger a configured promotion later, but a goal animation must never create a sale, alter a ticket, or fabricate attendance. A UI click must not invent vendor stock or a Digiten purchase.

Venue anchors must be stable identifiers resolved by the actual spatial model. A seat label such as N-12 is not itself a world-space coordinate. Vendor stalls should refer to venue anchors, not unrelated hard-coded decorative locations.

## 8. Conceptual data vocabulary

These are design concepts, not published API types. Implement only what the next verifiable milestone needs.

- **Event:** stable ID, schedule, venue ID, status, ticketing policy.
- **Venue:** stable ID, capacity source, structure ID, layout/version.
- **Seat / VenueAnchor:** stable anchor ID, section, seat label, capacity and eligibility metadata.
- **TicketOffer / Ticket:** offer and price, event, category, holder reference, seat/area, lifecycle status, price snapshot.
- **Vendor:** stable ID, operator, venue presence, catalog, agreement, operating status.
- **Product / Variant / InventoryItem:** catalog identity, options, location, available quantity, reservation policy.
- **Order / OrderLine:** buyer reference, event/venue, quantities, price snapshots, totals, lifecycle state.
- **PaymentRecord / RefundRecord:** simulated or provider mode, status, amount, currency, idempotency key, original transaction reference.
- **Settlement:** vendor, period, agreement version, itemized deductions and payable amount.
- **LedgerEntry:** auditable accounting movement with transaction references and currency.

## 9. Singularity Dollars and real-money boundaries

If Singularity City uses **Singularity Dollars** as an in-world currency, keep that balance distinct from USD or any real-world currency. A simulated wallet can support game-economy purchases, ticket issuance, and vendor sales without representing a bank account or payment provider.

Any exchange between in-world currency and real money is a separate product and compliance decision. Do not imply cash value, redemption, transferability, or real-money support without an approved design and the necessary legal, security, and payment work.

For the first demonstrator, prefer clearly labeled simulated transactions with deterministic prices and inventory. This proves domain flows without prematurely connecting real payment infrastructure.

## 10. Implementation sequence

1. **Catalog and venue anchors:** identify the arena, stands, seats, concourses, and vendor locations in the canonical model.
2. **Ticket inventory and admission:** implement a small deterministic ticket lifecycle, capacity checks, and idempotent check-in.
3. **One concession vendor:** catalog a few items, track stock, record orders, and demonstrate cancellation/fulfilment.
4. **Merchandise and memorabilia:** reuse the same order/inventory principles rather than building separate fake storefronts.
5. **Transaction ledger:** snapshot prices and reconcile totals, refunds, and vendor terms.
6. **Visitor integration:** connect actual Digiten location/activity state to entry, shopping, seating, and departure.
7. **Presentation:** show availability, price, order state, and event summaries from real domain snapshots.
8. **Validation:** test duplicate requests, overselling, cancellation, refund, stock exhaustion, invalid tickets, and settlement reconciliation.
9. **Optional real payment adapter:** only after explicit authorization and a separate security/compliance review.

## Acceptance checks

- Ticket inventory never exceeds configured capacity or seat availability.
- Concurrent requests cannot claim the same reserved seat.
- Replayed check-in and purchase requests do not duplicate effects.
- Orders cannot be fulfilled while required payment/order conditions are unsatisfied.
- Stock, cancellations, refunds, and settlement totals reconcile under documented rules.
- Historical transactions preserve the prices and terms that applied when created.
- Vendor sales are attributable to vendor, location, event, and transaction.
- The UI distinguishes design data, deterministic simulation, integrated services, and real transactions.
- Visitor activity reads from actual venue and Digiten state, not invented crowd counts or decorative motion.
- Match score and match events remain owned by the authoritative match simulation.

## First milestone

Build a small, inspectable **simulated** arena economy: one event, a few ticket categories, stable seat anchors, one concession vendor, one team shop, a handful of products, and a transaction summary. Make ticketing, stock, orders, and accounting deterministic and testable before expanding the storefront or connecting real services.

The goal is not to imitate a payment screen. It is a coherent event economy whose records explain who attended, what was offered, what was bought, what was fulfilled, and how proceeds were allocated—without manufacturing state the underlying systems do not provide.
