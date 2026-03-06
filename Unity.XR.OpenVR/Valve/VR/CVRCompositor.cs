using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRCompositor
	{
		internal CVRCompositor(IntPtr pInterface)
		{
			this.FnTable = (IVRCompositor)Marshal.PtrToStructure(pInterface, typeof(IVRCompositor));
		}

		public void SetTrackingSpace(ETrackingUniverseOrigin eOrigin)
		{
			this.FnTable.SetTrackingSpace(eOrigin);
		}

		public ETrackingUniverseOrigin GetTrackingSpace()
		{
			return this.FnTable.GetTrackingSpace();
		}

		public EVRCompositorError WaitGetPoses(TrackedDevicePose_t[] pRenderPoseArray, TrackedDevicePose_t[] pGamePoseArray)
		{
			return this.FnTable.WaitGetPoses(pRenderPoseArray, (uint)pRenderPoseArray.Length, pGamePoseArray, (uint)pGamePoseArray.Length);
		}

		public EVRCompositorError GetLastPoses(TrackedDevicePose_t[] pRenderPoseArray, TrackedDevicePose_t[] pGamePoseArray)
		{
			return this.FnTable.GetLastPoses(pRenderPoseArray, (uint)pRenderPoseArray.Length, pGamePoseArray, (uint)pGamePoseArray.Length);
		}

		public EVRCompositorError GetLastPoseForTrackedDeviceIndex(uint unDeviceIndex, ref TrackedDevicePose_t pOutputPose, ref TrackedDevicePose_t pOutputGamePose)
		{
			return this.FnTable.GetLastPoseForTrackedDeviceIndex(unDeviceIndex, ref pOutputPose, ref pOutputGamePose);
		}

		public EVRCompositorError Submit(EVREye eEye, ref Texture_t pTexture, ref VRTextureBounds_t pBounds, EVRSubmitFlags nSubmitFlags)
		{
			return this.FnTable.Submit(eEye, ref pTexture, ref pBounds, nSubmitFlags);
		}

		public void ClearLastSubmittedFrame()
		{
			this.FnTable.ClearLastSubmittedFrame();
		}

		public void PostPresentHandoff()
		{
			this.FnTable.PostPresentHandoff();
		}

		public bool GetFrameTiming(ref Compositor_FrameTiming pTiming, uint unFramesAgo)
		{
			return this.FnTable.GetFrameTiming(ref pTiming, unFramesAgo);
		}

		public uint GetFrameTimings(Compositor_FrameTiming[] pTiming)
		{
			return this.FnTable.GetFrameTimings(pTiming, (uint)pTiming.Length);
		}

		public float GetFrameTimeRemaining()
		{
			return this.FnTable.GetFrameTimeRemaining();
		}

		public void GetCumulativeStats(ref Compositor_CumulativeStats pStats, uint nStatsSizeInBytes)
		{
			this.FnTable.GetCumulativeStats(ref pStats, nStatsSizeInBytes);
		}

		public void FadeToColor(float fSeconds, float fRed, float fGreen, float fBlue, float fAlpha, bool bBackground)
		{
			this.FnTable.FadeToColor(fSeconds, fRed, fGreen, fBlue, fAlpha, bBackground);
		}

		public HmdColor_t GetCurrentFadeColor(bool bBackground)
		{
			return this.FnTable.GetCurrentFadeColor(bBackground);
		}

		public void FadeGrid(float fSeconds, bool bFadeIn)
		{
			this.FnTable.FadeGrid(fSeconds, bFadeIn);
		}

		public float GetCurrentGridAlpha()
		{
			return this.FnTable.GetCurrentGridAlpha();
		}

		public EVRCompositorError SetSkyboxOverride(Texture_t[] pTextures)
		{
			return this.FnTable.SetSkyboxOverride(pTextures, (uint)pTextures.Length);
		}

		public void ClearSkyboxOverride()
		{
			this.FnTable.ClearSkyboxOverride();
		}

		public void CompositorBringToFront()
		{
			this.FnTable.CompositorBringToFront();
		}

		public void CompositorGoToBack()
		{
			this.FnTable.CompositorGoToBack();
		}

		public void CompositorQuit()
		{
			this.FnTable.CompositorQuit();
		}

		public bool IsFullscreen()
		{
			return this.FnTable.IsFullscreen();
		}

		public uint GetCurrentSceneFocusProcess()
		{
			return this.FnTable.GetCurrentSceneFocusProcess();
		}

		public uint GetLastFrameRenderer()
		{
			return this.FnTable.GetLastFrameRenderer();
		}

		public bool CanRenderScene()
		{
			return this.FnTable.CanRenderScene();
		}

		public void ShowMirrorWindow()
		{
			this.FnTable.ShowMirrorWindow();
		}

		public void HideMirrorWindow()
		{
			this.FnTable.HideMirrorWindow();
		}

		public bool IsMirrorWindowVisible()
		{
			return this.FnTable.IsMirrorWindowVisible();
		}

		public void CompositorDumpImages()
		{
			this.FnTable.CompositorDumpImages();
		}

		public bool ShouldAppRenderWithLowResources()
		{
			return this.FnTable.ShouldAppRenderWithLowResources();
		}

		public void ForceInterleavedReprojectionOn(bool bOverride)
		{
			this.FnTable.ForceInterleavedReprojectionOn(bOverride);
		}

		public void ForceReconnectProcess()
		{
			this.FnTable.ForceReconnectProcess();
		}

		public void SuspendRendering(bool bSuspend)
		{
			this.FnTable.SuspendRendering(bSuspend);
		}

		public EVRCompositorError GetMirrorTextureD3D11(EVREye eEye, IntPtr pD3D11DeviceOrResource, ref IntPtr ppD3D11ShaderResourceView)
		{
			return this.FnTable.GetMirrorTextureD3D11(eEye, pD3D11DeviceOrResource, ref ppD3D11ShaderResourceView);
		}

		public void ReleaseMirrorTextureD3D11(IntPtr pD3D11ShaderResourceView)
		{
			this.FnTable.ReleaseMirrorTextureD3D11(pD3D11ShaderResourceView);
		}

		public EVRCompositorError GetMirrorTextureGL(EVREye eEye, ref uint pglTextureId, IntPtr pglSharedTextureHandle)
		{
			pglTextureId = 0U;
			return this.FnTable.GetMirrorTextureGL(eEye, ref pglTextureId, pglSharedTextureHandle);
		}

		public bool ReleaseSharedGLTexture(uint glTextureId, IntPtr glSharedTextureHandle)
		{
			return this.FnTable.ReleaseSharedGLTexture(glTextureId, glSharedTextureHandle);
		}

		public void LockGLSharedTextureForAccess(IntPtr glSharedTextureHandle)
		{
			this.FnTable.LockGLSharedTextureForAccess(glSharedTextureHandle);
		}

		public void UnlockGLSharedTextureForAccess(IntPtr glSharedTextureHandle)
		{
			this.FnTable.UnlockGLSharedTextureForAccess(glSharedTextureHandle);
		}

		public uint GetVulkanInstanceExtensionsRequired(StringBuilder pchValue, uint unBufferSize)
		{
			return this.FnTable.GetVulkanInstanceExtensionsRequired(pchValue, unBufferSize);
		}

		public uint GetVulkanDeviceExtensionsRequired(IntPtr pPhysicalDevice, StringBuilder pchValue, uint unBufferSize)
		{
			return this.FnTable.GetVulkanDeviceExtensionsRequired(pPhysicalDevice, pchValue, unBufferSize);
		}

		public void SetExplicitTimingMode(EVRCompositorTimingMode eTimingMode)
		{
			this.FnTable.SetExplicitTimingMode(eTimingMode);
		}

		public EVRCompositorError SubmitExplicitTimingData()
		{
			return this.FnTable.SubmitExplicitTimingData();
		}

		public bool IsMotionSmoothingEnabled()
		{
			return this.FnTable.IsMotionSmoothingEnabled();
		}

		public bool IsMotionSmoothingSupported()
		{
			return this.FnTable.IsMotionSmoothingSupported();
		}

		public bool IsCurrentSceneFocusAppLoading()
		{
			return this.FnTable.IsCurrentSceneFocusAppLoading();
		}

		public EVRCompositorError SetStageOverride_Async(string pchRenderModelPath, ref HmdMatrix34_t pTransform, ref Compositor_StageRenderSettings pRenderSettings, uint nSizeOfRenderSettings)
		{
			IntPtr intPtr = Utils.ToUtf8(pchRenderModelPath);
			EVRCompositorError result = this.FnTable.SetStageOverride_Async(intPtr, ref pTransform, ref pRenderSettings, nSizeOfRenderSettings);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public void ClearStageOverride()
		{
			this.FnTable.ClearStageOverride();
		}

		public bool GetCompositorBenchmarkResults(ref Compositor_BenchmarkResults pBenchmarkResults, uint nSizeOfBenchmarkResults)
		{
			return this.FnTable.GetCompositorBenchmarkResults(ref pBenchmarkResults, nSizeOfBenchmarkResults);
		}

		public EVRCompositorError GetLastPosePredictionIDs(ref uint pRenderPosePredictionID, ref uint pGamePosePredictionID)
		{
			pRenderPosePredictionID = 0U;
			pGamePosePredictionID = 0U;
			return this.FnTable.GetLastPosePredictionIDs(ref pRenderPosePredictionID, ref pGamePosePredictionID);
		}

		public EVRCompositorError GetPosesForFrame(uint unPosePredictionID, TrackedDevicePose_t[] pPoseArray)
		{
			return this.FnTable.GetPosesForFrame(unPosePredictionID, pPoseArray, (uint)pPoseArray.Length);
		}

		private IVRCompositor FnTable;
	}
}
