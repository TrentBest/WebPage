# The Singularity Workshop - FSM API
## Forging Software for the Singularity

[![Nuget Version](https://img.shields.io/nuget/v/TheSingularityWorkshop.FSM_API.svg)](https://www.nuget.org/packages/TheSingularityWorkshop.FSM_API)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.txt)

---

### 🚀 Mission Statement
The Singularity Workshop is engineering the foundational logic layer for the next generation of digital experiences. Our core belief is that **explicit state management** is the key to creating scalable, reliable, and testable software. The FSM API defines the **Finite State Machine** as the **Atomic Unit of Work**, providing an engine-agnostic framework for unified logic.

### ✨ Core Features
* **Engine-Agnostic Core:** Built on pure C#, the FSM API runs across diverse .NET platforms (WPF, Console, Blazor, and Unity).
* **Simplified Construction:** Use the fluent **`FSM_API.Create.FiniteStateMachine(...)`** builder to design complex state logic quickly.
* **Decoupled Architecture:** Logic is separated from data via the `IStateContext`, ensuring clean, context-driven execution.
* **Focus on Logic:** We provide the framework; you focus on the behavior. Your work is yours—we claim **zero ownership** or revenue from applications you create.

### 📦 Quick Start & Integration
The FSM API is available as a NuGet package.

#### 1. NuGet Installation
```bash
dotnet add package TheSingularityWorkshop.FSM_API
# Or use the Package Manager Console in Visual Studio
# Install-Package TheSingularityWorkshop.FSM_API