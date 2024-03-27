#include "IntegerUtils.h"

int IntegerUtils::pow(int a, int n)
{
    int result =
        (n < 0) ? 0 :
        (n == 0) ? 1 :
        a * pow(a, n - 1);
    return result;
}
