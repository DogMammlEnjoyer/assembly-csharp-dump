using System;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public readonly struct ARTrackablesParentTransformChangedEventArgs : IEquatable<ARTrackablesParentTransformChangedEventArgs>
	{
		public XROrigin Origin { get; }

		public Transform TrackablesParent { get; }

		public ARTrackablesParentTransformChangedEventArgs(XROrigin origin, Transform trackablesParent)
		{
			if (origin == null)
			{
				throw new ArgumentNullException("origin");
			}
			if (trackablesParent == null)
			{
				throw new ArgumentNullException("trackablesParent");
			}
			this.Origin = origin;
			this.TrackablesParent = trackablesParent;
		}

		public bool Equals(ARTrackablesParentTransformChangedEventArgs other)
		{
			return this.Origin == other.Origin && this.TrackablesParent == other.TrackablesParent;
		}

		public override bool Equals(object obj)
		{
			if (obj is ARTrackablesParentTransformChangedEventArgs)
			{
				ARTrackablesParentTransformChangedEventArgs other = (ARTrackablesParentTransformChangedEventArgs)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCodeUtil.Combine(HashCodeUtil.ReferenceHash(this.Origin), HashCodeUtil.ReferenceHash(this.TrackablesParent));
		}

		public static bool operator ==(ARTrackablesParentTransformChangedEventArgs lhs, ARTrackablesParentTransformChangedEventArgs rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ARTrackablesParentTransformChangedEventArgs lhs, ARTrackablesParentTransformChangedEventArgs rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
