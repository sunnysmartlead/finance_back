@echo off
color 0e
@echo ==================================
@echo 提醒：请右键本文件，用管理员方式打开。
@echo ==================================
@echo Start Install BakOracle

cd ..
sc create BakOracle binPath=%~dp0BakOracle.exe start= auto 
sc description BakOracle "财务系统数据库备份"
Net Start BakOracle
pause