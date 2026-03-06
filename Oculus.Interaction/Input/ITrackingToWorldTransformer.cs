using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public interface ITrackingToWorldTransformer
	{
		Transform Transform { get; }

		Pose ToWorldPose(Pose poseRh);

		Pose ToTrackingPose(in Pose worldPose);

		Quaternion WorldToTrackingWristJointFixup { get; }
	}
}
