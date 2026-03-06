using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRApplications
	{
		internal CVRApplications(IntPtr pInterface)
		{
			this.FnTable = (IVRApplications)Marshal.PtrToStructure(pInterface, typeof(IVRApplications));
		}

		public EVRApplicationError AddApplicationManifest(string pchApplicationManifestFullPath, bool bTemporary)
		{
			IntPtr intPtr = Utils.ToUtf8(pchApplicationManifestFullPath);
			EVRApplicationError result = this.FnTable.AddApplicationManifest(intPtr, bTemporary);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRApplicationError RemoveApplicationManifest(string pchApplicationManifestFullPath)
		{
			IntPtr intPtr = Utils.ToUtf8(pchApplicationManifestFullPath);
			EVRApplicationError result = this.FnTable.RemoveApplicationManifest(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public bool IsApplicationInstalled(string pchAppKey)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			bool result = this.FnTable.IsApplicationInstalled(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public uint GetApplicationCount()
		{
			return this.FnTable.GetApplicationCount();
		}

		public EVRApplicationError GetApplicationKeyByIndex(uint unApplicationIndex, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return this.FnTable.GetApplicationKeyByIndex(unApplicationIndex, pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public EVRApplicationError GetApplicationKeyByProcessId(uint unProcessId, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return this.FnTable.GetApplicationKeyByProcessId(unProcessId, pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public EVRApplicationError LaunchApplication(string pchAppKey)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			EVRApplicationError result = this.FnTable.LaunchApplication(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRApplicationError LaunchTemplateApplication(string pchTemplateAppKey, string pchNewAppKey, AppOverrideKeys_t[] pKeys)
		{
			IntPtr intPtr = Utils.ToUtf8(pchTemplateAppKey);
			IntPtr intPtr2 = Utils.ToUtf8(pchNewAppKey);
			EVRApplicationError result = this.FnTable.LaunchTemplateApplication(intPtr, intPtr2, pKeys, (uint)pKeys.Length);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return result;
		}

		public EVRApplicationError LaunchApplicationFromMimeType(string pchMimeType, string pchArgs)
		{
			IntPtr intPtr = Utils.ToUtf8(pchMimeType);
			IntPtr intPtr2 = Utils.ToUtf8(pchArgs);
			EVRApplicationError result = this.FnTable.LaunchApplicationFromMimeType(intPtr, intPtr2);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return result;
		}

		public EVRApplicationError LaunchDashboardOverlay(string pchAppKey)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			EVRApplicationError result = this.FnTable.LaunchDashboardOverlay(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public bool CancelApplicationLaunch(string pchAppKey)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			bool result = this.FnTable.CancelApplicationLaunch(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRApplicationError IdentifyApplication(uint unProcessId, string pchAppKey)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			EVRApplicationError result = this.FnTable.IdentifyApplication(unProcessId, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public uint GetApplicationProcessId(string pchAppKey)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			uint result = this.FnTable.GetApplicationProcessId(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public string GetApplicationsErrorNameFromEnum(EVRApplicationError error)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetApplicationsErrorNameFromEnum(error));
		}

		public uint GetApplicationPropertyString(string pchAppKey, EVRApplicationProperty eProperty, StringBuilder pchPropertyValueBuffer, uint unPropertyValueBufferLen, ref EVRApplicationError peError)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			uint result = this.FnTable.GetApplicationPropertyString(intPtr, eProperty, pchPropertyValueBuffer, unPropertyValueBufferLen, ref peError);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public bool GetApplicationPropertyBool(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			bool result = this.FnTable.GetApplicationPropertyBool(intPtr, eProperty, ref peError);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public ulong GetApplicationPropertyUint64(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			ulong result = this.FnTable.GetApplicationPropertyUint64(intPtr, eProperty, ref peError);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRApplicationError SetApplicationAutoLaunch(string pchAppKey, bool bAutoLaunch)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			EVRApplicationError result = this.FnTable.SetApplicationAutoLaunch(intPtr, bAutoLaunch);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public bool GetApplicationAutoLaunch(string pchAppKey)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			bool result = this.FnTable.GetApplicationAutoLaunch(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRApplicationError SetDefaultApplicationForMimeType(string pchAppKey, string pchMimeType)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			IntPtr intPtr2 = Utils.ToUtf8(pchMimeType);
			EVRApplicationError result = this.FnTable.SetDefaultApplicationForMimeType(intPtr, intPtr2);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return result;
		}

		public bool GetDefaultApplicationForMimeType(string pchMimeType, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			IntPtr intPtr = Utils.ToUtf8(pchMimeType);
			bool result = this.FnTable.GetDefaultApplicationForMimeType(intPtr, pchAppKeyBuffer, unAppKeyBufferLen);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public bool GetApplicationSupportedMimeTypes(string pchAppKey, StringBuilder pchMimeTypesBuffer, uint unMimeTypesBuffer)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			bool result = this.FnTable.GetApplicationSupportedMimeTypes(intPtr, pchMimeTypesBuffer, unMimeTypesBuffer);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public uint GetApplicationsThatSupportMimeType(string pchMimeType, StringBuilder pchAppKeysThatSupportBuffer, uint unAppKeysThatSupportBuffer)
		{
			IntPtr intPtr = Utils.ToUtf8(pchMimeType);
			uint result = this.FnTable.GetApplicationsThatSupportMimeType(intPtr, pchAppKeysThatSupportBuffer, unAppKeysThatSupportBuffer);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public uint GetApplicationLaunchArguments(uint unHandle, StringBuilder pchArgs, uint unArgs)
		{
			return this.FnTable.GetApplicationLaunchArguments(unHandle, pchArgs, unArgs);
		}

		public EVRApplicationError GetStartingApplication(StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return this.FnTable.GetStartingApplication(pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public EVRSceneApplicationState GetSceneApplicationState()
		{
			return this.FnTable.GetSceneApplicationState();
		}

		public EVRApplicationError PerformApplicationPrelaunchCheck(string pchAppKey)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			EVRApplicationError result = this.FnTable.PerformApplicationPrelaunchCheck(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public string GetSceneApplicationStateNameFromEnum(EVRSceneApplicationState state)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetSceneApplicationStateNameFromEnum(state));
		}

		public EVRApplicationError LaunchInternalProcess(string pchBinaryPath, string pchArguments, string pchWorkingDirectory)
		{
			IntPtr intPtr = Utils.ToUtf8(pchBinaryPath);
			IntPtr intPtr2 = Utils.ToUtf8(pchArguments);
			IntPtr intPtr3 = Utils.ToUtf8(pchWorkingDirectory);
			EVRApplicationError result = this.FnTable.LaunchInternalProcess(intPtr, intPtr2, intPtr3);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			Marshal.FreeHGlobal(intPtr3);
			return result;
		}

		public uint GetCurrentSceneProcessId()
		{
			return this.FnTable.GetCurrentSceneProcessId();
		}

		private IVRApplications FnTable;
	}
}
