# 📚 StoryBooks API

![CI](https://github.com/umaima-asad/StoryBooks-Dotnet-Training/actions/workflows/coverage-badge.yml/badge.svg?branch=main)
![Coverage](./coverage-report/badge_linecoverage.svg)

A modular **.NET 8 Web API** project for managing storybooks with built-in **authentication, caching (Redis)**, **validation (FluentValidation)**, **logging (Serilog with Opentelemetry)** ,and **integration tests** powered by **xUnit**.  

---

## 🚀 Features

- ✅ **JWT-based authentication** Identity + Roles + Policies
- 📘 **CRUD for StoryBooks** with validation
- ⚡ **Redis caching** for faster reads
- 🧩 **FluentValidation** for DTOs
- 🗺️ **AutoMapper** for entity to DTO or vice versa
- ⚡ **Serilog, Opentelemetry and Aspire Dashboard** for Logs Metrics and Traces
- 🧪 **xUnit integration tests** with coverage tracking
- 🔐 **Role-based and policy-based authorization**
- ☁️ **GitHub Actions CI** with coverage badge auto-update
- 🧹 **Clean Architecture** for scalability

---

## 🛠️ Tech Stack

| Component | Library / Framework |
|------------|--------------------|
| **Language** | C# (.NET 8) |
| **Framework** | ASP.NET Core Web API |
| **ORM** | Entity Framework Core |
| **Validation** | FluentValidation |
| **Testing** | xUnit, FluentAssertions, Moq |
| **Caching** | Redis via StackExchange.Redis |
| **Logging** | Serilog Opentelemetry and Aspire Dashboard |
| **Auth** | ASP.NET Core Identity + JWT |
| **CI/CD** | GitHub Actions |
| **Coverage** | Coverlet + `tj-actions/coverage-badge-py` |

---

## ⚙️ Setup Instructions

### 1️⃣ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [Redis](https://redis.io/download) (or run via Docker:  
  ```bash
  docker run -d -p 6379:6379 --name redis redis
  ```)

---

### 2️⃣ Clone & Build

```bash
git clone https://github.com/<your-username>/<your-repo>.git
cd StoryBooks
dotnet build
