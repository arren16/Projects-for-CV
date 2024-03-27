#include "EmployeeFactory.h"

shared_ptr<EmployeeFactory> EmployeeFactory::getInstance()
{
	if (_instance == NULL)
	{
		_instance = shared_ptr<EmployeeFactory>(new EmployeeFactory);
	}
	return _instance;
}

Employee* EmployeeFactory::createEmployee(int type)
{
	return _prototypes[type]->getNewInstance();
}

EmployeeFactory::EmployeeFactory()
{
	_prototypes.insert(pair<int, Employee*>(EmployeeType::et_Daily, new DailyEmployee));
	_prototypes.insert(pair<int, Employee*>(EmployeeType::et_Hourly, new HourlyEmployee));
	_prototypes.insert(pair<int, Employee*>(EmployeeType::et_Product, new ProductEmployee));
	_prototypes.insert(pair<int, Employee*>(EmployeeType::et_Manager, new Manager));
}

EmployeeFactory::~EmployeeFactory()
{
	auto i = _prototypes.begin();
	for (; i != _prototypes.end(); i++)
		delete i->second;
	_prototypes.clear();
}
