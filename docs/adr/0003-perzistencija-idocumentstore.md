# ADR-003 — Perzistencija preko IDocumentStore (JSON/SQLite)

- **Status:** Prihvaćeno
- **Datum:** 2026-06-09

## Kontekst

Aplikacija treba da podrži više načina skladištenja podataka (lokalni JSON
fajlovi i SQLite baza), bez vezivanja poslovne logike za konkretnu tehnologiju
perzistencije.

## Odluka

Uvedena je apstrakcija `IDocumentStore` sa dve implementacije:
`JsonFileDocumentStore` i `SqliteDocumentStore`. Generički
`DocumentRepository<T>` radi nad tom apstrakcijom.

Izbor implementacije je konfigurabilan kroz DI (enum `PersistenceKind`), bez
ikakve izmene poslovne logike:

```csharp
// src/TaskFlow.Desktop/ServiceConfiguration.cs
services.AddInfrastructure(PersistenceKind.Json, dataDirectory); // ili PersistenceKind.Sqlite
```

U testovima se koriste in-memory implementacije istih portova.

## Posledice

**Prednosti:**

- Zamena skladišta je jedna linija u kompozicionom korenu.
- Testovi koriste in-memory portove — bez baze i fajlova.

**Kompromisi:**

- Generički model dokumenata ograničava napredne upite specifične za bazu.

## Povezane odluke

- [ADR-002 — Clean + Hexagonal arhitektura](0002-clean-hexagonal-arhitektura.md)
