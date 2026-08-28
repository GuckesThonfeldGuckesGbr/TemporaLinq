# Country coverage

Every country below has a `NationalHolidays` class at
`TemporaLinq.Holidays/<Continent>/<Country>/NationalHolidays.cs`. A few also
have a `StateHolidays.cs` with regional variants (currently: Germany,
USA — check the folder for others as coverage grows).

"Mechanism" lists which [calendar calculation](calendar-calculations.md)(s)
the country's holidays depend on, beyond fixed dates. "Fixed/Easter" means
the country needs nothing beyond fixed civil dates and
`EasterSundayCalculation` — no non-Gregorian calendar math at all.

## Europe

| Country | Mechanism | Notes |
|---|---|---|
| Albania | Fixed/Easter, Hijri | |
| Andorra | Fixed/Easter | |
| Austria | Fixed/Easter | |
| Belgium | Fixed/Easter | |
| Bosnia and Herzegovina | Fixed | State/BiH-wide holidays only (New Year, Labour Day) — entity-specific and community-religious holidays (including the Eids) are out of scope |
| Bulgaria | Fixed/Easter | |
| Croatia | Fixed/Easter | |
| Cyprus | Fixed/Easter | |
| Czech Republic | Fixed/Easter | |
| Denmark | Fixed/Easter | |
| Estonia | Fixed/Easter | |
| Finland | Fixed/Easter | |
| France | Fixed/Easter | |
| Germany | Fixed/Easter | Has `StateHolidays.cs` for all 16 states |
| Greece | Fixed/Easter | |
| Hungary | Fixed/Easter | |
| Iceland | Fixed/Easter | |
| Ireland | Fixed/Easter | |
| Italy | Fixed/Easter | |
| Kosovo | Fixed/Easter, Hijri | |
| Latvia | Fixed/Easter | |
| Liechtenstein | Fixed/Easter | |
| Lithuania | Fixed/Easter | |
| Luxembourg | Fixed/Easter | |
| Malta | Fixed/Easter | |
| Moldova | Fixed/Easter | |
| Monaco | Fixed/Easter | |
| Montenegro | Fixed | |
| Netherlands | Fixed/Easter | |
| North Macedonia | Fixed/Easter | |
| Norway | Fixed/Easter | |
| Poland | Fixed/Easter | |
| Portugal | Fixed/Easter | |
| Romania | Fixed/Easter | |
| San Marino | Fixed/Easter | |
| Serbia | Fixed/Easter | |
| Slovakia | Fixed/Easter | |
| Slovenia | Fixed/Easter | |
| Spain | Fixed/Easter | |
| Sweden | Fixed/Easter | |
| Switzerland | Fixed/Easter | |
| Ukraine | Fixed/Easter | |
| United Kingdom | Fixed/Easter | |
| Vatican City | Fixed/Easter | |

Not yet implemented: Belarus, and the rest of Europe not listed above.

## Asia

| Country | Mechanism | Notes |
|---|---|---|
| Bangladesh | Hijri | Hijri-based and fixed civil holidays only; Hindu/Buddhist minority holidays (Durga Puja, Buddha Purnima) deferred — see [Known gaps](known-gaps.md) |
| Cambodia | Southeast Asian Buddhist lunisolar | Meak Bochea (dropped from Cambodia's statutory list since 2020), Asalha Bucha (never statutory there), Pchum Ben, the Water Festival, and the Royal Ploughing Ceremony are out of scope |
| China | Chinese lunisolar | |
| Hong Kong | Fixed/Easter, Chinese lunisolar | |
| India | Fixed/Easter, Hijri | Central Gazetted list only (Republic Day, Independence Day, Gandhi Jayanti, Good Friday, Christmas, Hijri-computable Eids/Muharram/Milad-un-Nabi); Hindu-calendar holidays and state-specific days deferred — see [Known gaps](known-gaps.md) |
| Indonesia | Fixed/Easter, Hijri, Chinese lunisolar, Southeast Asian Buddhist lunisolar | Nyepi (Balinese Saka calendar) and Hindu Deepavali deferred — see [Known gaps](known-gaps.md) |
| Iran | Hijri, Persian | |
| Iraq | Hijri | Sunni/Shia moon-sighting authorities occasionally differ by a day, on top of the usual Hijri ±1-2 day variance |
| Israel | Hebrew | |
| Kuwait | Hijri | |
| Laos | Fixed | No statutory Buddhist-calendar holiday at all (Labour Law 2013, Art. 55) — Visakha Bousa/Boun Khao Phansa/Boun Ok Phansa are culturally observed but not statutory |
| Malaysia | Chinese lunisolar, Hijri, Southeast Asian Buddhist lunisolar | Federal/national level only; Hindu Deepavali and state-specific holidays deferred |
| Mongolia | Mongolian/Tibetan | See the [Mongolian calculation's accuracy note](calendar-calculations.md#mongolian--tibetan-mongoliancalendarcalculation) — ±1 day on some Tsagaan Sar dates |
| Myanmar | Southeast Asian Buddhist lunisolar | |
| Pakistan | Hijri | |
| Qatar | Hijri | |
| Saudi Arabia | Hijri | |
| Singapore | Fixed/Easter, Chinese lunisolar, Hijri, Southeast Asian Buddhist lunisolar | Hindu Deepavali deferred |
| South Korea | Korean lunisolar | |
| Sri Lanka | Fixed/Easter, Hijri, Lunar phase | Full-moon-computed Poya holidays; Maha Sivarathri (a Hindu lunar holiday) deferred |
| Taiwan | Taiwan lunisolar | |
| Thailand | Southeast Asian Buddhist lunisolar | |
| Turkey | Fixed, Hijri | |
| UAE | Hijri | |
| Uzbekistan | Hijri | |
| Vietnam | Chinese lunisolar | Approximate — computed for China's UTC+8 rather than Vietnam's own UTC+7 timezone; in rare years a new moon near the day boundary between the two can shift the result a full lunar month off |

Not yet implemented: Japan, Philippines, Kazakhstan, remaining Central Asia,
Nepal (see [Known gaps](known-gaps.md)).

## Africa

| Country | Mechanism | Notes |
|---|---|---|
| Egypt | Fixed/Easter, Hijri | |
| Ethiopia | Fixed/Easter, Ethiopian | |
| Morocco | Hijri | |
| Nigeria | Fixed/Easter, Hijri | |

Not yet implemented: South Africa, Kenya, Ghana, and the rest of Africa.

## North America

| Country | Mechanism | Notes |
|---|---|---|
| Haiti | Fixed/Easter | Government-decreed holiday shifts and one-off commemorative days out of scope |
| USA | Fixed | Has `StateHolidays.cs` |

Not yet implemented: Canada, Mexico, and the rest of North America/Caribbean.

## South America

| Country | Mechanism | Notes |
|---|---|---|
| Venezuela | Fixed/Easter | "Puente" decree-based holiday shifts out of scope |

Not yet implemented: Brazil, Argentina, Chile, Colombia, and the rest of
South America.

## Oceania

Not yet implemented.
