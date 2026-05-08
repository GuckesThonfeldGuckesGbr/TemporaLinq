# Feature Ideas for TemporaLinq

This document outlines potential features that could be added to the TemporaLinq library.

## 1. Working Days Support

- Filter to business days (Mon-Fri) only
- Filter to weekends only
- Support for custom holiday exclusions

## 2. Date Filtering LINQ Extensions

- `WhereMonth(int month)` - filter to a specific month
- `WhereYear(int year)` - filter to a specific year  
- `WhereDay(int day)` - filter to a specific day of month
- `WhereInRange(DateOnly start, DateOnly end)` - filter to a date range

## 3. Date Sequence Generation

- Generate weekly dates (every Monday, etc.)
- Generate monthly dates (1st of each month)
- Generate quarterly dates

## 4. Aggregation Methods

- `CountWeekdays()` - count occurrences of each weekday
- `GetFirstOfMonth()` / `GetLastOfMonth()` - get month boundaries
- `GetMonths()` - get distinct months in the range

## 5. Calendar Support

- Generate dates for a specific culture's holidays
- Support for fiscal quarters