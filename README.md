# TimeLogger
Windows time logging

## Background
This module was written to make my own life easier - no one likes doing timesheets, but this automates the whole process by installing as a Windows service. This module writes all your timesheets to `C:\Temp\Timesheets\`.

##Logging activity
It logs the following events:
* System startup
* System shutdown
* System lock
* System unlock

The tool also logs that your system is on every 5 minutes. 

## Summaries
Additionally, it summarises your activity in the following ways:
* Daily activity - all the individual events for that day, including a summary of the hours/minutes your PC has been on for that day
* Monthly activity - a log of the hours/minutes your PC has been on all month