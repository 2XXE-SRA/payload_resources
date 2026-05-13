#include "windows.h"
#include <processthreadsapi.h>
#include <memoryapi.h>
#include <cstdint>

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

extern "C" __declspec(dllexport) DWORD InitHelperDll(DWORD dwNetshVersion, PVOID pReserved){
  Payload();
  return 0;
}

unsigned long PrintUIEntryW(struct HWND__* __ptr64 arg1, struct HINSTANCE__* __ptr64 arg2, uint16_t const* __ptr64 arg3, uint32_t arg4){
	Payload();
	return 0;
}