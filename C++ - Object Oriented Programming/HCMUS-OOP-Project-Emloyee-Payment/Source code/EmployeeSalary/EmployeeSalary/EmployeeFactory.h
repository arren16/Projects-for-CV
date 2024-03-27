#pragma once
#include <memory>
#include <map>

#include "BusinessLogic.h"
#include "DailyEmployee.h"
#include "HourlyEmployee.h"
#include "ProductEmployee.h"
#include "Manager.h"

class EmployeeFactory
{
private:
	static shared_ptr<EmployeeFactory> _instance;
	map<int, Employee*> _prototypes;
	EmployeeFactory();
public:
	~EmployeeFactory();
public:
	static shared_ptr<EmployeeFactory> getInstance();
	Employee* createEmployee(int);
};


