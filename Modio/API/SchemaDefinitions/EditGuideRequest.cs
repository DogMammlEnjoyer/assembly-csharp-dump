using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct EditGuideRequest : IApiRequest
	{
		[JsonConstructor]
		public EditGuideRequest(string name, string summary, string description, ModioAPIFileParameter logo, long date_live)
		{
			this.Name = name;
			this.Summary = summary;
			this.Description = description;
			this.Logo = logo;
			this.DateLive = date_live;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			EditGuideRequest._bodyParameters.Clear();
			EditGuideRequest._bodyParameters.Add("name", this.Name);
			EditGuideRequest._bodyParameters.Add("summary", this.Summary);
			EditGuideRequest._bodyParameters.Add("description", this.Description);
			EditGuideRequest._bodyParameters.Add("logo", this.Logo);
			EditGuideRequest._bodyParameters.Add("date_live", this.DateLive);
			return EditGuideRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Name;

		internal readonly string Summary;

		internal readonly string Description;

		internal readonly ModioAPIFileParameter Logo;

		internal readonly long DateLive;
	}
}
