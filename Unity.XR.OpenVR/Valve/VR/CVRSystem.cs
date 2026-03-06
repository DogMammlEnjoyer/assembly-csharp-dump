using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRSystem
	{
		internal CVRSystem(IntPtr pInterface)
		{
			this.FnTable = (IVRSystem)Marshal.PtrToStructure(pInterface, typeof(IVRSystem));
		}

		public void GetRecommendedRenderTargetSize(ref uint pnWidth, ref uint pnHeight)
		{
			pnWidth = 0U;
			pnHeight = 0U;
			this.FnTable.GetRecommendedRenderTargetSize(ref pnWidth, ref pnHeight);
		}

		public HmdMatrix44_t GetProjectionMatrix(EVREye eEye, float fNearZ, float fFarZ)
		{
			return this.FnTable.GetProjectionMatrix(eEye, fNearZ, fFarZ);
		}

		public void GetProjectionRaw(EVREye eEye, ref float pfLeft, ref float pfRight, ref float pfTop, ref float pfBottom)
		{
			pfLeft = 0f;
			pfRight = 0f;
			pfTop = 0f;
			pfBottom = 0f;
			this.FnTable.GetProjectionRaw(eEye, ref pfLeft, ref pfRight, ref pfTop, ref pfBottom);
		}

		public bool ComputeDistortion(EVREye eEye, float fU, float fV, ref DistortionCoordinates_t pDistortionCoordinates)
		{
			return this.FnTable.ComputeDistortion(eEye, fU, fV, ref pDistortionCoordinates);
		}

		public HmdMatrix34_t GetEyeToHeadTransform(EVREye eEye)
		{
			return this.FnTable.GetEyeToHeadTransform(eEye);
		}

		public bool GetTimeSinceLastVsync(ref float pfSecondsSinceLastVsync, ref ulong pulFrameCounter)
		{
			pfSecondsSinceLastVsync = 0f;
			pulFrameCounter = 0UL;
			return this.FnTable.GetTimeSinceLastVsync(ref pfSecondsSinceLastVsync, ref pulFrameCounter);
		}

		public int GetD3D9AdapterIndex()
		{
			return this.FnTable.GetD3D9AdapterIndex();
		}

		public void GetDXGIOutputInfo(ref int pnAdapterIndex)
		{
			pnAdapterIndex = 0;
			this.FnTable.GetDXGIOutputInfo(ref pnAdapterIndex);
		}

		public void GetOutputDevice(ref ulong pnDevice, ETextureType textureType, IntPtr pInstance)
		{
			pnDevice = 0UL;
			this.FnTable.GetOutputDevice(ref pnDevice, textureType, pInstance);
		}

		public bool IsDisplayOnDesktop()
		{
			return this.FnTable.IsDisplayOnDesktop();
		}

		public bool SetDisplayVisibility(bool bIsVisibleOnDesktop)
		{
			return this.FnTable.SetDisplayVisibility(bIsVisibleOnDesktop);
		}

		public void GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin eOrigin, float fPredictedSecondsToPhotonsFromNow, TrackedDevicePose_t[] pTrackedDevicePoseArray)
		{
			this.FnTable.GetDeviceToAbsoluteTrackingPose(eOrigin, fPredictedSecondsToPhotonsFromNow, pTrackedDevicePoseArray, (uint)pTrackedDevicePoseArray.Length);
		}

		public HmdMatrix34_t GetSeatedZeroPoseToStandingAbsoluteTrackingPose()
		{
			return this.FnTable.GetSeatedZeroPoseToStandingAbsoluteTrackingPose();
		}

		public HmdMatrix34_t GetRawZeroPoseToStandingAbsoluteTrackingPose()
		{
			return this.FnTable.GetRawZeroPoseToStandingAbsoluteTrackingPose();
		}

		public uint GetSortedTrackedDeviceIndicesOfClass(ETrackedDeviceClass eTrackedDeviceClass, uint[] punTrackedDeviceIndexArray, uint unRelativeToTrackedDeviceIndex)
		{
			return this.FnTable.GetSortedTrackedDeviceIndicesOfClass(eTrackedDeviceClass, punTrackedDeviceIndexArray, (uint)punTrackedDeviceIndexArray.Length, unRelativeToTrackedDeviceIndex);
		}

		public EDeviceActivityLevel GetTrackedDeviceActivityLevel(uint unDeviceId)
		{
			return this.FnTable.GetTrackedDeviceActivityLevel(unDeviceId);
		}

		public void ApplyTransform(ref TrackedDevicePose_t pOutputPose, ref TrackedDevicePose_t pTrackedDevicePose, ref HmdMatrix34_t pTransform)
		{
			this.FnTable.ApplyTransform(ref pOutputPose, ref pTrackedDevicePose, ref pTransform);
		}

		public uint GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole unDeviceType)
		{
			return this.FnTable.GetTrackedDeviceIndexForControllerRole(unDeviceType);
		}

		public ETrackedControllerRole GetControllerRoleForTrackedDeviceIndex(uint unDeviceIndex)
		{
			return this.FnTable.GetControllerRoleForTrackedDeviceIndex(unDeviceIndex);
		}

		public ETrackedDeviceClass GetTrackedDeviceClass(uint unDeviceIndex)
		{
			return this.FnTable.GetTrackedDeviceClass(unDeviceIndex);
		}

		public bool IsTrackedDeviceConnected(uint unDeviceIndex)
		{
			return this.FnTable.IsTrackedDeviceConnected(unDeviceIndex);
		}

		public bool GetBoolTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetBoolTrackedDeviceProperty(unDeviceIndex, prop, ref pError);
		}

		public float GetFloatTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetFloatTrackedDeviceProperty(unDeviceIndex, prop, ref pError);
		}

		public int GetInt32TrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetInt32TrackedDeviceProperty(unDeviceIndex, prop, ref pError);
		}

		public ulong GetUint64TrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetUint64TrackedDeviceProperty(unDeviceIndex, prop, ref pError);
		}

		public HmdMatrix34_t GetMatrix34TrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetMatrix34TrackedDeviceProperty(unDeviceIndex, prop, ref pError);
		}

		public uint GetArrayTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, uint propType, IntPtr pBuffer, uint unBufferSize, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetArrayTrackedDeviceProperty(unDeviceIndex, prop, propType, pBuffer, unBufferSize, ref pError);
		}

		public uint GetStringTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, StringBuilder pchValue, uint unBufferSize, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetStringTrackedDeviceProperty(unDeviceIndex, prop, pchValue, unBufferSize, ref pError);
		}

		public string GetPropErrorNameFromEnum(ETrackedPropertyError error)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetPropErrorNameFromEnum(error));
		}

		public bool PollNextEvent(ref VREvent_t pEvent, uint uncbVREvent)
		{
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				VREvent_t_Packed vrevent_t_Packed = default(VREvent_t_Packed);
				CVRSystem.PollNextEventUnion pollNextEventUnion;
				pollNextEventUnion.pPollNextEventPacked = null;
				pollNextEventUnion.pPollNextEvent = this.FnTable.PollNextEvent;
				bool result = pollNextEventUnion.pPollNextEventPacked(ref vrevent_t_Packed, (uint)Marshal.SizeOf(typeof(VREvent_t_Packed)));
				vrevent_t_Packed.Unpack(ref pEvent);
				return result;
			}
			return this.FnTable.PollNextEvent(ref pEvent, uncbVREvent);
		}

		public bool PollNextEventWithPose(ETrackingUniverseOrigin eOrigin, ref VREvent_t pEvent, uint uncbVREvent, ref TrackedDevicePose_t pTrackedDevicePose)
		{
			return this.FnTable.PollNextEventWithPose(eOrigin, ref pEvent, uncbVREvent, ref pTrackedDevicePose);
		}

		public string GetEventTypeNameFromEnum(EVREventType eType)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetEventTypeNameFromEnum(eType));
		}

		public HiddenAreaMesh_t GetHiddenAreaMesh(EVREye eEye, EHiddenAreaMeshType type)
		{
			return this.FnTable.GetHiddenAreaMesh(eEye, type);
		}

		public bool GetControllerState(uint unControllerDeviceIndex, ref VRControllerState_t pControllerState, uint unControllerStateSize)
		{
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				VRControllerState_t_Packed vrcontrollerState_t_Packed = new VRControllerState_t_Packed(pControllerState);
				CVRSystem.GetControllerStateUnion getControllerStateUnion;
				getControllerStateUnion.pGetControllerStatePacked = null;
				getControllerStateUnion.pGetControllerState = this.FnTable.GetControllerState;
				bool result = getControllerStateUnion.pGetControllerStatePacked(unControllerDeviceIndex, ref vrcontrollerState_t_Packed, (uint)Marshal.SizeOf(typeof(VRControllerState_t_Packed)));
				vrcontrollerState_t_Packed.Unpack(ref pControllerState);
				return result;
			}
			return this.FnTable.GetControllerState(unControllerDeviceIndex, ref pControllerState, unControllerStateSize);
		}

		public bool GetControllerStateWithPose(ETrackingUniverseOrigin eOrigin, uint unControllerDeviceIndex, ref VRControllerState_t pControllerState, uint unControllerStateSize, ref TrackedDevicePose_t pTrackedDevicePose)
		{
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				VRControllerState_t_Packed vrcontrollerState_t_Packed = new VRControllerState_t_Packed(pControllerState);
				CVRSystem.GetControllerStateWithPoseUnion getControllerStateWithPoseUnion;
				getControllerStateWithPoseUnion.pGetControllerStateWithPosePacked = null;
				getControllerStateWithPoseUnion.pGetControllerStateWithPose = this.FnTable.GetControllerStateWithPose;
				bool result = getControllerStateWithPoseUnion.pGetControllerStateWithPosePacked(eOrigin, unControllerDeviceIndex, ref vrcontrollerState_t_Packed, (uint)Marshal.SizeOf(typeof(VRControllerState_t_Packed)), ref pTrackedDevicePose);
				vrcontrollerState_t_Packed.Unpack(ref pControllerState);
				return result;
			}
			return this.FnTable.GetControllerStateWithPose(eOrigin, unControllerDeviceIndex, ref pControllerState, unControllerStateSize, ref pTrackedDevicePose);
		}

		public void TriggerHapticPulse(uint unControllerDeviceIndex, uint unAxisId, ushort usDurationMicroSec)
		{
			this.FnTable.TriggerHapticPulse(unControllerDeviceIndex, unAxisId, usDurationMicroSec);
		}

		public string GetButtonIdNameFromEnum(EVRButtonId eButtonId)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetButtonIdNameFromEnum(eButtonId));
		}

		public string GetControllerAxisTypeNameFromEnum(EVRControllerAxisType eAxisType)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetControllerAxisTypeNameFromEnum(eAxisType));
		}

		public bool IsInputAvailable()
		{
			return this.FnTable.IsInputAvailable();
		}

		public bool IsSteamVRDrawingControllers()
		{
			return this.FnTable.IsSteamVRDrawingControllers();
		}

		public bool ShouldApplicationPause()
		{
			return this.FnTable.ShouldApplicationPause();
		}

		public bool ShouldApplicationReduceRenderingWork()
		{
			return this.FnTable.ShouldApplicationReduceRenderingWork();
		}

		public EVRFirmwareError PerformFirmwareUpdate(uint unDeviceIndex)
		{
			return this.FnTable.PerformFirmwareUpdate(unDeviceIndex);
		}

		public void AcknowledgeQuit_Exiting()
		{
			this.FnTable.AcknowledgeQuit_Exiting();
		}

		public uint GetAppContainerFilePaths(StringBuilder pchBuffer, uint unBufferSize)
		{
			return this.FnTable.GetAppContainerFilePaths(pchBuffer, unBufferSize);
		}

		public string GetRuntimeVersion()
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetRuntimeVersion());
		}

		private IVRSystem FnTable;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _PollNextEventPacked(ref VREvent_t_Packed pEvent, uint uncbVREvent);

		[StructLayout(LayoutKind.Explicit)]
		private struct PollNextEventUnion
		{
			[FieldOffset(0)]
			public IVRSystem._PollNextEvent pPollNextEvent;

			[FieldOffset(0)]
			public CVRSystem._PollNextEventPacked pPollNextEventPacked;
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetControllerStatePacked(uint unControllerDeviceIndex, ref VRControllerState_t_Packed pControllerState, uint unControllerStateSize);

		[StructLayout(LayoutKind.Explicit)]
		private struct GetControllerStateUnion
		{
			[FieldOffset(0)]
			public IVRSystem._GetControllerState pGetControllerState;

			[FieldOffset(0)]
			public CVRSystem._GetControllerStatePacked pGetControllerStatePacked;
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetControllerStateWithPosePacked(ETrackingUniverseOrigin eOrigin, uint unControllerDeviceIndex, ref VRControllerState_t_Packed pControllerState, uint unControllerStateSize, ref TrackedDevicePose_t pTrackedDevicePose);

		[StructLayout(LayoutKind.Explicit)]
		private struct GetControllerStateWithPoseUnion
		{
			[FieldOffset(0)]
			public IVRSystem._GetControllerStateWithPose pGetControllerStateWithPose;

			[FieldOffset(0)]
			public CVRSystem._GetControllerStateWithPosePacked pGetControllerStateWithPosePacked;
		}
	}
}
