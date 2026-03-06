using System;

namespace Oculus.Platform.Models
{
	public class AssetFileDownloadUpdate
	{
		public AssetFileDownloadUpdate(IntPtr o)
		{
			this.AssetFileId = CAPI.ovr_AssetFileDownloadUpdate_GetAssetFileId(o);
			this.AssetId = CAPI.ovr_AssetFileDownloadUpdate_GetAssetId(o);
			this.BytesTotal = CAPI.ovr_AssetFileDownloadUpdate_GetBytesTotalLong(o);
			this.BytesTransferred = CAPI.ovr_AssetFileDownloadUpdate_GetBytesTransferredLong(o);
			this.Completed = CAPI.ovr_AssetFileDownloadUpdate_GetCompleted(o);
		}

		public readonly ulong AssetFileId;

		public readonly ulong AssetId;

		public readonly ulong BytesTotal;

		public readonly long BytesTransferred;

		public readonly bool Completed;
	}
}
