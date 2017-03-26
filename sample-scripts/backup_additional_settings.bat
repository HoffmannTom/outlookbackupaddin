@ECHO OFF
rem set backup folder here
cd /D d:\outlookbackup

setlocal EnableDelayedExpansion

rem backup signatures
copy "%APPDATA%\Microsoft\Signatures\*" *

rem backup registry settings, set parameter reg32 or reg64 depending on your outlook version
rem uncomment for outlook 2010
rem reg export "HKCU\Software\Microsoft\Windows NT\CurrentVersion\Windows Messaging Subsystem\Profiles" outlook_settings_backup.reg /y /reg:32

rem uncomment for outlook 2013
rem reg export "HKCU\Software\Microsoft\Office\15.0\Outlook\Profiles" outlook_settings_backup.reg /y /reg:32

rem uncomment for outlook 2016
rem reg export "HKCU\Software\Microsoft\Office\16.0\Outlook\Profiles\Outlook" outlook_settings_backup.reg /y /reg:32


endlocal