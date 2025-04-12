# CoreHub - Modular ASP.NET Core Sample App

CoreHub is a modular, scalable, and production-grade ASP.NET Core sample template designed to demonstrate best practices, cross-cutting concerns, performance benchmarking, and realistic application scenarios.

---

## 🚀 Overview

CoreHub includes two real-world example modules:

1. **QuickPost** – A lightweight social feed API
2. **GroceryGo** – A mini grocery store + ordering system

Both modules share common infrastructure, cross-cutting logic, and are designed to run efficiently on small cloud instances like AWS T3 Micro or Azure B1 series.

---

## 🧱 Base Template Features

### ✅ Cross-Cutting Concerns
- Centralized error handling
- Serilog-based logging
- Global model validation
- JWT Authentication (SSO-ready)
- Response formatting/wrapping
- Health check endpoints
- Localization-ready
- CORS & Security headers
- Basic rate limiting

### ✅ Hosting & Deployment
- Kestrel self-hosted
- Docker support
- AWS T2 Nano/T3 Micro-ready
- Azure B1s/B2s-ready
- RPS benchmarking middleware
- Health check diagnostics

### ✅ Shared Infrastructure
- PostgreSQL via Dapper
- In-memory cache (MemoryCache)
- Optional Redis support
- Hosted background jobs
- Benchmark endpoints for performance testing

---

## 📦 Module 1: QuickPost (Social Feed API)

| Feature               | Description                                             |
|----------------------|---------------------------------------------------------|
| Create Post          | Upload post with optional image                         |
| Like & Comment       | Basic interaction + triggers notification               |
| Realtime Updates     | Optional SignalR hub for updates                        |
| Notification System  | In-app + pluggable push/email notification abstraction  |
| Feed Pagination      | Public feed, sorted & cached                            |
| Background Cleanup   | Removes old posts using background worker               |
| Benchmark Endpoint   | Response time + RPS measurement route                   |

---

## 🛒 Module 2: GroceryGo (Mini Grocery Store API)

| Feature               | Description                                              |
|----------------------|----------------------------------------------------------|
| Store Management     | CRUD for grocery stores                                  |
| Product Catalog      | Manage products, pricing, and images                    |
| Order Placement      | Create orders, manage stock                             |
| Order Tracking       | Order status flow: pending → processing → delivered     |
| Pickup/Delivery Modes| Route logic split by delivery type                      |
| Notifications        | Order confirmation & updates                            |
| Auto-Cancel Orders   | Background task for expired unpaid orders               |

---

## 🌐 Project Structure (Suggested)

```
/src
  /CoreHub.Api
  /CoreHub.Application
  /CoreHub.Domain
  /CoreHub.Infrastructure
  /Modules
    /QuickPost
    /GroceryGo
/tests
  /CoreHub.Tests
```

---

## 📊 Performance Targets by Hosting Tier

| Tier          | Host Type      | Cost (est.) | RPM        | RPS       | Response Time |
|---------------|----------------|-------------|------------|-----------|----------------|
| Ultra-Small   | T2 Nano / B1s  | ~$5/month   | ~10,000    | ~5–10     | ~100–150ms     |
| Small (MVP)   | T3 Micro / B1ms| ~$10/month  | ~30,000    | ~15–25    | ~80–120ms      |
| Medium        | T3 Medium / B2s| ~$60/month  | ~60,000    | ~40–60    | ~60–100ms      |
| High-Traffic  | Multi T3 LG    | ~$300/month | ~150,000   | ~120–200  | ~40–80ms       |
| Enterprise    | Dedicated       | $1,000+/mo  | 500,000+   | 400+      | ~20–50ms       |

---

## ✅ Ready for Growth
- Start small (T3 Micro or B1ms) with 30,000 RPM
- Modular structure allows clean scaling
- Easily extendable to new domains (e.g., ecommerce, inventory, chat)

---

## 📍Next Steps
- Scaffold QuickPost module first
- Add core services (logging, validation, health)
- Add GroceryGo module and shared infra
- Write benchmark/load tests (K6, Artillery, etc.)

---

Happy coding! 🎯

