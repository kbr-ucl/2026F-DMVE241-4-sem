# Uge 1 – Domæneanalyse og Projektopsætning

## Formål
Analysér domænet og opsæt projektstrukturen for EventCatalog-servicen (skeleton).
Fokus er på DDD-modellering, Clean Architecture og forretningsregler i domænelaget.

## Services
| Service | Dapr App ID | Dapr HTTP Port |
|---------|-------------|----------------|
| EventCatalog.Api | `eventcatalog` | 5001 |

## Clean Architecture lagdeling (5 lag)
| Lag | Ansvar | Afhængigheder |
|-----|--------|---------------|
| **Domain** | Entities, Value Objects, DomainException | Ingen (inderste lag) |
| **UseCases** | Repository-interfaces (ports) | → Domain |
| **Facade** | Interfaces + DTOs (ren kontrakt) | Ingen |
| **Infrastructure** | EF Core, repos, queries, Dapr *(skeleton – udfyldes uge 2)* | → Alle |
| **Api** | Controllers, Program.cs | → Facade |

## Domænemodel
### Entities
- **Event** (Aggregate Root) – `Name`, `Description`, `Date`, `Venue`, `Status`, `TicketCategories`
  - Metoder: `AddTicketCategory()`, `Publish()`, `Cancel()`, `MarkAlmostSoldOut()`
  - Private setters, forretningsregler i metoder (DDD Entity-mønster)
- **TicketCategory** – `Name`, `Price` (Money), `TotalCapacity`
  - Metode: `UpdatePrice()`
- **EventStatus** (enum) – `Draft`, `Published`, `AlmostSoldOut`, `SoldOut`, `Cancelled`

### Value Objects
- **Money** – Immutable record med validering (`Amount > 0`), operatorer (`+`, `-`, `*`)
  - `FromDecimal()` (med validering), `FromDecimalUnsafe()` (til EF Core persistence)

### Exceptions
- **DomainException** – Kastes ved brud på forretningsregler

## Facade (kontrakt)
- `ICreateEventUseCase` – Command: Opretter event (returnerer `Task` – CQS)
- `IPublishEventUseCase` – Command: Publicerer event
- `IEventQueries` – Query: `GetByIdAsync`, `SearchAsync`
- DTOs: `CreateEventRequest`, `PublishEventRequest`, `SearchEventsRequest`, `EventDto`, `TicketCategoryDto` m.fl.

## UseCases
- `IEventRepository` – Repository-interface (port) i UseCases-laget
  - `GetByIdAsync()`, `AddAsync()`, `SaveAsync()`

## Infrastruktur
Skeleton-projekt – implementeres i uge 2 (DbContext, Repository, Queries, Dapr).

## Tests
- **EventCatalog.Domain.Tests** – xUnit-tests for `Event`, `TicketCategory`, `Money`
  - `EventTests.cs`, `TicketCategoryTests.cs`, `MoneyTests.cs`

## Dapr Components
- `statestore.yaml` – State Store (Redis)

## Hosting
- **.NET Aspire** (`TicketHub.AppHost`) med Dapr sidecar via `CommunityToolkit.Aspire.Hosting.Dapr`
- **Dapr Dashboard** på port 8080
- **Scalar** API-reference (OpenAPI)

## Kør projektet
```bash
# Fra src/TicketHub.AppHost
dotnet run
```

## Kør tests
```bash
# Fra solution-roden
dotnet test
```

## Opgaver
1. Gennemarbejd casebeskrivelsen og identificér Bounded Contexts
2. Dokumentér ubiquitous language per context, tegn Context Map
3. Studér Event-entity: private setters, forretningsregler i metoder
4. Forstå Dependency Rule: afhængigheder peger altid indad
5. Kør domain-tests: `dotnet test` fra solution-roden
6. Infrastructure og Api er skeletprojekter – udfyldes i uge 2
