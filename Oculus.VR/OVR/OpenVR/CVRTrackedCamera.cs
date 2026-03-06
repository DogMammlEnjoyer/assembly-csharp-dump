using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	public class CVRTrackedCamera
	{
		internal CVRTrackedCamera(IntPtr pInterface)
		{
			this.FnTable = (IVRTrackedCamera)Marshal.PtrToStructure(pInterface, typeof(IVRTrackedCamera));
		}

		public string GetCameraErrorNameFromEnum(EVRTrackedCameraError eCameraError)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetCameraErrorNameFromEnum(eCameraError));
		}

		public EVRTrackedCameraError HasCamera(uint nDeviceIndex, ref bool pHasCamera)
		{
			pHasCamera = false;
			return this.FnTable.HasCamera(nDeviceIndex, ref pHasCamera);
		}

		public EVRTrackedCameraError GetCameraFrameSize(uint nDeviceIndex, EVRTrackedCameraFrameType eFrameType, ref uint pnWidth, ref uint pnHeight, ref uint pnFrameBufferSize)
		{
			pnWidth = 0U;
			pnHeight = 0U;
			pnFrameBufferSize = 0U;
			return this.FnTable.GetCameraFrameSize(nDeviceIndex, eFrameType, ref pnWidth, ref pnHeight, ref pnFrameBufferSize);
		}

		public EVRTrackedCameraError GetCameraIntrinsics(uint nDeviceIndex, EVRTrackedCameraFrameType eFrameType, ref HmdVector2_t pFocalLength, ref HmdVector2_t pCenter)
		{
			return this.FnTable.GetCameraIntrinsics(nDeviceIndex, eFrameType, ref pFocalLength, ref pCenter);
		}

		public EVRTrackedCameraError GetCameraProjection(uint nDeviceIndex, EVRTrackedCameraFrameType eFrameType, float flZNear, float flZFar, ref HmdMatrix44_t pProjection)
		{
			return this.FnTable.GetCameraProjection(nDeviceIndex, eFrameType, flZNear, flZFar, ref pProjection);
		}

		public EVRTrackedCameraError AcquireVideoStreamingService(uint nDeviceIndex, ref ulong pHandle)
		{
			pHandle = 0UL;
			return this.FnTable.AcquireVideoStreamingService(nDeviceIndex, ref pHandle);
		}

		public EVRTrackedCameraError ReleaseVideoStreamingService(ulong hTrackedCamera)
		{
			return this.FnTable.ReleaseVideoStreamingService(hTrackedCamera);
		}

		public EVRTrackedCameraError GetVideoStreamFrameBuffer(ulong hTrackedCamera, EVRTrackedCameraFrameType eFrameType, IntPtr pFrameBuffer, uint nFrameBufferSize, ref CameraVideoStreamFrameHeader_t pFrameHeader, uint nFrameHeaderSize)
		{
			return this.FnTable.GetVideoStreamFrameBuffer(hTrackedCamera, eFrameType, pFrameBuffer, nFrameBufferSize, ref pFrameHeader, nFrameHeaderSize);
		}

		public EVRTrackedCameraError GetVideoStreamTextureSize(uint nDeviceIndex, EVRTrackedCameraFrameType eFrameType, ref VRTextureBounds_t pTextureBounds, ref uint pnWidth, ref uint pnHeight)
		{
			pnWidth = 0U;
			pnHeight = 0U;
			return this.FnTable.GetVideoStreamTextureSize(nDeviceIndex, eFrameType, ref pTextureBounds, ref pnWidth, ref pnHeight);
		}

		public EVRTrackedCameraError GetVideoStreamTextureD3D11(ulong hTrackedCamera, EVRTrackedCameraFrameType eFrameType, IntPtr pD3D11DeviceOrResource, ref IntPtr ppD3D11ShaderResourceView, ref CameraVideoStreamFrameHeader_t pFrameHeader, uint nFrameHeaderSize)
		{
			return this.FnTable.GetVideoStreamTextureD3D11(hTrackedCamera, eFrameType, pD3D11DeviceOrResource, ref ppD3D11ShaderResourceView, ref pFrameHeader, nFrameHeaderSize);
		}

		public EVRTrackedCameraError GetVideoStreamTextureGL(ulong hTrackedCamera, EVRTrackedCameraFrameType eFrameType, ref uint pglTextureId, ref CameraVideoStreamFrameHeader_t pFrameHeader, uint nFrameHeaderSize)
		{
			pglTextureId = 0U;
			return this.FnTable.GetVideoStreamTextureGL(hTrackedCamera, eFrameType, ref pglTextureId, ref pFrameHeader, nFrameHeaderSize);
		}

		public EVRTrackedCameraError ReleaseVideoStreamTextureGL(ulong hTrackedCamera, uint glTextureId)
		{
			return this.FnTable.ReleaseVideoStreamTextureGL(hTrackedCamera, glTextureId);
		}

		private IVRTrackedCamera FnTable;
	}
}
