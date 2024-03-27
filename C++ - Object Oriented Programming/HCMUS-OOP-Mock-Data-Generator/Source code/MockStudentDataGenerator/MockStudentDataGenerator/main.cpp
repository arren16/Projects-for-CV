#include "StudentRandomizer.h"
#include "NumberRandomizer.h"

int main()
{
	StudentRandomizer studentRdmzr;
	NumberRandomizer numberRdmzr;
    
    // Random a number n in [5, 10]
    int nStudent = numberRdmzr.randIntegerBetween(5, 10);
    
    // Read student list from file
    vector<Student> studentList;
    FileUtils::readStudentList(_STUDENT_LIST_DIR_, studentList);

    // List out used IDs
    vector<string> usedIDList = StudentUtils::getIDList(studentList);

    // Random n student (without duplicate IDs)
    studentRdmzr.next(studentList, nStudent, usedIDList);

    // Write those student to file
    FileUtils::writeStudentList(_STUDENT_LIST_DIR_, studentList);

    // Average GPA
    double averageGPA = StudentUtils::getAverageGPA(studentList);
    cout << "Average GPA: " << averageGPA << endl;

    cout << "Students have GPA greater than average GPA: " << endl;
    auto i = studentList.begin();
    for (i; i != studentList.end(); i++)
        if (i->gpa() > averageGPA)
        {
            string gpa = PrecisionUtils::doubleToStringWithTrailingZero(i->gpa(), 2);

            cout << i->id() << " | GPA: ";
            cout << gpa << " - ";
            cout << i->fullname() << endl;
        }

    system("pause");

	return 0;
}