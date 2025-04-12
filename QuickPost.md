# QuickPost MVP - Technical Overview

## 🌟 Purpose

**QuickPost** is a minimal social feed API designed to showcase clean architecture, modular design, and real-world performance, hosted affordably on AWS (T3 micro). It is built for experimentation, demonstrating extensibility and efficiency in a constrained environment.

---

## 🔧 Core Features

| Feature                | Description |
|------------------------|-------------|
| **Create Post**        | Users can submit text posts with optional images (uploaded to S3). |
| **Like Post**          | Lightweight interaction to demonstrate relational logic and notification trigger. |
| **Feed Pagination**    | Public feed with cursor-based pagination, cached in memory. |
| **In-app Notifications** | Alerts triggered on interactions, using a pluggable notification system. |
| **Background Cleanup** | Removes old posts using a hosted background service. |
| **Benchmark Endpoint** | Exposes uptime, response time, and RPS metrics. |
| **Auth Integration**   | Firebase OpenID Connect for secure and simple authentication. |
| **Optional SignalR**   | Real-time updates (disabled by default to reduce hosting load). |

---

## 🔗 API Endpoints

### 📄 Posts

| Method | Endpoint              | Description                        | Auth Required |
|--------|-----------------------|------------------------------------|---------------|
| `POST` | `/api/posts`          | Create a new post with optional image URL | ✅ |
| `GET`  | `/api/feed`           | Get public feed (cursor-based pagination) | ❌ |
| `POST` | `/api/posts/{id}/like`| Like/unlike a post (toggle)        | ✅ |

### 🔔 Notifications

| Method | Endpoint                          | Description                            | Auth Required |
|--------|-----------------------------------|----------------------------------------|---------------|
| `GET`  | `/api/notifications`              | Get current user's in-app notifications | ✅ |
| `GET`  | `/api/notifications/unread-count` | Get number of unread notifications     | ✅ |

> 🧠 Optionally: When a notification is triggered (like or post event), SignalR pushes it to the user if connected.

### 📶 Realtime (SignalR)

| Hub              | Path                   | Description                                 |
|------------------|------------------------|---------------------------------------------|
| `NotificationHub`| `/hubs/notifications`  | Push real-time updates (e.g. likes, posts) |

### 🧪 Benchmark

| Method | Endpoint      | Description                                | Auth Required |
|--------|---------------|--------------------------------------------|---------------|
| `GET`  | `/benchmark`  | Get uptime, response time, and RPS stats  | ❌ |

---

## 📊 Tech Stack

| Layer             | Technology |
|------------------|------------|
| Backend API       | ASP.NET Core (.NET 8) |
| ORM               | Dapper |
| Cache             | `IMemoryCache` (in-process) |
| Auth              | Firebase (OpenID Connect) |
| Background Jobs   | `IHostedService` |
| Image Storage     | AWS S3 |
| Hosting           | EC2 T3 micro |
| Logging/Monitoring| CloudWatch, Benchmark endpoint |
| Realtime (optional) | SignalR |

---

## ⚙️ Architecture Highlights

- Modular separation into Core, API, and Infrastructure layers.
- In-memory caching strategy with easy Redis swapability.
- Hosted background worker for cleanup and periodic tasks.
- Pluggable notification service (supports in-app, email, etc.).
- Extensible design for real-time support and external integrations.

---

## ☁️ AWS Hosting Plan

| Component     | AWS Service            |
|---------------|------------------------|
| API Host      | EC2 T3 micro           |
| Database      | RDS (PostgreSQL or SQL Server) |
| Image Storage | S3                     |
| Auth          | Firebase               |
| Metrics       | CloudWatch             |

Deployment will be kept lightweight to ensure performance within the constraints of a T3 micro instance.

---

## 🔁 Application Flow Summary

- Users create posts → stored in database; images saved to S3.
- Public feed is paginated and cached in memory.
- Likes trigger pluggable in-app notifications.
- Background worker purges expired posts.
- `/benchmark` endpoint exposes system performance.

---

## 📈 Performance Expectations (T3 Micro)

| Metric               | Estimate |
|----------------------|----------|
| Cold Start           | ~2 seconds |
| Avg Response (cached)| ~10-20 ms |
| RPS (feed cached)    | ~100-150 RPS |
| RPS (DB access)      | ~30-50 RPS |
| Memory Footprint     | < 500MB |
| CPU Utilization      | < 50% on average load |

---

## 🧪 Demo & Experimentation Goals

- Showcase clean and testable architecture.
- Run efficiently on limited-resource infrastructure.
- Enable forking, extension, and developer experimentation.
- Provide a base template for scalable social APIs.

