
Installation requirements:
==========================
 - .Net Framework 4.0
 - Installed Microsoft Outlook 2016, 2013 or 2010
 - For Outlook 2010 without SP you need to install the VSTO runtime:
   https://www.microsoft.com/en-us/download/details.aspx?id=48217


Installation instructions:
==========================
1) Unzip the download to a destination folder, e.g. c:\Program Files\Codeplex\Outlook-Backup
2) Run Setup.exe and follow the instructions on the screen
3) A certificate warning will show up. Accept the certificate in order to get the plugin installed


Usage instruction:
==================
1) Open Outlook and choose "Backup" on the ribbon bar
2) Open the settings window and select the pst-files, the time interval (in days), target Folder and Location 
    of the file "backupexecutor.exe" (e.g. c:\Program Files\Codeplex\Outlook-Backup\backupexecutor.exe)
    Afterwards press "save" to save your settings.
3) When you Exit Outlook the first time, the backup should start automatically


Uninstall instructions:
=======================
You can manually uninstall the plugin. 

Depending on the Office and Windows installation (32 or 64 Bit) you must delete a registry key
which can be found in one of these subtrees:
HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\Outlook\Addins\Codeplex.BackupAddIn
HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Office\Outlook\Addins\Codeplex.BackupAddIn

Delete the complete key named "Codeplex.BackupAddIn".

Another possibility is to run "BackupExecutor.exe /unregister" from the command line within the installation folder.

Afterwards you can also remove the installation folder.