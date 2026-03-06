using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public static class ShadowHandExtensions
	{
		public static void FromHandRoot(this ShadowHand shadow, IHand hand)
		{
			Pose root;
			hand.GetRootPose(out root);
			shadow.SetRoot(root);
			shadow.SetRootScale(hand.Scale);
		}

		public static void FromHandFingers(this ShadowHand shadow, IHand hand, bool flipHandedness = false)
		{
			ReadOnlyHandJointPoses localJointPoses;
			hand.GetJointPosesLocal(out localJointPoses);
			shadow.FromJoints(localJointPoses, flipHandedness);
		}

		public static void FromJoints(this ShadowHand shadow, IReadOnlyList<Pose> localJointPoses, bool flipHandedness)
		{
			if (localJointPoses.Count != 26)
			{
				return;
			}
			for (int i = 0; i < 26; i++)
			{
				Pose pose = localJointPoses[i];
				if (flipHandedness)
				{
					pose = HandMirroring.Mirror(pose);
				}
				shadow.SetLocalPose((HandJointId)i, pose);
			}
		}

		public static void FromHand(this ShadowHand shadow, IHand hand, bool flipHandedness = false)
		{
			shadow.FromHandRoot(hand);
			shadow.FromHandFingers(hand, flipHandedness);
		}
	}
}
