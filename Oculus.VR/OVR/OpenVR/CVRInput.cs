using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public class CVRInput
	{
		internal CVRInput(IntPtr pInterface)
		{
			this.FnTable = (IVRInput)Marshal.PtrToStructure(pInterface, typeof(IVRInput));
		}

		public EVRInputError SetActionManifestPath(string pchActionManifestPath)
		{
			return this.FnTable.SetActionManifestPath(pchActionManifestPath);
		}

		public EVRInputError GetActionSetHandle(string pchActionSetName, ref ulong pHandle)
		{
			pHandle = 0UL;
			return this.FnTable.GetActionSetHandle(pchActionSetName, ref pHandle);
		}

		public EVRInputError GetActionHandle(string pchActionName, ref ulong pHandle)
		{
			pHandle = 0UL;
			return this.FnTable.GetActionHandle(pchActionName, ref pHandle);
		}

		public EVRInputError GetInputSourceHandle(string pchInputSourcePath, ref ulong pHandle)
		{
			pHandle = 0UL;
			return this.FnTable.GetInputSourceHandle(pchInputSourcePath, ref pHandle);
		}

		public EVRInputError UpdateActionState(VRActiveActionSet_t[] pSets, uint unSizeOfVRSelectedActionSet_t)
		{
			return this.FnTable.UpdateActionState(pSets, unSizeOfVRSelectedActionSet_t, (uint)pSets.Length);
		}

		public EVRInputError GetDigitalActionData(ulong action, ref InputDigitalActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice)
		{
			return this.FnTable.GetDigitalActionData(action, ref pActionData, unActionDataSize, ulRestrictToDevice);
		}

		public EVRInputError GetAnalogActionData(ulong action, ref InputAnalogActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice)
		{
			return this.FnTable.GetAnalogActionData(action, ref pActionData, unActionDataSize, ulRestrictToDevice);
		}

		public EVRInputError GetPoseActionData(ulong action, ETrackingUniverseOrigin eOrigin, float fPredictedSecondsFromNow, ref InputPoseActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice)
		{
			return this.FnTable.GetPoseActionData(action, eOrigin, fPredictedSecondsFromNow, ref pActionData, unActionDataSize, ulRestrictToDevice);
		}

		public EVRInputError GetSkeletalActionData(ulong action, ref InputSkeletalActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice)
		{
			return this.FnTable.GetSkeletalActionData(action, ref pActionData, unActionDataSize, ulRestrictToDevice);
		}

		public EVRInputError GetSkeletalBoneData(ulong action, EVRSkeletalTransformSpace eTransformSpace, EVRSkeletalMotionRange eMotionRange, VRBoneTransform_t[] pTransformArray, ulong ulRestrictToDevice)
		{
			return this.FnTable.GetSkeletalBoneData(action, eTransformSpace, eMotionRange, pTransformArray, (uint)pTransformArray.Length, ulRestrictToDevice);
		}

		public EVRInputError GetSkeletalBoneDataCompressed(ulong action, EVRSkeletalTransformSpace eTransformSpace, EVRSkeletalMotionRange eMotionRange, IntPtr pvCompressedData, uint unCompressedSize, ref uint punRequiredCompressedSize, ulong ulRestrictToDevice)
		{
			punRequiredCompressedSize = 0U;
			return this.FnTable.GetSkeletalBoneDataCompressed(action, eTransformSpace, eMotionRange, pvCompressedData, unCompressedSize, ref punRequiredCompressedSize, ulRestrictToDevice);
		}

		public EVRInputError DecompressSkeletalBoneData(IntPtr pvCompressedBuffer, uint unCompressedBufferSize, ref EVRSkeletalTransformSpace peTransformSpace, VRBoneTransform_t[] pTransformArray)
		{
			return this.FnTable.DecompressSkeletalBoneData(pvCompressedBuffer, unCompressedBufferSize, ref peTransformSpace, pTransformArray, (uint)pTransformArray.Length);
		}

		public EVRInputError TriggerHapticVibrationAction(ulong action, float fStartSecondsFromNow, float fDurationSeconds, float fFrequency, float fAmplitude, ulong ulRestrictToDevice)
		{
			return this.FnTable.TriggerHapticVibrationAction(action, fStartSecondsFromNow, fDurationSeconds, fFrequency, fAmplitude, ulRestrictToDevice);
		}

		public EVRInputError GetActionOrigins(ulong actionSetHandle, ulong digitalActionHandle, ulong[] originsOut)
		{
			return this.FnTable.GetActionOrigins(actionSetHandle, digitalActionHandle, originsOut, (uint)originsOut.Length);
		}

		public EVRInputError GetOriginLocalizedName(ulong origin, StringBuilder pchNameArray, uint unNameArraySize)
		{
			return this.FnTable.GetOriginLocalizedName(origin, pchNameArray, unNameArraySize);
		}

		public EVRInputError GetOriginTrackedDeviceInfo(ulong origin, ref InputOriginInfo_t pOriginInfo, uint unOriginInfoSize)
		{
			return this.FnTable.GetOriginTrackedDeviceInfo(origin, ref pOriginInfo, unOriginInfoSize);
		}

		public EVRInputError ShowActionOrigins(ulong actionSetHandle, ulong ulActionHandle)
		{
			return this.FnTable.ShowActionOrigins(actionSetHandle, ulActionHandle);
		}

		public EVRInputError ShowBindingsForActionSet(VRActiveActionSet_t[] pSets, uint unSizeOfVRSelectedActionSet_t, ulong originToHighlight)
		{
			return this.FnTable.ShowBindingsForActionSet(pSets, unSizeOfVRSelectedActionSet_t, (uint)pSets.Length, originToHighlight);
		}

		private IVRInput FnTable;
	}
}
