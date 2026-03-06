using System;

namespace Oculus.Interaction.Input
{
	public class LastKnownGoodHand : Hand
	{
		protected override void Apply(HandDataAsset data)
		{
			bool flag = data.IsHighConfidence || data.RootPoseOrigin == PoseOrigin.FilteredTrackedPose || data.RootPoseOrigin == PoseOrigin.SyntheticPose;
			if (data.IsDataValid && data.IsTracked && flag)
			{
				this._lastState.CopyFrom(data);
				return;
			}
			if (this._lastState.IsDataValid && data.IsConnected)
			{
				data.CopyPosesFrom(this._lastState);
				data.RootPoseOrigin = PoseOrigin.SyntheticPose;
				data.IsDataValid = true;
				data.IsTracked = true;
				data.IsHighConfidence = true;
				return;
			}
			data.IsTracked = false;
			data.IsHighConfidence = false;
			data.RootPoseOrigin = PoseOrigin.None;
		}

		private readonly HandDataAsset _lastState = new HandDataAsset();
	}
}
