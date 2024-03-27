#include "DailyEmployee.h"

DailyEmployee::DailyEmployee()
{
	_totalDays = 0;
	_dailyPayment = 0;
}

DailyEmployee::DailyEmployee(string fullname, double dailyPayment, double totalDays)
{
	this->_fullname = fullname;
	this->_totalDays = totalDays;
	this->_dailyPayment = dailyPayment;
}

DailyEmployee::~DailyEmployee()
{
	// Do nothing
}

string DailyEmployee::toString()
{
	stringstream builder;
	
	builder << EmployeeTypeMapper.inversed_typeDictionary[EmployeeType::et_Daily] << ": ";
	builder << this->_fullname << endl;
	builder << _LINE_HEAD_TAB_ << "DailyPayment=" << this->_dailyPayment << "$; ";
	builder << "TotalDays=" << this->_totalDays << endl;
	builder << "=> FinalPayment=" << this->payment() << "$";

	return builder.str();
}

void DailyEmployee::setSalaryData(string salaryData)
{
	int employeeType = EmployeeType::et_Daily; 
	map<string, string> converter = parseSalaryData(salaryData, employeeType);
	
	if (converter.find("DailyPayment") == converter.end()
		|| converter.find("TotalDays") == converter.end())
	{
		cout << "DailyEmployee::setSalaryData: '";
		cout << salaryData << "' is not a valid format!" << endl;
		exit(0);
	}

	this->_dailyPayment = stod(converter["DailyPayment"]);
	this->_totalDays = stod(converter["TotalDays"]);
}

double DailyEmployee::payment()
{
	double payment = _totalDays * _dailyPayment;
	return payment;
}

Employee* DailyEmployee::getNewInstance()
{
	return new DailyEmployee;
}
