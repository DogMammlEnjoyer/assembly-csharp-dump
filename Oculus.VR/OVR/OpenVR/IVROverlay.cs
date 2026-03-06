using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public struct IVROverlay
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._FindOverlay FindOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._CreateOverlay CreateOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._DestroyOverlay DestroyOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetHighQualityOverlay SetHighQualityOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetHighQualityOverlay GetHighQualityOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayKey GetOverlayKey;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayName GetOverlayName;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayName SetOverlayName;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayImageData GetOverlayImageData;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayErrorNameFromEnum GetOverlayErrorNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayRenderingPid SetOverlayRenderingPid;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayRenderingPid GetOverlayRenderingPid;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayFlag SetOverlayFlag;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayFlag GetOverlayFlag;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayColor SetOverlayColor;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayColor GetOverlayColor;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayAlpha SetOverlayAlpha;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayAlpha GetOverlayAlpha;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayTexelAspect SetOverlayTexelAspect;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayTexelAspect GetOverlayTexelAspect;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlaySortOrder SetOverlaySortOrder;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlaySortOrder GetOverlaySortOrder;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayWidthInMeters SetOverlayWidthInMeters;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayWidthInMeters GetOverlayWidthInMeters;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayAutoCurveDistanceRangeInMeters SetOverlayAutoCurveDistanceRangeInMeters;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayAutoCurveDistanceRangeInMeters GetOverlayAutoCurveDistanceRangeInMeters;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayTextureColorSpace SetOverlayTextureColorSpace;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayTextureColorSpace GetOverlayTextureColorSpace;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayTextureBounds SetOverlayTextureBounds;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayTextureBounds GetOverlayTextureBounds;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayRenderModel GetOverlayRenderModel;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayRenderModel SetOverlayRenderModel;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayTransformType GetOverlayTransformType;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayTransformAbsolute SetOverlayTransformAbsolute;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayTransformAbsolute GetOverlayTransformAbsolute;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayTransformTrackedDeviceRelative SetOverlayTransformTrackedDeviceRelative;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayTransformTrackedDeviceRelative GetOverlayTransformTrackedDeviceRelative;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayTransformTrackedDeviceComponent SetOverlayTransformTrackedDeviceComponent;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayTransformTrackedDeviceComponent GetOverlayTransformTrackedDeviceComponent;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayTransformOverlayRelative GetOverlayTransformOverlayRelative;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayTransformOverlayRelative SetOverlayTransformOverlayRelative;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._ShowOverlay ShowOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._HideOverlay HideOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._IsOverlayVisible IsOverlayVisible;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetTransformForOverlayCoordinates GetTransformForOverlayCoordinates;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._PollNextOverlayEvent PollNextOverlayEvent;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayInputMethod GetOverlayInputMethod;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayInputMethod SetOverlayInputMethod;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayMouseScale GetOverlayMouseScale;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayMouseScale SetOverlayMouseScale;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._ComputeOverlayIntersection ComputeOverlayIntersection;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._IsHoverTargetOverlay IsHoverTargetOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetGamepadFocusOverlay GetGamepadFocusOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetGamepadFocusOverlay SetGamepadFocusOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayNeighbor SetOverlayNeighbor;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._MoveGamepadFocusToNeighbor MoveGamepadFocusToNeighbor;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayDualAnalogTransform SetOverlayDualAnalogTransform;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayDualAnalogTransform GetOverlayDualAnalogTransform;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayTexture SetOverlayTexture;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._ClearOverlayTexture ClearOverlayTexture;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayRaw SetOverlayRaw;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayFromFile SetOverlayFromFile;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayTexture GetOverlayTexture;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._ReleaseNativeOverlayHandle ReleaseNativeOverlayHandle;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayTextureSize GetOverlayTextureSize;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._CreateDashboardOverlay CreateDashboardOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._IsDashboardVisible IsDashboardVisible;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._IsActiveDashboardOverlay IsActiveDashboardOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetDashboardOverlaySceneProcess SetDashboardOverlaySceneProcess;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetDashboardOverlaySceneProcess GetDashboardOverlaySceneProcess;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._ShowDashboard ShowDashboard;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetPrimaryDashboardDevice GetPrimaryDashboardDevice;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._ShowKeyboard ShowKeyboard;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._ShowKeyboardForOverlay ShowKeyboardForOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetKeyboardText GetKeyboardText;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._HideKeyboard HideKeyboard;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetKeyboardTransformAbsolute SetKeyboardTransformAbsolute;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetKeyboardPositionForOverlay SetKeyboardPositionForOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._SetOverlayIntersectionMask SetOverlayIntersectionMask;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._GetOverlayFlags GetOverlayFlags;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._ShowMessageOverlay ShowMessageOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlay._CloseMessageOverlay CloseMessageOverlay;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _FindOverlay(string pchOverlayKey, ref ulong pOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _CreateOverlay(string pchOverlayKey, string pchOverlayName, ref ulong pOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _DestroyOverlay(ulong ulOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetHighQualityOverlay(ulong ulOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ulong _GetHighQualityOverlay();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetOverlayKey(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetOverlayName(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayName(ulong ulOverlayHandle, string pchName);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayImageData(ulong ulOverlayHandle, IntPtr pvBuffer, uint unBufferSize, ref uint punWidth, ref uint punHeight);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetOverlayErrorNameFromEnum(EVROverlayError error);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayRenderingPid(ulong ulOverlayHandle, uint unPID);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetOverlayRenderingPid(ulong ulOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, bool bEnabled);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, ref bool pbEnabled);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayColor(ulong ulOverlayHandle, float fRed, float fGreen, float fBlue);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayColor(ulong ulOverlayHandle, ref float pfRed, ref float pfGreen, ref float pfBlue);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayAlpha(ulong ulOverlayHandle, float fAlpha);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayAlpha(ulong ulOverlayHandle, ref float pfAlpha);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayTexelAspect(ulong ulOverlayHandle, float fTexelAspect);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayTexelAspect(ulong ulOverlayHandle, ref float pfTexelAspect);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlaySortOrder(ulong ulOverlayHandle, uint unSortOrder);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlaySortOrder(ulong ulOverlayHandle, ref uint punSortOrder);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayWidthInMeters(ulong ulOverlayHandle, float fWidthInMeters);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayWidthInMeters(ulong ulOverlayHandle, ref float pfWidthInMeters);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayAutoCurveDistanceRangeInMeters(ulong ulOverlayHandle, float fMinDistanceInMeters, float fMaxDistanceInMeters);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayAutoCurveDistanceRangeInMeters(ulong ulOverlayHandle, ref float pfMinDistanceInMeters, ref float pfMaxDistanceInMeters);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayTextureColorSpace(ulong ulOverlayHandle, EColorSpace eTextureColorSpace);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayTextureColorSpace(ulong ulOverlayHandle, ref EColorSpace peTextureColorSpace);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetOverlayRenderModel(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref HmdColor_t pColor, ref EVROverlayError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayRenderModel(ulong ulOverlayHandle, string pchRenderModel, ref HmdColor_t pColor);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayTransformType(ulong ulOverlayHandle, ref VROverlayTransformType peTransformType);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayTransformAbsolute(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayTransformAbsolute(ulong ulOverlayHandle, ref ETrackingUniverseOrigin peTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, uint unTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, ref uint punTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, uint unDeviceIndex, string pchComponentName);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, ref uint punDeviceIndex, StringBuilder pchComponentName, uint unComponentNameSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayTransformOverlayRelative(ulong ulOverlayHandle, ref ulong ulOverlayHandleParent, ref HmdMatrix34_t pmatParentOverlayToOverlayTransform);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayTransformOverlayRelative(ulong ulOverlayHandle, ulong ulOverlayHandleParent, ref HmdMatrix34_t pmatParentOverlayToOverlayTransform);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _ShowOverlay(ulong ulOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _HideOverlay(ulong ulOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsOverlayVisible(ulong ulOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetTransformForOverlayCoordinates(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, HmdVector2_t coordinatesInOverlay, ref HmdMatrix34_t pmatTransform);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _PollNextOverlayEvent(ulong ulOverlayHandle, ref VREvent_t pEvent, uint uncbVREvent);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayInputMethod(ulong ulOverlayHandle, ref VROverlayInputMethod peInputMethod);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayInputMethod(ulong ulOverlayHandle, VROverlayInputMethod eInputMethod);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _ComputeOverlayIntersection(ulong ulOverlayHandle, ref VROverlayIntersectionParams_t pParams, ref VROverlayIntersectionResults_t pResults);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsHoverTargetOverlay(ulong ulOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ulong _GetGamepadFocusOverlay();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetGamepadFocusOverlay(ulong ulNewFocusOverlay);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayNeighbor(EOverlayDirection eDirection, ulong ulFrom, ulong ulTo);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _MoveGamepadFocusToNeighbor(EOverlayDirection eDirection, ulong ulFrom);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayDualAnalogTransform(ulong ulOverlay, EDualAnalogWhich eWhich, IntPtr vCenter, float fRadius);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayDualAnalogTransform(ulong ulOverlay, EDualAnalogWhich eWhich, ref HmdVector2_t pvCenter, ref float pfRadius);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayTexture(ulong ulOverlayHandle, ref Texture_t pTexture);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _ClearOverlayTexture(ulong ulOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayRaw(ulong ulOverlayHandle, IntPtr pvBuffer, uint unWidth, uint unHeight, uint unDepth);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayFromFile(ulong ulOverlayHandle, string pchFilePath);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayTexture(ulong ulOverlayHandle, ref IntPtr pNativeTextureHandle, IntPtr pNativeTextureRef, ref uint pWidth, ref uint pHeight, ref uint pNativeFormat, ref ETextureType pAPIType, ref EColorSpace pColorSpace, ref VRTextureBounds_t pTextureBounds);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _ReleaseNativeOverlayHandle(ulong ulOverlayHandle, IntPtr pNativeTextureHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayTextureSize(ulong ulOverlayHandle, ref uint pWidth, ref uint pHeight);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _CreateDashboardOverlay(string pchOverlayKey, string pchOverlayFriendlyName, ref ulong pMainHandle, ref ulong pThumbnailHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsDashboardVisible();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsActiveDashboardOverlay(ulong ulOverlayHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetDashboardOverlaySceneProcess(ulong ulOverlayHandle, uint unProcessId);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetDashboardOverlaySceneProcess(ulong ulOverlayHandle, ref uint punProcessId);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ShowDashboard(string pchOverlayToShow);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetPrimaryDashboardDevice();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _ShowKeyboard(int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText, bool bUseMinimalMode, ulong uUserValue);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _ShowKeyboardForOverlay(ulong ulOverlayHandle, int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText, bool bUseMinimalMode, ulong uUserValue);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetKeyboardText(StringBuilder pchText, uint cchText);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _HideKeyboard();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetKeyboardTransformAbsolute(ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToKeyboardTransform);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetKeyboardPositionForOverlay(ulong ulOverlayHandle, HmdRect2_t avoidRect);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _SetOverlayIntersectionMask(ulong ulOverlayHandle, ref VROverlayIntersectionMaskPrimitive_t pMaskPrimitives, uint unNumMaskPrimitives, uint unPrimitiveSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _GetOverlayFlags(ulong ulOverlayHandle, ref uint pFlags);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate VRMessageOverlayResponse _ShowMessageOverlay(string pchText, string pchCaption, string pchButton0Text, string pchButton1Text, string pchButton2Text, string pchButton3Text);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _CloseMessageOverlay();
	}
}
