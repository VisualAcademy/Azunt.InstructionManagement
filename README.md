# Azunt.InstructionManagement

`Azunt.InstructionManagement` is a reusable .NET module for managing instruction templates with `Name`, `Content`, `Active`, `CreatedAt`, and `CreatedBy` fields.

This package follows the same structure as `Azunt.ConclusionManagement` and `Azunt.ReasonManagement`:

- `Azunt.InstructionManagement`: reusable class library/NuGet package
- `Azunt.SqlServer`: SQL Server Database Project containing the `Instructions` table
- `Azunt.Web`: Blazor Server test web app using EF Core In-Memory by default

## Projects

```txt
src/Azunt.InstructionManagement
 ├─ Azunt.InstructionManagement.sln
 ├─ Azunt.InstructionManagement
 │  ├─ 00_Common
 │  ├─ 01_Models
 │  ├─ 02_Contracts
 │  ├─ 03_Repositories
 │  ├─ 04_Extensions
 │  ├─ 05_Enhancers
 │  └─ 06_Exporters
 ├─ Azunt.SqlServer
 │  └─ dbo/Tables/Instructions.sql
 └─ Azunt.Web
    └─ Components/Pages/Instructions
```

## DI registration

For quick testing without SQL Server:

```csharp
builder.Services.AddDependencyInjectionContainerForInstructionApp(
    mode: InstructionServicesRegistrationExtensions.RepositoryMode.EfCoreInMemory);
```

For SQL Server:

```csharp
builder.Services.AddDependencyInjectionContainerForInstructionApp(
    connectionString,
    InstructionServicesRegistrationExtensions.RepositoryMode.EfCoreSqlServer);
```

Compatibility overloads are also included:

```csharp
builder.Services.AddDependencyInjectionContainerForInstructionApp(
    connectionString,
    InstructionRepositoryMode.EfCoreSqlServer);
```

## Azunt.Web test pages

- `/Instructions`
- `/InstructionsByTenant`
- `/api/InstructionApi`
- `/api/InstructionExport/Excel`

## SQL table

```sql
CREATE TABLE [dbo].[Instructions]
(
    [Id]        BIGINT         IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [Active]    BIT            DEFAULT ((1)) NULL,
    [CreatedAt] DATETIMEOFFSET NULL DEFAULT SYSDATETIMEOFFSET(),
    [CreatedBy] NVARCHAR(255)  NULL,
    [Name]      NVARCHAR(MAX)  NULL,
    [Content]   NVARCHAR(MAX)  NULL
);
```
