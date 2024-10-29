#include "windows.h"
#include <processthreadsapi.h>
#include <memoryapi.h>

#pragma comment(lib, "user32.lib")

void Payload()
{
    MessageBox(NULL, L"bar", L"foo", MB_OK);
}

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpReserved)
{
  switch (fdwReason)
    {
    case DLL_PROCESS_ATTACH:
      Payload();
      break;
    case DLL_THREAD_ATTACH:
      break;
    case DLL_THREAD_DETACH:
      break;
    case DLL_PROCESS_DETACH:
      break;
    }
  return TRUE;
}
