# Hindu Lunisolar Calendar (Roadmap Phase 4) — Research Notes and Deferral

## Status: deferred, not implemented

This phase was intended to complete India (Hindu-calendar holidays), Nepal (Dashain/Tihar),
Malaysia/Singapore (Deepavali), and Bangladesh (Durga Puja). After substantial research, it is
being deferred — not because the underlying astronomy is uncomputable, but because the specific
**tie-breaking rule** for which calendar day a festival falls on is genuinely contested in real
years, in a way that could not be resolved to a single verifiable formula.

## What was verified and works

The core astronomical primitives are sound and were verified empirically before use:

- **Solar ecliptic longitude** (Meeus, *Astronomical Algorithms* ch. 25, low-precision series):
  verified against known equinox/solstice longitudes (0°/90°/180°/270° at the four cardinal points
  of 2024) to within ~0.5°.
- **Lunar ecliptic longitude** (Meeus ch. 47, full ~60-term periodic series, ported from the same
  `soniakeys/meeus` source used for Phase 1/2): verified against the precise moment of a known full
  moon and new moon (2024-01-25 and 2024-01-11) — the sun-moon longitude difference came out to
  179.98° and 359.98° respectively, matching the expected 180°/0° to within 0.02°.
- **Tithi** (the lunar-day unit the whole Hindu calendar is built from) is simply
  `floor(((moonLongitude - sunLongitude) mod 360°) / 12°) + 1` — directly computable from the two
  verified longitude formulas above, no additional uncertainty here.

## Where it broke down: the tie-breaking rule

Diwali (Lakshmi Puja) is observed on the Amavasya (new-moon) tithi, but a tithi's exact start/end
time drifts across calendar-day boundaries (each tithi lasts roughly 19-26 hours, not a clean 24),
so in many years the Amavasya tithi spans parts of two different Gregorian calendar days. Which of
those two days is "the real Diwali" depends on a specific religious convention for which moment of
the day the tithi must be "prevailing" at (commonly Pradosh Kaal — dusk/early evening — or Nishita
Kaal — midnight — or Udaya Vyapini — sunrise; different conventions in different regional/
denominational traditions).

Two real, independently-documented recent years were checked against multiple candidate rules:

- **2024**: Amavasya tithi ran from Oct 31 06:22 to Nov 1 08:46 (IST). Every reasonable
  "prevailing at evening/dusk," "prevailing at midnight," and "longest-overlap-with-Oct-31-night"
  rule I checked pointed to **Oct 31** — but multiple sources report Diwali was actually observed
  on **Nov 1**.
- **2025**: Amavasya tithi ran from Oct 20 15:44 to Oct 21 17:54 (IST). The "prevailing at dusk"
  rule points to **Oct 20**, which matches most of India's actual observance — but the "prevailing
  at sunrise" rule points to **Oct 21**, and mainstream Indian media (Deccan Herald, Business
  Today, and others) reported genuine city-wise disagreement and public confusion about which date
  was correct, specifically because sunset time varies by city.

No single rule tried reproduced both years correctly, and the 2025 case shows human experts
disagreeing on the ground, not just a modeling gap on this project's part. Shipping a plausible-
looking Diwali calculator that might be wrong in exactly the years this matters most — one of the
most widely observed holidays this library could compute — is a worse outcome than deferring it
honestly.

## Recommendation for whoever picks this back up

- The astronomical primitives above (solar/lunar longitude, tithi) are solid and reusable —
  consider extracting them into `TemporaLinq.Astronomy` (e.g. `SolarPositionCalculation`,
  `LunarPositionCalculation`, `TithiCalculation`) even without a festival-date layer on top, since
  they're independently useful and already verified.
- Before attempting the festival-date layer again, get a clear, single, sourced statement of the
  tie-breaking rule from an authoritative Indian government or major Panchang-publishing
  institution's own published methodology (not news aggregation, which this research found to be
  inconsistent) — and verify it against at least 5-6 years including a repeat of the 2024/2025
  cases above.
- Consider whether India's central government's own official Gazetted-holiday notification (which
  is a single authoritative date per year, since only the "which day is the actual public holiday"
  question matters for this library, not which day is religiously "correct") sidesteps the
  ambiguity entirely — that notification already exists for other Indian holidays used in this
  codebase's India implementation, and checking whether it publishes Diwali's exact date far enough
  in advance to be useful might be a more tractable path than deriving the astrological rule from
  scratch.
