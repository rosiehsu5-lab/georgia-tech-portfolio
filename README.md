# Georgia Tech Technical Portfolio

**Hua-Rong (Rosie) Hsu**  
M.S. Computer Science student, Georgia Institute of Technology  
Trader and quantitative-research builder working across AI, finance, robotics, computer architecture, and health informatics.

## Featured project: Pengu Delivery

Pengu Delivery is a 3D Unity game developed by a Georgia Tech student team. The player guides a penguin through an Arctic delivery route, collecting fish, reaching checkpoints, navigating breakable ice, and avoiding patrolling seals.

[Read the technical case study](projects/pengu-delivery.md)

**[Browse the Unity game source](https://github.com/rosiehsu5-lab/georgia-tech-portfolio/tree/pengu-delivery-game)** — includes the game scripts, scenes, prefabs, packages, and Unity project settings.

My contributions included:

- Designing and implementing interactive storyline and tutorial flows.
- Adding a double-jump tutorial step and conditional progression logic.
- Building reusable Unity Editor setup tools for storyline and tutorial scenes.
- Implementing a demo-station helper and writing its build specification.
- Improving player onboarding, scene transitions, checkpoints, bonus-fish interactions, menus, and game-over flow.
- Coordinating changes through feature branches and Git-based team development.

**Stack:** Unity, C#, Unity Editor scripting, scene management, prefabs, event-driven gameplay, Git.

> The complete game includes work by multiple teammates and licensed Unity assets. This portfolio documents my work without republishing teammates' code or third-party asset packages. The original repository is hosted on Georgia Tech GitHub Enterprise and may require institutional access.

## Artificial Intelligence

### Search and probabilistic inference

Implemented and evaluated core AI techniques in Python, including graph search, heuristic reasoning, probability-based inference, and model validation. The work emphasized correctness, runtime behavior, testing, and reasoning under uncertainty.

### Kalman-filter robotics: Hopscotch

Developed a state-estimation approach for a simulated robot navigating through moving targets. The system used noisy observations to estimate motion state and predict future positions, connecting Kalman filtering with action selection and safety constraints.

### Robotics foundations

Implemented or studied localization, Kalman filters, particle filters, search, PID control, kinematic bicycle models, and simultaneous localization and mapping. These projects developed practical intuition for state estimation, uncertainty, feedback control, and planning.

> Georgia Tech graded solution code is intentionally not published. Project descriptions focus on engineering decisions and learning outcomes in accordance with academic-integrity requirements.

## Computer Architecture

### Processor simulation and benchmarking

Configured and ran SESC-based simulations, compiled MIPS-target workloads, and interpreted simulator reports to understand performance metrics and experimental methodology.

### Branch prediction

Studied and implemented branch-prediction approaches, then evaluated them using ray-tracing workloads. The project compared predictor behavior through simulation outputs and connected prediction accuracy with pipeline performance.

### Cache coherence and shared-memory systems

Used multiprocessor simulation results to analyze coherence behavior and performance at different processor counts. The work compared protocol configurations and examined how memory-system behavior changes as parallelism increases.

## Health Informatics

### Electronic health records and interoperability

Analyzed electronic-health-record workflows and represented clinical information using structured healthcare data models. Produced technical documentation in Georgia Tech's JDF format.

### FHIR resources

Worked with Patient and Observation resources, JSON serialization, validation, and US Core concepts. The project emphasized interoperability: representing clinical information consistently so different healthcare systems can exchange it.

### OMOP-on-FHIR

Worked with an OMOP-backed FHIR service using Python, SQLAlchemy, Docker, and clinical vocabulary tables. Explored how a standards-based API translates FHIR requests into a common clinical data model.

## Engineering practices demonstrated

- Python and C# implementation
- Unity game development and Editor automation
- Data structures, algorithms, and state estimation
- Computer-architecture simulation and performance analysis
- Docker-based reproducible environments
- JSON, FHIR, OMOP, and healthcare interoperability
- Git branching and collaborative development
- Technical reports, experiment design, and quantitative evaluation

## Repository policy

This repository is a curated portfolio, not a submission archive. It excludes graded solutions, answer keys, private institutional data, credentials, proprietary datasets, licensed asset packs, and code authored by teammates without permission.
