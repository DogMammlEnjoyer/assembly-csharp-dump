using System;

namespace UnityEngine.Rendering
{
	internal struct BinningConfig
	{
		public int visibilityConfigCount
		{
			get
			{
				int num = 1 + this.viewCount + (this.supportsCrossFade ? 1 : 0) + (this.supportsMotionCheck ? 1 : 0);
				return 1 << num;
			}
		}

		public int viewCount;

		public bool supportsCrossFade;

		public bool supportsMotionCheck;
	}
}
