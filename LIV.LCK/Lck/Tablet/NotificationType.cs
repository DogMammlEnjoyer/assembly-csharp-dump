using System;

namespace Liv.Lck.Tablet
{
	[Serializable]
	public enum NotificationType
	{
		VideoSaved,
		PhotoSaved,
		EnterStreamCode,
		CheckSubscribed,
		ConfigureStream,
		InternalError,
		MissingTrackingId,
		InvalidArgument
	}
}
