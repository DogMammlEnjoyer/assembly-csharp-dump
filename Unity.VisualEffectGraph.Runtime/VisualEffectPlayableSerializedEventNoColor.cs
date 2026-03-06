using System;
using UnityEngine.VFX.Utility;

namespace UnityEngine.VFX
{
	[Serializable]
	internal struct VisualEffectPlayableSerializedEventNoColor
	{
		public static implicit operator VisualEffectPlayableSerializedEvent(VisualEffectPlayableSerializedEventNoColor evt)
		{
			return new VisualEffectPlayableSerializedEvent
			{
				time = evt.time,
				timeSpace = evt.timeSpace,
				name = evt.name,
				eventAttributes = evt.eventAttributes
			};
		}

		public double time;

		public PlayableTimeSpace timeSpace;

		public ExposedProperty name;

		public EventAttributes eventAttributes;
	}
}
