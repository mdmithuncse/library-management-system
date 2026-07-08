# library-management-system

A layered ASP.NET Core demo API for managing books, copies, loans, fines, users, and roles.

## Projects

- `src/Unison.LibraryManagement.Api` - HTTP API, authentication, Swagger, startup wiring
- `src/Unison.LibraryManagement.Application` - commands, handlers, DTOs, mapping, application services
- `src/Unison.LibraryManagement.Domain` - entities and repository contracts
- `src/Unison.LibraryManagement.Infrastructure` - EF Core persistence, repositories, password hashing
- `tests` - xUnit test projects

## Prerequisites

- .NET 10 SDK
- SQL Server

## Configuration

Set the default connection string before running the API:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=127.0.0.1,1433;User Id=sa;Password=<password>;Database=LibraryManagementDb;TrustServerCertificate=True;" --project src/Unison.LibraryManagement.Api
```

Optional admin seeding:

```powershell
$env:INITIAL_ADMIN_EMAIL = "admin@example.com"
$env:INITIAL_ADMIN_PASSWORD = "ChangeMe123!"
```

Authentication currently uses Basic Auth because this is a demo. Do not use the current auth scheme as-is for production.

## Run

```powershell
dotnet restore
dotnet ef database update --project src/Unison.LibraryManagement.Infrastructure --startup-project src/Unison.LibraryManagement.Api
dotnet run --project src/Unison.LibraryManagement.Api
```

Swagger is available at the API root in Development.

## Test

```powershell
dotnet test
```

## Main API Areas

- `POST /api/auth/register` - register a member
- `GET /api/books` - search books
- `POST /api/books` - create a book, librarian/admin only
- `POST /api/borrowings/borrow` - borrow a book
- `POST /api/borrowings/return` - return a loan
- `GET /api/borrowings/me` - current user's active loans
- `GET /api/borrowings/overdue` - overdue loans, librarian/admin only
- `GET /api/fines/users/{id}/outstanding` - outstanding fines
