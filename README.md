# InvoiceClean

A **Clean Architecture** invoice management system built with **.NET 10**, demonstrating domain-driven design principles, CQRS pattern with MediatR, and Entity Framework Core.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

### Layers

- **Domain**: Pure business logic, no dependencies
  - `Invoice` aggregate with encapsulated `InvoiceLine` collection
  - Domain exceptions for business rule violations
  
- **Application**: Use cases with MediatR
  - `CreateInvoiceCommand` with FluentValidation
  - `ValidationBehavior` pipeline for automatic validation
  - Repository interfaces (ports)

- **Infrastructure**: Data persistence
  - Entity Framework Core with SQLite
  - Repository implementations
  - `AppDbContext` with explicit EF Core mappings for encapsulated collections

- **API**: ASP.NET Core Web API
  - RESTful endpoints
  - Global exception handling with ProblemDetails
  - OpenAPI/Swagger support

## 🚀 Technologies

- **.NET 10**
- **C# 14.0**
- **Entity Framework Core 10.0.1**
- **MediatR 14.0.0** - CQRS pattern
- **FluentValidation 12.1.1** - Request validation
- **SQLite** - Lightweight database

## 📋 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2026 / Rider / VS Code

## 🛠️ Setup

### 1. Clone the repository

### 2. Restore dependencies

### 3. Apply database migrations

### 4. Run the application

The API will be available at `https://localhost:5001` (or configured port).

## 🎯 Features

### Domain Model
- **Invoice Aggregate**: Encapsulated entity with business logic
- **Value Objects**: `InvoiceLine` with automatic total calculation
- **Domain Exceptions**: Business rule enforcement

### Validation Pipeline
- Automatic validation of all MediatR requests
- Centralized validation using `ValidationBehavior<TRequest, TResponse>`
- Returns structured validation errors (400 Bad Request)

### Entity Framework Configuration
- Explicit mapping for private backing fields (`_lines`)
- Cascade delete for invoice lines
- Property access mode configuration for encapsulated collections

## 📡 API Endpoints

### Create Invoice

**Request**:
- `POST /invoices`
- Invoice details in the request body

**Response**: `201 Created` with invoice ID

**Validation Rules**:
- Invoice number required (max 50 chars)
- Date cannot be in the future
- At least one line required
- Line description required (max 200 chars)
- Quantity must be positive
- Unit price must be positive

## 🧪 Project Structure Details

### ValidationBehavior Pattern
All commands are automatically validated before reaching handlers:

### EF Core Encapsulation
Mapping private collections with backing fields:

### Repository Pattern
Clean separation between domain and infrastructure:

## 🔧 Configuration

Database connection string in `appsettings.json`:

## 📝 Contributing

1. Follow Clean Architecture principles
2. Use MediatR for all use cases
3. Validate commands with FluentValidation
4. Keep domain entities encapsulated
5. Write meaningful commit messages

## 📄 License

This project is for educational purposes demonstrating Clean Architecture patterns.

## 🤝 Author

Built as a demonstration of modern .NET architecture patterns with Clean Architecture, CQRS, and Domain-Driven Design principles.
