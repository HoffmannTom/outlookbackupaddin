@ECHO OFF
rem set backup folder here
cd /D d:\outlookbackup

setlocal EnableDelayedExpansion
set dt=%DATE:~6,4%%DATE:~3,2%%DATE:~0,2%

rem iterate over all files and rename them
FOR /F "delims=" %%I IN ('DIR *.pst *.ost /B') DO (
set x=%%I
ren "!x!" "!x:~0,-4!_%dt%.bak"
) 

endlocal