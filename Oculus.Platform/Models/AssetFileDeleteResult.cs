using System;

namespace Oculus.Platform.Models
{
	public class AssetFileDeleteResult
	{
		public AssetFileDeleteResult(IntPtr o)
		{
			this.AssetFileId = CAPI.ovr_AssetFileDeleteResult_GetAssetFileId(o);
			this.AssetId = CAPI.ovr_AssetFileDeleteResult_GetAssetId(o);
			this.Filepath = CAPI.ovr_AssetFileDeleteResult_GetFilepath(o);
			this.Success = CAPI.ovr_AssetFileDeleteResult_GetSuccess(o);
		}

		public readonly ulong AssetFileId;

		public readonly ulong AssetId;

		public readonly string Filepath;

		public readonly bool Success;
	}
}
