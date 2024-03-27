#pragma once
#include<iostream>
#include<cmath>
#include<string>
#include<sstream>
#include "DateUtils.h"
using namespace std;

#define _MIN_YEAR_ 0
#define _MAX_YEAR_ 5000

class Date
{
private:
	int _day;
	int _month;
	int _year;
public:
	Date();
	Date(int, int, int);

	/// <summary>
	/// Construct an instance with the n-th-day of given year
	/// </summary>
	/// <param name="1st param">n</param>
	/// <param name="2nd param">year</param>
	Date(int, int);
	~Date();
public:
	int day() { return _day; }
	int month() { return _month; }
	int year() { return _year; }
	void setDay(int value) { _day = value; }
	void setMonth(int value) { _month = value; }
	void setYear(int value) { _year = value; }
public:
	bool isLeapYear();
	string toString();
};

