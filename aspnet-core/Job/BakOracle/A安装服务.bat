@echo off
color 0e
@echo ==================================
@echo ���ѣ����Ҽ����ļ����ù���Ա��ʽ�򿪡�
@echo ==================================
@echo Start Install BakOracle

cd ..
sc create BakOracle binPath=%~dp0BakOracle.exe start= auto 
sc description BakOracle "�����ʼ����ѷ���"
Net Start BakOracle
pause