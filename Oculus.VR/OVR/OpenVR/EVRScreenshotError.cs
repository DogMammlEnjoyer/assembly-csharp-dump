using System;

namespace OVR.OpenVR
{
	public enum EVRScreenshotError
	{
		None,
		RequestFailed,
		IncompatibleVersion = 100,
		NotFound,
		BufferTooSmall,
		ScreenshotAlreadyInProgress = 108
	}
}
