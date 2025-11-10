# WTNS
*An environment building SDK with tools and libraries for creating media and user-driven platforms, featuring extensive documentation and support for decentralized services like Web3 and IPFS.*

## 📖 Overview
**WTNS**<sup>[1](#gls-sdk)</sup> is an environment building SDK<sup>[1](#gls-sdk)</sup> providing tools, libraries, and documentation for creating **media and user-driven platforms** in .NET/C#.
It consists of a **core SDK** and two primary **implementations**:

- **WTNS CLI:** Console-based environment for developers and servers.
- **WTNS GUI:** Graphical, cross-platform interface built with Avalonia (or similar).

Together, they allow developers to build:

- Full-featured **social or decentralized platforms** (e.g., communities, forums, networks).
- **Standalone WTNS-based apps** leveraging the SDK's modular systems.
- **Extensions and plugins** that integrate seamlessly with the core runtime.

WTNS<sup>[1](#gls-sdk)</sup> emphasizes **clarity**, **dependency safety**, and **fun, modular development** — all in pure C#, with minimal external dependencies.

---

# 🧭 ROADMAP

## 🔑 Developer Key / Legend

| Symbol / Format | Meaning |
|------------------|---------|
| **Feature Branch:** `feature/namespace-subsystem-name` | **Feature**<sup>[3](#gls-feature)</sup> branch name. Represents a logical feature<sup>[3](#gls-feature)</sup> set or module. |
| **Task Checkbox:** ☐ | **Commit**<sup>[4](#gls-commit)</sup>/task within a feature<sup>[3](#gls-feature)</sup> branch. Checked when complete. |
| **Dependencies:** *Depends on: X, Y* | Shows namespace<sup>[5](#gls-namespace)</sup> dependencies. Avoid circular dependencies. |
| **Hierarchy Arrow:** ↑ | In dependency graphs, means "depends on" or "extends". |
| **Glossary Link:** WTNS<sup>[1](#gls-sdk)</sup> | Superscript links jump to the [Glossary](#glossary). |

**Rules:**
- Each feature<sup>[3](#gls-feature)</sup> branch contains one cohesive module or system.
- Each checkbox is a single or small set of commits<sup>[4](#gls-commit)</sup>.
- Avoid cross-namespace<sup>[5](#gls-namespace)</sup> imports that violate the dependency hierarchy.
- Keep `Wtns.Shared` dependency-free.
- All namespaces<sup>[5](#gls-namespace)</sup> are versioned and can be independently tested.

---

## 🧩 Table of Contents
1. [Foundation Layer — Shared & Domain](#️-foundation-layer--shared--domain)
2. [Core Runtime Infrastructure](#️-core-runtime-infrastructure)
3. [Data Layer](#-data-layer)
4. [Identity & Access](#-identity--access)
5. [Social Layer](#-social-layer)
6. [Media Layer](#️-media-layer)
7. [Web3 & Decentralization](#️-web3--decentralization)
8. [Core Services](#-core-services)
9. [Developer Experience](#-developer-experience)
10. [GUI Layer](#-gui-layer)
11. [Security & Integrity](#-security--integrity)
12. [Extensibility & APIs](#-extensibility--apis)
13. [Monitoring & Observability](#-monitoring--observability)
14. [Networking](#-networking)
15. [Gamification Systems](#-gamification-systems)
16. [Dependency Hierarchy](#-dependency-hierarchy)
17. [Glossary](#-glossary)

---

## 🏛️ Foundation Layer — Shared & Domain
**Namespace Root:** `Wtns.Shared`, `Wtns.Core.Domain`

---

### 🧩 `Wtns.Shared` — Global Helpers & Primitives
*Dependency-free. The universal utility layer.*

#### Branch: `feature/shared-core-primitives`
- [ ] Result and Option types (`Result<T>`, `Maybe<T>`)
- [ ] Error and exception model
- [ ] Common enums and constants
- [ ] Localization and translations
- [ ] Extension and helper methods
- [ ] Validation and serialization utilities

#### Branch: `feature/shared-crosscutting-utils`
- [ ] Reflection and type discovery
- [ ] Crypto helpers
- [ ] Randomization and hashing tools
- [ ] Time and date utilities
- [ ] Service marker interfaces

---

### 🧬 `Wtns.Core.Domain` — Entities & Aggregates
*Depends on:* `Wtns.Shared`

#### Branch: `feature/domain-entities-core`
- [ ] Core entities (User, Profile, Message, Thread, MediaItem)
- [ ] Roles, permissions, and policies
- [ ] Forum, Page, Notification aggregates<sup>[7](#gls-aggregate)</sup>
- [ ] Value objects (IDs, URIs, Money, Tags)
- [ ] Domain events (UserRegistered, MediaUploaded)

#### Branch: `feature/domain-logic-specifications`
- [ ] Domain services<sup>[8](#gls-service)</sup> and specifications
- [ ] Event sourcing base
- [ ] Repository contracts
- [ ] Aggregate<sup>[7](#gls-aggregate)</sup> root model

---

## ⚙️ Core Runtime Infrastructure
**Namespace Root:** `Wtns.Core`
*Depends on:* `Wtns.Core.Domain`, `Wtns.Shared`

#### Branch: `feature/core-runtime-di-container`
- [ ] Custom DI<sup>[9](#gls-di)</sup> framework
- [ ] Service<sup>[8](#gls-service)</sup> lifecycle manager
- [ ] Module loader

#### Branch: `feature/core-runtime-eventbus`
- [ ] Event bus<sup>[10](#gls-eventbus)</sup> implementation
- [ ] Async event dispatching
- [ ] Scoped event handling

#### Branch: `feature/core-runtime-config`
- [ ] Unified configuration system
- [ ] JSON/ENV/YAML/Encrypted backends
- [ ] Hot reload

#### Branch: `feature/core-runtime-diagnostics`
- [ ] Exception handling
- [ ] System profiler (WMI/*NIX/Mac)
- [ ] Runtime performance metrics

---

## 💾 Data Layer
**Namespace Root:** `Wtns.Data`
*Depends on:* `Wtns.Core.Domain`, `Wtns.Shared`

#### Branch: `feature/data-orm-providers`
- [ ] DB abstraction for MySQL, SQLite, MongoDB
- [ ] Connection manager
- [ ] Migrations

#### Branch: `feature/data-audit-logging`
- [ ] Data audit layer
- [ ] Snapshot rollback system

---

## 👤 Identity & Access
**Namespace Root:** `Wtns.Identity`
*Depends on:* `Wtns.Core.Domain`, `Wtns.Security`

#### Branch: `feature/identity-authentication`
- [ ] Token-based auth (JWT, OAuth2)
- [ ] MFA (TOTP, email)
- [ ] Session management

#### Branch: `feature/identity-did`
- [ ] Decentralized identity
- [ ] DID<sup>[11](#gls-did)</sup> resolver
- [ ] Credential issuance

---

## 💬 Social Layer
**Namespace Root:** `Wtns.Social`
*Depends on:* `Wtns.Core.Domain`, `Wtns.Media`

#### Branch: `feature/social-messaging`
- [ ] Group and direct messages
- [ ] E2E encryption
- [ ] Attachments and reactions

#### Branch: `feature/social-forums`
- [ ] Forum creation
- [ ] Moderation and polls
- [ ] Threaded discussions

#### Branch: `feature/social-gamification`
- [ ] Point system with configurable rules
- [ ] Achievement system with badges and milestones
- [ ] Virtual currency system with transactions
- [ ] Leaderboards and ranking systems
- [ ] Reward distribution and redemption

---

## 🖼️ Media Layer
**Namespace Root:** `Wtns.Media`
*Depends on:* `Wtns.Shared`

#### Branch: `feature/media-processing`
- [ ] Image/video encoding
- [ ] Audio normalization
- [ ] Metadata extraction

#### Branch: `feature/media-ipfs`
- [ ] IPFS<sup>[12](#gls-ipfs)</sup> integration
- [ ] Pinning service<sup>[8](#gls-service)</sup>
- [ ] Media deduplication

---

## 🕸️ Web3 & Decentralization
**Namespace Root:** `Wtns.Web3`
*Depends on:* `Wtns.Shared`, `Wtns.Media`

#### Branch: `feature/web3-ipfs-integration`
- [ ] IPFS<sup>[12](#gls-ipfs)</sup>/IPNS gateways
- [ ] WalletConnect bridge

#### Branch: `feature/web3-compute-mesh`
- [ ] Distributed compute node sharing
- [ ] Resource pooling & consensus model

---

## 🧠 Core Services
**Namespace Root:** `Wtns.Services`
*Depends on:* `Wtns.Core`, `Wtns.Shared`

#### Branch: `feature/services-telemetry-bus`
- [ ] Distributed telemetry system
- [ ] Event tracing and aggregation

#### Branch: `feature/services-cache`
- [ ] Redis-like caching
- [ ] Manifest-based invalidation

#### Branch: `feature/services-manifest`
- [ ] Manifest<sup>[6](#gls-manifest)</sup>/config/properties system
- [ ] Plugin descriptor parsing

---

## 🧰 Developer Experience
**Namespace Root:** `Wtns.DevTools`
*Depends on:* `Wtns.Core`, `Wtns.Shared`

#### Branch: `feature/devtools-cli`
- [ ] CLI tools
- [ ] Code generation
- [ ] Diagnostics console

#### Branch: `feature/devtools-compiler`
- [ ] On-demand compilation (Roslyn)
- [ ] Arbitrary code execution sandbox (WASM)

---

## 💻 GUI Layer
**Namespace Root:** `Wtns.Gui`
*Depends on:* `Wtns.Core`, `Wtns.Services`

#### Branch: `feature/gui-renderers`
- [ ] Markdown renderer
- [ ] HTML/CSS renderer
- [ ] Avalonia UI integration

---

## 🔐 Security & Integrity
**Namespace Root:** `Wtns.Security`
*Depends on:* `Wtns.Shared`

#### Branch: `feature/security-encryption`
- [ ] Crypto and hashing utilities
- [ ] Token signing
- [ ] Secrets vault

---

## 🔌 Extensibility & APIs
**Namespace Root:** `Wtns.Extensibility`
*Depends on:* `Wtns.Core`, `Wtns.Shared`

#### Branch: `feature/extensibility-plugins`
- [ ] Plugin architecture
- [ ] Manifest<sup>[6](#gls-manifest)</sup> resolver
- [ ] Sandbox validation

#### Branch: `feature/extensibility-apis`
- [ ] HTTP(S)/SSH REST APIs
- [ ] Binary protocol (custom WTNS transport)

---

## 📊 Monitoring & Observability
**Namespace Root:** `Wtns.Monitoring`
*Depends on:* `Wtns.Services`

#### Branch: `feature/monitoring-core`
- [ ] System profiler
- [ ] Telemetry dashboard

---

## 🌐 Networking
**Namespace Root:** `Wtns.Net`
*Depends on:* `Wtns.Shared`

#### Branch: `feature/net-binary-protocol`
- [ ] Binary communication protocol
- [ ] Compression and framing
- [ ] Local P2P server hosting

---

## 🎮 Gamification Systems
**Namespace Root:** `Wtns.Gamification`
*Depends on:* `Wtns.Core.Domain`, `Wtns.Social`

#### Branch: `feature/gamification-points`
- [ ] Point accumulation engine
- [ ] Configurable point rules and triggers
- [ ] Point history and analytics
- [ ] Point expiration and decay systems

#### Branch: `feature/gamification-achievements`
- [ ] Achievement definition system
- [ ] Badge and milestone tracking
- [ ] Progress indicators and notifications
- [ ] Achievement sharing and display

#### Branch: `feature/gamification-currency`
- [ ] Virtual currency management
- [ ] Transaction processing and ledger
- [ ] Currency exchange and conversion
- [ ] Wallet and balance tracking

---

## 🧩 Dependency Hierarchy
```
Wtns.Shared
   ↑
Wtns.Core.Domain
   ↑
Wtns.Core
   ↑
Wtns.Data, Wtns.Identity, Wtns.Media, Wtns.Security, Wtns.Social, Wtns.Services
   ↑
Wtns.DevTools, Wtns.Gui, Wtns.Extensibility, Wtns.Monitoring, Wtns.Net, Wtns.Gamification
```

---


## 📘 Glossary
<a name="glossary"></a>

<a name="gls-sdk"></a>
**[1] WTNS/SDK:**
An environment building SDK providing tools and libraries for creating media and user-driven platforms with decentralized service support.

<a name="gls-feature"></a>
**[3] Feature:**
A cohesive development branch that delivers a self-contained module or system.

<a name="gls-commit"></a>
**[4] Commit (Checkbox):**
Represents a development task within a feature branch.

<a name="gls-namespace"></a>
**[5] Namespace:**
A logical grouping of related C# components.

<a name="gls-manifest"></a>
**[6] Manifest:**
Metadata file describing configuration, module dependencies, or capabilities.

<a name="gls-aggregate"></a>
**[7] Aggregate:**
A domain-driven design pattern representing a cluster of related entities treated as a single unit.

<a name="gls-service"></a>
**[8] Service:**
A singleton or scoped component that provides specific functionality within the WTNS runtime.

<a name="gls-di"></a>
**[9] DI Container:**
Dependency Injection container that manages object creation and lifetime.

<a name="gls-eventbus"></a>
**[10] Event Bus:**
A messaging system that allows decoupled communication between different parts of the application.

<a name="gls-did"></a>
**[11] DID:**
Decentralized Identifier - a new type of identifier for verifiable, decentralized digital identity.

<a name="gls-ipfs"></a>
**[12] IPFS:**
InterPlanetary File System - a distributed file storage protocol.
