using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Serializable]
	public class HmdDataAsset : ICopyFrom<HmdDataAsset>
	{
		public void CopyFrom(HmdDataAsset source)
		{
			this.Root = source.Root;
			this.IsTracked = source.IsTracked;
			this.FrameId = source.FrameId;
			this.Config = source.Config;
		}

		public Pose Root;

		public bool IsTracked;

		public int FrameId;

		public HmdDataSourceConfig Config;
	}
}
