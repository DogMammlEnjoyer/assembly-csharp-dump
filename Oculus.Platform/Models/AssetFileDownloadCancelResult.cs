using System;

namespace Oculus.Platform.Models
{
	public class AssetFileDownloadCancelResult
	{
		public AssetFileDownloadCancelResult(IntPtr o)
		{
			this.AssetFileId = CAPI.ovr_AssetFileDownloadCancelResult_GetAssetFileId(o);
			this.AssetId = CAPI.ovr_AssetFileDownloadCancelResult_GetAssetId(o);
			this.Filepath = CAPI.ovr_AssetFileDownloadCancelResult_GetFilepath(o);
			this.Success = CAPI.ovr_AssetFileDownloadCancelResult_GetSuccess(o);
		}

		public readonly ulong AssetFileId;

		public readonly ulong AssetId;

		public readonly string Filepath;

		public readonly bool Success;
	}
}
