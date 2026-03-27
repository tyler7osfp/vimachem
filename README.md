# Library Management System (Microservices + Eventing)

Design and implementation of a Library Management System for a librarian to manage parties (authors/customers), books/categories, and borrow/return flows. The solution is split into two services: a transactional **Library API** and a read-only **Event API** that stores an audit/event history.

## What’s included

- **Parties & roles**: CRUD parties; assign roles (Author, Customer); a party can have both roles.
- **Books & categories**: CRUD books and categories; track availability; borrow/return with “one copy → one borrower at a time”.
- **Borrowing visibility**: list book titles with current borrowers.
- **Event publishing + history**: all actions published as events; consumed and stored for querying/auditing.
- **Retention**: events older than 1 year are deleted automatically (background job in Event API).

## Architecture overview

- **Library API** (`src/Library.Api`): write model (PostgreSQL via EF Core). Domain changes raise domain events; events are published via MassTransit using a **transactional outbox**.
- **RabbitMQ**: transports integration events.
- **Event API** (`src/Event.Api`): consumes events from RabbitMQ and persists them to **MongoDB** for audit/query endpoints. Runs an isolated background cleanup job to enforce retention.

High-level flow:

1. Request hits Library API → domain change saved to PostgreSQL.
2. Outbox feature stores the event within the same transaction.
3. Outbox relay forwards the event to RabbitMQ (at-least-once delivery).
4. Event API consumes the event and writes an `EventDocument` to MongoDB.

## How to run (primary): Docker Compose

### Prerequisites

- Docker Desktop / Docker Engine with Compose support

### Start everything

From root folder:

```
docker compose up --build
```

### Service endpoints / ports

- **Library API**: `http://localhost:5113`
- **Event API**: `http://localhost:5278`
- **RabbitMQ Management UI**: `http://localhost:15672` (user: `guest`, pass: `guest`)
- **PostgreSQL**: `localhost:5432` (db: `librarydb`, user: `postgres`, pass: `postgres`)
- **MongoDB**: `localhost:27017` (db: `eventdb`)

### Configuration

Docker Compose injects config via environment variables (see `docker-compose.yml`). Defaults in the services also match those values:

- `src/Library.Api/appsettings.json`
- `src/Event.Api/appsettings.json`

## How to run (secondary): .NET Aspire AppHost

The solution includes an Aspire orchestrator project:

- `src/Library.AppHost` (wires Library API, Event API, PostgreSQL, MongoDB, RabbitMQ for local development)

Run:

```
dotnet run --project src/Library.AppHost/Library.AppHost.csproj
```

## Key decisions & trade-offs

- **Two services / bounded contexts**: transactional domain (Library API) is isolated from audit/query concerns (Event API), allowing independent scaling and simpler persistence models.
- **PostgreSQL for transactional data**: strong relational model + EF Core + solid support for MassTransit outbox.
- **MongoDB for event history**: append-only, flexible metadata per event type, indexed for entity and time-based queries.
- **Transactional outbox**: prevents “phantom events” and lost events when DB commit and broker publish can’t be atomic; yields **at-least-once** delivery which implies consumers should be **idempotent**.
- **Caching via decorator**: read paths can be accelerated without leaking caching concerns into the application layer; cache invalidation happens after commit.

## Solution structure

- `LibraryManagement.sln`: solution entrypoint
- `src/Library.Api`: Library HTTP API
- `src/Event.Api`: Event HTTP API + consumers + retention job
- `src/Library.Domain`: domain model and domain events
- `src/Library.Application`: application services (commands/queries) and orchestration
- `src/Library.Infrastructure`: EF Core persistence, repositories, unit-of-work, caching implementations
- `src/Shared`: shared contracts/messaging contracts
- `src/Library.Tests`, `src/Events.Tests`: test projects

