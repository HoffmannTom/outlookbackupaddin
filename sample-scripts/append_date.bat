@ECHO OFF
rem set backup folder here
cd /D D:\Temp\outlook

setlocal EnableDelayedExpansion
rem set dt=%DATE:~6,4%%DATE:~3,2%%DATE:~0,2%

FOR /f %%a in ('WMIC OS GET LocalDateTime ^| find "."') DO set dt=%%a
set dt=%dt:~0,4%%dt:~4,2%%dt:~6,2%

rem iterate over all files and rename them
FOR /F "delims=" %%I IN ('DIR *.pst *.ost /B') DO (
set x=%%I
ren "!x!" "!x:~0,-4!_%dt%.bak"
) 

endlocal