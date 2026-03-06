using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVROverlay
	{
		internal CVROverlay(IntPtr pInterface)
		{
			this.FnTable = (IVROverlay)Marshal.PtrToStructure(pInterface, typeof(IVROverlay));
		}

		public EVROverlayError FindOverlay(string pchOverlayKey, ref ulong pOverlayHandle)
		{
			IntPtr intPtr = Utils.ToUtf8(pchOverlayKey);
			pOverlayHandle = 0UL;
			EVROverlayError result = this.FnTable.FindOverlay(intPtr, ref pOverlayHandle);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVROverlayError CreateOverlay(string pchOverlayKey, string pchOverlayName, ref ulong pOverlayHandle)
		{
			IntPtr intPtr = Utils.ToUtf8(pchOverlayKey);
			IntPtr intPtr2 = Utils.ToUtf8(pchOverlayName);
			pOverlayHandle = 0UL;
			EVROverlayError result = this.FnTable.CreateOverlay(intPtr, intPtr2, ref pOverlayHandle);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return result;
		}

		public EVROverlayError DestroyOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.DestroyOverlay(ulOverlayHandle);
		}

		public uint GetOverlayKey(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError)
		{
			return this.FnTable.GetOverlayKey(ulOverlayHandle, pchValue, unBufferSize, ref pError);
		}

		public uint GetOverlayName(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError)
		{
			return this.FnTable.GetOverlayName(ulOverlayHandle, pchValue, unBufferSize, ref pError);
		}

		public EVROverlayError SetOverlayName(ulong ulOverlayHandle, string pchName)
		{
			IntPtr intPtr = Utils.ToUtf8(pchName);
			EVROverlayError result = this.FnTable.SetOverlayName(ulOverlayHandle, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVROverlayError GetOverlayImageData(ulong ulOverlayHandle, IntPtr pvBuffer, uint unBufferSize, ref uint punWidth, ref uint punHeight)
		{
			punWidth = 0U;
			punHeight = 0U;
			return this.FnTable.GetOverlayImageData(ulOverlayHandle, pvBuffer, unBufferSize, ref punWidth, ref punHeight);
		}

		public string GetOverlayErrorNameFromEnum(EVROverlayError error)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetOverlayErrorNameFromEnum(error));
		}

		public EVROverlayError SetOverlayRenderingPid(ulong ulOverlayHandle, uint unPID)
		{
			return this.FnTable.SetOverlayRenderingPid(ulOverlayHandle, unPID);
		}

		public uint GetOverlayRenderingPid(ulong ulOverlayHandle)
		{
			return this.FnTable.GetOverlayRenderingPid(ulOverlayHandle);
		}

		public EVROverlayError SetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, bool bEnabled)
		{
			return this.FnTable.SetOverlayFlag(ulOverlayHandle, eOverlayFlag, bEnabled);
		}

		public EVROverlayError GetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, ref bool pbEnabled)
		{
			pbEnabled = false;
			return this.FnTable.GetOverlayFlag(ulOverlayHandle, eOverlayFlag, ref pbEnabled);
		}

		public EVROverlayError GetOverlayFlags(ulong ulOverlayHandle, ref uint pFlags)
		{
			pFlags = 0U;
			return this.FnTable.GetOverlayFlags(ulOverlayHandle, ref pFlags);
		}

		public EVROverlayError SetOverlayColor(ulong ulOverlayHandle, float fRed, float fGreen, float fBlue)
		{
			return this.FnTable.SetOverlayColor(ulOverlayHandle, fRed, fGreen, fBlue);
		}

		public EVROverlayError GetOverlayColor(ulong ulOverlayHandle, ref float pfRed, ref float pfGreen, ref float pfBlue)
		{
			pfRed = 0f;
			pfGreen = 0f;
			pfBlue = 0f;
			return this.FnTable.GetOverlayColor(ulOverlayHandle, ref pfRed, ref pfGreen, ref pfBlue);
		}

		public EVROverlayError SetOverlayAlpha(ulong ulOverlayHandle, float fAlpha)
		{
			return this.FnTable.SetOverlayAlpha(ulOverlayHandle, fAlpha);
		}

		public EVROverlayError GetOverlayAlpha(ulong ulOverlayHandle, ref float pfAlpha)
		{
			pfAlpha = 0f;
			return this.FnTable.GetOverlayAlpha(ulOverlayHandle, ref pfAlpha);
		}

		public EVROverlayError SetOverlayTexelAspect(ulong ulOverlayHandle, float fTexelAspect)
		{
			return this.FnTable.SetOverlayTexelAspect(ulOverlayHandle, fTexelAspect);
		}

		public EVROverlayError GetOverlayTexelAspect(ulong ulOverlayHandle, ref float pfTexelAspect)
		{
			pfTexelAspect = 0f;
			return this.FnTable.GetOverlayTexelAspect(ulOverlayHandle, ref pfTexelAspect);
		}

		public EVROverlayError SetOverlaySortOrder(ulong ulOverlayHandle, uint unSortOrder)
		{
			return this.FnTable.SetOverlaySortOrder(ulOverlayHandle, unSortOrder);
		}

		public EVROverlayError GetOverlaySortOrder(ulong ulOverlayHandle, ref uint punSortOrder)
		{
			punSortOrder = 0U;
			return this.FnTable.GetOverlaySortOrder(ulOverlayHandle, ref punSortOrder);
		}

		public EVROverlayError SetOverlayWidthInMeters(ulong ulOverlayHandle, float fWidthInMeters)
		{
			return this.FnTable.SetOverlayWidthInMeters(ulOverlayHandle, fWidthInMeters);
		}

		public EVROverlayError GetOverlayWidthInMeters(ulong ulOverlayHandle, ref float pfWidthInMeters)
		{
			pfWidthInMeters = 0f;
			return this.FnTable.GetOverlayWidthInMeters(ulOverlayHandle, ref pfWidthInMeters);
		}

		public EVROverlayError SetOverlayCurvature(ulong ulOverlayHandle, float fCurvature)
		{
			return this.FnTable.SetOverlayCurvature(ulOverlayHandle, fCurvature);
		}

		public EVROverlayError GetOverlayCurvature(ulong ulOverlayHandle, ref float pfCurvature)
		{
			pfCurvature = 0f;
			return this.FnTable.GetOverlayCurvature(ulOverlayHandle, ref pfCurvature);
		}

		public EVROverlayError SetOverlayTextureColorSpace(ulong ulOverlayHandle, EColorSpace eTextureColorSpace)
		{
			return this.FnTable.SetOverlayTextureColorSpace(ulOverlayHandle, eTextureColorSpace);
		}

		public EVROverlayError GetOverlayTextureColorSpace(ulong ulOverlayHandle, ref EColorSpace peTextureColorSpace)
		{
			return this.FnTable.GetOverlayTextureColorSpace(ulOverlayHandle, ref peTextureColorSpace);
		}

		public EVROverlayError SetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds)
		{
			return this.FnTable.SetOverlayTextureBounds(ulOverlayHandle, ref pOverlayTextureBounds);
		}

		public EVROverlayError GetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds)
		{
			return this.FnTable.GetOverlayTextureBounds(ulOverlayHandle, ref pOverlayTextureBounds);
		}

		public EVROverlayError GetOverlayTransformType(ulong ulOverlayHandle, ref VROverlayTransformType peTransformType)
		{
			return this.FnTable.GetOverlayTransformType(ulOverlayHandle, ref peTransformType);
		}

		public EVROverlayError SetOverlayTransformAbsolute(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform)
		{
			return this.FnTable.SetOverlayTransformAbsolute(ulOverlayHandle, eTrackingOrigin, ref pmatTrackingOriginToOverlayTransform);
		}

		public EVROverlayError GetOverlayTransformAbsolute(ulong ulOverlayHandle, ref ETrackingUniverseOrigin peTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform)
		{
			return this.FnTable.GetOverlayTransformAbsolute(ulOverlayHandle, ref peTrackingOrigin, ref pmatTrackingOriginToOverlayTransform);
		}

		public EVROverlayError SetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, uint unTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform)
		{
			return this.FnTable.SetOverlayTransformTrackedDeviceRelative(ulOverlayHandle, unTrackedDevice, ref pmatTrackedDeviceToOverlayTransform);
		}

		public EVROverlayError GetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, ref uint punTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform)
		{
			punTrackedDevice = 0U;
			return this.FnTable.GetOverlayTransformTrackedDeviceRelative(ulOverlayHandle, ref punTrackedDevice, ref pmatTrackedDeviceToOverlayTransform);
		}

		public EVROverlayError SetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, uint unDeviceIndex, string pchComponentName)
		{
			IntPtr intPtr = Utils.ToUtf8(pchComponentName);
			EVROverlayError result = this.FnTable.SetOverlayTransformTrackedDeviceComponent(ulOverlayHandle, unDeviceIndex, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVROverlayError GetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, ref uint punDeviceIndex, StringBuilder pchComponentName, uint unComponentNameSize)
		{
			punDeviceIndex = 0U;
			return this.FnTable.GetOverlayTransformTrackedDeviceComponent(ulOverlayHandle, ref punDeviceIndex, pchComponentName, unComponentNameSize);
		}

		public EVROverlayError GetOverlayTransformOverlayRelative(ulong ulOverlayHandle, ref ulong ulOverlayHandleParent, ref HmdMatrix34_t pmatParentOverlayToOverlayTransform)
		{
			ulOverlayHandleParent = 0UL;
			return this.FnTable.GetOverlayTransformOverlayRelative(ulOverlayHandle, ref ulOverlayHandleParent, ref pmatParentOverlayToOverlayTransform);
		}

		public EVROverlayError SetOverlayTransformOverlayRelative(ulong ulOverlayHandle, ulong ulOverlayHandleParent, ref HmdMatrix34_t pmatParentOverlayToOverlayTransform)
		{
			return this.FnTable.SetOverlayTransformOverlayRelative(ulOverlayHandle, ulOverlayHandleParent, ref pmatParentOverlayToOverlayTransform);
		}

		public EVROverlayError SetOverlayTransformCursor(ulong ulCursorOverlayHandle, ref HmdVector2_t pvHotspot)
		{
			return this.FnTable.SetOverlayTransformCursor(ulCursorOverlayHandle, ref pvHotspot);
		}

		public EVROverlayError GetOverlayTransformCursor(ulong ulOverlayHandle, ref HmdVector2_t pvHotspot)
		{
			return this.FnTable.GetOverlayTransformCursor(ulOverlayHandle, ref pvHotspot);
		}

		public EVROverlayError ShowOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.ShowOverlay(ulOverlayHandle);
		}

		public EVROverlayError HideOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.HideOverlay(ulOverlayHandle);
		}

		public bool IsOverlayVisible(ulong ulOverlayHandle)
		{
			return this.FnTable.IsOverlayVisible(ulOverlayHandle);
		}

		public EVROverlayError GetTransformForOverlayCoordinates(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, HmdVector2_t coordinatesInOverlay, ref HmdMatrix34_t pmatTransform)
		{
			return this.FnTable.GetTransformForOverlayCoordinates(ulOverlayHandle, eTrackingOrigin, coordinatesInOverlay, ref pmatTransform);
		}

		public bool PollNextOverlayEvent(ulong ulOverlayHandle, ref VREvent_t pEvent, uint uncbVREvent)
		{
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				VREvent_t_Packed vrevent_t_Packed = default(VREvent_t_Packed);
				CVROverlay.PollNextOverlayEventUnion pollNextOverlayEventUnion;
				pollNextOverlayEventUnion.pPollNextOverlayEventPacked = null;
				pollNextOverlayEventUnion.pPollNextOverlayEvent = this.FnTable.PollNextOverlayEvent;
				bool result = pollNextOverlayEventUnion.pPollNextOverlayEventPacked(ulOverlayHandle, ref vrevent_t_Packed, (uint)Marshal.SizeOf(typeof(VREvent_t_Packed)));
				vrevent_t_Packed.Unpack(ref pEvent);
				return result;
			}
			return this.FnTable.PollNextOverlayEvent(ulOverlayHandle, ref pEvent, uncbVREvent);
		}

		public EVROverlayError GetOverlayInputMethod(ulong ulOverlayHandle, ref VROverlayInputMethod peInputMethod)
		{
			return this.FnTable.GetOverlayInputMethod(ulOverlayHandle, ref peInputMethod);
		}

		public EVROverlayError SetOverlayInputMethod(ulong ulOverlayHandle, VROverlayInputMethod eInputMethod)
		{
			return this.FnTable.SetOverlayInputMethod(ulOverlayHandle, eInputMethod);
		}

		public EVROverlayError GetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale)
		{
			return this.FnTable.GetOverlayMouseScale(ulOverlayHandle, ref pvecMouseScale);
		}

		public EVROverlayError SetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale)
		{
			return this.FnTable.SetOverlayMouseScale(ulOverlayHandle, ref pvecMouseScale);
		}

		public bool ComputeOverlayIntersection(ulong ulOverlayHandle, ref VROverlayIntersectionParams_t pParams, ref VROverlayIntersectionResults_t pResults)
		{
			return this.FnTable.ComputeOverlayIntersection(ulOverlayHandle, ref pParams, ref pResults);
		}

		public bool IsHoverTargetOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.IsHoverTargetOverlay(ulOverlayHandle);
		}

		public EVROverlayError SetOverlayIntersectionMask(ulong ulOverlayHandle, ref VROverlayIntersectionMaskPrimitive_t pMaskPrimitives, uint unNumMaskPrimitives, uint unPrimitiveSize)
		{
			return this.FnTable.SetOverlayIntersectionMask(ulOverlayHandle, ref pMaskPrimitives, unNumMaskPrimitives, unPrimitiveSize);
		}

		public EVROverlayError TriggerLaserMouseHapticVibration(ulong ulOverlayHandle, float fDurationSeconds, float fFrequency, float fAmplitude)
		{
			return this.FnTable.TriggerLaserMouseHapticVibration(ulOverlayHandle, fDurationSeconds, fFrequency, fAmplitude);
		}

		public EVROverlayError SetOverlayCursor(ulong ulOverlayHandle, ulong ulCursorHandle)
		{
			return this.FnTable.SetOverlayCursor(ulOverlayHandle, ulCursorHandle);
		}

		public EVROverlayError SetOverlayCursorPositionOverride(ulong ulOverlayHandle, ref HmdVector2_t pvCursor)
		{
			return this.FnTable.SetOverlayCursorPositionOverride(ulOverlayHandle, ref pvCursor);
		}

		public EVROverlayError ClearOverlayCursorPositionOverride(ulong ulOverlayHandle)
		{
			return this.FnTable.ClearOverlayCursorPositionOverride(ulOverlayHandle);
		}

		public EVROverlayError SetOverlayTexture(ulong ulOverlayHandle, ref Texture_t pTexture)
		{
			return this.FnTable.SetOverlayTexture(ulOverlayHandle, ref pTexture);
		}

		public EVROverlayError ClearOverlayTexture(ulong ulOverlayHandle)
		{
			return this.FnTable.ClearOverlayTexture(ulOverlayHandle);
		}

		public EVROverlayError SetOverlayRaw(ulong ulOverlayHandle, IntPtr pvBuffer, uint unWidth, uint unHeight, uint unBytesPerPixel)
		{
			return this.FnTable.SetOverlayRaw(ulOverlayHandle, pvBuffer, unWidth, unHeight, unBytesPerPixel);
		}

		public EVROverlayError SetOverlayFromFile(ulong ulOverlayHandle, string pchFilePath)
		{
			IntPtr intPtr = Utils.ToUtf8(pchFilePath);
			EVROverlayError result = this.FnTable.SetOverlayFromFile(ulOverlayHandle, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVROverlayError GetOverlayTexture(ulong ulOverlayHandle, ref IntPtr pNativeTextureHandle, IntPtr pNativeTextureRef, ref uint pWidth, ref uint pHeight, ref uint pNativeFormat, ref ETextureType pAPIType, ref EColorSpace pColorSpace, ref VRTextureBounds_t pTextureBounds)
		{
			pWidth = 0U;
			pHeight = 0U;
			pNativeFormat = 0U;
			return this.FnTable.GetOverlayTexture(ulOverlayHandle, ref pNativeTextureHandle, pNativeTextureRef, ref pWidth, ref pHeight, ref pNativeFormat, ref pAPIType, ref pColorSpace, ref pTextureBounds);
		}

		public EVROverlayError ReleaseNativeOverlayHandle(ulong ulOverlayHandle, IntPtr pNativeTextureHandle)
		{
			return this.FnTable.ReleaseNativeOverlayHandle(ulOverlayHandle, pNativeTextureHandle);
		}

		public EVROverlayError GetOverlayTextureSize(ulong ulOverlayHandle, ref uint pWidth, ref uint pHeight)
		{
			pWidth = 0U;
			pHeight = 0U;
			return this.FnTable.GetOverlayTextureSize(ulOverlayHandle, ref pWidth, ref pHeight);
		}

		public EVROverlayError CreateDashboardOverlay(string pchOverlayKey, string pchOverlayFriendlyName, ref ulong pMainHandle, ref ulong pThumbnailHandle)
		{
			IntPtr intPtr = Utils.ToUtf8(pchOverlayKey);
			IntPtr intPtr2 = Utils.ToUtf8(pchOverlayFriendlyName);
			pMainHandle = 0UL;
			pThumbnailHandle = 0UL;
			EVROverlayError result = this.FnTable.CreateDashboardOverlay(intPtr, intPtr2, ref pMainHandle, ref pThumbnailHandle);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return result;
		}

		public bool IsDashboardVisible()
		{
			return this.FnTable.IsDashboardVisible();
		}

		public bool IsActiveDashboardOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.IsActiveDashboardOverlay(ulOverlayHandle);
		}

		public EVROverlayError SetDashboardOverlaySceneProcess(ulong ulOverlayHandle, uint unProcessId)
		{
			return this.FnTable.SetDashboardOverlaySceneProcess(ulOverlayHandle, unProcessId);
		}

		public EVROverlayError GetDashboardOverlaySceneProcess(ulong ulOverlayHandle, ref uint punProcessId)
		{
			punProcessId = 0U;
			return this.FnTable.GetDashboardOverlaySceneProcess(ulOverlayHandle, ref punProcessId);
		}

		public void ShowDashboard(string pchOverlayToShow)
		{
			IntPtr intPtr = Utils.ToUtf8(pchOverlayToShow);
			this.FnTable.ShowDashboard(intPtr);
			Marshal.FreeHGlobal(intPtr);
		}

		public uint GetPrimaryDashboardDevice()
		{
			return this.FnTable.GetPrimaryDashboardDevice();
		}

		public EVROverlayError ShowKeyboard(int eInputMode, int eLineInputMode, uint unFlags, string pchDescription, uint unCharMax, string pchExistingText, ulong uUserValue)
		{
			IntPtr intPtr = Utils.ToUtf8(pchDescription);
			IntPtr intPtr2 = Utils.ToUtf8(pchExistingText);
			EVROverlayError result = this.FnTable.ShowKeyboard(eInputMode, eLineInputMode, unFlags, intPtr, unCharMax, intPtr2, uUserValue);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return result;
		}

		public EVROverlayError ShowKeyboardForOverlay(ulong ulOverlayHandle, int eInputMode, int eLineInputMode, uint unFlags, string pchDescription, uint unCharMax, string pchExistingText, ulong uUserValue)
		{
			IntPtr intPtr = Utils.ToUtf8(pchDescription);
			IntPtr intPtr2 = Utils.ToUtf8(pchExistingText);
			EVROverlayError result = this.FnTable.ShowKeyboardForOverlay(ulOverlayHandle, eInputMode, eLineInputMode, unFlags, intPtr, unCharMax, intPtr2, uUserValue);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return result;
		}

		public uint GetKeyboardText(StringBuilder pchText, uint cchText)
		{
			return this.FnTable.GetKeyboardText(pchText, cchText);
		}

		public void HideKeyboard()
		{
			this.FnTable.HideKeyboard();
		}

		public void SetKeyboardTransformAbsolute(ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToKeyboardTransform)
		{
			this.FnTable.SetKeyboardTransformAbsolute(eTrackingOrigin, ref pmatTrackingOriginToKeyboardTransform);
		}

		public void SetKeyboardPositionForOverlay(ulong ulOverlayHandle, HmdRect2_t avoidRect)
		{
			this.FnTable.SetKeyboardPositionForOverlay(ulOverlayHandle, avoidRect);
		}

		public VRMessageOverlayResponse ShowMessageOverlay(string pchText, string pchCaption, string pchButton0Text, string pchButton1Text, string pchButton2Text, string pchButton3Text)
		{
			IntPtr intPtr = Utils.ToUtf8(pchText);
			IntPtr intPtr2 = Utils.ToUtf8(pchCaption);
			IntPtr intPtr3 = Utils.ToUtf8(pchButton0Text);
			IntPtr intPtr4 = Utils.ToUtf8(pchButton1Text);
			IntPtr intPtr5 = Utils.ToUtf8(pchButton2Text);
			IntPtr intPtr6 = Utils.ToUtf8(pchButton3Text);
			VRMessageOverlayResponse result = this.FnTable.ShowMessageOverlay(intPtr, intPtr2, intPtr3, intPtr4, intPtr5, intPtr6);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			Marshal.FreeHGlobal(intPtr3);
			Marshal.FreeHGlobal(intPtr4);
			Marshal.FreeHGlobal(intPtr5);
			Marshal.FreeHGlobal(intPtr6);
			return result;
		}

		public void CloseMessageOverlay()
		{
			this.FnTable.CloseMessageOverlay();
		}

		private IVROverlay FnTable;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _PollNextOverlayEventPacked(ulong ulOverlayHandle, ref VREvent_t_Packed pEvent, uint uncbVREvent);

		[StructLayout(LayoutKind.Explicit)]
		private struct PollNextOverlayEventUnion
		{
			[FieldOffset(0)]
			public IVROverlay._PollNextOverlayEvent pPollNextOverlayEvent;

			[FieldOffset(0)]
			public CVROverlay._PollNextOverlayEventPacked pPollNextOverlayEventPacked;
		}
	}
}
