#pragma once
#include<map>
#include "Student.h"
#include "StringUtils.h"
#include "NumberRandomizer.h"

class StudentUtils
{
private:
	NumberRandomizer _numberRdmzr;
public:
	/// <summary>
	/// Get unit's id and name from map
	/// </summary>
	/// <param name="1st param">srcMap</param>
	/// <param name="2nd param">key</param>
	/// <param name="3rd param">delim</param>
	/// <param name="4th param">idCol</param>
	/// <param name="5th param">nameCol</param>
	/// <returns>string vector of id and name</returns>
	vector<string> getUnitIDAndNameFromMap(const map<string, vector<string>>&, 
		const string&, const char&, int, int, int
	);
public:
	template <class T>
	static bool isIDUsed(const vector<T>&, T);
public:
	static string fullnameToEmailPrefixConverter(string);
	static void addEndCommaToUnitName(string&);
	static vector<string> getIDList(const vector<Student>&);
	static double getAverageGPA(const vector<Student>&);
};

template<class T> 
bool StudentUtils::isIDUsed(const vector<T>& idList, T id)
{
	for (auto it = idList.begin(); it != idList.end(); it++)
	{
		if (*it == id)
			return true;
	}
	return false;
}