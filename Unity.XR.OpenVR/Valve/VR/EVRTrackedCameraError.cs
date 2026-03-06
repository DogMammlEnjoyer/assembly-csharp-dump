using System;

namespace Valve.VR
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
