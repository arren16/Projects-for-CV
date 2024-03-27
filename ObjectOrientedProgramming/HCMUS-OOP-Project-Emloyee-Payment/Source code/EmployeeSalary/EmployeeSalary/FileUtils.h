#pragma once
#include <fstream>

#include "EmployeeFactory.h"

class FileUtils
{
public:
	static vector<Employee*> readEmployeeList(string);
};
