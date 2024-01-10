@echo off
color 0e
@echo ==================================
@echo 提醒：请右键本文件，用管理员方式打开。
@echo ==================================
@echo Start Remove EmailReminderJob

Net stop EmailReminderJob
sc delete EmailReminderJob
pause