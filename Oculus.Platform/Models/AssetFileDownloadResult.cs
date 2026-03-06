using System;

namespace Oculus.Platform.Models
{
	public class AssetFileDownloadResult
	{
		public AssetFileDownloadResult(IntPtr o)
		{
			this.AssetId = CAPI.ovr_AssetFileDownloadResult_GetAssetId(o);
			this.Filepath = CAPI.ovr_AssetFileDownloadResult_GetFilepath(o);
		}

		public readonly ulong AssetId;

		public readonly string Filepath;
	}
}
