#pragma once
#include "NumberRandomizer.h"
#include<vector>
#include<tuple>
#include<string>

class ProbabilityRandomizer
{
private:
	NumberRandomizer _numberRdmzer;
public:
	void reseed();
public:
	template<typename T>
	T randomElementWithProbability(vector<tuple<T, double>>);
};

template<typename T>
T ProbabilityRandomizer::randomElementWithProbability(vector<tuple<T, double>> distribution)
{
	vector<int> convertedDistribution;

	int resolvedSum = 0;
	for (auto it = distribution.begin(); it != distribution.end(); it++)
	{
		int resolve = (int)(100 * get<1>(*it));
		resolvedSum = resolvedSum + resolve;
		convertedDistribution.push_back(resolvedSum);
	}

	int choosenElem = 0;
	bool found = false;
	while (!found)
	{
		choosenElem = _numberRdmzer.randIntegerBetween(0, 100 * 100 - 1);
		if (choosenElem >= resolvedSum) continue;

		auto it = convertedDistribution.begin();
		for (it; it != convertedDistribution.end(); it++)
		{
			int sumAtIterator = *it;
			if (choosenElem < sumAtIterator)
			{
				choosenElem = (int)(it - convertedDistribution.begin());
				found = true;
				break;
			}
		}
	}

	T result = get<0>(distribution[choosenElem]);
	return result;
}

