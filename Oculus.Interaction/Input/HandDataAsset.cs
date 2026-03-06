using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Serializable]
	public class HandDataAsset : ICopyFrom<HandDataAsset>
	{
		public bool IsDataValidAndConnected
		{
			get
			{
				return this.IsDataValid && this.IsConnected;
			}
		}

		public void CopyFrom(HandDataAsset source)
		{
			this.IsDataValid = source.IsDataValid;
			this.IsConnected = source.IsConnected;
			this.IsTracked = source.IsTracked;
			this.IsHighConfidence = source.IsHighConfidence;
			this.IsDominantHand = source.IsDominantHand;
			this.Config = source.Config;
			this.CopyPosesFrom(source);
		}

		public void CopyPosesFrom(HandDataAsset source)
		{
			this.Root = source.Root;
			this.RootPoseOrigin = source.RootPoseOrigin;
			Array.Copy(source.JointPoses, this.JointPoses, 26);
			Array.Copy(source.JointRadii, this.JointRadii, source.JointRadii.Length);
			Array.Copy(source.Joints, this.Joints, 26);
			Array.Copy(source.IsFingerPinching, this.IsFingerPinching, this.IsFingerPinching.Length);
			Array.Copy(source.IsFingerHighConfidence, this.IsFingerHighConfidence, this.IsFingerHighConfidence.Length);
			Array.Copy(source.FingerPinchStrength, this.FingerPinchStrength, this.FingerPinchStrength.Length);
			this.HandScale = source.HandScale;
			this.PointerPose = source.PointerPose;
			this.PointerPoseOrigin = source.PointerPoseOrigin;
			this.Config = source.Config;
		}

		public bool IsDataValid;

		public bool IsConnected;

		public bool IsTracked;

		public Pose Root;

		public PoseOrigin RootPoseOrigin;

		public Pose[] JointPoses = new Pose[26];

		public float[] JointRadii = new float[26];

		[Obsolete("Deprecated. Use JointPoses instead.")]
		public Quaternion[] Joints = new Quaternion[26];

		public bool IsHighConfidence;

		public bool[] IsFingerPinching = new bool[5];

		public bool[] IsFingerHighConfidence = new bool[5];

		public float[] FingerPinchStrength = new float[5];

		public float HandScale;

		public Pose PointerPose;

		public PoseOrigin PointerPoseOrigin;

		public bool IsDominantHand;

		public HandDataSourceConfig Config = new HandDataSourceConfig();
	}
}
