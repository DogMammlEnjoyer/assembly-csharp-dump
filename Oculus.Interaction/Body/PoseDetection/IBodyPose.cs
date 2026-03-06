using System;
using Oculus.Interaction.Body.Input;
using UnityEngine;

namespace Oculus.Interaction.Body.PoseDetection
{
	public interface IBodyPose
	{
		event Action WhenBodyPoseUpdated;

		ISkeletonMapping SkeletonMapping { get; }

		bool GetJointPoseLocal(BodyJointId bodyJointId, out Pose pose);

		bool GetJointPoseFromRoot(BodyJointId bodyJointId, out Pose pose);
	}
}
