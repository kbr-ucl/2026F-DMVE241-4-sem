# Uge 5 – Notification Service + End-to-End Integration

## Formål
Tilføj Notification-servicen og test det komplette system end-to-end.
Notification er en ren event-drevet service der lytter på ordreevents via pub/sub.

## Services
| Service | Dapr App ID | Dapr HTTP Port | Database |
|---------|-------------|----------------|----------|
| EventCatalog.Api | `eventcatalog` | 5001 | `eventcatalogdb` (PostgreSQL) |
| Inventory.Api | `inventory` | 5002 | `inventorydb` (PostgreSQL) |
| Ordering.Api | `ordering` | 5003 | `orderingdb` (PostgreSQL) |
| Payment.Api | `payment` | 5004 | – (stateless) |
| Notification.Api | `notification` | 5005 | – (stateless) |

## Arkitektur
```
EventCatalog ──[EventCreated]──▶ Pub/Sub ──▶ Inventory
      ▲                                         ▲
      │                                         │
      └──────────── Ordering ───────────────────┘
                       │
                  SAGA Workflow
                       │
                    Payment
                       │
          ┌────────────┴────────────┐
          ▼                         ▼
  [OrderConfirmed]          [OrderCancelled]
          │                         │
          ▼                         ▼
      Notification              Notification
   (log bekræftelse)         (log annullering)
```

## Notification Service (ren event-drevet)
- **OrderEventsSubscriber** – Lytter på `order-confirmed` og `order-cancelled` via Dapr pub/sub (`tickethub-pubsub`)
- Ingen forretningslogik – kun logging (simuleret notifikation)
- Minimal service: kun `Program.cs` + `Subscribers/OrderEventsSubscriber.cs`
- Events:
  - `OrderConfirmedEvent(OrderId, EventId, CustomerEmail)`
  - `OrderCancelledEvent(OrderId, EventId, CustomerEmail, Reason)`
- Endpoints:
  - `POST /subscribe/order-confirmed` – Topic subscriber
  - `POST /subscribe/order-cancelled` – Topic subscriber

## End-to-end flow
1. **Opret event** (EventCatalog) – `POST /api/events`
2. **Publicér event** → Inventory opretter lager (pub/sub) – `PUT /api/events/{id}/publish`
3. **Placér bestilling** (Ordering) → Prisopslag (service invocation) – `POST /api/orders`
4. **SAGA**: ReserveTickets → ProcessPayment → ConfirmOrder (Dapr Workflow)
5. **Notification** modtager `OrderConfirmed` eller `OrderCancelled` (pub/sub)

## Tests
- **EventCatalog.Domain.Tests** – `EventTests.cs`, `TicketCategoryTests.cs`, `MoneyTests.cs`
- **Inventory.Domain.Tests** – `TicketStockTests.cs`
- **Ordering.Domain.Tests** – `OrderTests.cs`, `OrderLineTests.cs`, `MoneyTests.cs`

## Bruno API-tests
- EventCatalog: `US-01 Opret Event`, `US-03 Publicer Event`, `US-04 Browse Events`, `US-05 Se Event Detaljer`
- Inventory: `US-07 Verificer Lagerposter`, `US-08 Reserver Billetter`, `US-09 Frigiv Billetter`, `US-10 Se Lagerstatus`, `US-20 Verificer Lager efter SAGA`
- Ordering: `US-12 Placer Bestilling`, `US-13 SAGA Happy Path`, `US-13 Verificer SAGA Resultat`, `US-17 Se Ordrestatus`, `US-17 Ordre ikke fundet`, `US-19 End-to-end Komplet Flow`, `US-19 Verificer End-to-end`
- Payment: `US-18 Behandl Betaling`

## Dapr Components
- `pubsub.yaml` – Pub/Sub (Redis)
- `statestore.yaml` – State Store (Redis)

## Hosting
- **.NET Aspire** med PostgreSQL (persistent container) + pgAdmin
- Dapr sidecars via `CommunityToolkit.Aspire.Hosting.Dapr`
- **Dapr Dashboard** på port 8080
- Ordering refererer til eventcatalog, inventory og payment i AppHost

## Kør projektet
```bash
# Fra src/TicketHub.AppHost
dotnet run
# Åbn Aspire Dashboard for at se alle 5 services, traces og pub/sub
```

## Kør tests
```bash
# Fra solution-roden
dotnet test
```

## Opgaver
1. Kør alle 5 services og test komplet flow
2. Test alle fejlscenarier (billetter udsolgt, betaling fejler)
3. Observer Notification-logs i Aspire Dashboard ved confirmed/cancelled
4. Observer traces på tværs af services i Aspire Dashboard
5. Dokumentér arkitektur og reflektér over design-valg
