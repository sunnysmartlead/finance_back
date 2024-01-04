@echo off   
echo ================================================   
echo  Windows环境下Oracle数据库的自动备份脚本  
echo  1. 使用当前日期命名备份文件。  
echo  2. 自动删除7天前的备份。  
echo ================================================  
echo  以“YYYYMMDD”格式取出当前时间。  
set BACKUPDATE=%date:~0,4%%date:~5,2%%date:~8,2%%time:~0,2%%time:~3,2%%time:~6,2%
echo  设置用户名、密码和要备份的数据库
set USER=WISSEN_TEST_V2
set PASSWORD=admin
set DATABASE=WISSEN_TEST_V2
echo  创建备份目录
if not exist ".\backup\data"       mkdir .\backup\data  
if not exist ".\backup\log"        mkdir .\backup\log  
set DATADIR=.\backup\data
set LOGDIR=.\backup\log
exp %USER%/%PASSWORD%@10.1.1.131/ORCL file=%DATADIR%\data_%BACKUPDATE%.dmp owner=%DATABASE% log=%LOGDIR%\log_%BACKUPDATE%.log
echo  删除7天前的备份。
forfiles /p "%DATADIR%" /s /m *.* /d -7 /c "cmd /c del @path"
forfiles /p "%LOGDIR%" /s /m *.* /d -7 /c "cmd /c del @path"
exit