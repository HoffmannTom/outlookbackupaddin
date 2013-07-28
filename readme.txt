
Installation requirements:
==========================
 - .Net Framework 4.5
 - Visual Studio 2010 Tools for Office Runtime
   see:
   http://www.microsoft.com/en-us/download/details.aspx?id=39290
 - Installed Outlook 2013


Installation instructions:
==========================
1) Unzip the download to a destination folder, e.g. c:\Program Files\Codeplex\Outlook-Backup
2) Run Setup.exe and follow the instructions on the screen
3) A certificate warning will show up. Accept the certificate in order to get the plugin installed


Usage instruction:
==================
1) Open Outlook and choose "Backup" on the ribbon bar
2) Open the settings window and select the psd-files, the time interval (in days), target Folder and Location 
    of the file "backupexecutor.exe" (e.g. c:\Program Files\Codeplex\Outlook-Backup\backupexecutor.exe)
    Afterwards press "save" to save your settings.
3) When you Exit Outlook the first time, the backup should start automatically
