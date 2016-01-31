@echo off
setlocal enableextensions enabledelayedexpansion

rem set max no of backups to keep and backup folder
set MAX_BACKUP=5
cd /D D:\Temp\outlook

rem set default revision no to 0
set rev=0

rem read last revision no if exists
if exist revision.txt (
	for /f "delims=" %%a in (revision.txt) do (
		set rev=%%a
	)
)

rem calculate next revision no
Set /A rev=!rev!+1
Set /A rev=!rev! %% !MAX_BACKUP!

rem find pst- and osts-files and rename them
FOR /F "delims=" %%I IN ('DIR *.pst *.ost /B') DO (
	set x=%%I
	if exist "!x:~0,-4!_!rev!.bak" del "!x:~0,-4!_!rev!.bak"
	ren "!x!" "!x:~0,-4!_!rev!.bak"
) 

rem save current revision no
echo !rev! > revision.txt

endlocal
