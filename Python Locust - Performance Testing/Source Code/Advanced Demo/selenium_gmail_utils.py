import utils
import time
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC


def press_esc(browser):
    body = browser.find_element(By.XPATH, '//body')
    body.send_keys(Keys.ESCAPE)


def is_element_exist(browser, select_by, select_string):
    '''
        Kiểm tra một phần tử có tồn tại trên trang được mở hay không
    '''
    
    is_exist = False
    
    try:
        browser.find_element(select_by, select_string)    
        is_exist = True

    except Exception:
        is_exist = False

    return is_exist


def change_element_color(browser, element, color: str = 'lightgreen'):
    '''
        Hàm đổi màu phần tử trên trang web
    '''

    browser.execute_script(
        f"arguments[0].style.backgroundColor = '{color}';", 
        element
    )


def force_element_to_be_clicked(browser, select_by, select_string, max_try = 10) -> [int, bool]:
    
    total_delay_time = 0

    element = None

    total_try = 0
    is_element_clicked = False
    while (not is_element_clicked and total_try <= max_try):
        try:
            element = browser.find_element(select_by, select_string)
            change_element_color(browser, element)
            element.click()
            is_element_clicked = True
        except Exception:
            print(f"force_element_to_be_clicked ({total_try}): {select_string} (can't be found)")
            time.sleep(5)
            total_delay_time += 5
        
        total_try += 1

    return [total_delay_time, is_element_clicked]


def enter_username(browser, email) -> int:
    total_delay_time = 0
    
    time.sleep(2)
    total_delay_time += 2
    
    # total_delay_time += force_element_to_be_clicked(
    #     browser,
    #     By.ID,
    #     'identifierId'
    # )[0]

    username_input_field = browser.find_element(
        By.ID, 'identifierId'
    )
    change_element_color(browser, username_input_field)
    username_input_field.send_keys(email)

    total_delay_time += force_element_to_be_clicked(
        browser,
        By.XPATH,
        '//button[.//span[contains(text(),"Next") or contains(text(),"Tiếp theo")]]'
    )[0]

    return total_delay_time


def enter_password(browser, password) -> int:
    delay_time = 2
    time.sleep(delay_time)

    press_esc(browser)
    
    password_input_field = browser.find_element(
            By.XPATH, 
            '//input[contains(@aria-label,"Enter your password") or contains(@aria-label,"Nhập mật khẩu của bạn")]'
            # "//input[@name=password]"
        )
    change_element_color(browser, password_input_field)
    password_input_field.send_keys(password)

    return delay_time


def click_logged_out_account(browser) -> int:
    delay_time = 2
    total_delay_time = 0

    logged_out_account_xpath = f"//div[text()='{utils.source_email}']"
    next_button_xpath = '//button[.//span[contains(text(),"Next") or contains(text(),"Tiếp theo")]]'
    
    while (not is_element_exist(browser, By.XPATH, next_button_xpath)):
        script = f'''
            var elementsContainingText = document.evaluate(
                "{logged_out_account_xpath}", 
                document, 
                null, 
                XPathResult.ANY_TYPE, 
                null
            );
            var element = elementsContainingText.iterateNext();
            element.click();
        '''

        # Thực hiện thao tác click bằng JavaScript
        browser.execute_script(script)
        time.sleep(2)
        total_delay_time += 2

    return total_delay_time


def click_login_button(browser) -> int:
    total_delay_time = 0

    time.sleep(2)
    total_delay_time += 2

    next_button_xpath = '//button[.//span[contains(text(),"Next") or contains(text(),"Tiếp theo")]]'
    
    total_delay_time += force_element_to_be_clicked(
        browser,
        By.XPATH,
        next_button_xpath
    )[0]

    return total_delay_time

def start_a_new_email(browser):
    delay_time = 2
    time.sleep(delay_time)

    new_email_button_xpath = "//div[text()='Soạn thư']"
    
    new_email_button = browser.find_element(
        By.XPATH,
        new_email_button_xpath
    )
    change_element_color(browser, new_email_button)
    new_email_button.click()

    return delay_time


def enter_email_details(browser, dest_email, email_datetime: str = utils.current_datetime()) -> int:

    email_title = f"Locust Testing at {email_datetime}"
    email_content = f"This is a test (executed at {email_datetime})."

    total_delay_time = 0
    time.sleep(2)

    print("Entering dest_email")

    # total_delay_time += force_element_to_be_clicked(
    #     browser,
    #     By.XPATH,
    #     "//span[text()='Đến']"
    # )

    dest_email_field = browser.find_element(
        By.XPATH, 
        "//input[@role='combobox']"
    )
    change_element_color(browser, dest_email_field)
    dest_email_field.send_keys(dest_email)

    # Nhập tiêu đề
    print("Entering email title")
    email_title_field = browser.find_element(
        By.XPATH, 
        "//input[@name='subjectbox']"
    )
    change_element_color(browser, email_title_field)
    email_title_field.send_keys(email_title)

    # Nhập nội dung email
    print("Entering email content")
    email_content_field = browser.find_element(
        By.XPATH,
        "//div[@aria-label='Nội dung thư']"
    )
    change_element_color(browser, email_content_field)
    email_content_field.send_keys(email_content)

    return total_delay_time

def send_email(browser) -> [int, bool]:
    click_result = force_element_to_be_clicked(
        browser,
        By.XPATH, 
        "//div[text()='Gửi']"
    )[0]

    return click_result