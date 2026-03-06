using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct MultipartUploadPartObject
	{
		[JsonConstructor]
		public MultipartUploadPartObject(string upload_id, long part_number, long part_size, long date_added)
		{
			this.UploadId = upload_id;
			this.PartNumber = part_number;
			this.PartSize = part_size;
			this.DateAdded = date_added;
		}

		internal readonly string UploadId;

		internal readonly long PartNumber;

		internal readonly long PartSize;

		internal readonly long DateAdded;
	}
}
