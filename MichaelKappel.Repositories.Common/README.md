# MichaelKappel.Repositories.Common

`MichaelKappel.Repositories.Common` is the lowest-level shared contract and utility library used by the repository base projects and by `CogniVerses.Common`. It provides foundational types that other libraries build on.

## Project Type

- SDK: `Microsoft.NET.Sdk`
- Target framework: `net9.0`
- Output: class library

## What Lives Here

- `Interfaces`: shared repository-facing abstractions
- `Models`: common model types and option models
- `JsonConverters`: serialization helpers
- `Enumerations`: shared enum types
- `Exceptions`: shared exception types
- `Extensions`: reusable utility extensions

## How It Fits Into The Solution

This is base infrastructure. It sits below:

- `MichaelKappel.Repositories.SqlRepositoryBase`
- `MichaelKappel.Repositories.EntityFrameworkRepositoryBase`
- `CogniVerses.Common`

When a type needs to be shared across repository foundations instead of living in the application layer, this is the place for it.

## Local Development

Build from this directory with:

```powershell
dotnet build MichaelKappel.Repositories.Common.csproj
```

## Testing Status

A matching test project already exists in the sibling repository workspace:

- `..\MichaelKappel.Repositories.Common.Tests`

That project is the right place for coverage around shared extensions, converters, utility models, and foundational contract behavior.
