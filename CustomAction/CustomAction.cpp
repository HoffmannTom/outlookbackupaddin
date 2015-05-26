
#include "stdafx.h"
#include <msi.h>

#include <msiquery.h>
#include "Windows.h"
#include <iostream>
#include <string>

//Fetch Reg_sz from registry from HKLM taking 32/64-Bit-view into account
static WCHAR* getRegistryEntry(LPCWSTR sPath, int iRegView)
{
	BYTE *pData = NULL;
	HKEY hk;
	int result = RegOpenKeyEx(HKEY_LOCAL_MACHINE, sPath, 0, KEY_QUERY_VALUE | iRegView, &hk);
	if (result == ERROR_SUCCESS)
	{
		DWORD  size;
		result = RegQueryValueEx(hk, NULL, NULL, NULL, NULL, &size);
		if (result == ERROR_SUCCESS)
		{
			pData = (BYTE*)malloc(size);
			result = RegQueryValueEx(hk, NULL, NULL, NULL, pData, &size);
			if (result == ERROR_SUCCESS)
			{
				//std::wcout << (WCHAR*)pData;
			}
			else WcaLog(LOGMSG_STANDARD, "Failed to read default value from %ls", sPath);
		}
		else WcaLog(LOGMSG_STANDARD, "Failed to get size of %ls", sPath);
	}
	else WcaLog(LOGMSG_STANDARD, "Failed to open %ls, view %i", sPath, iRegView);

	RegCloseKey(hk);
	return (WCHAR*)pData;
}

//Check whether OS supports 64 Bit
static bool is64bitOS()
{
	typedef void (WINAPI *PGNSI)(LPSYSTEM_INFO);

	//targetver.h _WIN32_WINNT could also be increased to support GetNativeSystemInfo without fetching pointer
	SYSTEM_INFO si;
	PGNSI gsi = (PGNSI)GetProcAddress(GetModuleHandle(TEXT("kernel32.dll")), "GetNativeSystemInfo");
	gsi(&si);

	return si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_IA64
		|| si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64;
}

//Fetch outlook path from registry
static WCHAR* getOutlookPath()
{
	WCHAR* sPath = NULL;

	WCHAR* clsidp = getRegistryEntry(L"Software\\Classes\\Outlook.Application\\CLSID", 0);
	std::wcout << clsidp;

	if (clsidp != NULL)
	{
		std::wcout << clsidp;

		std::wstring wlsid(clsidp);
		std::wstring regEntry = L"Software\\Classes\\CLSID\\" + wlsid + L"\\LocalServer32";
		sPath = getRegistryEntry(regEntry.c_str(), 0);

		//not found in 32Bit View --> try 64Bit View
		if (sPath == NULL && is64bitOS())
		{
			sPath = getRegistryEntry(regEntry.c_str(), KEY_WOW64_64KEY);
		}

		free(clsidp);
		clsidp = NULL;
	}
	return sPath;
}
/* deprecated way --> WcaLog */
/*
static bool logInfo(MSIHANDLE hInstall, const WCHAR* info)
{
	PMSIHANDLE hRecord = ::MsiCreateRecord(1);
	if (hRecord != NULL)
	{
		UINT er = ::MsiRecordSetString(hRecord, 0, info);
		if (ERROR_SUCCESS == er)
		{
			er = ::MsiProcessMessage(hInstall, INSTALLMESSAGE_INFO, hRecord);
			return true;
		}
	}

	return false;
}
*/

static UINT getOfficeBits(int &iBits, bool bSetProperties)
{
	iBits = 0;

	WCHAR* sPath = getOutlookPath();
	//Formate siehe: http://msdn.microsoft.com/en-us/library/windows/desktop/ms647550%28v=vs.85%29.aspx
	WcaLog(LOGMSG_STANDARD, "Outlook-Path: %ls", sPath);

	//Outlook not found
	if (sPath == NULL)
		return ERROR_PATH_NOT_FOUND;

	if (bSetProperties)
		WcaSetProperty(L"OFFICE_PATH", sPath);

	bool is64 = false;
	if (is64bitOS())
	{
		try
		{
			DWORD Type;
			if (GetBinaryType(sPath, &Type))
				is64 = (Type == SCS_64BIT_BINARY);
		}
		catch (int)
		{
			// Ignore - better just to assume it's 32-bit than to let the installation
			// fail.  This could fail because the GetBinaryType function is not
			// available. XP / 2003 onwards are supported.
		}
	}

	WcaLog(LOGMSG_STANDARD, "Detected Office: %i Bit", is64 ? 64 : 32);

	if (is64) iBits = 64;
	else iBits = 32;

	return ERROR_SUCCESS;
}

UINT __stdcall CheckOfficeBits(MSIHANDLE hInstall)
{
	HRESULT hr = S_OK;
	UINT er = ERROR_SUCCESS;

	hr = WcaInitialize(hInstall, "CheckOfficeBits");
	ExitOnFailure(hr, "Failed to initialize");

	WcaLog(LOGMSG_STANDARD, "CheckOfficeBits initialized.");

	int iBits = 0;
	hr = getOfficeBits(iBits, true);
	ExitOnFailure(hr, "Path not found");

	//http://blogs.technet.com/b/alexshev/archive/2009/05/15/from-msi-to-wix-part-22-dll-custom-actions-introduction.aspx 
	WcaSetIntProperty(L"OFFICE_BITS", iBits);

LExit:
	er = SUCCEEDED(hr) ? ERROR_SUCCESS : ERROR_INSTALL_FAILURE;
	return WcaFinalize(er);
}

UINT __stdcall RegisterPlugin(MSIHANDLE hInstall)
{
	HRESULT hr = S_OK;
	UINT er = ERROR_SUCCESS;

	hr = WcaInitialize(hInstall, "RegisterPlugin");
	ExitOnFailure(hr, "Failed to initialize");

	WcaLog(LOGMSG_STANDARD, "RegisterPlugin...");

	int iBits = 0;
	hr = getOfficeBits(iBits, false);
	ExitOnFailure(hr, "Path not found");

	int iView = 0;
	if (iBits == 64)
	{
		iView = KEY_WOW64_64KEY;
		WcaLog(LOGMSG_STANDARD, "Register 64 Bit Plugin %i", iBits);
	}
	else WcaLog(LOGMSG_STANDARD, "Register 32 Bit Plugin %i", iBits);

	// http://blogs.msdn.com/b/icumove/archive/2009/06/23/custom-action-using-wix-reading-from-the-binary-table.aspx
	LPWSTR pszCustData = NULL;
	HRESULT hr2 = WcaGetProperty(L"CustomActionData", &pszCustData);
	ExitOnFailure(hr2, "Failed to get CustomActionData");

	HKEY hk;
	{
		wchar_t *context = NULL;
		LPWSTR pszPluginName = wcstok_s(pszCustData, L";", &context);
		std::wstring wsPluginName(pszPluginName);
		LPWSTR pszSourcePath = wcstok_s(NULL, L";", &context);

		WcaLog(LOGMSG_STANDARD, "Param1 %ls", pszPluginName);
		WcaLog(LOGMSG_STANDARD, "Param2 %ls", pszSourcePath);

		int res = RegOpenKeyEx(HKEY_LOCAL_MACHINE, (L"Software\\Microsoft\\Office\\Outlook\\Addins\\" + wsPluginName).c_str(), NULL, KEY_ALL_ACCESS | iView, &hk);
		if (res == ERROR_FILE_NOT_FOUND)
		{
			WcaLog(LOGMSG_STANDARD, "Creating Registrykey");
			res = RegCreateKeyEx(HKEY_LOCAL_MACHINE, (L"Software\\Microsoft\\Office\\Outlook\\Addins\\" + wsPluginName).c_str(), NULL, NULL, REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS | iView, NULL, &hk, NULL);
			ExitOnWin32Error(res, hr, "Error creating Registrykey:" + res);
		}
		ExitOnWin32Error(res, hr, "Error Opening key:" + res);

		std::wstring val = wsPluginName + TEXT(" - Plugin");
		res = RegSetValueEx(hk, L"Description", NULL, REG_SZ, (LPBYTE)val.c_str(), wcslen(val.c_str()) * sizeof(TCHAR) + 1);
		ExitOnWin32Error(res, hr, "Error creating Description:" + res);

		std::wstring val2 = wsPluginName + TEXT(" - Plugin");
		res = RegSetValueEx(hk, L"FriendlyName", NULL, REG_SZ, (LPBYTE)val2.c_str(), wcslen(val2.c_str()) * sizeof(TCHAR) + 1);
		ExitOnWin32Error(res, hr, "Error creating FriendlyName:" + res);

		DWORD val3 = 3;
		res = RegSetValueEx(hk, L"LoadBehavior", NULL, REG_DWORD, (LPBYTE)&val3, sizeof(REG_DWORD));
		ExitOnWin32Error(res, hr, "Error creating LoadBehavior:" + res);

		//can cause: Exception: Exception reading manifest from .../VSTA/Pipeline.v10.0/PipelineSegments.store: the manifest may not be valid or the file could not be opened.
		//DWORD val4 = 1;
		//res = RegSetValueEx(hk, L"Warmup", NULL, REG_DWORD, (LPBYTE)&val4, sizeof(REG_DWORD));
		//ExitOnWin32Error(res, hr, "Error creating Warmup:" + res);

		WcaLog(LOGMSG_STANDARD, "Manifest-Entry: %ls", pszSourcePath);
		res = RegSetValueEx(hk, L"Manifest", NULL, REG_SZ, (LPBYTE)pszSourcePath, wcslen(pszSourcePath) * sizeof(TCHAR) + 1);
		ExitOnWin32Error(res, hr, "Error creating Manifest:" + res);

		/*
		WIX-Properties speichern:
		http://robmensching.com/blog/posts/2010/5/2/the-wix-toolsets-remember-property-pattern/
		*/
	}

LExit:
	RegCloseKey(hk);
	//free(pszSourcePath);
	er = SUCCEEDED(hr) ? ERROR_SUCCESS : ERROR_INSTALL_FAILURE;
	return WcaFinalize(er);
}


UINT __stdcall UnregisterPlugin(MSIHANDLE hInstall)
{
	HRESULT hr = S_OK;
	UINT er = ERROR_SUCCESS;
	std::wstring wsPluginName;

	hr = WcaInitialize(hInstall, "UnregisterPlugin");
	ExitOnFailure(hr, "Failed to initialize");

	int iBits = 0;
	hr = getOfficeBits(iBits, false);
	ExitOnFailure(hr, "Path not found");

	int iView = 0;
	if (iBits == 64)
	{
		iView = KEY_WOW64_64KEY;
		WcaLog(LOGMSG_STANDARD, "Unregistering 64 Bit Plugin %i", iBits);
	}
	else WcaLog(LOGMSG_STANDARD, "Unregistering 32 Bit Plugin %i", iBits);

	LPWSTR pszPluginName = NULL;
	HRESULT hr2 = WcaGetProperty(L"CustomActionData", &pszPluginName);
	ExitOnFailure(hr2, "Failed to get CustomActionData");

	WcaLog(LOGMSG_STANDARD, "UnregisterPlugin... %ls", pszPluginName);

	{
		std::wstring wsPluginName(pszPluginName);
		int res = RegDeleteKeyEx(HKEY_LOCAL_MACHINE, (L"Software\\Microsoft\\Office\\Outlook\\Addins\\" + wsPluginName).c_str(), iView, 0);
		//ExitOnWin32Error(res, hr, "Error deleting key:" + res);
		if (res != ERROR_SUCCESS)
		{
			WcaLog(LOGMSG_STANDARD, "RegDeleteKeyEx failed %i", res);
		}
	}
LExit:
	er = SUCCEEDED(hr) ? ERROR_SUCCESS : ERROR_INSTALL_FAILURE;
	return WcaFinalize(er);
}


// DllMain - Initialize and cleanup WiX custom action utils.
extern "C" BOOL WINAPI DllMain(
	__in HINSTANCE hInst,
	__in ULONG ulReason,
	__in LPVOID
	)
{
	switch(ulReason)
	{
	case DLL_PROCESS_ATTACH:
		WcaGlobalInitialize(hInst);
		break;

	case DLL_PROCESS_DETACH:
		WcaGlobalFinalize();
		break;
	}

	return TRUE;
}
