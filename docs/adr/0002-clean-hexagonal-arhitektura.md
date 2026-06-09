# ADR-002 — Clean + Hexagonal arhitektura

- **Status:** Prihvaćeno
- **Datum:** 2026-06-09

## Kontekst

Poslovna logika mora biti nezavisna od konkretne infrastrukture (UI, baza,
fajlovi) kako bi bila testabilna u izolaciji i otporna na promene tehnologije.

## Odluka

Svaki modul je podeljen na `Domain`, `Application` i `Infrastructure`, sa
zavisnostima koje uvek pokazuju **ka unutra** (ka Domain sloju).

- `Application`/`Domain` definišu **portove** (interfejse: `I*Repository`,
  dispatcher-i).
- `Infrastructure` pruža **adaptere** koji se ubrizgavaju kroz DI.

Poslovni kod ne zna koja je konkretna implementacija aktivna.

## Posledice

**Prednosti:**

- Domain i Application su testabilni u izolaciji (bez baze i fajlova).
- Implementacije se mogu zameniti bez izmene poslovne logike.

**Kompromisi:**

- Više projekata i interfejsa po modulu — veći inicijalni boilerplate.

## Povezane odluke

- [ADR-001 — Modularni monolit](0001-modularni-monolit.md)
- [ADR-003 — Perzistencija preko IDocumentStore](0003-perzistencija-idocumentstore.md)
