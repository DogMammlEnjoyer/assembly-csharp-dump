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
	internal readonly struct EditModRequest : IApiRequest
	{
		[JsonConstructor]
		public EditModRequest(string name, string nameId, string summary, string description, ModioAPIFileParameter? logo, long? visible, long? maturity_option, long? community_options, string metadataBlob, [Nullable(new byte[]
		{
			2,
			1
		})] string[] tags, long? monetizationOptions, long? price, long? stock)
		{
			this.Name = name;
			this.NameId = nameId;
			this.Summary = summary;
			this.Description = description;
			this.Logo = logo;
			this.Visible = visible;
			this.MaturityOption = maturity_option;
			this.CommunityOptions = community_options;
			this.MetadataBlob = metadataBlob;
			this.Tags = tags;
			this.MonetizationOptions = monetizationOptions;
			this.Price = price;
			this.Stock = stock;
		}

		[NullableContext(0)]
		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			EditModRequest._bodyParameters.Clear();
			if (!string.IsNullOrEmpty(this.Name))
			{
				EditModRequest._bodyParameters.Add("name", this.Name);
			}
			if (!string.IsNullOrEmpty(this.NameId))
			{
				EditModRequest._bodyParameters.Add("name_id", this.NameId);
			}
			if (!string.IsNullOrEmpty(this.Summary))
			{
				EditModRequest._bodyParameters.Add("summary", this.Summary);
			}
			if (!string.IsNullOrEmpty(this.Description))
			{
				EditModRequest._bodyParameters.Add("description", this.Description);
			}
			if (this.Logo != null)
			{
				EditModRequest._bodyParameters.Add("logo", this.Logo);
			}
			if (this.Visible != null)
			{
				EditModRequest._bodyParameters.Add("visible", this.Visible);
			}
			if (this.MaturityOption != null)
			{
				EditModRequest._bodyParameters.Add("maturity_option", this.MaturityOption);
			}
			if (this.CommunityOptions != null)
			{
				EditModRequest._bodyParameters.Add("community_options", this.CommunityOptions);
			}
			if (!string.IsNullOrEmpty(this.MetadataBlob))
			{
				EditModRequest._bodyParameters.Add("metadata_blob", this.MetadataBlob);
			}
			if (this.Tags != null)
			{
				EditModRequest._bodyParameters.Add("tags", this.Tags);
			}
			if (this.MonetizationOptions != null)
			{
				EditModRequest._bodyParameters.Add("monetization_options", this.MonetizationOptions);
			}
			if (this.Price != null)
			{
				EditModRequest._bodyParameters.Add("price", this.Price);
			}
			if (this.Stock != null)
			{
				EditModRequest._bodyParameters.Add("stock", this.Stock);
			}
			return EditModRequest._bodyParameters;
		}

		[Nullable(0)]
		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Name;

		internal readonly string NameId;

		internal readonly string Summary;

		internal readonly string Description;

		internal readonly ModioAPIFileParameter? Logo;

		internal readonly long? Visible;

		internal readonly long? MaturityOption;

		internal readonly long? CommunityOptions;

		internal readonly string MetadataBlob;

		[Nullable(new byte[]
		{
			2,
			1
		})]
		internal readonly string[] Tags;

		internal readonly long? MonetizationOptions;

		internal readonly long? Price;

		internal readonly long? Stock;
	}
}
