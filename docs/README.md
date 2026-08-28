# TemporaLinq documentation

Concept-by-concept reference for the library. Start with the [README](../README.md)
for a quick tour; come here for depth.

- [Date and time streams](date-streams.md) — `Dates`, `Times`, the LINQ-style
  operators, and how to combine streams with `Merge`/`Union`/`Intersect`/`Except`.
- [Holidays](holidays.md) — the `Holiday`/`HolidayEnumerable` model, national vs.
  state/regional holiday sets, and combining holidays with business-day streams.
- [Calendar calculations](calendar-calculations.md) — every non-Gregorian
  calendar system used to compute holiday dates (Easter, Hijri, Hebrew, Persian,
  Chinese/Korean/Taiwanese lunisolar, Ethiopian, Southeast Asian Buddhist,
  Mongolian), including known accuracy limitations for each.
- [Country coverage](countries.md) — every implemented country/region, grouped
  by continent, with the calendar mechanism(s) it uses and any caveats.
- [Known gaps](known-gaps.md) — holidays deliberately left unimplemented
  (Nepal's civil calendar, Hindu lunisolar festival dates, Indonesia's Nyepi)
  and why, so you don't wonder if it was an oversight.
- [Package structure](package-structure.md) — how the library is packaged
  today, and the planned split into granular per-continent/per-country NuGet
  packages.
