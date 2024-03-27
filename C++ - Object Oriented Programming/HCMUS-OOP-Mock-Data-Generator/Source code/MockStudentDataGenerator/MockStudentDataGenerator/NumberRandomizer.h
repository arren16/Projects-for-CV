#pragma once
#include<iostream>
using namespace std;

class NumberRandomizer
{
public:
	NumberRandomizer(); // Include an init-seeding
	~NumberRandomizer();
public:
	/// <summary>
	/// Integer in [min, max]
	/// </summary>
	/// <param name="1st">min</param>
	/// <param name="2nd">max</param>
	/// <returns>an integer</returns>
	int randIntegerBetween(int, int);

	/// <summary>
	/// Double in [min, max]
	/// </summary>
	/// <param name="1st">min</param>
	/// <param name="2nd">max</param>
	/// <returns>a real number</returns>
	double randRealBetween(double, double); // Real number in [min, max]

	void reseed(); // Re-seed after the constructor's seeding (when needed)
};