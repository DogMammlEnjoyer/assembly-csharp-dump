using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public struct IVRApplications
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._AddApplicationManifest AddApplicationManifest;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._RemoveApplicationManifest RemoveApplicationManifest;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._IsApplicationInstalled IsApplicationInstalled;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationCount GetApplicationCount;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationKeyByIndex GetApplicationKeyByIndex;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationKeyByProcessId GetApplicationKeyByProcessId;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._LaunchApplication LaunchApplication;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._LaunchTemplateApplication LaunchTemplateApplication;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._LaunchApplicationFromMimeType LaunchApplicationFromMimeType;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._LaunchDashboardOverlay LaunchDashboardOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._CancelApplicationLaunch CancelApplicationLaunch;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._IdentifyApplication IdentifyApplication;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationProcessId GetApplicationProcessId;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationsErrorNameFromEnum GetApplicationsErrorNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationPropertyString GetApplicationPropertyString;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationPropertyBool GetApplicationPropertyBool;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationPropertyUint64 GetApplicationPropertyUint64;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._SetApplicationAutoLaunch SetApplicationAutoLaunch;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationAutoLaunch GetApplicationAutoLaunch;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._SetDefaultApplicationForMimeType SetDefaultApplicationForMimeType;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetDefaultApplicationForMimeType GetDefaultApplicationForMimeType;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationSupportedMimeTypes GetApplicationSupportedMimeTypes;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationsThatSupportMimeType GetApplicationsThatSupportMimeType;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationLaunchArguments GetApplicationLaunchArguments;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetStartingApplication GetStartingApplication;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetSceneApplicationState GetSceneApplicationState;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._PerformApplicationPrelaunchCheck PerformApplicationPrelaunchCheck;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetSceneApplicationStateNameFromEnum GetSceneApplicationStateNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._LaunchInternalProcess LaunchInternalProcess;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetCurrentSceneProcessId GetCurrentSceneProcessId;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _AddApplicationManifest(IntPtr pchApplicationManifestFullPath, bool bTemporary);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _RemoveApplicationManifest(IntPtr pchApplicationManifestFullPath);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsApplicationInstalled(IntPtr pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetApplicationCount();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _GetApplicationKeyByIndex(uint unApplicationIndex, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _GetApplicationKeyByProcessId(uint unProcessId, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _LaunchApplication(IntPtr pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _LaunchTemplateApplication(IntPtr pchTemplateAppKey, IntPtr pchNewAppKey, [In] [Out] AppOverrideKeys_t[] pKeys, uint unKeys);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _LaunchApplicationFromMimeType(IntPtr pchMimeType, IntPtr pchArgs);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _LaunchDashboardOverlay(IntPtr pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _CancelApplicationLaunch(IntPtr pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _IdentifyApplication(uint unProcessId, IntPtr pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetApplicationProcessId(IntPtr pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetApplicationsErrorNameFromEnum(EVRApplicationError error);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetApplicationPropertyString(IntPtr pchAppKey, EVRApplicationProperty eProperty, StringBuilder pchPropertyValueBuffer, uint unPropertyValueBufferLen, ref EVRApplicationError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetApplicationPropertyBool(IntPtr pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ulong _GetApplicationPropertyUint64(IntPtr pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _SetApplicationAutoLaunch(IntPtr pchAppKey, bool bAutoLaunch);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetApplicationAutoLaunch(IntPtr pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _SetDefaultApplicationForMimeType(IntPtr pchAppKey, IntPtr pchMimeType);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetDefaultApplicationForMimeType(IntPtr pchMimeType, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetApplicationSupportedMimeTypes(IntPtr pchAppKey, StringBuilder pchMimeTypesBuffer, uint unMimeTypesBuffer);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetApplicationsThatSupportMimeType(IntPtr pchMimeType, StringBuilder pchAppKeysThatSupportBuffer, uint unAppKeysThatSupportBuffer);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetApplicationLaunchArguments(uint unHandle, StringBuilder pchArgs, uint unArgs);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _GetStartingApplication(StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRSceneApplicationState _GetSceneApplicationState();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _PerformApplicationPrelaunchCheck(IntPtr pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetSceneApplicationStateNameFromEnum(EVRSceneApplicationState state);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _LaunchInternalProcess(IntPtr pchBinaryPath, IntPtr pchArguments, IntPtr pchWorkingDirectory);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetCurrentSceneProcessId();
	}
}
