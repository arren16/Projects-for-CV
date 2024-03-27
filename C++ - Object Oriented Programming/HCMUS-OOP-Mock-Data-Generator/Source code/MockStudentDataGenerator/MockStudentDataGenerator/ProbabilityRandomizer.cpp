#include "ProbabilityRandomizer.h"

void ProbabilityRandomizer::reseed()
{
	srand((unsigned)time(0));
}
