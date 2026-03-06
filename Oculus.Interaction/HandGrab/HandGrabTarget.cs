using System;
using Oculus.Interaction.Grab;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public class HandGrabTarget
	{
		public HandPose HandPose
		{
			get
			{
				if (!this._handGrabResult.HasHandPose)
				{
					return null;
				}
				return this._handGrabResult.HandPose;
			}
		}

		public Pose GetWorldPoseDisplaced(in Pose offset)
		{
			Pose pose = PoseUtils.Multiply(this._handGrabResult.RelativePose, offset);
			return this._relativeTo.GlobalPose(pose);
		}

		public HandAlignType HandAlignment { get; private set; }

		public GrabTypeFlags Anchor { get; private set; }

		[Obsolete("Use Set with GrabTypeFlags instead")]
		public void Set(Transform relativeTo, HandAlignType handAlignment, HandGrabTarget.GrabAnchor anchor, HandGrabResult handGrabResult)
		{
			this.HandAlignment = handAlignment;
			this._relativeTo = relativeTo;
			this._handGrabResult.CopyFrom(handGrabResult);
			if (anchor == HandGrabTarget.GrabAnchor.Pinch)
			{
				this.Anchor = GrabTypeFlags.Pinch;
				return;
			}
			if (anchor != HandGrabTarget.GrabAnchor.Palm)
			{
				this.Anchor = GrabTypeFlags.None;
				return;
			}
			this.Anchor = GrabTypeFlags.Palm;
		}

		public void Set(Transform relativeTo, HandAlignType handAlignment, GrabTypeFlags anchor, HandGrabResult handGrabResult)
		{
			this.Anchor = anchor;
			this.HandAlignment = handAlignment;
			this._relativeTo = relativeTo;
			this._handGrabResult.CopyFrom(handGrabResult);
		}

		private Transform _relativeTo;

		private HandGrabResult _handGrabResult = new HandGrabResult();

		[Obsolete]
		public enum GrabAnchor
		{
			None,
			Wrist,
			Pinch,
			Palm
		}
	}
}
