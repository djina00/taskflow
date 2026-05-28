# TaskFlow

A desktop **project & task management** application built with **WPF / .NET 8**, designed to demonstrate a clean, maintainable, and testable architecture.

## Architectural Goals

TaskFlow is intentionally **architecture-focused**. It demonstrates:

- **Modular Monolith** — the system is split into independent functional modules that live in a single deployable application.
- **Clean Architecture** — each module is layered into `Domain`, `Application`, and `Infrastructure`, with dependencies pointing inward.
- **Hexagonal Architecture (Ports & Adapters)** — the Application layer depends only on interfaces (ports); concrete implementations (adapters) are injected.
- **Dependency Injection** — all dependencies are registered and resolved through `Microsoft.Extensions.DependencyInjection`.
- **Event-driven communication** — modules communicate through domain/integration events instead of direct references, keeping them loosely coupled.
- **CQRS** — write operations (Commands) are separated from read operations (Queries).
- **Repository Pattern** — data access is hidden behind repository interfaces, with multiple interchangeable implementations (SQLite & JSON).

## Modules

| Module | Responsibility |
| --- | --- |
| **Users** | Registration, login, roles, user management |
| **Projects** | Create/manage projects and project members |
| **Tasks** | Task CRUD, status, priorities, comments, assignment |
| **Notifications** | Reacts to domain events (e.g. task assigned/completed) |
| **Reports** | Statistics, completed-task analytics, productivity score |
| **SharedKernel** | `BaseEntity`, `Result<T>`, domain-event & CQRS abstractions |
| **Infrastructure** | SQLite/JSON repositories, event bus, logging, DI composition |

## Solution Layout

```
TaskFlow.sln
├── src/
│   ├── TaskFlow.SharedKernel/                 (shared abstractions)
│   ├── TaskFlow.Infrastructure/               (adapters: SQLite, JSON, event bus)
│   ├── TaskFlow.Desktop/                       (WPF presentation, no business logic)
│   └── Modules/
│       ├── Users/        (.Domain / .Application / .Infrastructure)
│       ├── Projects/     (.Domain / .Application / .Infrastructure)
│       ├── Tasks/        (.Domain / .Application / .Infrastructure)
│       ├── Notifications/(.Domain / .Application / .Infrastructure)
│       └── Reports/      (.Domain / .Application / .Infrastructure)
└── tests/
    └── TaskFlow.Tests/                          (xUnit, fake repositories)
```

## Tech Stack

.NET 8 · WPF · XAML · Microsoft.Extensions.DependencyInjection · SQLite · xUnit · GitHub Actions

## Build & Run

```bash
dotnet build
dotnet test
dotnet run --project src/TaskFlow.Desktop
```

> Documentation (C4 diagrams, dependency diagram, architectural decisions) lives in the `docs/` folder.
