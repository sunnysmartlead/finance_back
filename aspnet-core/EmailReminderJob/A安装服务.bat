@echo off
color 0e
@echo ==================================
@echo 提醒：请右键本文件，用管理员方式打开。
@echo ==================================
@echo Start Install EmailReminderJob

cd ..
sc create EmailReminderJob binPath=%~dp0EmailReminderJob.exe start= auto 
sc description EmailReminderJob "财务邮件提醒服务"
Net Start EmailReminderJob
pause