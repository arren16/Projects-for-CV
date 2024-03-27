#pragma once
#include<iostream>
#include<string>
#include<vector>
using namespace std;

class StringUtils
{
public:
	static string tolower(string);
	static vector<string> parse(string, const char&);
	static void deletePrefix(string&, string);
};

