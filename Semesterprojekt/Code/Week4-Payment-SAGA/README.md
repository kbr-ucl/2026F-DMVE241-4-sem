# Uge 4 – Payment Service + SAGA (Dapr Workflow)

## Formål
Tilføj Payment-servicen og implementér SAGA-mønsteret med Dapr Workflow
for at sikre distribueret transaktionskonsistens med kompenserende transaktioner.

## Services
| Service | Dapr App ID | Dapr HTTP Port | Database |
|---------|-------------|----------------|----------|
| EventCatalog.Api | `eventcatalog` | 5001 | `eventcatalogdb` (PostgreSQL) |
| Inventory.Api | `inventory` | 5002 | `inventorydb` (PostgreSQL) |
| Ordering.Api | `ordering` | 5003 | `orderingdb` (PostgreSQL) |
| Payment.Api | `payment` | 5004 | – (stateless) |

## Arkitektur
```
EventCatalog ──[EventCreated]──▶ Pub/Sub ──▶ Inventory
      ▲                                         ▲
      │                                         │
      └──────────── Ordering ───────────────────┘
                       │
                       ▼
              ┌─── SAGA Workflow ───┐
              │ 1. ReserveTickets   │
              │ 2. ProcessPayment   │
              │ 3. ConfirmOrder     │
              │                     │
              │ Kompensation:       │
              │ · ReleaseTickets    │
              │ · CancelOrder       │
              └─────────────────────┘
                       │
                       ▼
                    Payment
```

## SAGA: PlaceOrderWorkflow (Dapr Workflow)
### Happy Path
1. **ReserveTicketsActivity** → Reservér billetter i Inventory via `IInventoryService`
2. **ProcessPaymentActivity** → Behandl betaling via `IPaymentService`
3. **ConfirmOrderActivity** → Bekræft ordren (sæt status til `Confirmed`)
4. **PublishOrderConfirmedActivity** → Publicér `OrderConfirmed` event via `IOrderEventPublisher`

### Kompenserende transaktioner (ved fejl)
- **ReleaseTicketsActivity** → Frigiver reserverede billetter i Inventory
- **CancelOrderActivity** → Sætter ordrestatus til `Cancelled` med årsag

### Workflow DTOs
- `PlaceOrderInput` – `OrderId`, `EventId`, `CustomerEmail`, `TotalAmount`, `Lines`
- `LineInfo` – `CategoryId`, `Quantity`
- `PlaceOrderResult` – `Success`, `Message`
- `PaymentActivityResult` – `Success`, `ErrorMessage`
- `CancelInput` – `OrderId`, `Reason`

## Payment Service
- Simpel stateless service med konfigurérbar fejlrate
- `ProcessPaymentUseCase` – Simuleret betaling med `Task.Delay` og tilfældig fejl
- Facade: `IProcessPaymentUseCase` + `ProcessPaymentRequest`/`PaymentResponse`
- DTOs: `ProcessPaymentRequest(OrderId, Amount, CustomerEmail)`, `PaymentResponse(Success, TransactionId, ErrorMessage)`

## Ordering – udvidet med SAGA
### Nye Ports (UseCases-laget)
- `IInventoryService` – Reserve/Release billetter
- `IPaymentService` – Betalingsbehandling (returnerer `PaymentResult`)
- `IOrderEventPublisher` – Publicér `OrderConfirmed`/`OrderCancelled`
- `IWorkflowStarter` – Starter Dapr Workflow

### Infrastructure – Workflows
- `PlaceOrderWorkflow.cs` – SAGA-orkestrator med kompensation
- `Activities.cs` – Alle 6 workflow-aktiviteter
- `DaprWorkflowStarter.cs` – Starter workflow via Dapr

### Infrastructure – External Services
- `DaprEventCatalogService` – Prisopslag (Service Invocation)
- `DaprInventoryService` – Reserve/Release billetter (Service Invocation)
- `DaprPaymentService` – Betalingsbehandling (Service Invocation)

### Infrastructure – Messaging
- `DaprOrderEventPublisher` – Publicerer `OrderConfirmed`/`OrderCancelled` via Dapr pub/sub

### PlaceOrderUseCase (opdateret)
- Opretter ordre med prisopslag → starter SAGA workflow via `IWorkflowStarter`

## Tests
- **EventCatalog.Domain.Tests** – `EventTests.cs`, `TicketCategoryTests.cs`, `MoneyTests.cs`
- **Inventory.Domain.Tests** – `TicketStockTests.cs`
- **Ordering.Domain.Tests** – `OrderTests.cs`, `OrderLineTests.cs`, `MoneyTests.cs`

## Bruno API-tests
- EventCatalog: `US-01 Opret Event`, `US-03 Publicer Event`, `US-04 Browse Events`, `US-05 Se Event Detaljer`
- Inventory: `US-07 Verificer Lagerposter`, `US-08 Reserver Billetter`, `US-09 Frigiv Billetter`, `US-10 Se Lagerstatus`
- Ordering: `US-12 Placer Bestilling`, `US-13 SAGA Happy Path`, `US-13 Verificer SAGA Resultat`, `US-17 Se Ordrestatus`, `US-17 Ordre ikke fundet`
- Payment: `US-18 Behandl Betaling`

## Dapr Components
- `pubsub.yaml` – Pub/Sub (Redis)
- `statestore.yaml` – State Store (Redis) – bruges af Dapr Workflow

## Hosting
- **.NET Aspire** med PostgreSQL (persistent container) + pgAdmin
- Dapr sidecars via `CommunityToolkit.Aspire.Hosting.Dapr`
- **Dapr Dashboard** på port 8080
- Ordering refererer til eventcatalog, inventory og payment i AppHost

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
1. Test happy path (betaling lykkes)
2. Test fejlscenarie (sæt `FailureRate` til `0.5+` i Payment)
3. Observer kompensation: billetter frigives ved fejl
4. Observer workflow-status i Aspire Dashboard
5. Brug Bruno til at teste SAGA-flowet end-to-end
