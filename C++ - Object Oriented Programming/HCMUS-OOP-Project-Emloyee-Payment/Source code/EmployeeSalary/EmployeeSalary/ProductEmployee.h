#pragma once
#include "Employee.h"

class ProductEmployee : public Employee
{
private:
	int _totalProducts;
	double _paymentPerProduct;
public:
	int totalProducts() const { return _totalProducts; }
	double paymentPerProduct() const { return _paymentPerProduct; }
	void setTotalHours(int value) { _totalProducts = value; }
	void setPaymentPerProduct(double value) { _paymentPerProduct = value; }
public:
	ProductEmployee();
	ProductEmployee(string, int, double);
	~ProductEmployee();
public:
	string toString();
	void setSalaryData(string);
	double payment();
	Employee* getNewInstance();
};

