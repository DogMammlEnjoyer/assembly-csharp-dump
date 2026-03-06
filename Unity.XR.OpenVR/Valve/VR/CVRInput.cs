using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRInput
	{
		internal CVRInput(IntPtr pInterface)
		{
			this.FnTable = (IVRInput)Marshal.PtrToStructure(pInterface, typeof(IVRInput));
		}

		public EVRInputError SetActionManifestPath(string pchActionManifestPath)
		{
			IntPtr intPtr = Utils.ToUtf8(pchActionManifestPath);
			EVRInputError result = this.FnTable.SetActionManifestPath(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRInputError GetActionSetHandle(string pchActionSetName, ref ulong pHandle)
		{
			IntPtr intPtr = Utils.ToUtf8(pchActionSetName);
			pHandle = 0UL;
			EVRInputError result = this.FnTable.GetActionSetHandle(intPtr, ref pHandle);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRInputError GetActionHandle(string pchActionName, ref ulong pHandle)
		{
			IntPtr intPtr = Utils.ToUtf8(pchActionName);
			pHandle = 0UL;
			EVRInputError result = this.FnTable.GetActionHandle(intPtr, ref pHandle);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRInputError GetInputSourceHandle(string pchInputSourcePath, ref ulong pHandle)
		{
			IntPtr intPtr = Utils.ToUtf8(pchInputSourcePath);
			pHandle = 0UL;
			EVRInputError result = this.FnTable.GetInputSourceHandle(intPtr, ref pHandle);
			Marshal.FreeHGlobal(intPtr);
			return result;
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

		public EVRInputError GetPoseActionDataRelativeToNow(ulong action, ETrackingUniverseOrigin eOrigin, float fPredictedSecondsFromNow, ref InputPoseActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice)
		{
			return this.FnTable.GetPoseActionDataRelativeToNow(action, eOrigin, fPredictedSecondsFromNow, ref pActionData, unActionDataSize, ulRestrictToDevice);
		}

		public EVRInputError GetPoseActionDataForNextFrame(ulong action, ETrackingUniverseOrigin eOrigin, ref InputPoseActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice)
		{
			return this.FnTable.GetPoseActionDataForNextFrame(action, eOrigin, ref pActionData, unActionDataSize, ulRestrictToDevice);
		}

		public EVRInputError GetSkeletalActionData(ulong action, ref InputSkeletalActionData_t pActionData, uint unActionDataSize)
		{
			return this.FnTable.GetSkeletalActionData(action, ref pActionData, unActionDataSize);
		}

		public EVRInputError GetDominantHand(ref ETrackedControllerRole peDominantHand)
		{
			return this.FnTable.GetDominantHand(ref peDominantHand);
		}

		public EVRInputError SetDominantHand(ETrackedControllerRole eDominantHand)
		{
			return this.FnTable.SetDominantHand(eDominantHand);
		}

		public EVRInputError GetBoneCount(ulong action, ref uint pBoneCount)
		{
			pBoneCount = 0U;
			return this.FnTable.GetBoneCount(action, ref pBoneCount);
		}

		public EVRInputError GetBoneHierarchy(ulong action, int[] pParentIndices)
		{
			return this.FnTable.GetBoneHierarchy(action, pParentIndices, (uint)pParentIndices.Length);
		}

		public EVRInputError GetBoneName(ulong action, int nBoneIndex, StringBuilder pchBoneName, uint unNameBufferSize)
		{
			return this.FnTable.GetBoneName(action, nBoneIndex, pchBoneName, unNameBufferSize);
		}

		public EVRInputError GetSkeletalReferenceTransforms(ulong action, EVRSkeletalTransformSpace eTransformSpace, EVRSkeletalReferencePose eReferencePose, VRBoneTransform_t[] pTransformArray)
		{
			return this.FnTable.GetSkeletalReferenceTransforms(action, eTransformSpace, eReferencePose, pTransformArray, (uint)pTransformArray.Length);
		}

		public EVRInputError GetSkeletalTrackingLevel(ulong action, ref EVRSkeletalTrackingLevel pSkeletalTrackingLevel)
		{
			return this.FnTable.GetSkeletalTrackingLevel(action, ref pSkeletalTrackingLevel);
		}

		public EVRInputError GetSkeletalBoneData(ulong action, EVRSkeletalTransformSpace eTransformSpace, EVRSkeletalMotionRange eMotionRange, VRBoneTransform_t[] pTransformArray)
		{
			return this.FnTable.GetSkeletalBoneData(action, eTransformSpace, eMotionRange, pTransformArray, (uint)pTransformArray.Length);
		}

		public EVRInputError GetSkeletalSummaryData(ulong action, EVRSummaryType eSummaryType, ref VRSkeletalSummaryData_t pSkeletalSummaryData)
		{
			return this.FnTable.GetSkeletalSummaryData(action, eSummaryType, ref pSkeletalSummaryData);
		}

		public EVRInputError GetSkeletalBoneDataCompressed(ulong action, EVRSkeletalMotionRange eMotionRange, IntPtr pvCompressedData, uint unCompressedSize, ref uint punRequiredCompressedSize)
		{
			punRequiredCompressedSize = 0U;
			return this.FnTable.GetSkeletalBoneDataCompressed(action, eMotionRange, pvCompressedData, unCompressedSize, ref punRequiredCompressedSize);
		}

		public EVRInputError DecompressSkeletalBoneData(IntPtr pvCompressedBuffer, uint unCompressedBufferSize, EVRSkeletalTransformSpace eTransformSpace, VRBoneTransform_t[] pTransformArray)
		{
			return this.FnTable.DecompressSkeletalBoneData(pvCompressedBuffer, unCompressedBufferSize, eTransformSpace, pTransformArray, (uint)pTransformArray.Length);
		}

		public EVRInputError TriggerHapticVibrationAction(ulong action, float fStartSecondsFromNow, float fDurationSeconds, float fFrequency, float fAmplitude, ulong ulRestrictToDevice)
		{
			return this.FnTable.TriggerHapticVibrationAction(action, fStartSecondsFromNow, fDurationSeconds, fFrequency, fAmplitude, ulRestrictToDevice);
		}

		public EVRInputError GetActionOrigins(ulong actionSetHandle, ulong digitalActionHandle, ulong[] originsOut)
		{
			return this.FnTable.GetActionOrigins(actionSetHandle, digitalActionHandle, originsOut, (uint)originsOut.Length);
		}

		public EVRInputError GetOriginLocalizedName(ulong origin, StringBuilder pchNameArray, uint unNameArraySize, int unStringSectionsToInclude)
		{
			return this.FnTable.GetOriginLocalizedName(origin, pchNameArray, unNameArraySize, unStringSectionsToInclude);
		}

		public EVRInputError GetOriginTrackedDeviceInfo(ulong origin, ref InputOriginInfo_t pOriginInfo, uint unOriginInfoSize)
		{
			return this.FnTable.GetOriginTrackedDeviceInfo(origin, ref pOriginInfo, unOriginInfoSize);
		}

		public EVRInputError GetActionBindingInfo(ulong action, ref InputBindingInfo_t pOriginInfo, uint unBindingInfoSize, uint unBindingInfoCount, ref uint punReturnedBindingInfoCount)
		{
			punReturnedBindingInfoCount = 0U;
			return this.FnTable.GetActionBindingInfo(action, ref pOriginInfo, unBindingInfoSize, unBindingInfoCount, ref punReturnedBindingInfoCount);
		}

		public EVRInputError ShowActionOrigins(ulong actionSetHandle, ulong ulActionHandle)
		{
			return this.FnTable.ShowActionOrigins(actionSetHandle, ulActionHandle);
		}

		public EVRInputError ShowBindingsForActionSet(VRActiveActionSet_t[] pSets, uint unSizeOfVRSelectedActionSet_t, ulong originToHighlight)
		{
			return this.FnTable.ShowBindingsForActionSet(pSets, unSizeOfVRSelectedActionSet_t, (uint)pSets.Length, originToHighlight);
		}

		public EVRInputError GetComponentStateForBinding(string pchRenderModelName, string pchComponentName, ref InputBindingInfo_t pOriginInfo, uint unBindingInfoSize, uint unBindingInfoCount, ref RenderModel_ComponentState_t pComponentState)
		{
			IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
			IntPtr intPtr2 = Utils.ToUtf8(pchComponentName);
			EVRInputError result = this.FnTable.GetComponentStateForBinding(intPtr, intPtr2, ref pOriginInfo, unBindingInfoSize, unBindingInfoCount, ref pComponentState);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return result;
		}

		public bool IsUsingLegacyInput()
		{
			return this.FnTable.IsUsingLegacyInput();
		}

		public EVRInputError OpenBindingUI(string pchAppKey, ulong ulActionSetHandle, ulong ulDeviceHandle, bool bShowOnDesktop)
		{
			IntPtr intPtr = Utils.ToUtf8(pchAppKey);
			EVRInputError result = this.FnTable.OpenBindingUI(intPtr, ulActionSetHandle, ulDeviceHandle, bShowOnDesktop);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRInputError GetBindingVariant(ulong ulDevicePath, StringBuilder pchVariantArray, uint unVariantArraySize)
		{
			return this.FnTable.GetBindingVariant(ulDevicePath, pchVariantArray, unVariantArraySize);
		}

		private IVRInput FnTable;
	}
}
