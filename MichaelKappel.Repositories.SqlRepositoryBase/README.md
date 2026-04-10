# MichaelKappel.Repositories.SqlRepositoryBase

`MichaelKappel.Repositories.SqlRepositoryBase` is the shared SQL access foundation used by raw-SQL repository implementations. It centralizes common ADO.NET-style behavior so feature repositories do not need to repeat connection, command, and parameter plumbing.

## Project Type

- SDK: `Microsoft.NET.Sdk`
- Target framework: `net9.0`
- Output: class library

## What Lives Here

- `SqlRepositoryBase.cs`: the core base class used by SQL repositories

This is intentionally a small, focused project. It exists to hold reusable infrastructure rather than business features.

## How It Fits Into The Solution

This project sits underneath:

- `CogniVerses.SqlRepositories`
- other repository libraries in the sibling `Repository` workspace

It helps keep SQL repositories consistent by providing a shared implementation point for lower-level database operations.

## Key Dependencies

- `MichaelKappel.Repositories.Common`
- `Microsoft.Data.SqlClient`
- `Microsoft.Extensions.Options`
- `System.Runtime.Caching`

## Local Development

Build from this directory with:

```powershell
dotnet build MichaelKappel.Repositories.SqlRepositoryBase.csproj
```

Because this is a base library, it is usually exercised indirectly through the repositories that inherit from it.

## Testing Status

A matching test project already exists in the sibling repository workspace:

- `..\MichaelKappel.Repositories.SqlRepositoryBase.Tests`

That is the right place for regression coverage around base SQL behavior, parameter handling, and shared helper methods.
