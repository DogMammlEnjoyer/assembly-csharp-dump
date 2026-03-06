using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class HandJointUtils
	{
		public static HandJointId GetHandFingerTip(HandFinger finger)
		{
			switch (finger)
			{
			default:
				return HandJointId.Invalid;
			case HandFinger.Thumb:
				return HandJointId.HandThumbTip;
			case HandFinger.Index:
				return HandJointId.HandIndexTip;
			case HandFinger.Middle:
				return HandJointId.HandMiddleTip;
			case HandFinger.Ring:
				return HandJointId.HandRingTip;
			case HandFinger.Pinky:
				return HandJointId.HandPinkyTip;
			}
		}

		public static bool IsFingerTip(HandJointId joint)
		{
			return joint == HandJointId.HandThumbTip || joint == HandJointId.HandIndexTip || joint == HandJointId.HandMiddleTip || joint == HandJointId.HandRingTip || joint == HandJointId.HandPinkyTip;
		}

		public static HandJointId GetHandFingerProximal(HandFinger finger)
		{
			return HandJointUtils._handFingerProximals[(int)finger];
		}

		public static bool WristJointPosesToLocalRotations(Pose[] jointPoses, ref Quaternion[] joints)
		{
			if (jointPoses.Length < 26 || joints.Length < 26)
			{
				return false;
			}
			for (int i = 0; i < 26; i++)
			{
				int num = (int)HandJointUtils.JointParentList[i];
				joints[i] = ((num < 0) ? Quaternion.identity : (Quaternion.Inverse(jointPoses[num].rotation) * jointPoses[i].rotation));
			}
			return true;
		}

		public static List<HandJointId[]> FingerToJointList = new List<HandJointId[]>
		{
			new HandJointId[]
			{
				HandJointId.HandThumb1,
				HandJointId.HandThumb2,
				HandJointId.HandThumb3,
				HandJointId.HandThumbTip
			},
			new HandJointId[]
			{
				HandJointId.HandIndex0,
				HandJointId.HandIndex1,
				HandJointId.HandIndex2,
				HandJointId.HandIndex3,
				HandJointId.HandIndexTip
			},
			new HandJointId[]
			{
				HandJointId.HandMiddle0,
				HandJointId.HandMiddle1,
				HandJointId.HandMiddle2,
				HandJointId.HandMiddle3,
				HandJointId.HandMiddleTip
			},
			new HandJointId[]
			{
				HandJointId.HandRing0,
				HandJointId.HandRing1,
				HandJointId.HandRing2,
				HandJointId.HandRing3,
				HandJointId.HandRingTip
			},
			new HandJointId[]
			{
				HandJointId.HandPinky0,
				HandJointId.HandPinky1,
				HandJointId.HandPinky2,
				HandJointId.HandPinky3,
				HandJointId.HandPinkyTip
			}
		};

		public static HandFinger[] JointToFingerList = new HandFinger[]
		{
			HandFinger.Invalid,
			HandFinger.Invalid,
			HandFinger.Thumb,
			HandFinger.Thumb,
			HandFinger.Thumb,
			HandFinger.Thumb,
			HandFinger.Index,
			HandFinger.Index,
			HandFinger.Index,
			HandFinger.Index,
			HandFinger.Index,
			HandFinger.Middle,
			HandFinger.Middle,
			HandFinger.Middle,
			HandFinger.Middle,
			HandFinger.Middle,
			HandFinger.Ring,
			HandFinger.Ring,
			HandFinger.Ring,
			HandFinger.Ring,
			HandFinger.Ring,
			HandFinger.Pinky,
			HandFinger.Pinky,
			HandFinger.Pinky,
			HandFinger.Pinky,
			HandFinger.Pinky
		};

		public static HandJointId[] JointParentList = new HandJointId[]
		{
			HandJointId.HandWristRoot,
			HandJointId.Invalid,
			HandJointId.HandWristRoot,
			HandJointId.HandThumb1,
			HandJointId.HandThumb2,
			HandJointId.HandThumb3,
			HandJointId.HandWristRoot,
			HandJointId.HandIndex0,
			HandJointId.HandIndex1,
			HandJointId.HandIndex2,
			HandJointId.HandIndex3,
			HandJointId.HandWristRoot,
			HandJointId.HandMiddle0,
			HandJointId.HandMiddle1,
			HandJointId.HandMiddle2,
			HandJointId.HandMiddle3,
			HandJointId.HandWristRoot,
			HandJointId.HandRing0,
			HandJointId.HandRing1,
			HandJointId.HandRing2,
			HandJointId.HandRing3,
			HandJointId.HandWristRoot,
			HandJointId.HandPinky0,
			HandJointId.HandPinky1,
			HandJointId.HandPinky2,
			HandJointId.HandPinky3
		};

		public static HandJointId[][] JointChildrenList = new HandJointId[][]
		{
			new HandJointId[0],
			new HandJointId[]
			{
				HandJointId.HandStart,
				HandJointId.HandThumb1,
				HandJointId.HandIndex0,
				HandJointId.HandMiddle0,
				HandJointId.HandRing0,
				HandJointId.HandPinky0
			},
			new HandJointId[]
			{
				HandJointId.HandThumb2
			},
			new HandJointId[]
			{
				HandJointId.HandThumb3
			},
			new HandJointId[]
			{
				HandJointId.HandThumbTip
			},
			new HandJointId[0],
			new HandJointId[]
			{
				HandJointId.HandIndex1
			},
			new HandJointId[]
			{
				HandJointId.HandIndex2
			},
			new HandJointId[]
			{
				HandJointId.HandIndex3
			},
			new HandJointId[]
			{
				HandJointId.HandIndexTip
			},
			new HandJointId[0],
			new HandJointId[]
			{
				HandJointId.HandMiddle1
			},
			new HandJointId[]
			{
				HandJointId.HandMiddle2
			},
			new HandJointId[]
			{
				HandJointId.HandMiddle3
			},
			new HandJointId[]
			{
				HandJointId.HandMiddleTip
			},
			new HandJointId[0],
			new HandJointId[]
			{
				HandJointId.HandRing1
			},
			new HandJointId[]
			{
				HandJointId.HandRing2
			},
			new HandJointId[]
			{
				HandJointId.HandRing3
			},
			new HandJointId[]
			{
				HandJointId.HandRingTip
			},
			new HandJointId[0],
			new HandJointId[]
			{
				HandJointId.HandPinky1
			},
			new HandJointId[]
			{
				HandJointId.HandPinky2
			},
			new HandJointId[]
			{
				HandJointId.HandPinky3
			},
			new HandJointId[]
			{
				HandJointId.HandPinkyTip
			},
			new HandJointId[0]
		};

		[Obsolete("Use JointToFingerListinstead.")]
		public static List<HandJointId> JointIds = new List<HandJointId>
		{
			HandJointId.HandIndex0,
			HandJointId.HandIndex1,
			HandJointId.HandIndex2,
			HandJointId.HandIndex3,
			HandJointId.HandMiddle0,
			HandJointId.HandMiddle1,
			HandJointId.HandMiddle2,
			HandJointId.HandMiddle3,
			HandJointId.HandRing0,
			HandJointId.HandRing1,
			HandJointId.HandRing2,
			HandJointId.HandRing3,
			HandJointId.HandPinky0,
			HandJointId.HandPinky1,
			HandJointId.HandPinky2,
			HandJointId.HandPinky3,
			HandJointId.HandThumb1,
			HandJointId.HandThumb2,
			HandJointId.HandThumb3
		};

		private static readonly HandJointId[] _handFingerProximals = new HandJointId[]
		{
			HandJointId.HandThumb2,
			HandJointId.HandIndex1,
			HandJointId.HandMiddle1,
			HandJointId.HandRing1,
			HandJointId.HandPinky1
		};
	}
}
