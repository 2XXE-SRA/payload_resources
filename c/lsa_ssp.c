// lsa ssp that does nothing
// based on: https://www.ired.team/offensive-security/credential-access-and-credential-dumping/intercepting-logon-credentials-via-custom-security-support-provider-and-authentication-package
// compile via: x86_64-w64-mingw32-gcc -shared -municode -o ssp.dll lsa_ssp.c -lsecur32
// then load with your preferred ssp loader

#define WIN32_NO_STATUS
#define SECURITY_WIN32
#include <windows.h>
#include <sspi.h>
#include <ntsecapi.h>
#include <ntsecpkg.h>

NTSTATUS NTAPI SpInitialize(ULONG_PTR PackageId, PSECPKG_PARAMETERS Parameters, PLSA_SECPKG_FUNCTION_TABLE FunctionTable) { return 0; }
NTSTATUS NTAPI SpShutDown(void) { return 0; }

NTSTATUS NTAPI SpGetInfo(PSecPkgInfoW PackageInfo)
{
	PackageInfo->Name = (SEC_WCHAR *)L"test";
	PackageInfo->Comment = (SEC_WCHAR *)L"test";
	PackageInfo->fCapabilities = SECPKG_FLAG_ACCEPT_WIN32_NAME | SECPKG_FLAG_CONNECTION;
	PackageInfo->wRPCID = SECPKG_ID_NONE;
	PackageInfo->cbMaxToken = 0;
	PackageInfo->wVersion = 1;
	return 0;
}

SECPKG_FUNCTION_TABLE SecurityPackageFunctionTable[] = 
{
	{
		NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,	SpInitialize, SpShutDown, SpGetInfo, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL 
	}
};

// SpLsaModeInitialize is called by LSA for each registered Security Package
__declspec(dllexport) NTSTATUS NTAPI SpLsaModeInitialize(ULONG LsaVersion, PULONG PackageVersion, PSECPKG_FUNCTION_TABLE *ppTables, PULONG pcTables)
{
	*PackageVersion = SECPKG_INTERFACE_VERSION;
	*ppTables = SecurityPackageFunctionTable;
	*pcTables = 1;
	return 0;
}
