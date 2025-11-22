# The Singularity Workshop: Publisher WebPage (Blazor WASM)
## Project Name: TheSingularityWorkshop

This repository hosts the source code for **The Singularity Workshop's official publisher website**, built using **Blazor WebAssembly**.

The primary goals of this project are:
1. **Publisher Compliance:** Serve as the functional, publicly accessible website required for publishing assets (like the FSM Unity Integration) to external marketplaces.
2. **Brand Presence:** Establish the official online presence and mission for "Forging Software for the Singularity."
3. **Technology Showcase:** Provide live demonstrations and documentation for The Singularity Workshop's core FSM technologies.

---

### 🚀 Quick Start (Local Development)

To run the project locally:

1.  **Clone the Repository:**
    ```bash
    git clone [Your Repository URL]
    cd [Repository Folder]/TheSingularityWorkshop
    ```
2.  **Restore Dependencies:**
    ```bash
    dotnet restore
    ```
3.  **Run the Application:**
    ```bash
    dotnet run
    ```
    The application will typically launch in your browser at `https://localhost:7001` (or a similar port defined in `launchSettings.json`).

---

### 🏛️ Project Architecture

* **Technology:** Blazor WebAssembly (.NET)
* **Core Logic:** The site uses the **FSMManagerService** to demonstrate decoupled, state-driven logic, reflecting the architecture of the FSM API itself.
* **Deployment:** Designed for static hosting environments (e.g., Azure Static Web Apps, GitHub Pages).

### 🔗 Live Site & Compliance
*(Update this section upon successful deployment to Azure)*

* **Live URL:** `[Your Azure Website URL Here]`
* **Compliance Links:** The site includes dedicated, public pages for **Privacy Policy**, **Terms of Service**, and **Support/Contact**, as required for asset publishing.

---
*Built by The Singularity Workshop. Check the FSM API repository for the source code of the core logic layer.*