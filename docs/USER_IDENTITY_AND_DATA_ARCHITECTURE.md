# User Identity and Data Architecture

This document establishes the direction for user identity, user-owned data, company identity, privacy, and consent in The Singularity Workshop.

## Governing principle

> **The Workshop may learn about a user because the user chose to let an Experience learn it. It must never become entitled to the user's life merely because the user entered the Workshop.**

Identity is a first-class domain boundary. It is not a substrate and it is not a bag of application-specific fields.

## Dynamic User

The working concept is a **Dynamic User Profile**:

```text
DynamicUser
  |
  +-- stable identity
  +-- core profile
  +-- preferences / capabilities
  +-- consent state
  +-- access policy
  +-- UserData
       |
       +-- user-defined keys
       +-- user-defined values
       +-- ownership
       +-- visibility
       +-- retention
       +-- provenance
```

The exact public type name remains an architectural decision. The important property is that the core identity remains stable while the user-owned data surface remains extensible.

## UserData is not permissionless metadata

An Experience or MicroBundle may request permission to record a metric, preference, observation, or other datum against a user's profile.

The default policy is:

- no silent collection of sensitive or unnecessary data;
- explicit purpose for collection;
- explicit scope;
- explicit retention;
- explicit audience;
- user-visible provenance;
- user-controlled withdrawal where technically possible;
- no sale or monetization merely because data exists.

An Experience may be able to observe runtime facts without automatically acquiring permission to persist them.

## Monetization

A future Workshop data marketplace may allow a user to voluntarily license specific data for a defined purpose.

The architectural model is:

```text
User owns data
      |
      v
User grants a defined license
      |
      v
Buyer receives only the licensed scope
      |
      v
Revenue attribution
      |
      +--> user share
      +--> Workshop service share
```

The previously proposed 90/10 split is a business-policy candidate, not a runtime entitlement. It must never be encoded as an assumption in the identity substrate.

A user who declines monetization remains protected by the same ownership and access model.

## Experience metrics

MicroBundles should eventually be able to declare metrics they can produce, for example:

- interaction counts;
- completion state;
- preferences explicitly supplied by the user;
- performance measurements;
- authored observations;
- Experience-specific progress.

The metric declaration is separate from permission to persist the metric.

```text
MicroBundle
   |
   +--> declares metric capability
   |
   v
Experience requests persistence
   |
   v
User consent / policy evaluation
   |
   v
User Profile data store
```

This keeps the MicroBundle capable of describing what it can measure without granting itself authority over a person's profile.

## Company and Workshop Patron identity

A company is a first-class identity with employees/members.

A person may therefore have multiple contextual relationships:

```text
Person
  |
  +--> Workshop Patron profile
  |       public-facing identity
  |
  +--> Company membership
          |
          +--> work-facing profile
          +--> role
          +--> organization permissions
          +--> organization-owned records
```

The person remains the person. Employment is a relationship, not a replacement identity.

Public-facing and work-facing data must be separately scoped.

## Authentication should follow demonstrated value

The public introduction should not demand credentials before the visitor understands why credentials matter.

The current direction is:

```text
ARRIVE
  -> WITNESS
  -> EXPLORE
  -> DEEP DIVE
  -> CREATE / SAVE / PUBLISH
  -> account becomes useful
  -> authentication becomes necessary for owned state
```

Credentials should be required when an operation needs durable ownership, private data, publishing authority, company membership, or another protected capability.

A visitor should be able to witness the Workshop without surrendering an identity simply to see the machine work.

## Privacy boundary

The identity service must eventually expose explicit access decisions rather than raw database access.

A conceptual policy is:

```text
requester
  +
resource
  +
purpose
  +
requested operation
  +
user/company policy
  =
allow / deny / redact
```

Experiences should receive the smallest useful projection of identity and profile data rather than a universal unrestricted profile object.

## Private configuration and targeted offers

The Workshop may eventually maintain private business-development configuration identifying specific invited organizations or patrons.

That configuration must **not** live in a public repository.

Preferred direction:

```text
private configuration repository / secret store
            |
            v
controlled deployment artifact
            |
            v
WebPage business-development feature
```

Public source may contain the schema and behavior. It must not contain private target lists, credentials, contact information, or confidential negotiation state.

A future targeted tab can be addressed by an opaque audience key rather than embedding private information in the public application.

## Safety and governance

Identity, privacy, monetization, and company relationships require legal and policy review before production launch. The architecture should make responsible policy possible; code should not silently become the policy.

This document intentionally does not define jurisdiction-specific legal compliance.

## Current status

This is an architectural contract, not a production data service.

The next implementation stages should be:

1. stable identity abstraction;
2. user-owned data envelope;
3. explicit access/consent policy;
4. company and membership relationships;
5. authentication at the first operation that genuinely requires ownership;
6. audit/provenance;
7. optional voluntary data licensing.

*The user is not the product. The user's agency is part of the architecture.*
