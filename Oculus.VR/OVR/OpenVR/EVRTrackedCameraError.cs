using System;

namespace OVR.OpenVR
{
	public enum EVRTrackedCameraError
	{
		None,
		OperationFailed = 100,
		InvalidHandle,
		InvalidFrameHeaderVersion,
		OutOfHandles,
		IPCFailure,
		NotSupportedForThisDevice,
		SharedMemoryFailure,
		FrameBufferingFailure,
		StreamSetupFailure,
		InvalidGLTextureId,
		InvalidSharedTextureHandle,
		FailedToGetGLTextureId,
		SharedTextureFailure,
		NoFrameAvailable,
		InvalidArgument,
		InvalidFrameBufferSize
	}
}
