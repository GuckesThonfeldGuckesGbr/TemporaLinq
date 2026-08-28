# Known gaps

This library's rule is: compute holidays with a real formula, never a
hand-maintained per-year lookup table (see
[Calendar calculations](calendar-calculations.md)). Three holidays were
investigated and deliberately left unimplemented because that rule couldn't
be satisfied honestly — not because nobody got around to them. Each is
summarized below; the linked research notes have the full investigation,
including the specific real-world dates checked.

## Nepal's civil calendar (Bikram Sambat)

Nepal's official civil calendar has no closed-form formula. Every known
implementation — including professional ones — relies on a pre-computed
per-year month-length table published by Nepal's own calendar authority.
Embedding such a table would be a first exception to this project's
formula-first approach, so it's deferred pending an explicit decision to do
that. Nepal's Hindu-calendar holidays (Dashain, Tihar) are a separate
question and may still become computable independently — see below.

Full notes: `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`.

## Hindu lunisolar festival dates (Diwali, Holi, etc.)

The underlying astronomy (solar/lunar ecliptic longitude, and the tithi —
lunar day — they derive) was implemented and verified to within ~0.02° of
known reference events. The blocker is the **tie-breaking rule**: a tithi
spans parts of two Gregorian days more often than not, and which day is "the"
festival day depends on a religious convention (Pradosh Kaal/dusk, Nishita
Kaal/midnight, Udaya Vyapini/sunrise) that varies by region and tradition.

Checked against two real years (2024, 2025): every rule tried reproduced at
most one of the two — in 2025, mainstream Indian media reported genuine
city-wise disagreement about which day was correct, because sunset time
varies by city. This isn't a modeling gap on this project's part; human
experts disagree on the ground in some years. Affects India, Nepal
(Dashain/Tihar), and the Hindu-minority holidays of
Malaysia/Singapore/Bangladesh/Indonesia (Deepavali).

Full notes: `docs/superpowers/specs/2026-08-27-hindu-lunisolar-calendar-research-notes.md`.

## Indonesia's Nyepi (Balinese Saka calendar)

The textbook rule — Nyepi is the day after the new moon (Tilem) of the 9th
Balinese month — reuses this project's already-verified new-moon
calculation, and reproduces 2024 exactly. It fails both 2025 and 2026: the
officially decreed Nyepi date both years was the *same day* as the new moon,
not the day after, and the 2025 case rules out a simple timezone fix. Same
underlying failure mode as the Hindu tithi problem above: a local
calendar-day tie-break at a lunar event's exact moment, not a gap in the
astronomy.

Full notes: `docs/superpowers/specs/2026-08-27-nyepi-balinese-saka-research-notes.md`.

---

If you need one of these for a specific year and can source the actual
decreed date from an authoritative body (a government gazette, or the
relevant religious authority's own published calendar), that's a value you
can inject yourself alongside this library's computed holidays — this
library just won't guess for you.
