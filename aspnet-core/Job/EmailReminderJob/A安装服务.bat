@echo off
color 0e
@echo ==================================
@echo ���ѣ����Ҽ����ļ����ù���Ա��ʽ�򿪡�
@echo ==================================
@echo Start Install EmailReminderJob

cd ..
sc create EmailReminderJob binPath=%~dp0EmailReminderJob.exe start= auto 
sc description EmailReminderJob "�����ʼ����ѷ���"
Net Start EmailReminderJob
pause