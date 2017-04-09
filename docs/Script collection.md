At this page I want to collect some scripts, which can be run after backup to do some work.
The sample scripts are also containted within the downloaded zip-file (version 1.5 onwards).

**Add date to the backup files**

{code:powershell}
@ECHO OFF
cd /D d:\outlookbackup
setlocal EnableDelayedExpansion
rem set dt=%DATE:~6,4%%DATE:~3,2%%DATE:~0,2%
FOR /f %%a in ('WMIC OS GET LocalDateTime ^| find "."') DO set dt=%%a
set dt=%dt:~0,4%%dt:~4,2%%dt:~6,2%

FOR /F "delims=" %%I IN ('DIR **.pst **.ost /B') DO (
set x=%%I
if exist "!x:~0,-4!_%dt%.bak" del "!x:~0,-4!_%dt%.bak"
ren "!x!" "!x:~0,-4!_%dt%.bak"
) 
{code:powershell}

**Keep last N backups**

{code:powershell}
@echo off
setlocal enableextensions enabledelayedexpansion

rem set max no of backups to keep and backup folder
set MAX_BACKUP=5
cd /D D:\outlook


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
FOR /F "delims=" %%I IN ('DIR **.pst **.ost /B') DO (
	set x=%%I
	if exist "!x:~0,-4!_!rev!.bak" del "!x:~0,-4!_!rev!.bak"
	ren "!x!" "!x:~0,-4!_!rev!.bak"
) 

echo !rev! > revision.txt

endlocal
{code:powershell}

**Backup signatures and profile settings**

{code:powershell}
@ECHO OFF
rem set backup folder here
cd /D d:\outlookbackup

setlocal EnableDelayedExpansion

rem backup signatures
copy "%APPDATA%\Microsoft\Signatures\**" **

rem backup registry settings, set parameter reg32 or reg64 depending on your outlook version
rem uncomment for outlook 2010
rem reg export "HKCU\Software\Microsoft\Windows NT\CurrentVersion\Windows Messaging Subsystem\Profiles" outlook_settings_backup.reg /y /reg:32

rem uncomment for outlook 2013
rem reg export "HKCU\Software\Microsoft\Office\15.0\Outlook\Profiles" outlook_settings_backup.reg /y /reg:32

rem uncomment for outlook 2016
rem reg export "HKCU\Software\Microsoft\Office\16.0\Outlook\Profiles\Outlook" outlook_settings_backup.reg /y /reg:32


endlocal
{code:powershell}