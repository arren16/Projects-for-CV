#include "StringUtils.h"

string StringUtils::tolower(string src)
{
    int len = src.length();
    for (int i = 0; i < len; i++)
    {
        if ('A' <= src[i] && src[i] <= 'Z')
        {
            src[i] = src[i] - ('A' - 'a');
        }
    }

    return src;
}

vector<string> StringUtils::parse(string src, const char& delim)
{
    vector<string> result;
    src = src.substr(1, src.length() - 1);

    string reader = "";
    int len = src.length();
    for (int i = 0; i < len; i++)
    {
        if (src[i] != delim)
        {
            reader += src[i];
        }
        else
        {
            result.push_back(reader);
            reader = "";
        }
    }

    return result;
}

void StringUtils::deletePrefix(string &src, string prefix)
{
    int prefixLen = prefix.length();
    if (src.length() >= prefixLen)
    {
        string cut = src.substr(0, prefixLen);
        if (cut == prefix)
            src = src.substr(prefixLen + 1);
    }
}
