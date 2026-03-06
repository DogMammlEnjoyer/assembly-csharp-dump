using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class FixedScaleHand : Hand
	{
		protected override void Apply(HandDataAsset data)
		{
			Pose pose = PoseUtils.Delta(data.Root, data.PointerPose);
			pose.position = pose.position / data.HandScale * this._scale;
			PoseUtils.Multiply(data.Root, pose, ref data.PointerPose);
			data.HandScale = this._scale;
		}

		public void InjectAllFixedScaleDataModifier(DataSource<HandDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, DataModifier<HandDataAsset> modifyDataFromSource, bool applyModifier, float scale)
		{
			base.InjectAllHand(updateMode, updateAfter, modifyDataFromSource, applyModifier);
			this.InjectScale(scale);
		}

		public void InjectScale(float scale)
		{
			this._scale = scale;
		}

		[SerializeField]
		private float _scale = 1f;
	}
}
