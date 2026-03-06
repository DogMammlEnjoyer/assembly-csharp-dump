using System;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	[Serializable]
	public class TransformConfig
	{
		public TransformConfig()
		{
			this.PositionOffset = Vector3.zero;
			this.RotationOffset = Vector3.zero;
			this.UpVectorType = UpVectorType.Head;
			this.FeatureThresholds = null;
			this.InstanceId = 0;
		}

		public int InstanceId { get; set; }

		public Vector3 PositionOffset;

		public Vector3 RotationOffset;

		public UpVectorType UpVectorType;

		public TransformFeatureStateThresholds FeatureThresholds;
	}
}
