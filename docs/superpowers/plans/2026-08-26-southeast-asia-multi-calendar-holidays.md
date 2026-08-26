# Singapore, Malaysia, Indonesia National Holidays — Implementation Plan

## Context

Tier AS1/AS2 remaining countries from the worldwide-holidays design. All three combine
multiple already-implemented calendar mechanisms: `HijriCalendarCalculation`,
`ChineseLunisolarCalendarCalculation`, `EasterSundayCalculation.Christian`, and the newly
merged `SoutheastAsianBuddhistCalendar.VisakhaBuchaDate` (Vesak). Verified via WebSearch
against 2026 official holiday lists (MOM Singapore, Malaysia federal gazette summaries,
Indonesia's SKB joint-ministerial-decree list).

## Enum additions

Add one new `HolidayNames` member, alphabetically ordered, following the existing
"one concept, one member, local-name variants noted in the comment" convention:

- `VesakDay` — comment `// Singapore, Malaysia, Indonesia`

Reused existing members: `EidAlFitr`, `EidAlAdha`, `IslamicNewYear`, `ProphetsBirthday`,
`LunarNewYearsDay`, `GoodFriday`, `AscensionDay`, `LabourDay`, `NewYearsDay`, `ChristmasDay`,
`IndependenceDay`.

## Per-country scope

### Singapore (`Asia/Singapore/NationalHolidays.cs`)
Fixed: New Year's Day (Jan 1), Labour Day (May 1), National Day (Aug 9), Christmas Day (Dec 25).
Computed: Chinese New Year (2 days, `ChineseLunisolarCalendarCalculation`), Good Friday
(`EasterSundayCalculation.Christian` - 2 days), Hari Raya Puasa/Eid al-Fitr (Hijri 10/1),
Hari Raya Haji/Eid al-Adha (Hijri 12/10), Vesak Day (`SoutheastAsianBuddhistCalendar.VisakhaBuchaDate`).
Deferred (documented in XML doc comment): Deepavali (Hindu lunisolar calendar, out of scope).

### Malaysia (`Asia/Malaysia/NationalHolidays.cs`)
Scope: federal/national-level public holidays only (not state-specific Sultan/Governor
birthdays or Federal Territory Day) — same precedent as Germany's federal/state split, but
Malaysia gets national-only since state-level days are numerous and jurisdiction-specific.
Fixed: New Year's Day (Jan 1), Labour Day (May 1), Merdeka/National Day (Aug 31), Malaysia Day
(Sep 16), Christmas Day (Dec 25). Computed: Chinese New Year (2 days), Hari Raya
Puasa/Eid al-Fitr (2 days, Hijri 10/1-2), Hari Raya Haji/Eid al-Adha (Hijri 12/10), Awal
Muharram/Islamic New Year (Hijri 1/1), Maulidur Rasul/ProphetsBirthday (Hijri 3/12), Yang
di-Pertuan Agong's Birthday (first Monday of June, fixed by law since 2018 - formulaic),
Vesak Day. Deferred: Deepavali (Hindu), Sultan's/Governor's Birthday or Federal Territory Day
and other state-specific holidays (jurisdiction-varying, out of scope for the national-only
list this file models).

### Indonesia (`Asia/Indonesia/NationalHolidays.cs`)
Fixed: New Year's Day (Jan 1), Labour Day (May 1), Independence Day (Aug 17), Christmas Day
(Dec 25). Computed: Good Friday, Ascension Day (Easter+39), Chinese New Year/Imlek, Eid
al-Fitr (Hijri 10/1-2), Eid al-Adha (Hijri 12/10), Islamic New Year (Hijri 1/1), Mawlid Nabi
(Hijri 3/12), Vesak/Waisak (`VisakhaBuchaDate`). Deferred: Nyepi (Balinese Saka calendar,
Phase 5), Hindu Deepavali.

## Process

TDD per country: update/write test file expecting the full holiday set first (it will fail
against the not-yet-existing `NationalHolidays` type), then implement, then verify green.
One commit per country, plus one for the enum addition, plus one for the design-doc update.
