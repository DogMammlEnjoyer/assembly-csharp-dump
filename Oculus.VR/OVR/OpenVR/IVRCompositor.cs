using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public struct IVRCompositor
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._SetTrackingSpace SetTrackingSpace;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetTrackingSpace GetTrackingSpace;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._WaitGetPoses WaitGetPoses;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetLastPoses GetLastPoses;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetLastPoseForTrackedDeviceIndex GetLastPoseForTrackedDeviceIndex;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._Submit Submit;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ClearLastSubmittedFrame ClearLastSubmittedFrame;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._PostPresentHandoff PostPresentHandoff;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetFrameTiming GetFrameTiming;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetFrameTimings GetFrameTimings;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetFrameTimeRemaining GetFrameTimeRemaining;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetCumulativeStats GetCumulativeStats;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._FadeToColor FadeToColor;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetCurrentFadeColor GetCurrentFadeColor;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._FadeGrid FadeGrid;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetCurrentGridAlpha GetCurrentGridAlpha;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._SetSkyboxOverride SetSkyboxOverride;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ClearSkyboxOverride ClearSkyboxOverride;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._CompositorBringToFront CompositorBringToFront;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._CompositorGoToBack CompositorGoToBack;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._CompositorQuit CompositorQuit;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._IsFullscreen IsFullscreen;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetCurrentSceneFocusProcess GetCurrentSceneFocusProcess;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetLastFrameRenderer GetLastFrameRenderer;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._CanRenderScene CanRenderScene;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ShowMirrorWindow ShowMirrorWindow;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._HideMirrorWindow HideMirrorWindow;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._IsMirrorWindowVisible IsMirrorWindowVisible;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._CompositorDumpImages CompositorDumpImages;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ShouldAppRenderWithLowResources ShouldAppRenderWithLowResources;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ForceInterleavedReprojectionOn ForceInterleavedReprojectionOn;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ForceReconnectProcess ForceReconnectProcess;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._SuspendRendering SuspendRendering;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetMirrorTextureD3D11 GetMirrorTextureD3D11;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ReleaseMirrorTextureD3D11 ReleaseMirrorTextureD3D11;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetMirrorTextureGL GetMirrorTextureGL;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ReleaseSharedGLTexture ReleaseSharedGLTexture;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._LockGLSharedTextureForAccess LockGLSharedTextureForAccess;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._UnlockGLSharedTextureForAccess UnlockGLSharedTextureForAccess;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetVulkanInstanceExtensionsRequired GetVulkanInstanceExtensionsRequired;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetVulkanDeviceExtensionsRequired GetVulkanDeviceExtensionsRequired;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._SetExplicitTimingMode SetExplicitTimingMode;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._SubmitExplicitTimingData SubmitExplicitTimingData;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetTrackingSpace(ETrackingUniverseOrigin eOrigin);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackingUniverseOrigin _GetTrackingSpace();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _WaitGetPoses([In] [Out] TrackedDevicePose_t[] pRenderPoseArray, uint unRenderPoseArrayCount, [In] [Out] TrackedDevicePose_t[] pGamePoseArray, uint unGamePoseArrayCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _GetLastPoses([In] [Out] TrackedDevicePose_t[] pRenderPoseArray, uint unRenderPoseArrayCount, [In] [Out] TrackedDevicePose_t[] pGamePoseArray, uint unGamePoseArrayCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _GetLastPoseForTrackedDeviceIndex(uint unDeviceIndex, ref TrackedDevicePose_t pOutputPose, ref TrackedDevicePose_t pOutputGamePose);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _Submit(EVREye eEye, ref Texture_t pTexture, ref VRTextureBounds_t pBounds, EVRSubmitFlags nSubmitFlags);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ClearLastSubmittedFrame();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _PostPresentHandoff();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetFrameTiming(ref Compositor_FrameTiming pTiming, uint unFramesAgo);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetFrameTimings(ref Compositor_FrameTiming pTiming, uint nFrames);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate float _GetFrameTimeRemaining();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetCumulativeStats(ref Compositor_CumulativeStats pStats, uint nStatsSizeInBytes);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _FadeToColor(float fSeconds, float fRed, float fGreen, float fBlue, float fAlpha, bool bBackground);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate HmdColor_t _GetCurrentFadeColor(bool bBackground);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _FadeGrid(float fSeconds, bool bFadeIn);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate float _GetCurrentGridAlpha();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _SetSkyboxOverride([In] [Out] Texture_t[] pTextures, uint unTextureCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ClearSkyboxOverride();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _CompositorBringToFront();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _CompositorGoToBack();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _CompositorQuit();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsFullscreen();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetCurrentSceneFocusProcess();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetLastFrameRenderer();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _CanRenderScene();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ShowMirrorWindow();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _HideMirrorWindow();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsMirrorWindowVisible();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _CompositorDumpImages();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _ShouldAppRenderWithLowResources();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ForceInterleavedReprojectionOn(bool bOverride);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ForceReconnectProcess();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SuspendRendering(bool bSuspend);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _GetMirrorTextureD3D11(EVREye eEye, IntPtr pD3D11DeviceOrResource, ref IntPtr ppD3D11ShaderResourceView);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ReleaseMirrorTextureD3D11(IntPtr pD3D11ShaderResourceView);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _GetMirrorTextureGL(EVREye eEye, ref uint pglTextureId, IntPtr pglSharedTextureHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _ReleaseSharedGLTexture(uint glTextureId, IntPtr glSharedTextureHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _LockGLSharedTextureForAccess(IntPtr glSharedTextureHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _UnlockGLSharedTextureForAccess(IntPtr glSharedTextureHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetVulkanInstanceExtensionsRequired(StringBuilder pchValue, uint unBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetVulkanDeviceExtensionsRequired(IntPtr pPhysicalDevice, StringBuilder pchValue, uint unBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetExplicitTimingMode(EVRCompositorTimingMode eTimingMode);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _SubmitExplicitTimingData();
	}
}
