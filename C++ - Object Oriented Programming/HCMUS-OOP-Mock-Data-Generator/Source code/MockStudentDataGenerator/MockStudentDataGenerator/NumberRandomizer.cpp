#include "NumberRandomizer.h"

//
NumberRandomizer::NumberRandomizer()
{
	srand((unsigned)time(0));
}
NumberRandomizer::~NumberRandomizer()
{
	// Do nothing
}
//
int NumberRandomizer::randIntegerBetween(int min, int max)
{
	/*
		(x % u) + k in [min, max]
		min = 0 + k
		max = (u - 1) + k

		=> k = min
		=> u = max - k + 1 = max - min + 1
	*/
	int x = rand();
	int u = max - min + 1;
	int k = min;

	return x % u + k;
}

double NumberRandomizer::randRealBetween(double min, double max)
{
	//    min <= r <= max
	// => 0 <= r - min <= max - min
	// => 0 <= (r - min) / (max - min) <= 1 
	// k = (r - min) / (max - min)
	// => 0 <= k <= 1
	// => r = min + k * (max - min)

	// r = min : k = 0 
	// r = max : k = 1
	// min < r < max : 0 < k < 1
	// r = max = min : max = min

	int numerator = rand() % 5001; // [0, 5000] 
	int denominator = rand() % (10000 - 5000 + 1) + 5000; // [5000, 10000]
	double k = (1.0 * numerator) / (1.0 * denominator); // 0 <= k <= 1

	double result = min + k * (max - min);
	return result;
}
//
void NumberRandomizer::reseed()
{
	srand((unsigned)time(0));
}