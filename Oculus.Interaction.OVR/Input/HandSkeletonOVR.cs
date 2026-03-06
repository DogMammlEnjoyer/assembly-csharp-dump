using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public class HandSkeletonOVR : MonoBehaviour, IHandSkeletonProvider
	{
		public HandSkeleton this[Handedness handedness]
		{
			get
			{
				return this._skeletons[(int)handedness];
			}
		}

		protected void Awake()
		{
			HandSkeletonOVR.ApplyToSkeleton(OVRSkeletonData.LeftSkeleton, this._skeletons[0]);
			HandSkeletonOVR.ApplyToSkeleton(OVRSkeletonData.RightSkeleton, this._skeletons[1]);
		}

		public static HandSkeleton CreateSkeletonData(Handedness handedness)
		{
			HandSkeleton handSkeleton = new HandSkeleton();
			if (handedness == Handedness.Left)
			{
				HandSkeletonOVR.ApplyToSkeleton(OVRSkeletonData.LeftSkeleton, handSkeleton);
			}
			else
			{
				HandSkeletonOVR.ApplyToSkeleton(OVRSkeletonData.RightSkeleton, handSkeleton);
			}
			return handSkeleton;
		}

		private static void ApplyToSkeleton(in OVRPlugin.Skeleton2 ovrSkeleton, HandSkeleton handSkeleton)
		{
			int num = handSkeleton.joints.Length;
			for (int i = 0; i < num; i++)
			{
				ref OVRPlugin.Posef ptr = ref ovrSkeleton.Bones[i].Pose;
				handSkeleton.joints[i] = new HandSkeletonJoint
				{
					pose = new Pose
					{
						position = ptr.Position.FromFlippedZVector3f(),
						rotation = ptr.Orientation.FromFlippedZQuatf()
					},
					parent = (int)ovrSkeleton.Bones[i].ParentBoneIndex
				};
			}
		}

		internal static float GetBoneRadius(in OVRPlugin.Skeleton2 ovrSkeleton, int boneIndex)
		{
			if (boneIndex == 6)
			{
				boneIndex = 7;
			}
			else if (boneIndex == 11)
			{
				boneIndex = 12;
			}
			else if (boneIndex == 16)
			{
				boneIndex = 17;
			}
			else if (boneIndex == 21)
			{
				boneIndex = 22;
			}
			int num = Array.FindIndex<OVRPlugin.BoneCapsule>(ovrSkeleton.BoneCapsules, (OVRPlugin.BoneCapsule c) => (int)c.BoneIndex == boneIndex);
			if (num >= 0)
			{
				return ovrSkeleton.BoneCapsules[num].Radius;
			}
			return 0f;
		}

		private readonly HandSkeleton[] _skeletons = new HandSkeleton[]
		{
			new HandSkeleton(),
			new HandSkeleton()
		};
	}
}
