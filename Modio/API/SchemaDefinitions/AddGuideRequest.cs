using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddGuideRequest : IApiRequest
	{
		[JsonConstructor]
		public AddGuideRequest(string name, string summary, string description, string logo, long date_live, long status, long community_options, string[] tags, string name_id)
		{
			this.Name = name;
			this.Summary = summary;
			this.Description = description;
			this.Logo = logo;
			this.DateLive = date_live;
			this.Status = status;
			this.CommunityOptions = community_options;
			this.Tags = tags;
			this.NameId = name_id;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddGuideRequest._bodyParameters.Clear();
			AddGuideRequest._bodyParameters.Add("name", this.Name);
			AddGuideRequest._bodyParameters.Add("summary", this.Summary);
			AddGuideRequest._bodyParameters.Add("description", this.Description);
			AddGuideRequest._bodyParameters.Add("logo", this.Logo);
			AddGuideRequest._bodyParameters.Add("date_live", this.DateLive);
			AddGuideRequest._bodyParameters.Add("status", this.Status);
			AddGuideRequest._bodyParameters.Add("community_options", this.CommunityOptions);
			AddGuideRequest._bodyParameters.Add("tags", this.Tags);
			AddGuideRequest._bodyParameters.Add("name_id", this.NameId);
			return AddGuideRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Name;

		internal readonly string Summary;

		internal readonly string Description;

		internal readonly string Logo;

		internal readonly long DateLive;

		internal readonly long Status;

		internal readonly long CommunityOptions;

		internal readonly string[] Tags;

		internal readonly string NameId;
	}
}
