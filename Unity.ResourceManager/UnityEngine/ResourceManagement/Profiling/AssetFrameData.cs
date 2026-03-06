using System;

namespace UnityEngine.ResourceManagement.Profiling
{
	internal struct AssetFrameData
	{
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is AssetFrameData)
			{
				AssetFrameData assetFrameData = (AssetFrameData)obj;
				return this.AssetCode == assetFrameData.AssetCode && this.BundleCode == assetFrameData.BundleCode;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<int, int, int, int, int>(this.AssetCode.GetHashCode(), this.BundleCode.GetHashCode(), this.ReferenceCount.GetHashCode(), this.PercentComplete.GetHashCode(), this.Status.GetHashCode());
		}

		public int AssetCode;

		public int BundleCode;

		public int ReferenceCount;

		public float PercentComplete;

		public ContentStatus Status;
	}
}
