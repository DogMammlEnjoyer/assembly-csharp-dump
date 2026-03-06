using System;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	public struct DownloadStatus
	{
		public float Percent
		{
			get
			{
				if (this.TotalBytes > 0L)
				{
					return (float)this.DownloadedBytes / (float)this.TotalBytes;
				}
				if (!this.IsDone)
				{
					return 0f;
				}
				return 1f;
			}
		}

		public long TotalBytes;

		public long DownloadedBytes;

		public bool IsDone;
	}
}
