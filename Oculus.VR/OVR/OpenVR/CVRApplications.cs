using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public class CVRApplications
	{
		internal CVRApplications(IntPtr pInterface)
		{
			this.FnTable = (IVRApplications)Marshal.PtrToStructure(pInterface, typeof(IVRApplications));
		}

		public EVRApplicationError AddApplicationManifest(string pchApplicationManifestFullPath, bool bTemporary)
		{
			return this.FnTable.AddApplicationManifest(pchApplicationManifestFullPath, bTemporary);
		}

		public EVRApplicationError RemoveApplicationManifest(string pchApplicationManifestFullPath)
		{
			return this.FnTable.RemoveApplicationManifest(pchApplicationManifestFullPath);
		}

		public bool IsApplicationInstalled(string pchAppKey)
		{
			return this.FnTable.IsApplicationInstalled(pchAppKey);
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
			return this.FnTable.LaunchApplication(pchAppKey);
		}

		public EVRApplicationError LaunchTemplateApplication(string pchTemplateAppKey, string pchNewAppKey, AppOverrideKeys_t[] pKeys)
		{
			return this.FnTable.LaunchTemplateApplication(pchTemplateAppKey, pchNewAppKey, pKeys, (uint)pKeys.Length);
		}

		public EVRApplicationError LaunchApplicationFromMimeType(string pchMimeType, string pchArgs)
		{
			return this.FnTable.LaunchApplicationFromMimeType(pchMimeType, pchArgs);
		}

		public EVRApplicationError LaunchDashboardOverlay(string pchAppKey)
		{
			return this.FnTable.LaunchDashboardOverlay(pchAppKey);
		}

		public bool CancelApplicationLaunch(string pchAppKey)
		{
			return this.FnTable.CancelApplicationLaunch(pchAppKey);
		}

		public EVRApplicationError IdentifyApplication(uint unProcessId, string pchAppKey)
		{
			return this.FnTable.IdentifyApplication(unProcessId, pchAppKey);
		}

		public uint GetApplicationProcessId(string pchAppKey)
		{
			return this.FnTable.GetApplicationProcessId(pchAppKey);
		}

		public string GetApplicationsErrorNameFromEnum(EVRApplicationError error)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetApplicationsErrorNameFromEnum(error));
		}

		public uint GetApplicationPropertyString(string pchAppKey, EVRApplicationProperty eProperty, StringBuilder pchPropertyValueBuffer, uint unPropertyValueBufferLen, ref EVRApplicationError peError)
		{
			return this.FnTable.GetApplicationPropertyString(pchAppKey, eProperty, pchPropertyValueBuffer, unPropertyValueBufferLen, ref peError);
		}

		public bool GetApplicationPropertyBool(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError)
		{
			return this.FnTable.GetApplicationPropertyBool(pchAppKey, eProperty, ref peError);
		}

		public ulong GetApplicationPropertyUint64(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError)
		{
			return this.FnTable.GetApplicationPropertyUint64(pchAppKey, eProperty, ref peError);
		}

		public EVRApplicationError SetApplicationAutoLaunch(string pchAppKey, bool bAutoLaunch)
		{
			return this.FnTable.SetApplicationAutoLaunch(pchAppKey, bAutoLaunch);
		}

		public bool GetApplicationAutoLaunch(string pchAppKey)
		{
			return this.FnTable.GetApplicationAutoLaunch(pchAppKey);
		}

		public EVRApplicationError SetDefaultApplicationForMimeType(string pchAppKey, string pchMimeType)
		{
			return this.FnTable.SetDefaultApplicationForMimeType(pchAppKey, pchMimeType);
		}

		public bool GetDefaultApplicationForMimeType(string pchMimeType, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return this.FnTable.GetDefaultApplicationForMimeType(pchMimeType, pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public bool GetApplicationSupportedMimeTypes(string pchAppKey, StringBuilder pchMimeTypesBuffer, uint unMimeTypesBuffer)
		{
			return this.FnTable.GetApplicationSupportedMimeTypes(pchAppKey, pchMimeTypesBuffer, unMimeTypesBuffer);
		}

		public uint GetApplicationsThatSupportMimeType(string pchMimeType, StringBuilder pchAppKeysThatSupportBuffer, uint unAppKeysThatSupportBuffer)
		{
			return this.FnTable.GetApplicationsThatSupportMimeType(pchMimeType, pchAppKeysThatSupportBuffer, unAppKeysThatSupportBuffer);
		}

		public uint GetApplicationLaunchArguments(uint unHandle, StringBuilder pchArgs, uint unArgs)
		{
			return this.FnTable.GetApplicationLaunchArguments(unHandle, pchArgs, unArgs);
		}

		public EVRApplicationError GetStartingApplication(StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return this.FnTable.GetStartingApplication(pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public EVRApplicationTransitionState GetTransitionState()
		{
			return this.FnTable.GetTransitionState();
		}

		public EVRApplicationError PerformApplicationPrelaunchCheck(string pchAppKey)
		{
			return this.FnTable.PerformApplicationPrelaunchCheck(pchAppKey);
		}

		public string GetApplicationsTransitionStateNameFromEnum(EVRApplicationTransitionState state)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetApplicationsTransitionStateNameFromEnum(state));
		}

		public bool IsQuitUserPromptRequested()
		{
			return this.FnTable.IsQuitUserPromptRequested();
		}

		public EVRApplicationError LaunchInternalProcess(string pchBinaryPath, string pchArguments, string pchWorkingDirectory)
		{
			return this.FnTable.LaunchInternalProcess(pchBinaryPath, pchArguments, pchWorkingDirectory);
		}

		public uint GetCurrentSceneProcessId()
		{
			return this.FnTable.GetCurrentSceneProcessId();
		}

		private IVRApplications FnTable;
	}
}
