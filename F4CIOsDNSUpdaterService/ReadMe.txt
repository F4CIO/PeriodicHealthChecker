CraftSynth.PeriodicHealthChecker 1.0

What it does?
It periodicaly queries URL to HealthStatus.aspx on remote server. Then parses string returned by remote server. If any error or string is not as expexted alert mail is sent.

How it does?
Program have two parts:
1. F4CIOsDNSUpdater <-- A windows service. 
   It periodicaly reads '(start folder)\F4CIOsDNSUpdater.ini' 
   File contains interval in miliseconds, Uri and mail alert parameters.
   This part is doing all periodic processing.
2. F4CIOsDNSUpdaterManager <-- Windows tray manager application for testing and configuring .ini file.
   Note that changes are reflected by F4CIOsDNSUpdaterService on next reading from .ini file or on next restart of F4CIOsDNSUpdaterService.
   Since F4CIOsDNSUpdater service runs in background after every system start you don't need manager application after you have done configuration.

System requirements?
-Any windows NT operating system
-Microsoft .Net Framework 3.5

How to install?
1. Copy all files to destination folder.
2. Run InstallService.bat
3. Run F4CIOsDNSManager.exe
4. In F4CIOsDNSManager set interval, uri, mail parameters, test and start service.

More about it?
http://www.f4cio.com/periodic-website-health-checker-pinger
