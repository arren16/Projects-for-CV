#pragma once
#include<iostream>
#include<string>
#include<iomanip>
#include<sstream>
#include "DateRandomizer.h"
#include "PrecisionUtils.h"
#include "FileUtils.h"

using namespace std;

class Student
{
private:
	string _id;
	string _fullname;
	double _gpa;
	string _telephone;
	string _email;
	Date _dob;
	string _address;
public:
	string id() const { return _id; }
	string fullname() const { return _fullname; }
	double gpa() const { return _gpa; }
	string telephone() const { return _telephone; }
	string email() const { return _email; }
	Date dob() const { return _dob; }
	string address() const { return _address; }
public:
	void setID(string value) { _id = value; }
	void setFullname(string value) { _fullname = value; }
	void setGPA(double value) { _gpa = value; }
	void setTelephone(string value) { _telephone = value; }
	void setEmail(string value) { _email = value; }
	void setDOB(Date value) { _dob = value; }
	void setAddress(string value) { _address = value; }
public:
	Student(string, string, double, string, string, Date, string);
	Student();
	~Student();
public:
	string toString();
};

