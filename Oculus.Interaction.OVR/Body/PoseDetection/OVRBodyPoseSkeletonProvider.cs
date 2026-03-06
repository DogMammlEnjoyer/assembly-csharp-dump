using System;
using System.Runtime.CompilerServices;
using Meta.XR.Util;
using Oculus.Interaction.Body.Input;
using UnityEngine;

namespace Oculus.Interaction.Body.PoseDetection
{
	[Feature(Feature.Interaction)]
	public class OVRBodyPoseSkeletonProvider : MonoBehaviour, OVRSkeleton.IOVRSkeletonDataProvider
	{
		protected virtual void Awake()
		{
			this.BodyPose = (this._bodyPose as IBodyPose);
		}

		protected virtual void Start()
		{
			this._mapping = new OVRSkeletonMapping(this._bodyJointSet);
		}

		OVRSkeleton.SkeletonPoseData OVRSkeleton.IOVRSkeletonDataProvider.GetSkeletonPoseData()
		{
			this._boneRotations = OVRBodyPoseSkeletonProvider.<OVRSkeleton.IOVRSkeletonDataProvider.GetSkeletonPoseData>g__EnsureLength|9_0<OVRPlugin.Quatf>(this._boneRotations, 84);
			this._boneTranslations = OVRBodyPoseSkeletonProvider.<OVRSkeleton.IOVRSkeletonDataProvider.GetSkeletonPoseData>g__EnsureLength|9_0<OVRPlugin.Vector3f>(this._boneTranslations, 84);
			for (int i = 0; i < 84; i++)
			{
				OVRPlugin.BoneId jointId = (OVRPlugin.BoneId)i;
				BodyJointId bodyJointId;
				Pose pose;
				if (this._mapping.TryGetBodyJointId(jointId, out bodyJointId) && this.BodyPose.GetJointPoseFromRoot(bodyJointId, out pose))
				{
					this._boneRotations[i] = pose.rotation.ToFlippedZQuatf();
					this._boneTranslations[i] = pose.position.ToFlippedZVector3f();
				}
			}
			Pose pose2;
			OVRPlugin.Posef rootPose;
			if (this.BodyPose.GetJointPoseFromRoot(BodyJointId.Body_Start, out pose2))
			{
				rootPose = new OVRPlugin.Posef
				{
					Orientation = pose2.rotation.ToFlippedXQuatf(),
					Position = pose2.position.ToFlippedZVector3f()
				};
			}
			else
			{
				rootPose = default(OVRPlugin.Posef);
			}
			return new OVRSkeleton.SkeletonPoseData
			{
				IsDataValid = true,
				IsDataHighConfidence = true,
				RootPose = rootPose,
				RootScale = 1f,
				BoneRotations = this._boneRotations,
				BoneTranslations = this._boneTranslations
			};
		}

		public OVRSkeleton.SkeletonType GetSkeletonType()
		{
			OVRPlugin.BodyJointSet bodyJointSet = this._bodyJointSet;
			OVRSkeleton.SkeletonType result;
			if (bodyJointSet != OVRPlugin.BodyJointSet.UpperBody)
			{
				if (bodyJointSet != OVRPlugin.BodyJointSet.FullBody)
				{
					result = OVRSkeleton.SkeletonType.None;
				}
				else
				{
					result = OVRSkeleton.SkeletonType.FullBody;
				}
			}
			else
			{
				result = OVRSkeleton.SkeletonType.Body;
			}
			return result;
		}

		bool OVRSkeleton.IOVRSkeletonDataProvider.get_enabled()
		{
			return base.enabled;
		}

		[CompilerGenerated]
		internal static T[] <OVRSkeleton.IOVRSkeletonDataProvider.GetSkeletonPoseData>g__EnsureLength|9_0<T>(T[] array, int length)
		{
			if (array == null || array.Length != length)
			{
				return new T[length];
			}
			return array;
		}

		private const int OVR_NUM_JOINTS = 84;

		[SerializeField]
		[Interface(typeof(IBodyPose), new Type[]
		{

		})]
		private Object _bodyPose;

		private IBodyPose BodyPose;

		[SerializeField]
		private OVRPlugin.BodyJointSet _bodyJointSet;

		private OVRPlugin.Quatf[] _boneRotations = new OVRPlugin.Quatf[84];

		private OVRPlugin.Vector3f[] _boneTranslations = new OVRPlugin.Vector3f[84];

		private OVRSkeletonMapping _mapping;
	}
}
