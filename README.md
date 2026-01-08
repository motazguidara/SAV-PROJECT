# SAV Project - Microservices SAV Platform

## Monorepo Structure
```
.
├── docker-compose.yml
└── src
    ├── CatalogService
    ├── CustomerClaimsService
    ├── Gateway
    ├── IdentityService
    ├── InterventionsService
    └── Frontend
        ├── Client
        ├── Server
        └── Shared
```

## Services Overview
- **IdentityService**: user registration, login, JWT issuing.
- **CatalogService**: articles catalog management.
- **CustomerClaimsService**: customer claims lifecycle.
- **InterventionsService**: interventions, parts, billing.
- **Gateway**: YARP reverse proxy, single entrypoint.
- **Frontend**: Blazor WebAssembly hosted SPA with MudBlazor UI.

## Run Instructions
### Prerequisites
- Docker + Docker Compose

### Start All Services
```bash
docker compose up --build
```

### URLs
- **Gateway**: http://localhost:5000
- **Frontend SPA**: http://localhost:5005
- **IdentityService**: http://localhost:5001
- **CatalogService**: http://localhost:5002
- **CustomerClaimsService**: http://localhost:5003
- **InterventionsService**: http://localhost:5004

### Default Accounts
- **SAV Manager**
  - Email: `manager@sav.local`
  - Password: `ChangeMe123!`

### Database Migrations
Each service applies migrations automatically on startup.
To create new migrations locally:
```bash
dotnet ef migrations add InitialCreate -p src/IdentityService -s src/IdentityService
dotnet ef migrations add InitialCreate -p src/CatalogService -s src/CatalogService
dotnet ef migrations add InitialCreate -p src/CustomerClaimsService -s src/CustomerClaimsService
dotnet ef migrations add InitialCreate -p src/InterventionsService -s src/InterventionsService
```

## User Guide
### Client Flow
1. Register in the SPA.
2. Log in and access the dashboard.
3. Create a claim by selecting an article and entering purchase info.
4. Track claim status and view interventions/invoice when available.

### SAV Manager Flow
1. Log in with the seeded manager account.
2. Open the claims inbox and accept/reject claims.
3. Create interventions from the claim detail page.
4. Add parts, adjust labor, start/end interventions.
5. Closing the intervention marks the claim as resolved.

## Developer Guide
### Authentication Flow
- IdentityService issues JWT access tokens.
- Gateway routes requests to services.
- Services validate JWT with the shared issuer/audience/key.

### Warranty & Billing
- InterventionsService pulls claim + article details.
- Warranty is computed as `PurchaseDate + WarrantyMonths >= Today`.
- Under warranty: `LaborCost = 0`, `InvoiceTotal = 0` (parts covered).
- Outside warranty: `InvoiceTotal = LaborCost + parts total`.

### Add a New Microservice
1. Create a new ASP.NET Core Web API project under `src/`.
2. Add a Dockerfile and PostgreSQL connection string.
3. Add a YARP route/cluster in `src/Gateway/appsettings.json`.
4. Add the service and database to `docker-compose.yml`.
5. (Optional) Add client pages and API methods in the frontend.
