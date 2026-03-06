using System;

namespace Oculus.Platform.Models
{
	public class AssetDetails
	{
		public AssetDetails(IntPtr o)
		{
			this.AssetId = CAPI.ovr_AssetDetails_GetAssetId(o);
			this.AssetType = CAPI.ovr_AssetDetails_GetAssetType(o);
			this.DownloadStatus = CAPI.ovr_AssetDetails_GetDownloadStatus(o);
			this.Filepath = CAPI.ovr_AssetDetails_GetFilepath(o);
			this.IapStatus = CAPI.ovr_AssetDetails_GetIapStatus(o);
			IntPtr intPtr = CAPI.ovr_AssetDetails_GetLanguage(o);
			this.Language = new LanguagePackInfo(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				this.LanguageOptional = null;
			}
			else
			{
				this.LanguageOptional = this.Language;
			}
			this.Metadata = CAPI.ovr_AssetDetails_GetMetadata(o);
		}

		public readonly ulong AssetId;

		public readonly string AssetType;

		public readonly string DownloadStatus;

		public readonly string Filepath;

		public readonly string IapStatus;

		public readonly LanguagePackInfo LanguageOptional;

		[Obsolete("Deprecated in favor of LanguageOptional")]
		public readonly LanguagePackInfo Language;

		public readonly string Metadata;
	}
}
