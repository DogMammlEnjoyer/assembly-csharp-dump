using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public struct PointerEvent : IEvent
	{
		public readonly int Identifier { get; }

		public readonly ulong EventId { get; }

		public readonly PointerEventType Type { get; }

		public readonly Pose Pose { get; }

		public readonly object Data { get; }

		public PointerEvent(int identifier, PointerEventType type, Pose pose, object data = null)
		{
			this.Identifier = identifier;
			this.EventId = (PointerEvent._nextEventId += 1UL);
			this.Type = type;
			this.Pose = pose;
			this.Data = data;
		}

		private static ulong _nextEventId;
	}
}
