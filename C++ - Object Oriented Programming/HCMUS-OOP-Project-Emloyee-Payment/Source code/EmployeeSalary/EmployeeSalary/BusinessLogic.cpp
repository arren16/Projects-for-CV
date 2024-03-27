#include "BusinessLogic.h"

string reduceNoNeedWhitespace(string src)
{
	src.insert(0, " ");
	src.append(" ");

	
	size_t i = 0;
	size_t strLen = src.length();
	for (; i < strLen - 1; i++)
		if (src[size_t(i) + 1] != ' ') break;
	src = src.substr(size_t(i) + 1);

	i = src.length();
	for (; i > 0; i--)
		if (src[size_t(i) - 1] != ' ') break;
	src = src.substr(size_t(0), size_t(i));

	return src;
}

map<string, string> parseNElementString(string str, int nElem, string valueSpecifier, string delim)
{
	str.append(delim);

	string key = "";
	string value = "";

	map<string, string> result;
	for (int i = 0; i < nElem; i++)
	{
		auto valueSpecifierIdx = str.find(valueSpecifier);
		key = str.substr(0, valueSpecifierIdx);
		str = str.substr(valueSpecifierIdx + 1);

		auto delimIdx = str.find(delim);
		value = str.substr(0, delimIdx);
		str = str.substr(delimIdx + 1);

		key = reduceNoNeedWhitespace(key);
		value = reduceNoNeedWhitespace(value);

		result.insert(pair<string, string>(key, value));
	}

	return result;
}

map<string, string> parseSalaryData(string salaryData, int employeeType)
{
	map<string, string> result;

	switch (employeeType)
	{
	case EmployeeType::et_Daily:
		result = parseDailySalaryData(salaryData);
		break;
	case EmployeeType::et_Hourly:
		result = parseHourlySalaryData(salaryData);
		break;
	case EmployeeType::et_Product:
		result = parseProductSalaryData(salaryData);
		break;
	case EmployeeType::et_Manager:
		result = parseManagerSalaryData(salaryData);
		break;
	default:
		cout << "parseSalaryData: ";
		cout << "Employee type '" << employeeType << "' ";
		cout << "was not predefined in EmployeeFactory::EmployeeType!" << endl;
		break;
	}

	return result;
}

map<string, string> parseHourlySalaryData(string salaryData)
{
	int nElem = 2;
	string valueSpecifier = "=";
	string delim = ";";
	
	map<string, string> converter =
		parseNElementString(salaryData, nElem, valueSpecifier, delim);

	string convertedPayment = converter["HourlyPayment"];
	convertedPayment = convertedPayment.substr(0, convertedPayment.length() - 1);
	converter.at("HourlyPayment") = convertedPayment;

	return converter;
}

map<string, string> parseDailySalaryData(string salaryData)
{
	int nElem = 2;
	string valueSpecifier = "=";
	string delim = ";";

	map<string, string> converter = 
		parseNElementString(salaryData, nElem, valueSpecifier, delim);
	
	string convertedPayment = converter["DailyPayment"];
	convertedPayment = convertedPayment.substr(0, convertedPayment.length() - 1);
	converter.at("DailyPayment") = convertedPayment;

	return converter;
}

map<string, string> parseProductSalaryData(string salaryData)
{
	int nElem = 2;
	string valueSpecifier = "=";
	string delim = ";";

	map<string, string> converter =
		parseNElementString(salaryData, nElem, valueSpecifier, delim);

	string convertedPayment = converter["PaymentPerProduct"];
	convertedPayment = convertedPayment.substr(0, convertedPayment.length() - 1);
	converter.at("PaymentPerProduct") = convertedPayment;

	return converter;
}

map<string, string> parseManagerSalaryData(string salaryData)
{
	int nElem = 3;
	string valueSpecifier = "=";
	string delim = ";";
	
	map<string, string> converter =
		parseNElementString(salaryData, nElem, valueSpecifier, delim);

	string convertedPayment = converter["FixedPayment"];
	convertedPayment = convertedPayment.substr(0, convertedPayment.length() - 1);
	converter.at("FixedPayment") = convertedPayment;

	convertedPayment = converter["PaymentPerEmployee"];
	convertedPayment = convertedPayment.substr(0, convertedPayment.length() - 1);
	converter.at("PaymentPerEmployee") = convertedPayment;

	return converter;
}

map<string, string> parseTypeAndNameData(string typeAndNameData)
{
	int nElem = 1;
	string valueSpecifier = ":";
	string delim = ";";

	map<string, string> converter =
		parseNElementString(typeAndNameData, nElem, valueSpecifier, delim);

	map<string, string> typeAndName = {
		{"EmployeeType", converter.begin()->first},
		{"EmployeeName", converter.begin()->second}
	};

	return typeAndName;
}

string monthFullWritten(string monthShorthand)
{
	string Jan("Jan"), fullJan("January");
	string Feb("Feb"), fullFeb("February");
	string Mar("Mar"), fullMar("March");
	string Apr("Apr"), fullApr("April");
	string May("May"), fullMay("May");
	string Jun("Jun"), fullJun("June");
	string Jul("Jul"), fullJul("July");
	string Aug("Aug"), fullAug("August");
	string Sep("Sep"), fullSep("September");
	string Oct("Oct"), fullOct("October");
	string Nov("Nov"), fullNov("November");
	string Dec("Dec"), fullDec("December");

	map<string, string> fullMonthString = {
		{Jan, fullJan},
		{Feb, fullFeb},
		{Mar, fullMar},
		{Apr, fullApr},
		{May, fullMay},
		{Jun, fullJun},
		{Jul, fullJul},
		{Aug, fullAug},
		{Sep, fullSep},
		{Oct, fullOct},
		{Nov, fullNov},
		{Dec, fullDec}
	};

	string result = "wrong-month";

	auto findItr = fullMonthString.find(monthShorthand);
	if (findItr != fullMonthString.end())
		result = findItr->second;

	return result;
}