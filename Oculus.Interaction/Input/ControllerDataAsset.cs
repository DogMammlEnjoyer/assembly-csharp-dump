using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Serializable]
	public class ControllerDataAsset : ICopyFrom<ControllerDataAsset>
	{
		public void CopyFrom(ControllerDataAsset source)
		{
			this.IsDataValid = source.IsDataValid;
			this.IsConnected = source.IsConnected;
			this.IsTracked = source.IsTracked;
			this.IsDominantHand = source.IsDominantHand;
			this.Config = source.Config;
			this.CopyPosesAndStateFrom(source);
		}

		public void CopyPosesAndStateFrom(ControllerDataAsset source)
		{
			this.Input = source.Input;
			this.RootPose = source.RootPose;
			this.RootPoseOrigin = source.RootPoseOrigin;
			this.PointerPose = source.PointerPose;
			this.PointerPoseOrigin = source.PointerPoseOrigin;
		}

		public bool IsDataValid;

		public bool IsConnected;

		public bool IsTracked;

		public ControllerInput Input;

		public Pose RootPose;

		public PoseOrigin RootPoseOrigin;

		public Pose PointerPose;

		public PoseOrigin PointerPoseOrigin;

		public bool IsDominantHand;

		public ControllerDataSourceConfig Config;
	}
}
