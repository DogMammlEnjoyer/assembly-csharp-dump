using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public struct LocomotionEvent
	{
		public readonly int Identifier { get; }

		public readonly Pose Pose { get; }

		public readonly LocomotionEvent.TranslationType Translation { get; }

		public readonly LocomotionEvent.RotationType Rotation { get; }

		public readonly ulong EventId { get; }

		public LocomotionEvent(int identifier, Pose pose, LocomotionEvent.TranslationType translationType, LocomotionEvent.RotationType rotationType)
		{
			this.Identifier = identifier;
			this.EventId = (LocomotionEvent._nextEventId += 1UL);
			this.Pose = pose;
			this.Translation = translationType;
			this.Rotation = rotationType;
		}

		public LocomotionEvent(int identifier, Vector3 position, LocomotionEvent.TranslationType translationType)
		{
			this = new LocomotionEvent(identifier, new Pose(position, Quaternion.identity), translationType, LocomotionEvent.RotationType.None);
		}

		public LocomotionEvent(int identifier, Quaternion rotation, LocomotionEvent.RotationType rotationType)
		{
			this = new LocomotionEvent(identifier, new Pose(Vector3.zero, rotation), LocomotionEvent.TranslationType.None, rotationType);
		}

		private static ulong _nextEventId;

		public enum TranslationType
		{
			None,
			Velocity,
			Absolute,
			AbsoluteEyeLevel,
			Relative
		}

		public enum RotationType
		{
			None,
			Velocity,
			Absolute,
			Relative
		}
	}
}
