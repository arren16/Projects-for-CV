#pragma once
#include "Employee.h"

class Manager : public Employee
{
private:
	double _fixedPayment;
	int _totalEmployees;
	double _paymentPerEmployee;
public:
	double fixedPayment() const { return _fixedPayment; }
	int totalEmployees() const { return _totalEmployees; }
	double paymentPerEmployee() const { return _paymentPerEmployee; }
	void setFixedPayment(double value) { _fixedPayment = value; }
	void setTotalEmployees(int value) { _totalEmployees = value; }
	void setPaymentPerEmployee(double value) { _paymentPerEmployee = value; }
public:
	Manager();
	Manager(string, double, int, double);
	~Manager();
public:
	const Manager& operator=(const Manager&);
	Manager(const Manager&);
public:
	string toString();
	void setSalaryData(string);
	double payment();
	Employee* getNewInstance();
};

