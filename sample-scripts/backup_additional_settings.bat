@ECHO OFF
rem set backup folder here
cd /D d:\outlookbackup

setlocal EnableDelayedExpansion

rem backup signatures
copy "%APPDATA%\Microsoft\Signatures\*" *

rem backup registry settings
rem uncomment for outlook 2010
rem regedit /e outlook_settings_backup.reg "HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\Windows Messaging Subsystem\Profiles" 

rem uncomment for outlook 2013
rem regedit /e outlook_settings_backup.reg "HKEY_CURRENT_USER\Software\Microsoft\Office\15.0\Outlook\Profiles"

rem uncomment for outlook 2016
rem regedit /e outlook_settings_backup.reg "HKEY_CURRENT_USER\Software\Microsoft\Office\16.0\Outlook\Profiles"

endlocal