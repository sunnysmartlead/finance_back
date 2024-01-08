set curdir=%~dp0
echo %curdir%
if not exist "%curdir%backup\data"       mkdir %curdir%backup\data
pause