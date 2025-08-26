# ShortenUrl

A lightweight, layered **URL shortener** built with .NET 8, following Clean Architecture principles.

---

## ✨ Features

- Shorten long URLs into compact codes
- Redirect short codes → original URL
- Configurable base URL & code length
- In-memory repository for development
- (Optional) EF Core support for persistence
- (Planned) Analytics, expiration, rate limiting, vanity URLs

---

## 🧱 Architecture

- **Domain** → entities & core business rules
- **Application** → use cases, CQRS, ports
- **Infrastructure** → EF Core, persistence adapters
- **API** → ASP.NET Core Web API endpoints, DI wiring

Projects in solution:

- `shorten.url` — Web API (entry point)  
- `shorten.url.application` — application layer  
- `shorten.url.domain` — entities & logic  
- `shorten.url.infrastructure` — persistence, external services  

---

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server/PostgreSQL (optional, for EF Core persistence)

### Run the API
```bash
git clone https://github.com/bujar-ademi/shortenurl
cd shortenurl
dotnet build
dotnet run --project ./shorten.url/shorten.url.csproj