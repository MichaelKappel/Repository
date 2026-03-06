# MichaelKappel.Repository

Generic repository building blocks for ADO.NET and Entity Framework, plus a complete sample implementation that demonstrates paging, search, caching, and unit testing patterns.

## Solution Overview

- `MichaelKappel.Repositories.Common`
  - Shared paging contracts/models and lenient JSON converters.
- `MichaelKappel.Repositories.SqlRepositoryBase`
  - SQL repository base classes with helper methods for executing commands and composing paging SQL.
- `MichaelKappel.Repositories.EntityFrameworkRepositoryBase`
  - Entity Framework migration extensions and expression composition utilities.
- `MichaelKappel.Repositories.DistributedCache`
  - Distributed cache wrapper for read/list/paging scenarios.
- `ExampleImplementation.Repositories`
  - Sample product catalog domain with a testable service and in-memory repository.
- `ExampleImplementation.Website`
  - MVC sample site that consumes the sample service and renders searchable paged results.
- `ExampleImplementation.ConsoleApp`
  - Console runner that exercises the sample service.

## Unit Test Coverage

The solution now includes dedicated test projects for each major area:

- `MichaelKappel.Repositories.Common.Tests`
- `MichaelKappel.Repositories.SqlRepositoryBase.Tests`
- `MichaelKappel.Repositories.EntityFrameworkRepositoryBase.Tests`
- `MichaelKappel.Repositories.DistributedCache.Tests`
- `ExampleImplementation.Repositories.Tests`
- `ExampleImplementation.Website.Tests`

Tests focus on behavior, not implementation details:

- Converter edge cases and paging model invariants.
- SQL-generation helpers and type-conversion behavior.
- Migration SQL generation and predicate composition.
- Cache hit/miss policy and serialization outcomes.
- Service-level filtering, sorting, and pagination.
- Controller/view-model composition for the sample site.

## Run Tests

```powershell
dotnet test MichaelKappel.Repository.sln -c Debug
```

## Sample Site

Run the sample website:

```powershell
dotnet run --project ExampleImplementation.Website\ExampleImplementation.Website.csproj
```

Run the console sample:

```powershell
dotnet run --project ExampleImplementation.ConsoleApp\ExampleImplementation.ConsoleApp.csproj -- tool
```
