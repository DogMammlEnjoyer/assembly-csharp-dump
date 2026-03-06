using System;

namespace OVR.OpenVR
{
	public enum EVRCompositorError
	{
		None,
		RequestFailed,
		IncompatibleVersion = 100,
		DoNotHaveFocus,
		InvalidTexture,
		IsNotSceneApplication,
		TextureIsOnWrongDevice,
		TextureUsesUnsupportedFormat,
		SharedTexturesNotSupported,
		IndexOutOfRange,
		AlreadySubmitted,
		InvalidBounds
	}
}
