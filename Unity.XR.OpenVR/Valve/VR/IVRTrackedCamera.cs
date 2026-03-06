using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRTrackedCamera
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._GetCameraErrorNameFromEnum GetCameraErrorNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._HasCamera HasCamera;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._GetCameraFrameSize GetCameraFrameSize;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._GetCameraIntrinsics GetCameraIntrinsics;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._GetCameraProjection GetCameraProjection;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._AcquireVideoStreamingService AcquireVideoStreamingService;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._ReleaseVideoStreamingService ReleaseVideoStreamingService;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._GetVideoStreamFrameBuffer GetVideoStreamFrameBuffer;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._GetVideoStreamTextureSize GetVideoStreamTextureSize;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._GetVideoStreamTextureD3D11 GetVideoStreamTextureD3D11;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._GetVideoStreamTextureGL GetVideoStreamTextureGL;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._ReleaseVideoStreamTextureGL ReleaseVideoStreamTextureGL;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._SetCameraTrackingSpace SetCameraTrackingSpace;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRTrackedCamera._GetCameraTrackingSpace GetCameraTrackingSpace;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetCameraErrorNameFromEnum(EVRTrackedCameraError eCameraError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _HasCamera(uint nDeviceIndex, ref bool pHasCamera);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _GetCameraFrameSize(uint nDeviceIndex, EVRTrackedCameraFrameType eFrameType, ref uint pnWidth, ref uint pnHeight, ref uint pnFrameBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _GetCameraIntrinsics(uint nDeviceIndex, uint nCameraIndex, EVRTrackedCameraFrameType eFrameType, ref HmdVector2_t pFocalLength, ref HmdVector2_t pCenter);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _GetCameraProjection(uint nDeviceIndex, uint nCameraIndex, EVRTrackedCameraFrameType eFrameType, float flZNear, float flZFar, ref HmdMatrix44_t pProjection);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _AcquireVideoStreamingService(uint nDeviceIndex, ref ulong pHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _ReleaseVideoStreamingService(ulong hTrackedCamera);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _GetVideoStreamFrameBuffer(ulong hTrackedCamera, EVRTrackedCameraFrameType eFrameType, IntPtr pFrameBuffer, uint nFrameBufferSize, ref CameraVideoStreamFrameHeader_t pFrameHeader, uint nFrameHeaderSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _GetVideoStreamTextureSize(uint nDeviceIndex, EVRTrackedCameraFrameType eFrameType, ref VRTextureBounds_t pTextureBounds, ref uint pnWidth, ref uint pnHeight);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _GetVideoStreamTextureD3D11(ulong hTrackedCamera, EVRTrackedCameraFrameType eFrameType, IntPtr pD3D11DeviceOrResource, ref IntPtr ppD3D11ShaderResourceView, ref CameraVideoStreamFrameHeader_t pFrameHeader, uint nFrameHeaderSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _GetVideoStreamTextureGL(ulong hTrackedCamera, EVRTrackedCameraFrameType eFrameType, ref uint pglTextureId, ref CameraVideoStreamFrameHeader_t pFrameHeader, uint nFrameHeaderSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRTrackedCameraError _ReleaseVideoStreamTextureGL(ulong hTrackedCamera, uint glTextureId);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetCameraTrackingSpace(ETrackingUniverseOrigin eUniverse);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackingUniverseOrigin _GetCameraTrackingSpace();
	}
}
