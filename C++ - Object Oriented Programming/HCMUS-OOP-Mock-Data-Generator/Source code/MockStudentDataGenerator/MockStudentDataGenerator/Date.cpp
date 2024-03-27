#include "Date.h"

Date::Date()
{
    _day = 1;
    _month = 1;
    _year = 1990;
}

Date::Date(int day, int month, int year)
{
    if (year < _MIN_YEAR_ || year > _MAX_YEAR_)
    {
        cout << "Date::Date(int day, int month, int year): ";
        cout << "value = " << day << "/" << month << "/" << year << endl;
        cout << "Year is out-range! ";
        cout << "abs(year) % (" << _MAX_YEAR_ << " + 1)";
        cout << " is automatically assigned!" << endl;
    }
    _year = abs(year) % (_MAX_YEAR_ + 1);

    if (month < 1 || month > 12)
    {
        cout << "Date::Date(int day, int month, int year): ";
        cout << "value = " << day << "/" << month << "/" << year << endl;
        cout << "Month is out-range! ";
        cout << "(abs(month - 1) % 12) + 1 ";
        cout << "is automatically assigned!" << endl;
    }
    _month = abs(month - 1) % 12 + 1;

    int maxDay = DateUtils::getLastDayOfMonth(month, _year);
    if (day < 1 || day > maxDay)
    {
        cout << "Date::Date(int day, int month, int year): ";
        cout << "value = " << day << "/" << month << "/" << year << endl;
        cout << "Day is out-range! ";
        cout << "(abs(day - 1) % lastDayOfMonth) + 1 ";
        cout << "is automatically assigned!" << endl;
    }
    _day = abs(day - 1) % maxDay + 1;
}

Date::Date(int n, int year)
{
    if (year < _MIN_YEAR_ || year > _MAX_YEAR_)
    {
        cout << "Date::Date(int n, int year): ";
        cout << "year = " << year << endl;
        cout << "Year is out-range! ";
        cout << "abs(year) % (" << _MAX_YEAR_ << " + 1)";
        cout << " is automatically assigned!" << endl;
    }
    _year = abs(year) % (_MAX_YEAR_ + 1);

    int maxN = DateUtils::getLastDayOfYear(_year);

    if (n < 1 || n > maxN)
    {
        cout << "Date::Date(int n, int year): ";
        cout << "n = " << n << endl;
        cout << "n is out-range! ";
        cout << "abs(n - 1) % lastDayOfYear + 1";
        cout << " is automatically assigned!" << endl;
    }
    n = abs(n - 1) % maxN + 1;

    int month = 1;
    int maxDay = 31; // Last day of Jan is 31

    while (n > maxDay)
    {
        n = n - maxDay;
        month = month + 1;
        maxDay = DateUtils::getLastDayOfMonth(month, year);
    }
    _day = n;
    _month = month;
}

Date::~Date()
{
    // Do nothing
}

bool Date::isLeapYear()
{
    bool result = DateUtils::isLeapYear(_year);
    return result;
}

string Date::toString()
{
    stringstream builder;

    if (_day < 10) builder << "0";
    builder << _day << "/";

    if (_month < 10) builder << "0";
    builder << _month << "/" << _year;

    string result = builder.str();
    return result;
}
