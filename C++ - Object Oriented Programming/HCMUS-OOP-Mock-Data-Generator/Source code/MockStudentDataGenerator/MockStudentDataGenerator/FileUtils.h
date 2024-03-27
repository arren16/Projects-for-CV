#pragma once
#include<iostream>
#include<string>
#include<vector>
#include<fstream>
#include<tuple>
#include<map>
#include "StringUtils.h"
#include "Student.h"
using namespace std;

#define _FILE_TAB_ "    " // 4 whitespaces

#define _STUDENT_LIST_DIR_ "student.txt"

#define _ID_YEAR_CODE_DIR_ "Resource/StudentIDYearCode.txt"
#define _ID_FACULTY_CODE_ "Resource/StudentIDFacultyCode.txt"
#define _ID_PROGRAM_CODE_ "Resource/StudentIDProgramCode.txt"

#define _FIRST_NAME_DIR_ "Resource/FirstName.txt"
#define _MIDDLE_NAME_DIR_ "Resource/MiddleName.txt"
#define _LAST_NAME_DIR_ "Resource/LastName.txt"

#define _TELEPHONE_PREFIX_DIR_ "Resource/TelephonePrefix.txt"
#define _EMAIL_DOMAIN_DIR_ "Resource/EmailDomain.txt"

#define _DISTRICTS_DIR_ "Resource/Districts.txt"
#define _PROJECTS_DIR_ "Resource/Projects.txt"
#define _PROVINCES_DIR_ "Resource/Provinces.txt"
#define _STREETS_DIR_ "Resource/Streets.txt"
#define _WARDS_DIR_ "Resource/Wards.txt"
#define _HOUSE_NUMBER_DIR_ "Resource/HouseNumber.txt"

#define _DISTRICTS_IGNORED_LINES_ 13
#define _PROJECTS_IGNORED_LINES_ 15
#define _PROVINCES_IGNORED_LINES_ 12
#define _STREETS_IGNORED_LINES_ 14
#define _WARDS_IGNORED_LINES_ 14

class Student;

class FileUtils
{
public:
    static void readStudentList(const string&, vector<Student>&);
    static void writeStudentList(const string&, vector<Student>);
public:
	template<typename T>
	static vector<T> readOneElementLineType(string);
	
	template<typename T1, typename T2>
	static vector<tuple<T1, T2>> readTwoElementLineType(string);

    static void readNElementLineType(const string&, const char&,
        int, vector<int>, map<string, vector<string>>&
    );
};

template<typename T>
vector<T> FileUtils::readOneElementLineType(string filename)
{
    vector<T> elementList;

    fstream f;
    f.open(filename, ios::in);

    T reader;
    if (!f.fail())
    {
        while (f >> reader)
            elementList.push_back(reader);
    }
    else
    {
        cout << "FileUtils::readOneElementLineType";
        cout << " can't open " << filename << endl;
    }

    f.close();
    return elementList;
}

template<typename T1, typename T2>
vector<tuple<T1, T2>> FileUtils::readTwoElementLineType(string filename)
{
    vector<tuple<T1, T2>> elementList;
    T1 leftReader;
    T2 rightReader;

    fstream f;
    f.open(filename, ios::in);

    if (!f.fail())
    {
        while (f >> leftReader && f >> rightReader)
        {
            tuple<T1, T2> myTuple(leftReader, rightReader);
            elementList.push_back(myTuple);
        }
    }
    else
    {
        cout << "FileUtils::readOneElementLineType";
        cout << " can't open " << filename << endl;
    }

    f.close();
    return elementList;
}
