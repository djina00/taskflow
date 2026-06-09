# ADR-001 — Modularni monolit

- **Status:** Prihvaćeno
- **Datum:** 2026-06-09

## Kontekst

TaskFlow treba da podrži više poslovnih oblasti (korisnici, projekti, zadaci,
obaveštenja, izveštaji) sa jasnim granicama, uz jednostavan deployment desktop
aplikacije.

## Odluka

Aplikacija je jedan izvršni proces sa 5 nezavisnih modula: **Users, Projects,
Tasks, Notifications, Reports**. Svaki modul ima svoje slojeve i svoj DI registar;
moduli se ne referenciraju direktno.

## Posledice

**Prednosti:**

- Jasne granice između poslovnih oblasti.
- Jednostavan deployment (jedan proces).
- Mogućnost kasnijeg izdvajanja modula u zaseban servis.

**Kompromisi:**

- Komunikacija između modula mora ići kroz apstrakcije (vidi [ADR-004](0004-event-driven-komunikacija.md)),
  a ne kroz direktne reference.

## Povezane odluke

- [ADR-002 — Clean + Hexagonal arhitektura](0002-clean-hexagonal-arhitektura.md)
- [ADR-004 — Event-driven komunikacija](0004-event-driven-komunikacija.md)
