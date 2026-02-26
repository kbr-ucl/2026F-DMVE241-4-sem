# Uge 3 – Ordering Service + Service Invocation

## Formål
Tilføj Ordering-servicen med synkront prisopslag via Dapr Service Invocation.
Introduktion af Ports-pattern for løs kobling mellem services.

## Services
| Service | Dapr App ID | Dapr HTTP Port | Database |
|---------|-------------|----------------|----------|
| EventCatalog.Api | `eventcatalog` | 5001 | `eventcatalogdb` (PostgreSQL) |
| Inventory.Api | `inventory` | 5002 | `inventorydb` (PostgreSQL) |
| Ordering.Api | `ordering` | 5003 | `orderingdb` (PostgreSQL) |

## Arkitektur
```
EventCatalog ──[EventCreated]──▶ Pub/Sub ──▶ Inventory
      ▲
      │ Service Invocation (pris)
      └──────────── Ordering
```

## Dapr Service Invocation (velbegrundet REST)
Ordering → EventCatalog: Prisopslag er synkront fordi ordren kræver den aktuelle pris NU for at kunne oprettes.
Billetreservation håndteres IKKE i uge 3 – det tilføjes i uge 4 med SAGA.

## Ports-pattern (løs kobling)
- `IEventCatalogService` – Port i UseCases-laget (interface)
- `DaprEventCatalogService` – Infrastructure-implementering via Dapr Service Invocation
- Ordering kender IKKE til Dapr – kun til sine port-interfaces

## Ordering – komplet implementering
### Domain
- **Order** (Aggregate Root) – `EventId`, `CustomerEmail`, `Status`, `Lines`, `TotalAmount` (computed), `CreatedAt`, `ConfirmedAt`, `CancelledAt`, `CancellationReason`
  - Metoder: `Confirm()`, `Cancel(reason)`
- **OrderLine** – `CategoryId`, `CategoryName`, `Quantity`, `UnitPrice` (Money)
- **OrderStatus** (enum) – `Created`, `Confirmed`, `Cancelled`
- **CustomerEmail** (Value Object) – Validering af email-format
- **Money** (Value Object) – Genbrugt mønster fra EventCatalog

### Facade
- `IPlaceOrderUseCase` – Command: Placerer bestilling
- `IOrderQueries` – Query: `GetByIdAsync`
- DTOs: `PlaceOrderRequest`, `OrderLineRequest`, `OrderDto`, `OrderLineDto`, `GetOrderRequest`

### UseCases
- `PlaceOrderUseCase` – Opretter ordre med prisopslag via `IEventCatalogService`
  - Henter pris per kategori synkront → opretter `OrderLine` med korrekt pris
  - `TODO Uge 4: Start SAGA workflow her`
- `IEventCatalogService` – Port for prisopslag
- `IOrderRepository` – Repository-port

### Infrastructure
- `OrderingDbContext` – EF Core med PostgreSQL
- `OrderRepository`, `OrderQueriesImpl`
- `CustomerEmailConverter`, `MoneyConverter` – EF Core Value Converters
- `DaprEventCatalogService` – Service Invocation til EventCatalog

### Api
- `OrdersController` – `POST /api/orders` (PlaceOrder), `GET /api/orders/{id}` (GetById)

## Tests
- **EventCatalog.Domain.Tests** – `EventTests.cs`, `TicketCategoryTests.cs`, `MoneyTests.cs`
- **Inventory.Domain.Tests** – `TicketStockTests.cs`
- **Ordering.Domain.Tests** – `OrderTests.cs`, `OrderLineTests.cs`, `MoneyTests.cs`

## Bruno API-tests
- EventCatalog: `US-01 Opret Event`, `US-03 Publicer Event`, `US-04 Browse Events`, `US-05 Se Event Detaljer`
- Inventory: `US-07 Verificer Lagerposter`, `US-08 Reserver Billetter`, `US-10 Se Lagerstatus`
- Ordering: `US-12 Placer Bestilling`, `US-17 Se Ordrestatus`, `US-17 Ordre ikke fundet`

## Dapr Components
- `pubsub.yaml` – Pub/Sub (Redis)
- `statestore.yaml` – State Store (Redis)

## Hosting
- **.NET Aspire** med PostgreSQL (persistent container) + pgAdmin
- Dapr sidecars via `CommunityToolkit.Aspire.Hosting.Dapr`
- **Dapr Dashboard** på port 8080
- Ordering refererer til eventcatalog og inventory i AppHost

## Kør projektet
```bash
# Fra src/TicketHub.AppHost
dotnet run
# Åbn Aspire Dashboard for at se services, traces og service invocation
```

## Kør tests
```bash
# Fra solution-roden
dotnet test
```

## Opgaver
1. Kør alle 3 services, test hele flowet
2. Placér bestilling og observer service invocation i Aspire Dashboard
3. Test fejlscenarier (ugyldigt event, udsolgte billetter)
4. Forbered uge 4: Overvej hvordan PlaceOrderUseCase bliver SAGA
