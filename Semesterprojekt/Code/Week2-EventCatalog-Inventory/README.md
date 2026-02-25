# Uge 2 – EventCatalog & Inventory med Pub/Sub

## Formål
Implementér EventCatalog fuldt ud og tilføj Inventory-servicen.
Introduktion af Dapr Pub/Sub til asynkron kommunikation mellem services.

## Services
| Service | Dapr App ID | Dapr HTTP Port | Database |
|---------|-------------|----------------|----------|
| EventCatalog.Api | `eventcatalog` | 5001 | `eventcatalogdb` (PostgreSQL) |
| Inventory.Api | `inventory` | 5002 | `inventorydb` (PostgreSQL) |

## Arkitektur
```
EventCatalog ──[EventCreated]──▶ Pub/Sub ──▶ Inventory
                                (tickethub-pubsub)
```

## Clean Architecture i praksis
- **Commands** (Use Cases) returnerer `Task` (void) – CQS-princip
- **Queries** implementeres i Infrastructure med `AsNoTracking()` – CQS
- **Repository**: `SaveAsync()` kalder IKKE `Update()` (EF Change Tracking)
- **Controller** refererer KUN til Facade (interfaces + DTOs)

## EventCatalog – komplet implementering
### Infrastructure
- `EventCatalogDbContext` – EF Core med PostgreSQL (via Aspire `AddNpgsqlDbContext`)
- `EventRepository` – Repository-pattern med Change Tracking
- `EventQueriesImpl` – Direkte queries med `AsNoTracking()`
- `DaprEventPublisher` – Publicerer `EventCreated` via Dapr pub/sub (`tickethub-pubsub`)

### UseCases
- `CreateEventUseCase` – Opretter event med billetkategorier
- `PublishEventUseCase` – Publicerer event og udsender `EventCreated`
- `IEventPublisher` – Port-interface for event-publicering (impl. i Infrastructure)

### Api
- `EventsController` – REST-endpoints for CRUD + publicering
- CloudEvents + SubscribeHandler registreret i `Program.cs`

## Inventory – komplet implementering
### Domain
- **TicketStock** – Entity: `EventId`, `CategoryId`, `CategoryName`, `Available`, `Reserved`, `TotalCapacity`, `RowVersion`
  - Metoder: `Reserve()`, `Release()`, `ConfirmSale()`, `IsLow()`

### UseCases
- `ReserveTicketsUseCase` – Reserverer billetter
- `ReleaseTicketsUseCase` – Frigiver reserverede billetter

### Facade
- `IReserveTicketsUseCase`, `IReleaseTicketsUseCase`, `IStockQueries`
- DTOs: `ReserveTicketsRequest`, `ReleaseTicketsRequest`, `TicketStockDto`, `ReservationLine`

### Infrastructure
- `InventoryDbContext` – EF Core med PostgreSQL
- `TicketStockRepository`, `StockQueriesImpl`

### Api
- `StockController` – REST-endpoints for lagerstatus
- `EventCreatedSubscriber` – Dapr pub/sub subscriber (opretter `TicketStock` fra `EventCreated`)

## Pub/Sub: EventCatalog → Inventory
Inventory subscriber opretter `TicketStock`-entities automatisk når et nyt event publiceres.
Topic: `event-created` på `tickethub-pubsub`.

## Tests
- **EventCatalog.Domain.Tests** – `EventTests.cs`, `TicketCategoryTests.cs`, `MoneyTests.cs`
- **Inventory.Domain.Tests** – `TicketStockTests.cs`

## Bruno API-tests
- EventCatalog: `US-01 Opret Event`, `US-01 Opret Event - Validation fejl`, `US-03 Publicer Event`, `US-04 Browse Events`, `US-05 Se Event Detaljer`
- Inventory: `US-07 Verificer Lagerposter (efter publish)`, `US-10 Se Lagerstatus`

## Dapr Components
- `pubsub.yaml` – Pub/Sub (Redis)
- `statestore.yaml` – State Store (Redis)

## Hosting
- **.NET Aspire** med PostgreSQL (persistent container) + pgAdmin
- Dapr sidecars via `CommunityToolkit.Aspire.Hosting.Dapr`
- **Dapr Dashboard** på port 8080

## Kør projektet
```bash
# Fra src/TicketHub.AppHost
dotnet run
# Åbn Aspire Dashboard for at se services og traces
```

## Kør tests
```bash
# Fra solution-roden
dotnet test
```

## Opgaver
1. Kør begge services med Dapr sidecars
2. Opret event → Publicér → Observer Inventory opretter lager (pub/sub)
3. Test forretningsregler (opret event i fortiden, publicér uden kategorier)
4. Brug Bruno til at teste alle API-endpoints
