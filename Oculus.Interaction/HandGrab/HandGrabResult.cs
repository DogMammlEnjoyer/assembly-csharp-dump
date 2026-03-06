using System;
using Oculus.Interaction.Grab;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public class HandGrabResult
	{
		public HandGrabResult()
		{
			this.RelativePose = Pose.identity;
			this.HandPose = new HandPose();
		}

		public void CopyFrom(HandGrabResult other)
		{
			this.HasHandPose = other.HasHandPose;
			this.HandPose.CopyFrom(other.HandPose, false);
			ref this.RelativePose.CopyFrom(other.RelativePose);
			this.Score = other.Score;
		}

		public bool HasHandPose;

		public HandPose HandPose;

		public Pose RelativePose;

		public GrabPoseScore Score;
	}
}
