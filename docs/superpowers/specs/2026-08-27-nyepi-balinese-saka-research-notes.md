# Nyepi / Balinese Saka Calendar (Roadmap Phase 5) — Research Notes and Deferral

## Status: deferred, not implemented

This phase was intended to complete Indonesia's Nyepi (Balinese Saka New Year), the last
remaining gap in Indonesia's holiday list. After checking real dates against the obvious
astronomical rule, it is being deferred for the same reason as the Hindu lunisolar calendar
(Phase 4, see `2026-08-27-hindu-lunisolar-calendar-research-notes.md`): a day-boundary
tie-breaking rule that does not reduce to a single verifiable formula.

## The rule that was tried

Nyepi is widely described as "the day after Tilem Kesanga" — the day after the new moon (Tilem)
of the ninth month (Kesanga) of the Balinese Saka calendar, which normally falls in March. This
looks directly reusable from the already-verified `LunarPhaseCalculation.NewMoonsInGregorianYear`
(Meeus ch. 49, the same primitive used for Sri Lanka's full-moon Poya calculation).

## Where it broke down

Checked against three consecutive real years (new-moon dates from
`LunarPhaseCalculation.NewMoonsInGregorianYear`, actual Nyepi dates verified via WebSearch,
including Indonesia's official Kemenag/3-Ministers-Decree source for 2025):

| Year | Computed new moon (UTC date) | New moon UTC time | "New moon + 1 day" | Actual observed Nyepi |
|---|---|---|---|---|
| 2024 | Mar 10 | (not checked precisely) | **Mar 11** | **Mar 11** ✅ matches |
| 2025 | Mar 29 | 10:57 UTC (18:57 Bali/WITA, UTC+8) | Mar 30 | **Mar 29** ❌ — same day as the new moon, not +1 |
| 2026 | Mar 19 | (not checked precisely) | Mar 20 | **Mar 19** ❌ — same day as the new moon, not +1 |

The "day after Tilem" rule reproduces 2024 but fails both 2025 and 2026, and the 2025 case rules
out a simple timezone fix (UTC-vs-Bali-local re-dating still lands the new moon comfortably within
March 29 Bali time, yet Nyepi is observed on March 29 itself, the new-moon day). Two out of three
recent years contradict the textbook rule, which is exactly the "confident-looking formula that's
actually wrong in most real years" failure mode this project avoids shipping.

## Recommendation for whoever picks this back up

- Indonesia's Nyepi date is set annually by decree (Kemenag / the 3-Ministers Joint Decision on
  national holidays and joint leave), informed by the Balinese Hindu authority (PHDI)'s own Saka
  calendar calculation — not a rule this project has been able to reverse-engineer from the
  public astronomical primitives alone.
- The reused primitive (`LunarPhaseCalculation.NewMoonsInGregorianYear`) is sound; the gap is
  specifically the local calendar-day convention/tie-break at the moment of the new moon, the same
  category of problem documented for Hindu Diwali in Phase 4.
- As with Phase 4, prefer a single sourced statement of the exact rule (if the Bali PHDI/Kemenag
  publishes one — e.g. whether the Balinese calendar day starts at a fixed local time other than
  midnight) over inferring one from a handful of examples, and verify against 5+ years including
  the 2025/2026 counterexamples above before attempting to ship this.
