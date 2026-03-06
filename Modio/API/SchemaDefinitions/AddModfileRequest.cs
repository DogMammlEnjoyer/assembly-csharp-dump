using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Modio.API.SchemaDefinitions
{
	[NullableContext(2)]
	[Nullable(0)]
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	internal readonly struct AddModfileRequest : IApiRequest
	{
		[JsonConstructor]
		public AddModfileRequest(ModioAPIFileParameter filedata, string version, string changelog, string metadataBlob, [Nullable(new byte[]
		{
			2,
			1
		})] string[] platforms, string uploadId)
		{
			this.Filedata = filedata;
			this.Version = version;
			this.Changelog = changelog;
			this.MetadataBlob = metadataBlob;
			this.Platforms = platforms;
			this.UploadId = uploadId;
		}

		[NullableContext(0)]
		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddModfileRequest._bodyParameters.Clear();
			AddModfileRequest._bodyParameters.Add("filedata", this.Filedata);
			if (string.IsNullOrEmpty(this.Version))
			{
				AddModfileRequest._bodyParameters.Add("version", this.Version);
			}
			if (string.IsNullOrEmpty(this.Changelog))
			{
				AddModfileRequest._bodyParameters.Add("changelog", this.Changelog);
			}
			if (string.IsNullOrEmpty(this.MetadataBlob))
			{
				AddModfileRequest._bodyParameters.Add("metadata_blob", this.MetadataBlob);
			}
			if (this.Platforms != null && this.Platforms.Length != 0)
			{
				for (int i = 0; i < this.Platforms.Length; i++)
				{
					AddModfileRequest._bodyParameters.Add(string.Format("platforms[{0}]", i), this.Platforms[i]);
				}
			}
			if (string.IsNullOrEmpty(this.UploadId))
			{
				AddModfileRequest._bodyParameters.Add("upload_id", this.UploadId);
			}
			return AddModfileRequest._bodyParameters;
		}

		[Nullable(0)]
		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly ModioAPIFileParameter Filedata;

		internal readonly string Version;

		internal readonly string Changelog;

		internal readonly string MetadataBlob;

		[Nullable(new byte[]
		{
			2,
			1
		})]
		internal readonly string[] Platforms;

		internal readonly string UploadId;
	}
}
