using System;

namespace Valve.VR
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
		InvalidBounds,
		AlreadySet
	}
}
