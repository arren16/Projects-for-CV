#pragma once
#include<vector>
#include "DateUtils.h"
#include "NumberRandomizer.h"

class DateRandomizer
{
private:
	NumberRandomizer _numberRdmzr;
public:
	Date next(int, int);
	void reseed();
};

