using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(MemberSerialization.Fields)]
	internal readonly struct AddModRequest : IApiRequest
	{
		[NullableContext(2)]
		[JsonConstructor]
		public AddModRequest([Nullable(1)] string name, string name_id, [Nullable(1)] string summary, string description, ModioAPIFileParameter logo, long? visible, long? maturity_option, long? community_options, string metadata_blob, [Nullable(new byte[]
		{
			2,
			1
		})] string[] tags)
		{
			this.Name = name;
			this.NameId = name_id;
			this.Summary = summary;
			this.Description = description;
			this.Logo = logo;
			this.Visible = visible;
			this.MaturityOption = maturity_option;
			this.CommunityOptions = community_options;
			this.MetadataBlob = metadata_blob;
			this.Tags = tags;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddModRequest._bodyParameters.Clear();
			AddModRequest._bodyParameters.Add("name", this.Name);
			AddModRequest._bodyParameters.Add("summary", this.Summary);
			AddModRequest._bodyParameters.Add("logo", this.Logo);
			if (!string.IsNullOrEmpty(this.NameId))
			{
				AddModRequest._bodyParameters.Add("name_id", this.NameId);
			}
			if (!string.IsNullOrEmpty(this.Description))
			{
				AddModRequest._bodyParameters.Add("description", this.Description);
			}
			if (this.Visible != null)
			{
				AddModRequest._bodyParameters.Add("visible", this.Visible);
			}
			if (this.MaturityOption != null)
			{
				AddModRequest._bodyParameters.Add("maturity_option", this.MaturityOption);
			}
			if (this.CommunityOptions != null)
			{
				AddModRequest._bodyParameters.Add("community_options", this.CommunityOptions);
			}
			if (this.MetadataBlob != null)
			{
				AddModRequest._bodyParameters.Add("metadata_blob", this.MetadataBlob);
			}
			if (this.Tags != null)
			{
				for (int i = 0; i < this.Tags.Length; i++)
				{
					AddModRequest._bodyParameters.Add(string.Format("tags[{0}]", i), this.Tags[i]);
				}
			}
			return AddModRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		[Nullable(1)]
		internal readonly string Name;

		[Nullable(2)]
		internal readonly string NameId;

		[Nullable(1)]
		internal readonly string Summary;

		[Nullable(2)]
		internal readonly string Description;

		internal readonly ModioAPIFileParameter Logo;

		internal readonly long? Visible;

		internal readonly long? MaturityOption;

		internal readonly long? CommunityOptions;

		[Nullable(2)]
		internal readonly string MetadataBlob;

		[Nullable(new byte[]
		{
			2,
			1
		})]
		internal readonly string[] Tags;
	}
}
