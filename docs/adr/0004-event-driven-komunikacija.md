# ADR-004 — Event-driven komunikacija

- **Status:** Prihvaćeno
- **Datum:** 2026-05-30

## Kontekst

Moduli su nezavisni i ne referenciraju se direktno (vidi
[ADR-001](0001-modularni-monolit.md)), ali ipak moraju reagovati na promene u
drugim modulima — npr. Notifications treba da reaguje na događaje iz Tasks
modula.

## Odluka

Agregati objavljuju **domenske događaje** (npr. `TaskAssignedEvent`,
`TaskCompletedEvent`). Posle perzistencije, `DomainEventDispatcher` preko DI
pronalazi i poziva sve `IDomainEventHandler<TEvent>`.

Tako Notifications modul reaguje na događaje iz Tasks modula **bez direktne
reference** na njega.

## Posledice

**Prednosti:**

- Labava spregnutost između modula.
- Novi reaktivni ponašači se dodaju kao novi handler-i, bez izmene izvora
  događaja.

**Kompromisi:**

- Tok kontrole je indirektan (preko dispatcher-a), što otežava praćenje.
- Događaji se objavljuju tek posle uspešne perzistencije.

## Povezane odluke

- [ADR-001 — Modularni monolit](0001-modularni-monolit.md)
- [ADR-002 — Clean + Hexagonal arhitektura](0002-clean-hexagonal-arhitektura.md)
