MpCmdRun.exe DLL sideloading for MpClient.dll. This method is known to be used by Lockbit per https://www.sentinelone.com/blog/living-off-windows-defender-lockbit-ransomware-sideloads-cobalt-strike-through-microsoft-security-tool/

Source files created based on https://www.redteam.cafe/red-team/dll-sideloading/dll-sideloading-not-by-dllmain

Build (using MinGW-w64): 

> x86_64-w64-mingw32-g++ mpclient.def payload.cpp -o mpclient.dll -shared -municode


