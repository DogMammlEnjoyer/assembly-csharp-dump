using System;

namespace Valve.VR
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
