@ECHO OFF
rem set backup folder here
cd /D d:\outlookbackup

setlocal EnableDelayedExpansion
set dt=%DATE:~6,4%%DATE:~3,2%%DATE:~0,2%

FOR /F "delims=" %%I IN ('DIR *.pst /B') DO (
set x=%%I
ren "!x!" "!x:~0,-4!_%dt%.bak"
) 