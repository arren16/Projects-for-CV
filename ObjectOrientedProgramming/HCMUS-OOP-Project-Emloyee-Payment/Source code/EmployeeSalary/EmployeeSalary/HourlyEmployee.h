#pragma once
#include "Employee.h"

class HourlyEmployee : public Employee
{
private:
	double _totalHours;
	double _hourlyPayment;
public:
	double totalHours() const { return _totalHours; }
	double hourlyPayment() const { return _hourlyPayment; }
	void setTotalHours(double value) { _totalHours = value; }
	void setHourlyPayment(double value) { _hourlyPayment = value; }
public:
	HourlyEmployee();
	HourlyEmployee(string, double, double);
	~HourlyEmployee();
public:
	string toString();
	void setSalaryData(string);
	double payment();
	Employee* getNewInstance();
};

