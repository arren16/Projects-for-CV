#pragma once
#include "Student.h"
#include "DateRandomizer.h"
#include "NumberRandomizer.h"
#include "FileUtils.h"
#include "ProbabilityRandomizer.h"
#include "StudentUtils.h"

class StudentRandomizer
{
private:
	DateRandomizer _dateRdmzr;
	NumberRandomizer _numberRdmzr;
	ProbabilityRandomizer _probabilityRdmzr;
	StudentUtils _studentUtils;
private:
	vector<string> _yearCodeList;
	vector<string> _facultyCodeList;
	vector<string> _programCodeList;
private:
	vector<tuple<string, double>> _firstNameDistribution;
	vector<string> _middleName;
	vector<string> _lastName;
private:
	vector<string> _telePrefixList;
	vector<tuple<string, double>> _emailDomainDistribution;
private:
	map<string, vector<string>> _provinceList;
	map<string, vector<string>> _districtList;
	map<string, vector<string>> _wardList;
	map<string, vector<string>> _streetList;
	map<string, vector<string>> _projectList;
	vector<string> _houseNumberList;
public:
	StudentRandomizer();
public:
	void reseed();
public:
	Student next();
	void next(vector<Student>&, int, vector<string>&);
public:
	/// <summary>
	/// Random student ID
	/// </summary>
	/// <param name="1st">ID lenght</param>
	/// <returns>student ID</returns>
	string nextID(int, Date);

	/// <summary>
	/// Random student ID without duplicate ID(s)
	/// </summary>
	/// <param name="1st">idLen</param>
	/// <param name="2nd">dob</param>
	/// <param name="3rd">usedIDs</param>
	/// <returns>student ID</returns>
	string nextID(int, Date, const vector<string>&);
	
	string nextFullname();
	double nextGPA();
	string nextTelephone();
	string nextEmail(string);
	Date nextDOB();
	string nextAddress();
};

