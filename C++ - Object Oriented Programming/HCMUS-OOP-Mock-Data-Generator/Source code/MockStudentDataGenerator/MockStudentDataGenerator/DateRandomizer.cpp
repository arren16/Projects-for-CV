#include "DateRandomizer.h"

/// <summary>
/// Random dates
/// </summary>
/// <param name="nDates">the number of dates</param>
/// <param name="minYear">minimum year accepted</param>
/// <param name="maxYear">maximum year accepted</param>
/// <returns>a vector<Date></returns>
Date DateRandomizer::next(int minYear = _MIN_YEAR_, int maxYear = _MAX_YEAR_)
{
	int year = _numberRdmzr.randIntegerBetween(minYear, maxYear);
	int maxNthDay = DateUtils::getLastDayOfYear(year); // The last day of year
	int minNthDay = 1; // The first day of year

	int dateIndex = _numberRdmzr.randIntegerBetween(minNthDay, maxNthDay);

	Date date(dateIndex, year);
	return date;
}

void DateRandomizer::reseed()
{
	srand((unsigned)time(0));
}