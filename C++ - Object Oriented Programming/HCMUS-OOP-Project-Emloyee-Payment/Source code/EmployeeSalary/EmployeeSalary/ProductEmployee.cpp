#include "ProductEmployee.h"

ProductEmployee::ProductEmployee()
{
	_totalProducts = 0;
	_paymentPerProduct = 0;
}

ProductEmployee::ProductEmployee(string fullname, int totalProducts, double paymentPerProduct)
{
	this->_fullname = fullname;
	this->_totalProducts = totalProducts;
	this->_paymentPerProduct = paymentPerProduct;
}

ProductEmployee::~ProductEmployee()
{
	// Do nothing
}

string ProductEmployee::toString()
{
	stringstream builer;
	builer << EmployeeTypeMapper.inversed_typeDictionary[EmployeeType::et_Manager] << ": ";
	builer << this->_fullname << endl;
	builer << _LINE_HEAD_TAB_ << "PaymentPerProduct=" << this->_paymentPerProduct << "$; ";
	builer << "TotalProducts=" << this->_totalProducts << endl;
	builer << "=> FinalPayment=" << this->payment() << "$";
	return builer.str();
}

void ProductEmployee::setSalaryData(string salaryData)
{
	int employeeType = EmployeeType::et_Product;
	map<string, string> converter = parseSalaryData(salaryData, employeeType);

	if (converter.find("PaymentPerProduct") == converter.end()
		|| converter.find("TotalProducts") == converter.end())
	{
		cout << "ProductEmployee::setSalaryData: '";
		cout << salaryData << "' is not a valid format!" << endl;
		exit(0);
	}

	this->_paymentPerProduct= stod(converter["PaymentPerProduct"]);
	this->_totalProducts = stoi(converter["TotalProducts"]);
}

double ProductEmployee::payment()
{
	double payment = _totalProducts * _paymentPerProduct;
	return payment;
}

Employee* ProductEmployee::getNewInstance()
{
	return new ProductEmployee;
}
