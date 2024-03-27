#pragma once
#include <iostream>
#include <string>
#include <map>
using namespace std;

#include "BusinessLogic.h"
class DailyEmployee;
class HourlyEmployee;
class ProductEmployee;
class Manager;

class Employee
{
protected:
	string _fullname;
public:
	string fullname() const { return _fullname; }
	void setFullname(string value) { _fullname = value; }
public: 
	Employee();
	Employee(string);
	~Employee();
public:
	virtual string toString() = 0;
	virtual void setSalaryData(string) = 0;
	virtual double payment() = 0;
	virtual Employee* getNewInstance() = 0;
};

