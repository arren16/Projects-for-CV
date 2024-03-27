#include "StudentRandomizer.h"

StudentRandomizer::StudentRandomizer()
{
	_yearCodeList = FileUtils::readOneElementLineType<string>(_ID_YEAR_CODE_DIR_);
	_facultyCodeList = FileUtils::readOneElementLineType<string>(_ID_FACULTY_CODE_);
	_programCodeList = FileUtils::readOneElementLineType<string>(_ID_PROGRAM_CODE_);

	_firstNameDistribution = FileUtils::FileUtils::readTwoElementLineType<string, double>(_FIRST_NAME_DIR_);
	_middleName = FileUtils::readOneElementLineType<string>(_MIDDLE_NAME_DIR_);
	_lastName = FileUtils::readOneElementLineType<string>(_LAST_NAME_DIR_);
	
	_telePrefixList = FileUtils::readOneElementLineType<string>(_TELEPHONE_PREFIX_DIR_);
	
	_emailDomainDistribution =
		FileUtils::readTwoElementLineType<string, double>(_EMAIL_DOMAIN_DIR_);

	vector<int> provinceClassifyColumn = { 0 }; // provinceID
	vector<int> districtClassifyColumn = { 3 }; // provinceID
	vector<int> wardClassifyColumn = { 3, 4 }; // provinceID && districtID
	vector<int> streetClassifyColumn = { 3, 4 }; // provinceID && districtID 
	vector<int> projectClassifyColumn = { 2, 3 }; // provinceID && districtID

	const char delim = '|';
	FileUtils::readNElementLineType(_PROVINCES_DIR_, delim, _PROVINCES_IGNORED_LINES_, provinceClassifyColumn, _provinceList);
	FileUtils::readNElementLineType(_DISTRICTS_DIR_, delim, _DISTRICTS_IGNORED_LINES_, districtClassifyColumn, _districtList);
	FileUtils::readNElementLineType(_WARDS_DIR_, delim, _WARDS_IGNORED_LINES_, wardClassifyColumn, _wardList);
	FileUtils::readNElementLineType(_STREETS_DIR_, delim, _STREETS_IGNORED_LINES_, streetClassifyColumn, _streetList);
	FileUtils::readNElementLineType(_PROJECTS_DIR_, delim, _PROJECTS_IGNORED_LINES_, projectClassifyColumn, _projectList);
	_houseNumberList = FileUtils::readOneElementLineType<string>(_HOUSE_NUMBER_DIR_);
}

void StudentRandomizer::reseed()
{
	srand((unsigned)time(0));
}

string StudentRandomizer::nextID(int idLen, Date dob)
{
	int yearCodeIdx = _numberRdmzr.randIntegerBetween(0, _yearCodeList.size() - 1);
	int facultyCodeIdx = _numberRdmzr.randIntegerBetween(0, _facultyCodeList.size() - 1);
	int programCodeIdx = _numberRdmzr.randIntegerBetween(0, _programCodeList.size() - 1);
	
	// Student born in 2002 is K20
	int minYearCode = dob.year() - 2002 + 20; 
	string yearCode = _yearCodeList[yearCodeIdx];
	yearCode = stoi(yearCode) < minYearCode 
		? to_string(minYearCode) : yearCode;

	string facultyCode = _facultyCodeList[facultyCodeIdx];
	string programCode = _programCodeList[programCodeIdx];
	
	string id = "";
	id.append(yearCode);	
	id.append(facultyCode);
	id.append(programCode);

	int zero = '0';
	int nine = '9';
	
	int fillIdx = (yearCode + facultyCode + programCode).length();
	for (int i = fillIdx; i < idLen; i++)
	{
		char ch = _numberRdmzr.randIntegerBetween(zero, nine);
		id.push_back(ch);
	}

	return id;
}

Student StudentRandomizer::next()
{
	Date dob = nextDOB();
	string id = nextID(8, dob);
	string fullname = nextFullname();
	double gpa = nextGPA();
	string telephone = nextTelephone();
	string email = nextEmail(fullname);
	string address = nextAddress();
	
	Student student(id, fullname, gpa, telephone, email, dob, address);
	return student;
}

void StudentRandomizer::next(vector<Student>& studentList, int nStudent, vector<string>& usedIDList)
{
	for (int i = 0; i < nStudent; i++)
	{
		Date dob = nextDOB();
		string id = nextID(8, dob, usedIDList);
		string fullname = nextFullname();
		double gpa = nextGPA();
		string telephone = nextTelephone();
		string email = nextEmail(fullname);
		string address = nextAddress();

		Student student(id, fullname, gpa, telephone, email, dob, address);
		studentList.push_back(student);
		usedIDList.push_back(id);
	}
}

string StudentRandomizer::nextID(int idLen, Date dob, const vector<string>& usedIDs)
{
	string id = nextID(idLen, dob);

	while (StudentUtils::isIDUsed<string>(usedIDs, id))
		id = nextID(idLen, dob);
	
	return id;
}
string StudentRandomizer::nextFullname()
{
	int midIdx = _numberRdmzr.randIntegerBetween(0, _middleName.size() - 1);
	int lastIdx = _numberRdmzr.randIntegerBetween(0, _lastName.size() - 1);

	string fullname = _probabilityRdmzr.randomElementWithProbability(_firstNameDistribution)
		+ " " + _middleName[midIdx] 
		+ " " + _lastName[lastIdx];

	return fullname;
}

double StudentRandomizer::nextGPA()
{
	double gpa = _numberRdmzr.randRealBetween(0, 10);
	
	stringstream converter;
	converter << setprecision(2) << gpa;

	gpa = stod(converter.str());
	return gpa;
}

string StudentRandomizer::nextTelephone()
{
	int nPrefix = _telePrefixList.size() - 1;
	int telePrefixIdx = _numberRdmzr.randIntegerBetween(0, nPrefix);
	string telePrefix = _telePrefixList[telePrefixIdx];
	
	string telephone = telePrefix;

	int zero = '0';
	int nine = '9';

	int teleLen = 10;
	int fillIdx = telePrefix.length();
	for (int i = fillIdx; i < teleLen; i++)
	{
		char ch = _numberRdmzr.randIntegerBetween(zero, nine);
		telephone.push_back(ch);
	}

	// Telephone structure: xxxx-xxx-xxx
	// Two '-' at index 4 and index 8
	telephone.insert(4, "-");
	telephone.insert(8, "-");

	return telephone;
}

string StudentRandomizer::nextEmail(string fullname)
{
	string emailDomain = "@" +
		_probabilityRdmzr.randomElementWithProbability<string>(
			_emailDomainDistribution
		);

	string emailPrefix = StudentUtils::fullnameToEmailPrefixConverter(fullname);

	string email = emailPrefix + emailDomain;
	return email;
}

Date StudentRandomizer::nextDOB()
{
	int minYear = 1990;
	int maxYear = 2003;
	Date dob = _dateRdmzr.next(minYear, maxYear);
	return dob;
}

string StudentRandomizer::nextAddress()
{
	// province: tinh
	// district: huyen
	// ward    : phuong
	// street  : duong
	// project : cong trinh, toa nha (tam dich)

	string address = "";
	string classifyKey = "";
	const char delim = '|';

	int idCol = 0;
	int nameCol = 1;
	int prefixCol = 0;

	vector<string> idAndName;

	// Get province
	prefixCol = -1;
	classifyKey = to_string(_numberRdmzr.randIntegerBetween(1, 63)) + "-";

	idAndName = _studentUtils.getUnitIDAndNameFromMap(
		_provinceList, classifyKey, delim, idCol, nameCol, prefixCol
	);
	
	string provinceID = idAndName[0];
	string provinceName = idAndName[1];
	
	// Get district
	prefixCol = 2;
	classifyKey = provinceID + "-";

	idAndName = _studentUtils.getUnitIDAndNameFromMap(
		_districtList, classifyKey, delim, idCol, nameCol, prefixCol
	);
	
	string districtID = idAndName[0];
	string districtName = idAndName[1];
	
	// Get ward
	prefixCol = 2;
	classifyKey = provinceID + "-" + districtID + "-";

	idAndName = _studentUtils.getUnitIDAndNameFromMap(
		_wardList, classifyKey, delim, idCol, nameCol, prefixCol
	);

	string wardName = idAndName[1];
	
	// Get street
	prefixCol = 2;
	classifyKey = provinceID + "-" + districtID + "-";

	idAndName = _studentUtils.getUnitIDAndNameFromMap(
		_streetList, classifyKey, delim, idCol, nameCol, prefixCol
	);

	string streetName = idAndName[1];
	string prefix = "Duong";
	StringUtils::deletePrefix(streetName, prefix);

	// Get project
	prefixCol = -1;
	classifyKey = provinceID + "-" + districtID + "-";

	idAndName = _studentUtils.getUnitIDAndNameFromMap(
		_projectList, classifyKey, delim, idCol, nameCol, prefixCol
	);

	string projectName = idAndName[1];

	// Get house number
	int idx = _numberRdmzr.randIntegerBetween(0, _houseNumberList.size() - 1);
	string houseNumber = _houseNumberList[idx];

	StudentUtils::addEndCommaToUnitName(projectName);
	StudentUtils::addEndCommaToUnitName(streetName);
	StudentUtils::addEndCommaToUnitName(wardName);
	StudentUtils::addEndCommaToUnitName(districtName);

	address += houseNumber + " ";
	address += projectName;
	address += streetName;
	address += wardName;
	address += districtName;
	address += provinceName;	
	return address;
}

