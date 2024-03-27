#include "FileUtils.h"

void FileUtils::readNElementLineType(const string& filename, const char& delim,
    int nIngnoredLine, vector<int> classifyColumn,
    map<string, vector<string>> &ouputMap)
{
    string line;
    string originalLine;

    fstream f;
    f.open(filename, ios::in);

    if (!f.fail())
    {
        while (nIngnoredLine != 0)
        {
            getline(f, line);
            nIngnoredLine--;
        }

        string key = "";
        while (getline(f, line))
        {
            originalLine = line;
            line = line + delim;
            
            vector<string> parsedLine = StringUtils::parse(line, delim);

            auto it = classifyColumn.begin();
            for (it; it != classifyColumn.end(); it++)
                key = key + parsedLine[*it] + "-";

            map<string, vector<string>>::iterator builder = ouputMap.find(key);
            if (builder != ouputMap.end())
            {
                builder->second.push_back(originalLine);
            }
            else
            {
                vector<string> value = { originalLine };
                ouputMap.insert({ key, value});
            }

            key = "";         
        }
    }
    else
    {
        cout << "FileUtils::readOneElementLineType";
        cout << " can't open file!" << endl;
        system("pause");
    }

    f.close();
}

void FileUtils::readStudentList(const string& filepath, vector<Student>& studentList)
{   
    fstream f;
    f.open(filepath, ios::in);

    if (!f.fail())
    {
        string line = "";
        while (!f.eof())
        {
            // Student: _id - _fullname
            getline(f, line);
         
            if (line.length() == 0)
                continue;

            string idPrefix = "Student: ";
            string fullnamePrefix = " - ";

            int idIdx = idPrefix.length();
            int fullnameIdx = line.find(fullnamePrefix);
               
            string id = line.substr(idIdx, fullnameIdx - idIdx);
            string fullname = line.substr(fullnameIdx + fullnamePrefix.length());

            // GPA = _gpa, Telephone = _telephone
            getline(f, line);

            string gpaPrefix = (string)_FILE_TAB_ + "GPA=";
            string telePrefix = ", ";

            int gpaIdx = gpaPrefix.length();
            int teleIdx = line.find(telePrefix);

            double gpa = stod(line.substr(gpaIdx, teleIdx - gpaIdx));
            string telephone = line.substr(teleIdx + telePrefix.length()); 

            // Email = _email
            getline(f, line);
            string emailPrefix = (string)_FILE_TAB_ + "Email=";

            int emailIdx = emailPrefix.length();
            string email = line.substr(emailIdx);

            // DOB = _dob
            getline(f, line);
            string dobPrefix = (string)_FILE_TAB_ + "DOB=";

            int dobIdx = dobPrefix.length();
            string dobString = line.substr(dobIdx);
            
            // Replace '/' by whitespace
            int replaceIdx = dobString.find("/");
            dobString.at(replaceIdx) = ' ';
            replaceIdx = dobString.find("/");
            dobString.at(replaceIdx) = ' ';

            int day = 0;
            int month = 0;
            int year = 0;
            
            stringstream parser(dobString);
            parser >> day >> month >> year;
            
            Date dob(day, month, year);

            // Address = _address
            getline(f, line);
            string addressPrefix = (string)_FILE_TAB_ + "Address=";

            int addressIdx = addressPrefix.length();
            string address = line.substr(addressIdx);

            Student student(id, fullname, gpa, telephone, email, dob, address);
            studentList.push_back(student);
        }
    }
    else
    {
        cout << "FileUtils::readStudentList ";
        cout << "can't open file " << filepath << endl;
    }

    f.close();
}

void FileUtils::writeStudentList(
    const string& filepath, vector<Student> studentList)
{
    fstream f;
    f.open(filepath, ios::out);

    if (!f.fail())
    {
        auto i = studentList.begin();
        for (i; i != studentList.end(); i++)
        {
            f << i->toString();      
            f << endl;
        }
    }
    else
    {
        cout << "FileUtils::writeStudentListInAppendMode ";
        cout << "can't open file " << filepath << endl;
    }

    f.close();
}
