# Evaluation: DDD & Clean Architecture Compliance

**TicketHub Microservices Semester Project**

Measured against the principles in the Clean-Architecture.md document (Kaj Bromose, Februar 2026)

---

## 1. Overall Architecture Alignment

The codebase follows the four-layer model from the document closely:

| Document Layer | Project Layer | Verdict |
|---|---|---|
| **Domain** | `*.Domain` | Correct — pure, no external deps |
| **Use Case** | `*.UseCases` | Correct — orchestration + repo interfaces |
| **Facade** | `*.Facade` | Correct — only interfaces + DTOs |
| **Infrastructure** | `*.Infrastructure` | Correct — EF Core, Dapr, repos, query handlers |

The **Dependency Rule** is respected. `.csproj` references confirm:
- Domain references nothing
- UseCases references Domain only
- Facade references nothing (pure contracts)
- Infrastructure references Domain, UseCases, and Facade
- API references Facade + Infrastructure (for DI wiring)

**Score: Excellent** — matches the document's architecture exactly.

---

## 2. Domain Layer (DDD Patterns)

### Strengths

- **Private setters everywhere** — all entities (`Event`, `Order`, `TicketStock`, `OrderLine`, `TicketCategory`) use `{ get; private set; }` exactly as the document prescribes.

- **Validation in constructors** — entities throw `DomainException` on invalid input (e.g., empty name, past date, zero capacity), ensuring objects are never in an invalid state.

- **State changes through methods only** — `Event.Publish()`, `Event.Cancel()`, `Order.Confirm()`, `Order.Cancel()`, `TicketStock.Reserve()` all enforce business rules before mutating state. This matches the document's `Konsultation` pattern.

- **Value Objects** — `Money` and `CustomerEmail` are implemented as C# `record` types with private constructors and validation. The `FromDecimalUnsafe()`/`FromUnsafe()` pattern for EF Core deserialization is a pragmatic solution.

- **Aggregate Root pattern** — `Event` manages its `TicketCategories` collection internally via `AddTicketCategory()`, never exposing a mutable list. The `IReadOnlyList` accessor is correct.

- **Private parameterless constructors** for EF Core — present on all entities.

- **No framework dependencies** — Domain projects have zero NuGet packages.

### Areas for Improvement

1. **Missing Domain Services** — The document describes `KonsultationOverlapService` as an example of logic spanning multiple entities. The codebase has no Domain Services (e.g., no overlap check, no cross-entity validation). The `TicketStock.IsLow()` method is good, but more complex business rules involving multiple aggregates would benefit from explicit Domain Services.

2. **`DomainException` is minimal** — The document uses a simple `DomainException(string)` and the codebase matches. However, as complexity grows, consider adding error codes or structured error types for better API error handling.

3. **`Order.TotalAmount` computed property** — This calls `Money.FromDecimal()` on every access, which runs validation. Since the sum of valid `OrderLine` amounts is always positive, this works but is slightly inefficient. A minor point.

4. **Missing Aggregate boundary enforcement** — `TicketCategory` has a public constructor callable from outside the `Event` aggregate. Ideally, only the aggregate root should create child entities. The constructor is `public TicketCategory(Event @event, ...)` — while it requires an Event reference, it could still be called from outside. Making it `internal` would be stricter.

---

## 3. Use Case Layer (CQS)

### Strengths

- **Clear CQS separation** — Commands (`ICreateEventUseCase`, `IPlaceOrderUseCase`) return `Task` (void). Queries (`IEventQueries`, `IOrderQueries`) return DTOs. This matches the document's principle exactly.

- **One Use Case = One responsibility** — Each use case class has a single `Execute()` method, matching the document's pattern of `Udfør()`.

- **Repository interfaces in Use Case layer** — `IEventRepository`, `IOrderRepository`, `ITicketStockRepository` are defined in UseCases projects, matching the document's placement.

- **Delegation to Domain** — Use cases call domain methods (e.g., `evt.Publish()`, `stock.Reserve()`) rather than implementing business logic themselves.

- **`SaveAsync()` without `Update()`** — Repositories use EF Core change tracking correctly, exactly as the document emphasizes. No anti-pattern `_db.Update()` calls.

### Areas for Improvement

1. **Some Use Cases call `InvalidOperationException` instead of a proper `NotFoundException`** — The document uses `NotFoundException("Konsultation ikke fundet.")`. The codebase uses `throw new InvalidOperationException("Event not found.")` in several places. A dedicated `NotFoundException` (or reusing `DomainException`) would be more consistent with the document's pattern and enable proper HTTP 404 mapping.

2. **No explicit Use Case-specific validation** — The document notes that Use Cases should validate Use Case-specific rules (not domain rules). The codebase relies entirely on domain validation. This is fine for now but worth noting as complexity grows.

3. **Cross-service calls in Use Cases** — `PlaceOrderUseCase` calls `IEventCatalogService` to get pricing. This interface lives in UseCases but represents an external service. The document places such interfaces in Use Case or Facade. The placement is acceptable but could be documented more explicitly.

---

## 4. Facade Layer

### Strengths

- **Pure contracts** — Facade projects contain only interfaces and DTO records. Zero implementation. This matches the document precisely.

- **DTO-only signatures** — All interface methods use primitive types and DTOs. Domain types (`Money`, `CustomerEmail`, `EventStatus`) never leak through the Facade. This matches the document's emphasis on keeping Facade independent of Domain internals.

- **C# records for DTOs** — Immutable by default, minimal code, value equality. Matches the document's recommendation.

- **Request DTOs for both Commands and Queries** — `CreateEventRequest`, `GetEventRequest`, `SearchEventsRequest` all follow the document's pattern.

### Areas for Improvement

1. **No clear naming convention distinction** — The document uses `IOpdaterBehandlingstypeUseCase` for commands and `IKonsultationQueries` for queries. The codebase follows the same pattern (`ICreateEventUseCase` vs `IEventQueries`), which is good. However, some interfaces like `IEventPublisher` and `IEventCatalogService` sit in UseCases rather than Facade. Consider whether these external contracts should be in Facade for consistency.

2. **Command Use Cases don't return created IDs** — `ICreateEventUseCase.Execute()` returns `Task` (void), and the controller returns `Created()` without a location header or ID. In practice, clients often need the created resource's ID. The document acknowledges this CQS trade-off but doesn't address it. Consider returning `Task<Guid>` for creation commands, or having the client query afterward.

---

## 5. Infrastructure Layer

### Strengths

- **AsNoTracking() for queries** — `EventQueriesImpl` and `OrderQueriesImpl` use `.AsNoTracking()` for read operations, matching the document's emphasis on this for CQS query performance.

- **DTO projection in queries** — `.Select(e => new EventDto(...))` avoids loading full entities for reads. Matches the document's pattern.

- **Value Object converters** — `MoneyConverter` and `CustomerEmailConverter` bridge the gap between domain value objects and database columns cleanly.

- **Query handlers implement Facade interfaces directly** — `EventQueriesImpl : IEventQueries` is in Infrastructure, not UseCases. This matches the document's CQS architecture where queries bypass the domain layer.

- **Optimistic concurrency** — `TicketStock.RowVersion` for concurrency control is a good infrastructure concern.

### Areas for Improvement

1. **No `IUnitOfWork` abstraction** — The document uses `GemAsync()` on repositories (which calls `SaveChangesAsync`). The codebase follows this pattern, but when a Use Case touches multiple repositories (e.g., `ReserveTicketsUseCase` iterates over stocks), each repository shares the same `DbContext` implicitly. This works but is implicit. An explicit `IUnitOfWork` could make the transaction boundary clearer.

2. **Database migration strategy** — `Database.EnsureCreated()` is used in Program.cs. This is fine for development but won't handle schema evolution. The document doesn't address this, but production code would need EF Core migrations.

---

## 6. Testability

### Strengths

- **Domain tests with zero mocks** — `EventTests`, `MoneyTests`, `TicketStockTests`, `OrderTests` test pure domain logic without any mocking framework. This matches the document's recommendation exactly.

- **Comprehensive invariant testing** — Tests verify constructor validation, state transitions, boundary conditions, and business rules. The `Reserve_Release_MaintainsConsistency` test that checks `Available + Reserved == TotalCapacity` is particularly good — it tests a domain invariant.

- **xUnit with [Fact] pattern** — Clean, readable test structure.

### Areas for Improvement

1. **No Use Case tests with Moq** — The document explicitly shows Use Case tests using `Mock<IKonsultationRepository>()`. The codebase has domain tests only — no Use Case layer tests. This is a significant gap. Use Case tests would verify orchestration logic (e.g., that `PublishEventUseCase` calls `Publish()` on the event and then `SaveAsync()` and then `PublishEventCreatedAsync()`).

2. **No query handler tests** — The document shows query-handler tests. The codebase has none.

3. **No integration tests** — No `*.Integration.Tests` projects despite the document's suggested project structure including them.

---

## 7. Microservices-Specific Patterns (Beyond the Document)

The codebase extends the document's architecture into microservices territory with:

- **Dapr Pub/Sub** for integration events between bounded contexts
- **Dapr Service Invocation** for synchronous cross-service queries
- **SAGA Orchestration** (Week4) with compensating transactions
- **.NET Aspire** for service orchestration and discovery

These are well-implemented and consistent with the layered architecture. Infrastructure concerns (Dapr, messaging) stay in the Infrastructure layer. Domain remains pure.

---

## Summary Scorecard

| Criterion | Score | Notes |
|---|---|---|
| Dependency Rule | **10/10** | Perfect. All `.csproj` references follow inward-only rule |
| Entity DDD patterns | **9/10** | Private setters, validation, methods. Minor: public child constructors |
| Value Objects | **9/10** | Immutable records with validation. `FromUnsafe` pattern is pragmatic |
| Domain Services | **5/10** | Missing — no complex cross-entity logic encapsulated |
| CQS separation | **10/10** | Commands return void, Queries return DTOs, clearly separated |
| Facade layer | **10/10** | Pure interfaces + DTOs, no implementation leaked |
| Repository pattern | **9/10** | Correct EF tracking, no `Update()` anti-pattern. Minor: no explicit UoW |
| Query handlers | **10/10** | AsNoTracking, DTO projection, in Infrastructure |
| API controllers | **10/10** | Only reference Facade, minimal logic, proper REST |
| Domain tests | **9/10** | Thorough invariant testing, zero mocks |
| Use Case tests | **2/10** | Missing entirely — document explicitly shows these with Moq |
| Query handler tests | **2/10** | Missing entirely |

**Overall: Strong adherence to the document's architecture.** The biggest gap is the absence of Use Case and query handler unit tests with mocking, which the document explicitly recommends. The Domain Services layer is also underutilized. Otherwise, this is a clean, well-structured implementation of the four-layer Clean Architecture with DDD and CQS.
