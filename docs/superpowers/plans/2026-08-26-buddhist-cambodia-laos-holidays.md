# Cambodia and Laos Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for Cambodia and Laos to `TemporaLinq.Holidays`, using the newly-merged `TemporaLinq.Astronomy.SoutheastAsianBuddhistCalendar` (Makha/Visakha/Asalha Bucha) plus fixed civil holidays, following the exact pattern already used by Sri Lanka/Turkey/other Asia countries.

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs`, computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`). Cambodia uses `SoutheastAsianBuddhistCalendar.VisakhaBuchaDate` for Visak Bochea Day. Laos, per research below, has **no** statutory Buddhist-calendar holiday at all — its fixed civil list is the entire implementation. Each country also gets a test file at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs` following the existing `SriLankaTest`/`TurkeyTest` pattern, tested against year 2026.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, `docs/superpowers/specs/2026-08-26-southeast-asian-buddhist-calendar-design.md`

## Research findings (WebSearch, 2026-08-26)

### Cambodia
Cambodia's Royal Government publishes an annual Sub-Decree fixing the statutory paid-holiday
calendar (e.g. Sub-Decree No. 167, 18 Sept 2025, for 2026 — 21 paid days). Cross-checking
multiple aggregator/law-firm sources for the 2026 list converges on:

New Year's Day (Jan 1), Victory over Genocide Day (Jan 7), International Women's Day (Mar 8),
Khmer New Year (Apr 14-16, 3 days — a solar-calendar festival, fixed Gregorian dates per the same
convention already used for Sri Lanka's Sinhala/Tamil New Year, not computed from a formula),
International Labour Day (May 1), Visak Bochea Day (Buddhist lunar, computed), King Norodom
Sihamoni's Birthday (May 14), Queen Mother Norodom Monineath's Birthday (Jun 18), Constitution Day
(Sep 24), Commemoration Day of King Father Norodom Sihanouk (Oct 15), King Norodom Sihamoni's
Coronation Day (Oct 29), National Independence Day (Nov 9), National Peace Day (Dec 29).

**Out of scope, confirmed by research, documented on the class:**
- **Meak Bochea** (Makha Bucha) — was on Cambodia's official calendar through 2019 but was
  removed from the statutory list starting 2020 and has not returned since. Not implemented
  because it is not currently an official holiday, not because it's uncomputable.
- **Asalha Bucha** — never on Cambodia's statutory civil-servant/worker holiday list.
- **Pchum Ben** (Ancestors' Day, 3 days, Oct) — a distinct Khmer lunar-calendar festival that does
  not map onto Makha/Visakha/Asalha Bucha; per the task's out-of-scope guidance, not computed.
- **Water Festival / Bon Om Touk** (3 days, Nov) — tied to a full moon of the traditional Khmer
  calendar (12th lunar month) but not one of the three Buddhist holy days this project can
  compute; left out of scope.
- **Royal Ploughing Ceremony** (May) — date is set by royal astrologers each year, not a fixed
  formula; left out of scope.

Visak Bochea Day itself is computed via `SoutheastAsianBuddhistCalendar.VisakhaBuchaDate`. Some
low-quality aggregators report Cambodia's official 2026 Visak Bochea date as coinciding with
Labour Day (May 1) or various other dates in May, inconsistent with each other and with
Thailand's independently-confirmed 2026 Vesak (May 31). This is the same kind of
moon-sighting/local-authority variance already documented for Hijri-based holidays elsewhere in
this codebase (e.g. Turkey's bayram caveat) — Cambodia's own religious/government authority may
publish a date that differs from the astronomical calculation by more than the usual +/-1 day.
The astronomical calculation is used here, with the variance documented as a caveat on the class,
consistent with existing precedent (Iraq, Turkey) rather than trusting an unverifiable secondhand
aggregator date.

### Laos
Laos's Labour Law (2013, No. 43/NA), Article 55, is the authoritative enumeration of statutory
holidays: National Day (Dec 2), International New Year (Jan 1), International Women's Day (Mar 8,
restricted to female employees), Lao New Year festival (3 days), International Labour Day (May 1),
and National Teachers' Day (Oct 7, restricted to teachers/education staff). This is corroborated
by multiple independent sources (embassy-adjacent summaries, legal-compliance sites) converging on
the same short list, and by explicit statements that Visakha Bousa/Boun Khao Phansa/Boun Ok Phansa
— the same Buddhist holy days Thailand/Myanmar/Cambodia observe as statutory holidays — are
**widely observed culturally in Laos but are not part of its statutory public holiday law**.

**Out of scope, confirmed by research, documented on the class:**
- **Visakha Bousa (Vesak), Boun Khao Phansa (Asalha Bucha equivalent), Boun Ok Phansa** — not
  statutory holidays in Laos despite being the same computable Buddhist calendar this project can
  compute; not implemented because research shows they are not official holidays here (contrast
  with Cambodia, where Visak Bochea *is* on the statutory list).
- **National Teachers' Day** (Oct 7) — restricted to teachers/education staff, not a
  general-population holiday, consistent with how this codebase omits Cambodia's Royal Ploughing
  Ceremony and similar narrow-scope observances.

Net result: Laos's `NationalHolidays` is fixed-civil-only — New Year's Day, International Women's
Day, Lao New Year (Apr 14-16, 2026 dates, same fixed-date convention as Khmer/Sinhala-Tamil New
Year), Labour Day, Lao National Day.

## Global Constraints

- Countries live at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs`; tests at
  `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs`.
- Reuse existing `HolidayNames` enum members wherever the concept matches (`NewYearsDay`,
  `InternationalWomensDay`, `LabourDay`, `IndependenceDay`), broadening their `//` comment to list
  the additional country. Only add new enum members for genuinely new concepts.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task.
- After both countries are done, update the checklist in
  `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, moving Cambodia and Laos out of
  the Tier AS4 🔴 list into a "done" line, noting the out-of-scope lunar festivals.

---

## Reference: full holiday list per country

### Cambodia (`SoutheastAsianBuddhistCalendar.VisakhaBuchaDate`) — 15 fixed/computed holidays (+3 Khmer New Year days)
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 7 | `VictoryOverGenocideDay` (new) |
| Mar 8 | `InternationalWomensDay` |
| Apr 14 | `KhmerNewYear` (new) |
| Apr 15 | `KhmerNewYear` |
| Apr 16 | `KhmerNewYear` |
| May 1 | `LabourDay` |
| VisakhaBuchaDate(year) | `VisakBocheaDay` (new) |
| May 14 | `BirthdayOfKingNorodomSihamoni` (new) |
| Jun 18 | `BirthdayOfQueenMotherNorodomMonineath` (new) |
| Sep 24 | `ConstitutionDayOfCambodia` (new) |
| Oct 15 | `CommemorationDayOfKingFatherNorodomSihanouk` (new) |
| Oct 29 | `CoronationDayOfKingNorodomSihamoni` (new) |
| Nov 9 | `IndependenceDay` (reuse) |
| Dec 29 | `PeaceDayOfCambodia` (new) |

### Laos (no Buddhist-calendar component — see research findings) — 7 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Mar 8 | `InternationalWomensDay` |
| Apr 14 | `LaoNewYear` (new) |
| Apr 15 | `LaoNewYear` |
| Apr 16 | `LaoNewYear` |
| May 1 | `LabourDay` |
| Dec 2 | `LaoNationalDay` (new) |

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

**Steps:**
- [ ] Insert new members in alphabetical position among existing neighbors (do not reorder
      existing entries): `BirthdayOfKingNorodomSihamoni`, `BirthdayOfQueenMotherNorodomMonineath`,
      `CommemorationDayOfKingFatherNorodomSihanouk`, `ConstitutionDayOfCambodia`,
      `CoronationDayOfKingNorodomSihamoni`, `KhmerNewYear`, `LaoNationalDay`, `LaoNewYear`,
      `PeaceDayOfCambodia`, `VictoryOverGenocideDay`, `VisakBocheaDay` — each with a `// Cambodia`
      or `// Laos` comment.
- [ ] Broaden existing comments: `IndependenceDay` gains `, Cambodia`; `LabourDay` gains
      `, Cambodia, Laos`.
- [ ] `dotnet build` succeeds.
- [ ] Commit: `feat: add Cambodia and Laos HolidayNames enum members`.

## Task 2: Cambodia national holidays (TDD)

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Cambodia/NationalHolidays.cs`
- Create: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/CambodiaTest.cs`

**Steps:**
- [ ] Write failing test `CambodiaTest.cs`: asserts 2026 fixed dates (Jan 1, Jan 7, Mar 8, Apr
      14-16, May 1, May 14, Jun 18, Sep 24, Oct 15, Oct 29, Nov 9, Dec 29) each with the correct
      `HolidayNames` member, plus a test that Visak Bochea Day equals
      `SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(2026)`, plus a count test for the total
      number of holidays in 2026 (15 fixed + Khmer New Year already counted = 15 total distinct
      dates, all single-occurrence).
- [ ] Run `dotnet test --framework net10.0 --filter CambodiaTest` — confirm compile failure / red
      (type doesn't exist yet).
- [ ] Implement `NationalHolidays.cs` per the reference table above, with an XML doc comment
      documenting the out-of-scope items (Meak Bochea removed since 2020, Asalha Bucha never
      statutory, Pchum Ben, Water Festival, Royal Ploughing Ceremony) and the Visak Bochea
      computed-vs-published-date caveat.
- [ ] Run `dotnet test --framework net10.0 --filter CambodiaTest` — confirm green.
- [ ] Commit: `feat: add Cambodia national holidays`.

## Task 3: Laos national holidays (TDD)

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Laos/NationalHolidays.cs`
- Create: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/LaosTest.cs`

**Steps:**
- [ ] Write failing test `LaosTest.cs`: asserts 2026 fixed dates (Jan 1, Mar 8, Apr 14-16, May 1,
      Dec 2) each with the correct `HolidayNames` member, plus a count test (7 total).
- [ ] Run `dotnet test --framework net10.0 --filter LaosTest` — confirm red.
- [ ] Implement `NationalHolidays.cs` per the reference table above, with an XML doc comment
      documenting that Visakha Bousa/Boun Khao Phansa/Boun Ok Phansa and National Teachers' Day are
      out of scope (not statutory / restricted-population respectively), per Article 55 of the 2013
      Labour Law.
- [ ] Run `dotnet test --framework net10.0 --filter LaosTest` — confirm green.
- [ ] Commit: `feat: add Laos national holidays`.

## Task 4: Update spec checklist and full-suite verification

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

**Steps:**
- [ ] In the Asia section, remove Cambodia and Laos from the Tier AS4 🔴 list and add a "Done"
      line noting them, with the out-of-scope-lunar-festival caveats summarized inline (matching
      the style of the India/Bangladesh/Sri Lanka "done" lines).
- [ ] Run `dotnet test --framework net10.0` (full suite) — confirm 0 failures.
- [ ] Commit: `docs: mark Cambodia and Laos done in worldwide holidays checklist`.

## Out of scope for this plan

- Thailand, Myanmar (separate country tiers per the calendar design doc's country scope section).
- Adding Vesak to Singapore/Malaysia/Indonesia (separate task in the same design doc).
- Any further refinement of `SoutheastAsianBuddhistCalendar` itself — it is treated as a verified,
  trusted building block per this task's instructions.
