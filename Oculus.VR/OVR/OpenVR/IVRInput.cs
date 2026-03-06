using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public struct IVRInput
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._SetActionManifestPath SetActionManifestPath;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetActionSetHandle GetActionSetHandle;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetActionHandle GetActionHandle;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetInputSourceHandle GetInputSourceHandle;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._UpdateActionState UpdateActionState;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetDigitalActionData GetDigitalActionData;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetAnalogActionData GetAnalogActionData;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetPoseActionData GetPoseActionData;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetSkeletalActionData GetSkeletalActionData;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetSkeletalBoneData GetSkeletalBoneData;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetSkeletalBoneDataCompressed GetSkeletalBoneDataCompressed;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._DecompressSkeletalBoneData DecompressSkeletalBoneData;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._TriggerHapticVibrationAction TriggerHapticVibrationAction;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetActionOrigins GetActionOrigins;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetOriginLocalizedName GetOriginLocalizedName;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._GetOriginTrackedDeviceInfo GetOriginTrackedDeviceInfo;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._ShowActionOrigins ShowActionOrigins;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRInput._ShowBindingsForActionSet ShowBindingsForActionSet;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _SetActionManifestPath(string pchActionManifestPath);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetActionSetHandle(string pchActionSetName, ref ulong pHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetActionHandle(string pchActionName, ref ulong pHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetInputSourceHandle(string pchInputSourcePath, ref ulong pHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _UpdateActionState([In] [Out] VRActiveActionSet_t[] pSets, uint unSizeOfVRSelectedActionSet_t, uint unSetCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetDigitalActionData(ulong action, ref InputDigitalActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetAnalogActionData(ulong action, ref InputAnalogActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetPoseActionData(ulong action, ETrackingUniverseOrigin eOrigin, float fPredictedSecondsFromNow, ref InputPoseActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetSkeletalActionData(ulong action, ref InputSkeletalActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetSkeletalBoneData(ulong action, EVRSkeletalTransformSpace eTransformSpace, EVRSkeletalMotionRange eMotionRange, [In] [Out] VRBoneTransform_t[] pTransformArray, uint unTransformArrayCount, ulong ulRestrictToDevice);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetSkeletalBoneDataCompressed(ulong action, EVRSkeletalTransformSpace eTransformSpace, EVRSkeletalMotionRange eMotionRange, IntPtr pvCompressedData, uint unCompressedSize, ref uint punRequiredCompressedSize, ulong ulRestrictToDevice);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _DecompressSkeletalBoneData(IntPtr pvCompressedBuffer, uint unCompressedBufferSize, ref EVRSkeletalTransformSpace peTransformSpace, [In] [Out] VRBoneTransform_t[] pTransformArray, uint unTransformArrayCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _TriggerHapticVibrationAction(ulong action, float fStartSecondsFromNow, float fDurationSeconds, float fFrequency, float fAmplitude, ulong ulRestrictToDevice);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetActionOrigins(ulong actionSetHandle, ulong digitalActionHandle, [In] [Out] ulong[] originsOut, uint originOutCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetOriginLocalizedName(ulong origin, StringBuilder pchNameArray, uint unNameArraySize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _GetOriginTrackedDeviceInfo(ulong origin, ref InputOriginInfo_t pOriginInfo, uint unOriginInfoSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _ShowActionOrigins(ulong actionSetHandle, ulong ulActionHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRInputError _ShowBindingsForActionSet([In] [Out] VRActiveActionSet_t[] pSets, uint unSizeOfVRSelectedActionSet_t, uint unSetCount, ulong originToHighlight);
	}
}
