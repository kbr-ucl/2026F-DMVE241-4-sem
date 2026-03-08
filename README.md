# DMVE241 – Udleveret kode

Dette repository indeholder al udleveret kode til semesterprojektet **TicketHub** samt tilhørende demokode og quickstart-eksempler. Nedenfor beskrives alle mapper og projekter, så I hurtigt kan orientere jer i kodebasen.

---

## Mappestruktur (overblik)

```
2026F-DMVE241-4-sem/
├── Semesterprojekt/Code/          Hovedprojektet – TicketHub (uge 1-5)
│   ├── Week1-DomainAnalysis/
│   ├── Week2-EventCatalog-Inventory/
│   ├── Week3-Ordering/
│   ├── Week4-Payment-SAGA/
│   ├── Week5-Notification-Integration/
│   ├── Documents/                 Opgavebeskrivelser, vejledninger og diagrammer
│   └── TicketHub/                 Bruno OpenCollection-konfiguration
├── DemoCode/                      Demoprojekter til undervisning
│   ├── Aspire101/
│   ├── DetLilleBibliotek/
│   ├── PubsubDeclarativeHelloWorld/
│   ├── PubsubProgramaticHelloWorld/
│   ├── PubsubProgramaticAspireHelloWorld/
│   └── TestAfPrivateSet/
├── DaprQuickstarts/               Dapr workflow-eksempler
│   ├── Workflows/
│   └── WorkflowsAspire/
└── Directory.Packages.props       Central NuGet-pakkestyring for hele repo
```

---

## Semesterprojekt – TicketHub

TicketHub er et mikroservice-baseret billetsystem bygget med **DDD**, **Clean Architecture**, **Dapr** og **.NET Aspire**. Koden er organiseret i 5 uger, hvor hver uge tilføjer nye services og koncepter. Hver uge er et selvstændigt solution der kan køres uafhængigt.

### Ugentlig progression

| Uge | Mappe | Fokus | Nye services |
|-----|-------|-------|--------------|
| 1 | `Week1-DomainAnalysis/` | Domæneanalyse, Clean Architecture opsætning | EventCatalog (skeleton) |
| 2 | `Week2-EventCatalog-Inventory/` | Fuld EventCatalog + Inventory, Dapr pub/sub | EventCatalog, Inventory |
| 3 | `Week3-Ordering/` | Ordering service, Dapr service invocation | + Ordering |
| 4 | `Week4-Payment-SAGA/` | Payment service, SAGA-mønster (Dapr Workflow) | + Payment |
| 5 | `Week5-Notification-Integration/` | Notification service, end-to-end integration | + Notification |

Hver uge-mappe har sin egen `README.md` med detaljeret beskrivelse.

---

### Week1-DomainAnalysis

Domæneanalyse og opsætning af Clean Architecture-strukturen. EventCatalog-servicen oprettes som skeleton med domænelag og tests.

**Projekter i `src/`:**

| Projekt | Beskrivelse |
|---------|-------------|
| `EventCatalog.Api` | REST API med controllers og Scalar (OpenAPI) |
| `EventCatalog.Domain` | Entities (`Event`, `TicketCategory`), Value Objects (`Money`), `EventStatus` enum |
| `EventCatalog.Facade` | Interfaces (`ICreateEventUseCase`, `IEventQueries`) og DTOs |
| `EventCatalog.UseCases` | Use case-implementeringer, `IEventRepository` port |
| `EventCatalog.Infrastructure` | Skeleton – udfyldes i uge 2 |
| `TicketHub.AppHost` | .NET Aspire-orkestrering med Dapr sidecar |
| `TicketHub.ServiceDefaults` | Fælles servicekonfiguration (OpenTelemetry, health checks) |

**Projekter i `test/`:**

| Projekt | Beskrivelse |
|---------|-------------|
| `EventCatalog.Domain.Tests` | xUnit-tests for `Event`, `TicketCategory`, `Money` |

---

### Week2-EventCatalog-Inventory

EventCatalog implementeres fuldt ud med database og Dapr pub/sub. Inventory-servicen tilføjes og opretter automatisk lagerposter når et event publiceres.

**Nye/ændrede projekter i `src/`:**

| Projekt | Beskrivelse |
|---------|-------------|
| `EventCatalog.Infrastructure` | Nu implementeret: `EventCatalogDbContext`, `EventRepository`, `EventQueriesImpl`, `DaprEventPublisher` |
| `Inventory.Api` | REST API med `StockController` |
| `Inventory.Domain` | `TicketStock` entity med `Reserve()`, `Release()`, `ConfirmSale()` |
| `Inventory.Facade` | Interfaces (`IReserveTicketsUseCase`, `IStockQueries`) og DTOs |
| `Inventory.UseCases` | Use cases og `ITicketStockRepository` port |
| `Inventory.Infrastructure` | `InventoryDbContext`, `TicketStockRepository`, `EventCreatedSubscriber` (pub/sub) |

**Nye projekter i `test/`:**

| Projekt | Beskrivelse |
|---------|-------------|
| `Inventory.Domain.Tests` | xUnit-tests for `TicketStock` |

**Øvrige filer:** `dapr/components/pubsub.yaml` og `statestore.yaml`, `bruno/`-testsamlinger.

---

### Week3-Ordering

Ordering-servicen tilføjes med synkron prisopslag via Dapr service invocation til EventCatalog.

**Nye projekter i `src/`:**

| Projekt | Beskrivelse |
|---------|-------------|
| `Ordering.Api` | REST API: `POST /api/orders`, `GET /api/orders/{id}` |
| `Ordering.Domain` | `Order` (Aggregate Root), `OrderLine`, `CustomerEmail` (Value Object), `OrderStatus` enum |
| `Ordering.Facade` | Interfaces (`IPlaceOrderUseCase`, `IOrderQueries`) og DTOs |
| `Ordering.UseCases` | `PlaceOrderUseCase`, `IEventCatalogService` port |
| `Ordering.Infrastructure` | `OrderingDbContext`, `OrderRepository`, `DaprEventCatalogService` (service invocation), EF Core Value Converters |

**Nye projekter i `test/`:**

| Projekt | Beskrivelse |
|---------|-------------|
| `Ordering.Domain.Tests` | xUnit-tests for `Order`, `OrderLine`, `Money` |

---

### Week4-Payment-SAGA

Payment-servicen tilføjes og Ordering udvides med SAGA-orkestrering via Dapr Workflow.

**Nye projekter i `src/`:**

| Projekt | Beskrivelse |
|---------|-------------|
| `Payment.Api` | Stateless betalingsservice med fejlsimulering |
| `Payment.Facade` | `IProcessPaymentUseCase` interface og DTOs |
| `Payment.UseCases` | `ProcessPaymentUseCase` med simuleret forsinkelse og tilfældig fejl |

**Udvidelser i Ordering:**

| Komponent | Beskrivelse |
|-----------|-------------|
| `PlaceOrderWorkflow` | Dapr Workflow: ReserveTickets → ProcessPayment → ConfirmOrder → PublishOrderConfirmed |
| Workflow Activities | `ReserveTicketsActivity`, `ProcessPaymentActivity`, `ConfirmOrderActivity`, `PublishOrderConfirmedActivity`, `ReleaseTicketsActivity`, `CancelOrderActivity` |
| Nye ports i UseCases | `IInventoryService`, `IPaymentService`, `IOrderEventPublisher`, `IWorkflowStarter` |
| Nye Dapr-services | `DaprInventoryService`, `DaprPaymentService`, `DaprOrderEventPublisher`, `DaprWorkflowStarter` |

---

### Week5-Notification-Integration

Notification-servicen tilføjes som ren event-drevet service. Komplet end-to-end system med alle 5 services.

**Nyt projekt i `src/`:**

| Projekt | Beskrivelse |
|---------|-------------|
| `Notification.Api` | Minimal service: lytter på `order-confirmed` og `order-cancelled` topics via Dapr pub/sub. Indeholder kun `Program.cs` og `OrderEventsSubscriber.cs` |

**Komplet projektliste (21 projekter i `src/`):**

| Service | Projekter |
|---------|-----------|
| EventCatalog | `.Api`, `.Domain`, `.Facade`, `.Infrastructure`, `.UseCases` |
| Inventory | `.Api`, `.Domain`, `.Facade`, `.Infrastructure`, `.UseCases` |
| Ordering | `.Api`, `.Domain`, `.Facade`, `.Infrastructure`, `.UseCases` |
| Payment | `.Api`, `.Facade`, `.UseCases` |
| Notification | `.Api` |
| Fælles | `TicketHub.AppHost`, `TicketHub.ServiceDefaults` |

**Tests (3 projekter i `test/`):** `EventCatalog.Domain.Tests`, `Inventory.Domain.Tests`, `Ordering.Domain.Tests`

**Bruno API-tests (`bruno/`):** Testsamlinger for EventCatalog, Inventory, Ordering og Payment med happy path og fejlscenarier.

**Dapr-konfiguration (`dapr/components/`):** `pubsub.yaml` (Redis pub/sub), `statestore.yaml` (Redis state store til SAGA).

---

### Documents

Opgavebeskrivelser, vejledninger og diagrammer.

| Fil | Beskrivelse |
|-----|-------------|
| `TicketHub-Opgave.docx` / `TicketHub-Opgave-v1.docx` | Opgavebeskrivelse (casebeskrivelse) |
| `TicketHub-UserStories.docx` | User stories og krav |
| `TicketHub-SprintPlan.docx` | Sprintplanlægning |
| `TicketHub-Miniprojekt-Design-Loesning.docx` | Designløsning |
| `TicketHub-Miniprojekt_9.docx` | Designiteration |
| `TicketHub-UI-Design.html` / `.jsx` | UI-design mockup |
| `TicketHub-Vejledning-03-Designniveauer.docx` | Vejledning: designniveauer |
| `TicketHub-Vejledning-04-Kommunikationsmoenstre.docx` | Vejledning: kommunikationsmønstre |
| `TicketHub-Vejledning-05-SAGA.docx` | Vejledning: SAGA-mønster |
| `TicketHub-Vejledning-06a-BPMN.docx` | Vejledning: BPMN-notation |
| `TicketHub-Vejledning-06b-CleanArchitecture.docx` | Vejledning: Clean Architecture |
| `TicketHub-Vejledning-09-Arkitektur.docx` | Vejledning: arkitekturoversigt |
| `TicketHub-Vejledning-10-Tips.docx` | Vejledning: implementeringstips |
| `Bruno test filer.md` | Dokumentation af Bruno-testsamlinger |
| `figurer/bpmn-placeorder.drawio` | BPMN-diagram for PlaceOrder (DrawIO) |
| `figurer/bpmn-placeorder.mmd` | BPMN-diagram for PlaceOrder (Mermaid) |

---

## DemoCode

Selvstændige demoprojekter der illustrerer centrale koncepter brugt i semesterprojektet.

### Aspire101

Viser forskellen mellem manuel service-opsætning og .NET Aspire-orkestrering.

**`Aspire101Before/`** – Uden Aspire:

| Projekt | Beskrivelse |
|---------|-------------|
| `ServiceA` | Simpel API-service med Swagger og HTTP-klient til ServiceB |
| `ServiceB` | Simpel API-service med Swagger |

**`Aspire101After/`** – Med Aspire:

| Projekt | Beskrivelse |
|---------|-------------|
| `ServiceA` | Samme service, nu med Aspire-integration |
| `ServiceB` | Samme service, nu med Aspire-integration |
| `Aspire101After.AppHost` | Aspire-orkestrering af begge services |
| `Aspire101After.ServiceDefaults` | Fælles servicekonfiguration |

---

### DetLilleBibliotek

Bibliotekssystem der demonstrerer Clean Architecture-lagdeling i en monolitisk applikation (uden mikroservices).

| Projekt | Beskrivelse |
|---------|-------------|
| `BibliotekApi` | REST API med Swagger og DI-opsætning |
| `Domain` | Domænemodeller |
| `Application` | Applikationslogik |
| `Facade` | Interfaces og DTOs |
| `Infrastructure` | Dataadgang og eksterne services |
| `Shared` | Delte hjælpeklasser |

---

### PubsubDeclarativeHelloWorld

Simpelt eksempel på Dapr pub/sub med **deklarativ** konfiguration (YAML-filer).

| Projekt | Beskrivelse |
|---------|-------------|
| `HelloWorld.Publish` | Publisher – sender beskeder via Dapr pub/sub |
| `HelloWorld.Subscribe` | Subscriber – modtager beskeder via Dapr pub/sub |
| `Hello.Crosscut` | Delte modeller |

Dapr-konfiguration i `myComponents/`.

---

### PubsubProgramaticHelloWorld

Samme pub/sub-mønster som ovenfor, men med **programmatisk** konfiguration (kodebaseret i stedet for YAML).

| Projekt | Beskrivelse |
|---------|-------------|
| `HelloWorld.Publish` | Publisher med programmatisk pub/sub-opsætning |
| `HelloWorld.Subscribe` | Subscriber med programmatisk subscription |
| `Hello.Crosscut` | Delte modeller |

---

### PubsubProgramaticAspireHelloWorld

Pub/sub-mønsteret med programmatisk konfiguration **og .NET Aspire**-orkestrering.

| Projekt | Beskrivelse |
|---------|-------------|
| `HelloWorld.Publish` | Publisher-service |
| `HelloWorld.Subscribe` | Subscriber-service |
| `Hello.Crosscut` | Delte modeller |
| `PubsubProgramaticAspireHelloWorld.AppHost` | Aspire-orkestrering med Dapr sidecars |
| `PubsubProgramaticAspireHelloWorld.ServiceDefaults` | Fælles servicekonfiguration |

---

### TestAfPrivateSet

Demonstrerer testmønstre for klasser med private setters (immutable properties i DDD-stil).

| Projekt | Beskrivelse |
|---------|-------------|
| `CodeToTest` | Klasser med private setters |
| `TestProject` | xUnit-tests der viser hvordan man tester immutable objekter |

---

## DaprQuickstarts

Eksempler på Dapr Workflow-programmering.

### Workflows

Konsolapplikation der demonstrerer Dapr Workflow med en ordre-behandlingsworkflow.

| Projekt | Beskrivelse |
|---------|-------------|
| `WorkflowConsoleApp` (i `order-processor/`) | Konsol-app med `OrderProcessingWorkflow`: NotifyActivity → VerifyInventoryActivity → RequestApprovalActivity → ProcessPaymentActivity → UpdateInventoryActivity |

Dapr-konfiguration i `components/`. Workflow-traces kan inspiceres i Zipkin.

---

### WorkflowsAspire

Samme workflow som ovenfor, men eksponeret som REST API og orkestreret med .NET Aspire.

| Projekt | Beskrivelse |
|---------|-------------|
| `WorkflowApi` (i `OrderProcessor/`) | REST API der wrapper workflow-logikken |
| `WorkflowsAspire.AppHost` | Aspire-orkestrering med Dapr sidecar |
| `WorkflowsAspire.ServiceDefaults` | Fælles servicekonfiguration |

---

## Teknologier

| Kategori | Teknologi | Version |
|----------|-----------|---------|
| Framework | .NET, ASP.NET Core | 10.0 |
| Microservice Runtime | Dapr (pub/sub, service invocation, workflows) | 1.17 |
| Orkestrering | .NET Aspire | 13.1 |
| Database | PostgreSQL (database-per-service) | – |
| Message Broker / State Store | Redis | – |
| ORM | Entity Framework Core | 10.0 |
| API-dokumentation | Scalar (OpenAPI) | – |
| Observability | OpenTelemetry | 1.15 |
| Test | xUnit v3, Bruno (API-tests) | – |

## Kør semesterprojektet

### Forudsætninger
- .NET 10 SDK
- Docker (til PostgreSQL og Redis)
- Dapr CLI

### Start (Week 5 – komplet system)
```bash
cd Semesterprojekt/Code/Week5-Notification-Integration/src/TicketHub.AppHost
dotnet run
```

### Kør tests
```bash
cd Semesterprojekt/Code/Week5-Notification-Integration
dotnet test
```
