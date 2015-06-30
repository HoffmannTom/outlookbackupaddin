@echo off
setlocal enableextensions enabledelayedexpansion

rem set max no of backups to keep and backup folder
set MAX_BACKUP=5
cd /D D:\Temp\outlook


set rev=0

rem read last revision no
if exist revision.txt (
	for /f "delims=" %%a in (revision.txt) do (
		set rev=%%a
	)
)
Set /A rev=!rev!+1
Set /A rev=!rev! %% !MAX_BACKUP!

rem find pst-file and rename
FOR /F "delims=" %%I IN ('DIR *.pst /B') DO (
	set x=%%I
	if exist "!x:~0,-4!_!rev!.bak" del "!x:~0,-4!_!rev!.bak"
	ren "!x!" "!x:~0,-4!_!rev!.bak"
) 

echo !rev! > revision.txt

endlocal
