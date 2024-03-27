#include "FileUtils.h"

shared_ptr<EmployeeFactory> EmployeeFactory::_instance = NULL;
int main()
{
	string containingFolder = "Resource/";

	string month = "Nov";
	string year = "2021";
	string fileName = containingFolder + monthFullWritten(month) + year + ".txt";

	vector<Employee*> employeeList = FileUtils::readEmployeeList(fileName);

	auto i = employeeList.begin();
	for (; i != employeeList.end(); i++)
	{
		cout << (*i)->toString() << endl;
	}

	for (; i != employeeList.end(); i++)
	{
		delete (*i);
	}

	system("pause");
	return 0;
}