using System;

namespace System
{
	/// <summary>Represents the base class for classes that contain event data, and provides a value to use for events that do not include event data.</summary>
	[Serializable]
	public class EventArgs
	{
		/// <summary>Provides a value to use with events that do not have event data.</summary>
		public static readonly EventArgs Empty = new EventArgs();
	}
}
