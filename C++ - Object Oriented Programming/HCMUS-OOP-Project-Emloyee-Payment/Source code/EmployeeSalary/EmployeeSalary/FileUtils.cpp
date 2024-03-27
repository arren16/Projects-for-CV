#include "FileUtils.h"

vector<Employee*> FileUtils::readEmployeeList(string filename)
{
	vector<Employee*> result;
	
	fstream f;
	f.open(filename, ios::in);

	if (!f.fail())
	{
		map<string, int>* mapper = &EmployeeTypeMapper.typeDictionary;

		while (!f.eof())
		{
			string typeAndNameLine = "";
			string salaryLine = "";

			do getline(f, typeAndNameLine);
			while (!f.eof() && typeAndNameLine.length() == 0);

			do getline(f, salaryLine);
			while (!f.eof() && salaryLine.length() == 0);

			if (typeAndNameLine.length() == 0 || salaryLine.length() == 0) continue;

			map<string, string> typeAndNameData = parseTypeAndNameData(typeAndNameLine);
			
			string typeName = typeAndNameData["EmployeeType"];

			if (mapper->find(typeName) == mapper->end())
			{
				cout << "EmployeeFactory::createEmployee: ";
				cout << "employee type '" << typeName << "' (" << filename << ") ";
				cout << "was not predefined in EmployeeTypeMapper (BusinessLogic.h)!" << endl;
				exit(0);
			}

			int int_employeeType = mapper->operator[](typeName);
			string employeeName = typeAndNameData["EmployeeName"];

			Employee* employee = EmployeeFactory::getInstance()->createEmployee(int_employeeType);
			
			employee->setFullname(employeeName); 
			employee->setSalaryData(salaryLine);
			
			result.push_back(employee);
		}
	}
	else
	{
		cout << "Can't open " << filename << "! " << endl;
	}

	return result;
}

