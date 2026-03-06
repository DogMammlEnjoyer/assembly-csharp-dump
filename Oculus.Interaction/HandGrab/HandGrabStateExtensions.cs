using System;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public static class HandGrabStateExtensions
	{
		public static Pose GetVisualWristPose(this IHandGrabState grabState)
		{
			Pose pose;
			if (grabState.HandGrabTarget.HandPose != null)
			{
				HandGrabTarget handGrabTarget = grabState.HandGrabTarget;
				pose = Pose.identity;
				return handGrabTarget.GetWorldPoseDisplaced(pose);
			}
			Pose identity = Pose.identity;
			pose = grabState.WristToGrabPoseOffset;
			PoseUtils.Inverse(pose, ref identity);
			return grabState.HandGrabTarget.GetWorldPoseDisplaced(identity);
		}

		public static Pose GetTargetGrabPose(this IHandGrabState grabState)
		{
			Pose pose;
			if (grabState.HandGrabTarget.HandPose != null)
			{
				HandGrabTarget handGrabTarget = grabState.HandGrabTarget;
				pose = grabState.WristToGrabPoseOffset;
				return handGrabTarget.GetWorldPoseDisplaced(pose);
			}
			HandGrabTarget handGrabTarget2 = grabState.HandGrabTarget;
			pose = Pose.identity;
			return handGrabTarget2.GetWorldPoseDisplaced(pose);
		}
	}
}
