using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct MultipartUploadObject
	{
		[JsonConstructor]
		public MultipartUploadObject(string upload_id, long status)
		{
			this.UploadId = upload_id;
			this.Status = status;
		}

		internal readonly string UploadId;

		internal readonly long Status;
	}
}
