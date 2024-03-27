#pragma once
#include "Date.h"

class DateUtils
{
public:
	static bool isLeapYear(int);

	/// <summary>
	/// Get last day of given month
	/// </summary>
	/// <param name="1st param">month</param>
	/// <param name="2nd param">year</param>
	/// <returns>Last day of given month</returns>
	static int getLastDayOfMonth(int, int);
	static int getLastDayOfYear(int);
};

