#include "Manager.h"

Manager::Manager()
{
	_fixedPayment = 0;
	_totalEmployees = 0;
	_paymentPerEmployee = 0;
}

Manager::Manager(string fullname, double fixedPayment, int totalEmployees, double paymentPerEmployee)
{
	this->_fullname = fullname;
	this->_fixedPayment = fixedPayment;
	this->_totalEmployees = totalEmployees;
	this->_paymentPerEmployee = paymentPerEmployee;
}

Manager::~Manager()
{
	// Do nothing
}

const Manager& Manager::operator=(const Manager& other)
{
	this->_fullname = other.fullname();
	this->_totalEmployees = other.totalEmployees();
	return *this;
}

Manager::Manager(const Manager& other)
{
	this->_fullname = other.fullname();
	this->_totalEmployees = other.totalEmployees();
}
 
string Manager::toString()
{
	stringstream builer;
	builer << EmployeeTypeMapper.inversed_typeDictionary[EmployeeType::et_Manager] << ": ";
	builer << this->_fullname << endl;
	builer << _LINE_HEAD_TAB_ << "FixedPayment=" << this->_fixedPayment << "$; ";
	builer << "TotalEmployees=" << this->_totalEmployees << "; ";
	builer << "PaymentPerEmployee=" << this->_paymentPerEmployee << "$" << endl;
	builer << "=> FinalPayment=" << this->payment() << "$";
	return builer.str();
}

void Manager::setSalaryData(string salaryData)
{
	int employeeType = EmployeeType::et_Manager;
	map<string, string> converter = parseSalaryData(salaryData, employeeType);

	if (converter.find("FixedPayment") == converter.end()
		|| converter.find("PaymentPerEmployee") == converter.end()
		|| converter.find("TotalEmployees") == converter.end())
	{
		cout << "Manager::setSalaryData: '";
		cout << salaryData << "' is not a valid format!" << endl;
		exit(0);
	}

	this->_fixedPayment = stod(converter["FixedPayment"]);
	this->_paymentPerEmployee = stod(converter["PaymentPerEmployee"]);
	this->_totalEmployees = stoi(converter["TotalEmployees"]);
}

double Manager::payment()
{
	double payment = _fixedPayment + _totalEmployees * _paymentPerEmployee;
	return payment;
}

Employee* Manager::getNewInstance()
{
	return new Manager;
}


