#include "PrecisionUtils.h"

string PrecisionUtils::doubleToStringWithTrailingZero(double src, int nPrecisionDigit)
{
	int integerPart = (int)src;
	double decimalPart = src - (double)integerPart;
	
	stringstream converter;
	converter << setprecision(nPrecisionDigit) << decimalPart;
	converter >> decimalPart;

	decimalPart = decimalPart * IntegerUtils::pow(10, nPrecisionDigit);

	int decimalPartToInteger = (int)decimalPart;

	string decimalString = to_string(decimalPartToInteger);
	if (decimalString == "0")
		for (int i = 0; i < nPrecisionDigit - 1; i++)
			decimalString.push_back('0');

	string result = to_string(integerPart) + "." + decimalString;
	return result;
}
