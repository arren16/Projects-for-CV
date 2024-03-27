#include "Student.h"

string Student::toString()
{
	string gpaNoTrailingZeros = PrecisionUtils::doubleToStringWithTrailingZero(_gpa, 2);

	string tab = (string)_FILE_TAB_;
	string result = "";
	result = result + "Student: " + _id + " - " + _fullname + '\n';
	result = result + tab + "GPA=" + gpaNoTrailingZeros + ", Telephone=" + _telephone + '\n';
	result = result + tab + "Email=" + _email + '\n';
	result = result + tab + "DOB=" + _dob.toString() + '\n';
	result = result + tab + "Address=" + _address;
	return result;
}

Student::Student(string id, string fullname, double gpa,
	string telephone, string email, Date dob, string address)
{
	_id = id;
	_fullname = fullname;
	_gpa = gpa;
	_telephone = telephone;
	_email = email;
	_dob = dob;
	_address = address;
}

Student::Student()
{
	_id = "no-id";
	_fullname = "no-name";
	_gpa = 0;
	_telephone = "no-telephone";
	_email= "no-email";
	// _dob: Date class has it own contructor
	_address = "no-address";
}

Student::~Student()
{
	// Do nothing
}
