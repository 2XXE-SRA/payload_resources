# DLL Sideloads

## MpCmdRun

MpCmdRun.exe DLL sideloading for MpClient.dll. 

This method is known to be used by Lockbit per https://www.sentinelone.com/blog/living-off-windows-defender-lockbit-ransomware-sideloads-cobalt-strike-through-microsoft-security-tool/

## Certutil

Certutil DLL sideload for NetApi32.dll

https://hijacklibs.net/entries/microsoft/built-in/netapi32.html

## Building

Using MinGW-w64: 

> x86_64-w64-mingw32-g++ < def > payload.cpp -o < dll > -shared -municode

Note: Source files created based on https://www.redteam.cafe/red-team/dll-sideloading/dll-sideloading-not-by-dllmain

