Install-Shield Express is shipped with a bunch of setup prerequisites.
Among them is "Visual Studio 2010 Tools for Office". This additional dependency
is only properly working for Office 32 Bit.
For 64 Bit this dependency has to be adjusted / corrrected.

In order to fix this, you have to modify the prg-files within the installation folder
of Install-Shield express (subfolder \SetupPrerequisites). The conditions must be adjusted
to meet 32 or 64 Bit and the registry path for checking the installation. 
Whether 32-Bit-setup or 64-Bit-Setup must be chosen depends on the OS (not on the installed Office)!!

Within this project, the modified prq-files for 32 and 64 Bit are added.
You need to copy the two prq-files to the subfolder \SetupPrerequisites of Install-Shield Express.


See also:
http://stackoverflow.com/questions/15863267/how-to-redistribute-visual-studio-2010-tools-for-office-runtime-with-installshie