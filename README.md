# InvoiceClean

A **Clean Architecture** invoice management system built with **.NET 10**, demonstrating domain-driven design principles, CQRS pattern with MediatR, and Entity Framework Core.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

### Layers

- **Domain**: Pure business logic, no dependencies
  - `Invoice` aggregate with encapsulated `InvoiceLine` collection
  - Customer information (name, address, VAT)
  - Domain exceptions for business rule violations
  
- **Application**: Use cases with MediatR
  - `CreateInvoiceCommand` - Create new invoices
  - `UpdateInvoiceCommand` - Update invoice details and customer information
  - `GetInvoiceByIdQuery` - Retrieve invoice details
  - FluentValidation for all commands
  - `ValidationBehavior` pipeline for automatic validation
  - Result pattern for error handling
  - Repository interfaces (ports)

- **Infrastructure**: Data persistence
  - Entity Framework Core with SQLite
  - Repository implementations
  - `AppDbContext` with explicit EF Core mappings for encapsulated collections

- **API**: ASP.NET Core Web API
  - RESTful endpoints
  - Global exception handling with ProblemDetails
  - OpenAPI/Swagger support
  - Contract-based request/response DTOs

## 🚀 Technologies

- **.NET 10**
- **C# 14.0**
- **Entity Framework Core 10.0.1**
- **MediatR 14.0.0** - CQRS pattern
- **FluentValidation 12.1.1** - Request validation
- **SQLite** - Lightweight database
- **xUnit** - Unit and integration testing
- **FluentAssertions** - Test assertions

## 📋 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2026 / Rider / VS Code

## 🛠️ Setup

### 1. Clone the repository
```bash
git clone https://github.com/ivek81cro/InvoiceClean.git 
cd InvoiceClean
```

### 2. Restore dependencies
```bash
dotnet restore
```

### 3. Apply database migrations
```bash
dotnet ef database update --project InvoiceClean.Infrastructure --startup-project InvoiceClean.Api
```

### 4. Run the application
```bash
dotnet run --project InvoiceClean.Api
```

The API will be available at `https://localhost:5001` (or configured port).

### 5. Run tests
```bash
dotnet test
```

## 🎯 Features

### Domain Model
- **Invoice Aggregate**: Encapsulated entity with business logic
- **Customer Information**: Name, address, and VAT number support
- **Value Objects**: `InvoiceLine` with automatic total calculation
- **Domain Exceptions**: Business rule enforcement
- **Immutable Operations**: Update methods maintain aggregate consistency

### Validation Pipeline
- Automatic validation of all MediatR requests
- Centralized validation using `ValidationBehavior<TRequest, TResponse>`
- Returns structured validation errors (400 Bad Request)
- Comprehensive validation rules for all commands

### Entity Framework Configuration
- Explicit mapping for private backing fields (`_lines`)
- Cascade delete for invoice lines
- Property access mode configuration for encapsulated collections

### Result Pattern
- Type-safe error handling with `Result<T>`
- Eliminates exceptions for expected failures
- Clear success/failure semantics

## 📡 API Endpoints

### Create Invoice
**Request**:
```http
POST /api/invoices
Content-Type: application/json

{
  "number": "INV-001",
  "date": "2025-12-21",
  "lines": [
    {
      "description": "Consulting Service",
      "quantity": 10,
      "unitPrice": 150.00
    }
  ]
}
```

**Response**: `201 Created` with invoice ID

**Validation Rules**:
- Invoice number required (max 50 chars)
- Date cannot be more than 30 days in the future
- At least one line required
- Line description required (max 200 chars)
- Quantity must be positive
- Unit price must be positive

### Update Invoice
**Request**:
```http
PUT /api/invoices/{id}
Content-Type: application/json

{
  "number": "INV-001-UPDATED",
  "date": "2025-12-21",
  "customerName": "Acme Corp",
  "customerAddress": "123 Business St",
  "customerVat": "HR12345678901"
}
```

**Response**: `200 OK` with updated invoice details

**Validation Rules**:
- Invoice ID required
- Invoice number required (max 50 chars)
- Date cannot be more than 30 days in the future
- Customer name required (max 200 chars)
- Customer address optional (max 500 chars)
- Customer VAT optional (max 50 chars)

### Get Invoice by ID
**Request**:
```http
GET /api/invoices/{id}
```

**Response**: `200 OK` with invoice details or `404 Not Found`

## 🧪 Project Structure

```
InvoiceClean/
├── InvoiceClean.Domain/          # Core business logic
│   └── Invoices/
│       ├── Invoice.cs            # Aggregate root
│       ├── InvoiceLine.cs        # Entity
│       └── DomainException.cs    # Domain exceptions
│
├── InvoiceClean.Application/     # Use cases & validation
│   ├── Common/
│   │   ├── Behaviors/            # MediatR pipeline behaviors
│   │   └── Results/              # Result pattern
│   └── Invoices/
│       ├── CreateInvoice/        # Command + Validator + Handler
│       ├── UpdateInvoice/        # Command + Validator + Handler
│       └── GetInvoiceById/       # Query + Handler
│
├── InvoiceClean.Infrastructure/  # Data access
│   ├── Persistence/
│   │   ├── AppDbContext.cs       # EF Core context
│   │   ├── Configurations/       # Entity configurations
│   │   └── Repositories/         # Repository implementations
│   └── Migrations/
│
├── InvoiceClean.Api/             # Web API
│   ├── Endpoints/                # Minimal API endpoints
│   ├── Contracts/                # Request/Response DTOs
│   └── Program.cs                # Application startup
│
└── InvoiceClean.Api.Tests/       # Integration tests
    └── Invoices/                 # Endpoint tests
```

## 🧪 Project Structure Details

### ValidationBehavior Pattern
All commands are automatically validated before reaching handlers:
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // Intercepts all MediatR requests
    // Runs FluentValidation validators
    // Returns validation errors if any
}
```

### EF Core Encapsulation
Mapping private collections with backing fields:
```csharp
builder.HasMany(i => i.Lines)
    .WithOne()
    .HasForeignKey("InvoiceId")
    .Metadata.PrincipalToDependent
    .SetPropertyAccessMode(PropertyAccessMode.Field);
```

### Repository Pattern
Clean separation between domain and infrastructure:
```csharp
// Domain defines the interface
public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task AddAsync(Invoice invoice);
    Task UpdateAsync(Invoice invoice);
}
```

## 🔧 Configuration

Database connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=invoices.db"
  }
}
```

## 🧪 Testing

The project includes comprehensive integration tests:
- **API Endpoint Tests**: Full HTTP workflow testing
- **Validation Tests**: Ensures all business rules are enforced
- **Test Database**: Uses in-memory SQLite for isolated tests
- **WebApplicationFactory**: Real API testing without external dependencies

Run tests with:
```bash
dotnet test
```

## 📝 Contributing

1. Follow Clean Architecture principles
2. Use MediatR for all use cases (commands and queries)
3. Validate commands with FluentValidation
4. Keep domain entities encapsulated
5. Use the Result pattern for error handling
6. Write integration tests for all endpoints
7. Write meaningful commit messages

## 📄 License

This project is for educational purposes demonstrating Clean Architecture patterns.

---

**Repository**: [github.com/ivek81cro/InvoiceClean](https://github.com/ivek81cro/InvoiceClean)