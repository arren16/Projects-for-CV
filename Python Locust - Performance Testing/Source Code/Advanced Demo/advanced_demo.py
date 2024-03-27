'''
MÔI TRƯỜNG THỰC THI:

- MacOS Sonoma 14.0
- CPU Apple M1
- Trình duyệt Microsoft Edge Version 118.0.2088.76 (Official build) (arm64)
- Python 3.10.10
- locust 2.18.0 (cài đặt với pip)
- selenium 4.14.0 (cài đặt với pip)

'''

import json
import shutil
from locust import User, SequentialTaskSet, task, between
from locust.event import EventHook

from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys

import datetime
import time
import os

import selenium_gmail_utils
import utils

logging_event_hook = EventHook()

class SequentialSeleniumTasks(SequentialTaskSet):
    init_time = None

    browser_instance_data_dir = None
    free_port = None

    browser = None

    @task
    def init(self):
        self.init_time = utils.current_datetime()

    @task
    def open_remote_debugging_edge(self):
        '''
            Mở Edge ở chế độ Remote Debugging
        '''
        
        # Tìm một port trống để chạy browser instance
        self.free_port, socket = utils.find_free_port()

        reformatted_datetime = self.init_time.replace('/', ' ').replace(':', ' ')
        # Chọn thư mục lưu dữ liệu của browser instance
        self.browser_instance_data_dir = \
            f"{utils.browser_runtime_instance_data_path}/{reformatted_datetime} {self.free_port}"

        print(f"Opening remote debugging edge at port {self.free_port}")

        # Mở Edge ở chế độ Remote Debugging với port được cấp
        remote_debugging_command = f'''open -na "Microsoft Edge.app" --args
            --remote-debugging-port={self.free_port}
            --user-data-dir="{self.browser_instance_data_dir}"
        '''
        
        socket.close()
        os.system(remote_debugging_command.replace("\n", " "))


    @task
    def init_selenium_instance(self):
        print(f"Initializing Selenium browser instance with port {self.free_port}")
    
        # Kết nối Selenium đến Edge ở port được mở
        options = webdriver.EdgeOptions()
        options.add_experimental_option("debuggerAddress", f"localhost:{self.free_port}")
        options.add_argument("--incognito")  
        # options.add_argument("headless")

        self.browser = webdriver.Edge(options=options)
        

    @task
    def load_gmail_page(self):

        delay_time = 0

        start_time = time.time()

        print(f"{self.free_port} Loading Gmail page")
        self.browser.get('https://mail.google.com/mail/u/0/')

        time.sleep(2)
        delay_time += 2

        print(f"{self.free_port} Entering username")
        delay_time += selenium_gmail_utils.enter_username(
            self.browser, 
            utils.source_email
        )
        
        print(f"{self.free_port} Entering password")
        delay_time += selenium_gmail_utils.enter_password(
            self.browser, 
            utils.source_password
        )

        print(f"{self.free_port} Clicking login button")
        delay_time += selenium_gmail_utils.click_login_button(
            self.browser
        )

        end_time = time.time()

        response_time = ((end_time - start_time) - delay_time) * 1000
        response_time = int(response_time)

        logging_event_hook.fire(
            method = "Selenium",
            name = "Open Gmail page",
            content_length = 0, # Ở HttpUser là kích thước của các response (đơn vị byte)
            response_time = response_time
        )


    @task
    def click_new_email_button(self):
        start_time = time.time()

        print(f"{self.free_port} Opening new email")
        delay_time = selenium_gmail_utils.start_a_new_email(self.browser)
    
        end_time = time.time()
        response_time = ((end_time - start_time) - delay_time) * 1000
        response_time = int(response_time)

        logging_event_hook.fire(
            method = "Selenium",
            name = "Open new email",
            content_length = 0, # Ở HttpUser là kích thước của các response (đơn vị byte)
            response_time = response_time
        )

    
    @task
    def enter_email_details(self):
        start_time = time.time()

        print(f"{self.free_port} Entering email details")
        delay_time = selenium_gmail_utils.enter_email_details(
            self.browser, 
            utils.dest_email, 
            self.init_time
        )

        end_time = time.time()
        response_time = ((end_time - start_time) - delay_time) * 1000
        response_time = int(response_time)

        logging_event_hook.fire(
            method = "Selenium",
            name = "Enter email details",
            content_length = 0, # Ở HttpUser là kích thước của các response (đơn vị byte)
            response_time = response_time
        )


    @task
    def click_send_email_button(self):
        try:
            start_time = time.time()

            print(f"{self.free_port} Sending email")
            send_mail_result = selenium_gmail_utils.send_email(self.browser)[0]

            if (not send_mail_result[1]):
                raise Exception

            end_time = time.time()
            response_time = ((end_time - start_time) - send_mail_result[0]) * 1000
            response_time = int(response_time)

            logging_event_hook.fire(
                method = "Selenium",
                name = "Send email",
                content_length = 0, # Ở HttpUser là kích thước của các response (đơn vị byte)
                response_time = response_time
            )
        except:
            pass
            os.system(f"lsof -ti tcp:{self.free_port} | xargs kill -9")

    @task
    def browser_instance_data(self):
        if os.path.exists(self.browser_instance_data_dir):
            shutil.rmtree(self.browser_instance_data_dir)


    def on_stop(self):
        os.system(f"lsof -ti tcp:{self.free_port} | xargs kill -9")
    

class MyUser(User):
    tasks = [SequentialSeleniumTasks]

    def logging_event_handler(self, method, name, content_length, response_time):

        print(f"Event was fired with arguments: {method}, {name}")

        self.environment.runner.stats.log_request(
            method=method,
            content_length=content_length,
            name=name, 
            response_time=response_time
        )

    def on_start(self):
        logging_event_hook.add_listener(self.logging_event_handler)
