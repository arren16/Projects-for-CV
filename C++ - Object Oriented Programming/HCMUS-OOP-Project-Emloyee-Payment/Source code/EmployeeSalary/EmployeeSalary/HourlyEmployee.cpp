#include "HourlyEmployee.h"

HourlyEmployee::HourlyEmployee()
{
	_totalHours = 0;
	_hourlyPayment = 0;
}

HourlyEmployee::HourlyEmployee(string fullname, double totalHours, double hourlyPayment)
{
	this->_fullname = fullname;
	this->_totalHours = totalHours;
	this->_hourlyPayment = hourlyPayment;
}

HourlyEmployee::~HourlyEmployee()
{
	// Do nothing
}

string HourlyEmployee::toString()
{
	stringstream builer;
	builer << EmployeeTypeMapper.inversed_typeDictionary[EmployeeType::et_Hourly] << ": ";
	builer << this->_fullname << endl;
	builer << _LINE_HEAD_TAB_ << "HourlyPayment=" << this->_hourlyPayment << "$; ";
	builer << "TotalHours=" << this->_totalHours << endl;
	builer << "=> FinalPayment=" << this->payment() << "$";
	return builer.str();
}

void HourlyEmployee::setSalaryData(string salaryData)
{
	int employeeType = EmployeeType::et_Hourly;
	map<string, string> converter = parseSalaryData(salaryData, employeeType);

	if (converter.find("HourlyPayment") == converter.end()
		|| converter.find("TotalHours") == converter.end())
	{
		cout << "HourlyEmployee::setSalaryData: '";
		cout << salaryData << "' is not a valid format!" << endl;
		exit(0);
	}

	this->_hourlyPayment= stod(converter["HourlyPayment"]);
	this->_totalHours= stod(converter["TotalHours"]);
}

double HourlyEmployee::payment()
{
	double payment = _totalHours * _hourlyPayment;
	return payment;
}

Employee* HourlyEmployee::getNewInstance()
{
	return new HourlyEmployee;
}
