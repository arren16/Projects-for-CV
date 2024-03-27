#pragma once
#include <iostream>
#include <vector>
#include <string>
#include <sstream>
#include <memory>
#include <map>
using namespace std;

#define _LINE_HEAD_TAB_ "   "

class EmployeeType
{
public:
	static const int et_Daily = 0;
	static const int et_Hourly = 1;
	static const int et_Product = 2;
	static const int et_Manager = 3;
};

template<class KeyT, class ValT>
map<ValT, KeyT> inverseMap(map<KeyT, ValT>);

struct
{
	map<string, int> typeDictionary = {
		{"DailyEmployee", EmployeeType::et_Daily},
		{"HourlyEmployee", EmployeeType::et_Hourly},
		{"ProductEmployee", EmployeeType::et_Product},
		{"Manager", EmployeeType::et_Manager}
	};
	map<int, string> inversed_typeDictionary
		= inverseMap<string, int>(typeDictionary);
} EmployeeTypeMapper;

string reduceNoNeedWhitespace(string);

/// <summary>
/// E.g: Oct -> October, Jan -> January
/// </summary>
/// <param name="1st param">string monthShorthand</param>
/// <returns>Month string</returns>
string monthFullWritten(string);

/// <summary>
/// Parse string with N-Element format
/// </summary>
/// <param name="1st param">str</param>
/// <param name="2nd param">nElem</param>
/// <param name="3rd param">valueSpecifier</param>
/// <param name="4th param">delim</param>
/// <returns>A map of key-value pairs</returns>
map<string, string> parseNElementString(string, int, string, string);

map<string, string> parseSalaryData(string, int);
map<string, string> parseTypeAndNameData(string);

map<string, string> parseHourlySalaryData(string);
map<string, string> parseDailySalaryData(string);
map<string, string> parseProductSalaryData(string);
map<string, string> parseManagerSalaryData(string);

// ------ IMPLEMENT TEMPLATE-COMPONENTS ------
template<class KeyT, class ValT>
map<ValT, KeyT> inverseMap(map<KeyT, ValT> src)
{
	map<ValT, KeyT> result;

	auto i = src.begin();
	for (; i != src.end(); i++)
		result.insert(pair<ValT, KeyT>(i->second, i->first));

	return result;
}
