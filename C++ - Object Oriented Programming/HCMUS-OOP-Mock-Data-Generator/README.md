# HCMUS-OOP-Mock-Data-Generator

# Projects-for-CV

| Project | Language | Description | Team size | Position | Technical Details | Achievement | Time spent | Ending month |
| :-- | :-: | :-- | :-: | :-: | :-- | :-- | :-: | :-: |
| [HCMUS-OOP-Mock-Data-Generator](../Projects-for-CV/C++%20-%20Object%20Oriented%20Programming/HCMUS-OOP-Mock-Data-Generator/) | C++ | A student information (MOCK Data) generator. | 1 | Developer. | C++, OOP, Converter Pattern, randomization. | Knowledge of how randomization in computers works. | About 2 weeks. | 04/2022 |


20120406 - Phạm Quốc Vương

Video DEMO (Youtube Link - Bật phụ đề khi xem): https://youtu.be/KfGTgLdar-M


Mức độ hoàn thành các yêu cầu:

- 100% - Read all students saved in the file **"students.txt"** back into a vector of Student.
- 100% - Generate randomly a number n in the range of [5, 10]
- 100% - Generate randomly n Students and add to the previous vector
- 100% - Overwrite and save and the students in the current vector back to the file "students.txt"
- 100% - Print out the average GPA of all students
- 100% - Print out all the students that have a GPA greater than the average GPA

Vài điểm 'thú vị' về code:

- Việc random ngày không random lần lượt ngày, tháng, năm mà là chọn một số trong đoạn 
[1, số ngày tối đa của năm] rồi từ số đó chuyển về ngày, tháng, năm, 
giúp luồng suy nghĩ rành mạch hơn.

- Các tập dữ liệu để random địa chỉ có tập lên đến hàng chục nghìn dòng, 
trong đó mỗi đơn vị hành chính có thể có rất nhiều đơn vị nhỏ hơn thuộc đơn vị đó, 
do đó dữ liệu khi load lên chương trình được tổ chức theo cấu trúc dữ liệu map 
với key và value hợp lí để truy xuất nhanh chóng.
(key được tạo theo business logic do sinh viên đặt ra, 
value là vector chứa các string mang thông tin cần thiết của đơn vị hành chính nhỏ hơn)

- Do các tập dữ liệu để random rất lớn nên tất cả đều được load trước,
mỗi lần random chỉ cần lấy dữ liệu ra từ các cấu trúc dữ liệu trong chương trình,
tránh được việc mỗi lần random, mỗi lần đọc hàng chục nghìn dòng dữ liệu từ tập tin,
giúp cải thiện hiệu suất công việc.
