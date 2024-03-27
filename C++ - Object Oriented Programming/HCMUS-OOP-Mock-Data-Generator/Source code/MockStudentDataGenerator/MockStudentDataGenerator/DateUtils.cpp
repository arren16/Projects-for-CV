#include "DateUtils.h"

bool DateUtils::isLeapYear(int year)
{
    bool typeA = (year % 400 == 0);
    bool typeB = (year % 100 != 0 && year % 4 == 0);

    bool isLeap = typeA || typeB;
    return isLeap;
}

int DateUtils::getLastDayOfMonth(int month, int year)
{
    int nDay = 0;
    switch (month)
    {
    case 1: case 3: case 5: case 7: case 8: case 10: case 12:
        nDay = 31;
        break;
    case 2:
        nDay = (isLeapYear(year)) ? 29 : 28;
        break;
    default:
        nDay = 30;
        break;
    }
    return nDay;
}

int DateUtils::getLastDayOfYear(int year)
{
    int lastDay = DateUtils::isLeapYear(year) ? 366 : 365;
    return lastDay;
}