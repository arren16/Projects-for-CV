#include "StudentUtils.h"

string StudentUtils::fullnameToEmailPrefixConverter(string fullname)
{
    string emailPrefix = fullname.substr(0, 1); // Copy 1 letter at index 0
	int fullnameLen = fullname.length();
	
	int i = fullnameLen - 1;
	for (i; i >= 0 ; i--)
	{
		if (fullname[i] == ' ') break;
		emailPrefix.insert(1, fullname.substr(i, 1));
	}

	for (i; i > 0; i--)
	{
		if (fullname[i - 1] == ' ')
			emailPrefix.insert(1, fullname.substr(i, 1));
	}
	emailPrefix =  StringUtils::tolower(emailPrefix);
    return emailPrefix;
}

void StudentUtils::addEndCommaToUnitName(string& unitName)
{
	string endComma = (unitName == "") ? "" : ", ";
	unitName += endComma;
}

vector<string> StudentUtils::getIDList(const vector<Student>& studentList)
{
	vector<string> idList;
	
	auto i = studentList.begin();

	for (i; i != studentList.end(); i++)
		idList.push_back(i->id());

	return idList;
}

double StudentUtils::getAverageGPA(const vector<Student>& studentList)
{
	double averageGPA = 0;
	int nStudent = studentList.size();

	auto i = studentList.begin();
	for (i; i != studentList.end(); i++)
		averageGPA += i->gpa() / (1.0 * nStudent);
	
	return averageGPA;
}

vector<string> StudentUtils::getUnitIDAndNameFromMap(const map<string, vector<string>>& srcMap,
	const string& key, const char& delim, int idCol, int nameCol, int prefixCol)
{
	string id = "";
	string name = "";
	string prefix = "";

	if (srcMap.find(key) != srcMap.end())
	{
		int idx = _numberRdmzr.randIntegerBetween(0, srcMap.at(key).size() - 1);
		vector<string> parseList = StringUtils::parse(srcMap.at(key)[idx], delim);

		id = parseList[idCol];
		name = parseList[nameCol];

		if (0 < prefixCol && prefixCol < srcMap.size() - 1)
			prefix = parseList[prefixCol];
		
		string midSpace = (prefix == "") ? "" : " ";
		name = prefix + midSpace + name;
	}

	return { id, name };
}

