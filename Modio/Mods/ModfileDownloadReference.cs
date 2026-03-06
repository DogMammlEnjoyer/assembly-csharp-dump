using System;
using Modio.API.SchemaDefinitions;
using Modio.Extensions;

namespace Modio.Mods
{
	public struct ModfileDownloadReference
	{
		internal ModfileDownloadReference(string binaryUrl, DateTime expiresAfter)
		{
			this.BinaryUrl = binaryUrl;
			this.ExpiresAfter = expiresAfter;
		}

		internal ModfileDownloadReference(DownloadObject downloadObject)
		{
			this.BinaryUrl = downloadObject.BinaryUrl;
			this.ExpiresAfter = downloadObject.DateExpires.GetUtcDateTime();
		}

		public readonly string BinaryUrl;

		public readonly DateTime ExpiresAfter;
	}
}
