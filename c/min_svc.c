/* Minimal Service Template

   cross-compile using mingw:
     x86_64-w64-mingw32-gcc service.c -s -o service.exe

   original source: https://raw.githubusercontent.com/tothi/malicious-service/master/service.c
*/

#include <windows.h>
#include <stdio.h>
#include <lm.h>

#define SERVICE_NAME "RandomService"

SERVICE_STATUS ServiceStatus; 
SERVICE_STATUS_HANDLE hStatus; 
  
void ServiceMain(int argc, char** argv); 
void ControlHandler(DWORD request); 
int InitService();

void Payload(void)
{
  printf("--Persistence--");
}

void main() 
{
  SERVICE_TABLE_ENTRY ServiceTable[] =
    {{ SERVICE_NAME, (LPSERVICE_MAIN_FUNCTION)ServiceMain },
     { NULL, NULL }};
  StartServiceCtrlDispatcher(ServiceTable);  
}
 
void ServiceMain(int argc, char** argv) 
{ 
  int error; 

  hStatus = RegisterServiceCtrlHandler(SERVICE_NAME, (LPHANDLER_FUNCTION)ControlHandler);
  
  if (hStatus == (SERVICE_STATUS_HANDLE)0) { return; }
  
  ServiceStatus.dwServiceType        = SERVICE_WIN32; 
  ServiceStatus.dwCurrentState       = SERVICE_START_PENDING; 
  ServiceStatus.dwControlsAccepted   = SERVICE_ACCEPT_STOP | SERVICE_ACCEPT_SHUTDOWN;
  ServiceStatus.dwWin32ExitCode      = 0; 
  ServiceStatus.dwServiceSpecificExitCode = 0; 
  ServiceStatus.dwCheckPoint         = 0; 
  ServiceStatus.dwWaitHint           = 0; 
  
  error = InitService(); 
  if (error) 
    {
      ServiceStatus.dwCurrentState       = SERVICE_STOPPED; 
      ServiceStatus.dwWin32ExitCode      = -1; 
      SetServiceStatus(hStatus, &ServiceStatus); 
      return; 
    } 

  ServiceStatus.dwCurrentState = SERVICE_RUNNING; 
  SetServiceStatus(hStatus, &ServiceStatus);
  
  while (ServiceStatus.dwCurrentState == SERVICE_RUNNING) {
    int result;
    
    while(1)
      {
	Sleep(300);         
      }
  }
  return; 
}
  
int InitService() 
{ 
    Payload();
    return 0;
} 
 
void ControlHandler(DWORD request)
{ 
  switch(request)
    { 
    case SERVICE_CONTROL_STOP: 
      ServiceStatus.dwWin32ExitCode = 0; 
      ServiceStatus.dwCurrentState  = SERVICE_STOPPED; 
      SetServiceStatus(hStatus, &ServiceStatus);
      return; 
  
    case SERVICE_CONTROL_SHUTDOWN: 
      ServiceStatus.dwWin32ExitCode = 0; 
      ServiceStatus.dwCurrentState  = SERVICE_STOPPED; 
      SetServiceStatus (hStatus, &ServiceStatus);
      return; 
         
    default:
      break;
    }

  SetServiceStatus (hStatus,  &ServiceStatus);
  
  return; 
}
