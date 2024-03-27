#pragma once
#include "Employee.h"

class DailyEmployee : public Employee
{
private:
	double _totalDays;
	double _dailyPayment;
public:
	double totalDays() const { return _totalDays; }
	double dailyPayment() const { return _dailyPayment; }
	void setTotalDays(double value) { _totalDays = value; }
	void setDailyPayment(double value) { _dailyPayment = value; }
public:
	DailyEmployee();
	DailyEmployee(string, double, double);
	~DailyEmployee();
public:
	string toString();
	void setSalaryData(string);
	double payment();
	Employee* getNewInstance();
};

