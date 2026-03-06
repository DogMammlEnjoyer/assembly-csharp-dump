using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class FingerShapes
	{
		public virtual float GetValue(HandFinger finger, FingerFeature feature, IHand hand)
		{
			switch (feature)
			{
			case FingerFeature.Curl:
				return this.GetCurlValue(finger, hand);
			case FingerFeature.Flexion:
				return this.GetFlexionValue(finger, hand);
			case FingerFeature.Abduction:
				return this.GetAbductionValue(finger, hand);
			case FingerFeature.Opposition:
				return this.GetOppositionValue(finger, hand);
			default:
				return 0f;
			}
		}

		private static float PosesCurlValue(Pose p0, Pose p1, Pose p2)
		{
			Vector3 from = p0.position - p1.position;
			Vector3 to = p2.position - p1.position;
			Vector3 axis = p1.rotation * Constants.LeftThumbSide;
			float num = Vector3.SignedAngle(from, to, axis);
			if (num < 0f)
			{
				num += 360f;
			}
			return num;
		}

		public static float PosesListCurlValue(Pose[] poses)
		{
			float num = 0f;
			for (int i = 0; i < poses.Length - 2; i++)
			{
				num += FingerShapes.PosesCurlValue(poses[i], poses[i + 1], poses[i + 2]);
			}
			return num;
		}

		protected float JointsCurlValue(HandJointId[] joints, IHand hand)
		{
			ReadOnlyHandJointPoses readOnlyHandJointPoses;
			if (!hand.GetJointPosesFromWrist(out readOnlyHandJointPoses))
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < joints.Length - 2; i++)
			{
				num += FingerShapes.PosesCurlValue(readOnlyHandJointPoses[(int)joints[i]], readOnlyHandJointPoses[(int)joints[i + 1]], readOnlyHandJointPoses[(int)joints[i + 2]]);
			}
			return num;
		}

		public float GetCurlValue(HandFinger finger, IHand hand)
		{
			HandJointId[] array = FingerShapes.CURL_ANGLE_JOINTS[(int)finger];
			return this.JointsCurlValue(array, hand) / (float)(array.Length - 2);
		}

		public float GetFlexionValue(HandFinger finger, IHand hand)
		{
			ReadOnlyHandJointPoses readOnlyHandJointPoses;
			if (!hand.GetJointPosesFromWrist(out readOnlyHandJointPoses))
			{
				return 0f;
			}
			HandJointId handFingerProximal = HandJointUtils.GetHandFingerProximal(finger);
			Vector3 rightDorsal = Constants.RightDorsal;
			Vector3 to = Vector3.ProjectOnPlane(readOnlyHandJointPoses[handFingerProximal].rotation * Constants.RightDorsal, Constants.RightThumbSide);
			return 180f + Vector3.SignedAngle(rightDorsal, to, Constants.RightPinkySide);
		}

		public float GetAbductionValue(HandFinger finger, IHand hand)
		{
			ReadOnlyHandJointPoses readOnlyHandJointPoses;
			if (finger == HandFinger.Pinky || !hand.GetJointPosesFromWrist(out readOnlyHandJointPoses))
			{
				return 0f;
			}
			HandFinger finger2 = finger + 1;
			Vector3 position = readOnlyHandJointPoses[HandJointUtils.GetHandFingerProximal(finger)].position;
			Vector3 b = Vector3.Lerp(position, readOnlyHandJointPoses[HandJointUtils.GetHandFingerProximal(finger2)].position, 0.5f);
			Vector3 vector;
			if (finger == HandFinger.Thumb)
			{
				vector = readOnlyHandJointPoses[HandJointUtils.GetHandFingerTip(finger)].position - position;
			}
			else
			{
				vector = readOnlyHandJointPoses[HandJointUtils.GetHandFingerTip(finger)].position - b;
			}
			Vector3 vector2 = readOnlyHandJointPoses[HandJointUtils.GetHandFingerTip(finger2)].position - b;
			Vector3 axis = Vector3.Cross(vector, vector2);
			return Vector3.SignedAngle(vector, vector2, axis);
		}

		public float GetOppositionValue(HandFinger finger, IHand hand)
		{
			ReadOnlyHandJointPoses readOnlyHandJointPoses;
			if (finger == HandFinger.Thumb || !hand.GetJointPosesFromWrist(out readOnlyHandJointPoses))
			{
				return 0f;
			}
			Vector3 position = readOnlyHandJointPoses[HandJointUtils.GetHandFingerTip(finger)].position;
			Vector3 position2 = readOnlyHandJointPoses[HandJointId.HandThumbTip].position;
			return Vector3.Magnitude(position - position2);
		}

		public virtual IReadOnlyList<HandJointId> GetJointsAffected(HandFinger finger, FingerFeature feature)
		{
			switch (feature)
			{
			case FingerFeature.Curl:
				return FingerShapes.CURL_LINE_JOINTS[(int)finger];
			case FingerFeature.Flexion:
				return FingerShapes.FLEXION_LINE_JOINTS[(int)finger];
			case FingerFeature.Abduction:
				return FingerShapes.ABDUCTION_LINE_JOINTS[(int)finger];
			case FingerFeature.Opposition:
				return FingerShapes.OPPOSITION_LINE_JOINTS[(int)finger];
			default:
				return null;
			}
		}

		private static readonly HandJointId[][] CURL_LINE_JOINTS = new HandJointId[][]
		{
			new HandJointId[]
			{
				HandJointId.HandThumb2,
				HandJointId.HandThumb3,
				HandJointId.HandThumbTip
			},
			new HandJointId[]
			{
				HandJointId.HandIndex2,
				HandJointId.HandIndex3,
				HandJointId.HandIndexTip
			},
			new HandJointId[]
			{
				HandJointId.HandMiddle2,
				HandJointId.HandMiddle3,
				HandJointId.HandMiddleTip
			},
			new HandJointId[]
			{
				HandJointId.HandRing2,
				HandJointId.HandRing3,
				HandJointId.HandRingTip
			},
			new HandJointId[]
			{
				HandJointId.HandPinky2,
				HandJointId.HandPinky3,
				HandJointId.HandPinkyTip
			}
		};

		private static readonly HandJointId[][] FLEXION_LINE_JOINTS = new HandJointId[][]
		{
			new HandJointId[]
			{
				HandJointId.HandThumb1,
				HandJointId.HandThumb2,
				HandJointId.HandThumb3
			},
			new HandJointId[]
			{
				HandJointId.HandIndex1,
				HandJointId.HandIndex2,
				HandJointId.HandIndex3
			},
			new HandJointId[]
			{
				HandJointId.HandMiddle1,
				HandJointId.HandMiddle2,
				HandJointId.HandMiddle3
			},
			new HandJointId[]
			{
				HandJointId.HandRing1,
				HandJointId.HandRing2,
				HandJointId.HandRing3
			},
			new HandJointId[]
			{
				HandJointId.HandPinky1,
				HandJointId.HandPinky2,
				HandJointId.HandPinky3
			}
		};

		private static readonly HandJointId[][] ABDUCTION_LINE_JOINTS = new HandJointId[][]
		{
			new HandJointId[]
			{
				HandJointId.HandThumbTip,
				HandJointId.HandThumb1,
				HandJointId.HandIndex1,
				HandJointId.HandIndexTip
			},
			new HandJointId[]
			{
				HandJointId.HandIndexTip,
				HandJointId.HandIndex1,
				HandJointId.HandMiddle1,
				HandJointId.HandMiddleTip
			},
			new HandJointId[]
			{
				HandJointId.HandMiddleTip,
				HandJointId.HandMiddle1,
				HandJointId.HandRing1,
				HandJointId.HandRingTip
			},
			new HandJointId[]
			{
				HandJointId.HandRingTip,
				HandJointId.HandRing1,
				HandJointId.HandPinky1,
				HandJointId.HandPinkyTip
			},
			Array.Empty<HandJointId>()
		};

		private static readonly HandJointId[][] OPPOSITION_LINE_JOINTS = new HandJointId[][]
		{
			Array.Empty<HandJointId>(),
			new HandJointId[]
			{
				HandJointId.HandThumbTip,
				HandJointId.HandIndexTip
			},
			new HandJointId[]
			{
				HandJointId.HandThumbTip,
				HandJointId.HandMiddleTip
			},
			new HandJointId[]
			{
				HandJointId.HandThumbTip,
				HandJointId.HandRingTip
			},
			new HandJointId[]
			{
				HandJointId.HandThumbTip,
				HandJointId.HandPinkyTip
			}
		};

		private static readonly HandJointId[][] CURL_ANGLE_JOINTS = new HandJointId[][]
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
				HandJointId.HandIndex1,
				HandJointId.HandIndex2,
				HandJointId.HandIndex3,
				HandJointId.HandIndexTip
			},
			new HandJointId[]
			{
				HandJointId.HandMiddle1,
				HandJointId.HandMiddle2,
				HandJointId.HandMiddle3,
				HandJointId.HandMiddleTip
			},
			new HandJointId[]
			{
				HandJointId.HandRing1,
				HandJointId.HandRing2,
				HandJointId.HandRing3,
				HandJointId.HandRingTip
			},
			new HandJointId[]
			{
				HandJointId.HandPinky1,
				HandJointId.HandPinky2,
				HandJointId.HandPinky3,
				HandJointId.HandPinkyTip
			}
		};
	}
}
