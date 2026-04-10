# MichaelKappel.Repositories.EntityFrameworkRepositoryBase

`MichaelKappel.Repositories.EntityFrameworkRepositoryBase` contains reusable Entity Framework infrastructure shared by EF-backed repository projects. It exists to keep common EF patterns in one place instead of duplicating them across solution-specific repositories.

## Project Type

- SDK: `Microsoft.NET.Sdk`
- Target framework: `net9.0`
- Output: class library

## What Lives Here

- `EntityFrameworkRepositoryBase.cs`: common EF repository behavior
- `MigrationBuilderExtensions.cs`: migration-time helper extensions
- `PredicateBuilder.cs`: reusable predicate composition support
- `Models`: shared EF-related model types

## How It Fits Into The Solution

This project is the shared base for EF repository implementations, especially:

- `CogniVerses.EntityFrameworkRepositories`

It gives consuming repositories a common place for generic EF logic, predicate composition, and migration helpers.

## Key Dependencies

- `Microsoft.EntityFrameworkCore.Relational`

## Local Development

Build from this directory with:

```powershell
dotnet build MichaelKappel.Repositories.EntityFrameworkRepositoryBase.csproj
```

## Testing Status

A matching test project already exists in the sibling repository workspace:

- `..\MichaelKappel.Repositories.EntityFrameworkRepositoryBase.Tests`

That test project is the natural home for coverage around predicate composition, migration helpers, and shared EF repository behavior.
