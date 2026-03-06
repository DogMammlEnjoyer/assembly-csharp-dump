using System;

namespace Valve.VR
{
	public enum EVROverlayError
	{
		None,
		UnknownOverlay = 10,
		InvalidHandle,
		PermissionDenied,
		OverlayLimitExceeded,
		WrongVisibilityType,
		KeyTooLong,
		NameTooLong,
		KeyInUse,
		WrongTransformType,
		InvalidTrackedDevice,
		InvalidParameter,
		ThumbnailCantBeDestroyed,
		ArrayTooSmall,
		RequestFailed,
		InvalidTexture,
		UnableToLoadFile,
		KeyboardAlreadyInUse,
		NoNeighbor,
		TooManyMaskPrimitives = 29,
		BadMaskPrimitive,
		TextureAlreadyLocked,
		TextureLockCapacityReached,
		TextureNotLocked
	}
}
