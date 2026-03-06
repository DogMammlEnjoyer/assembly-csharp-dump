using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddReportRequest : IApiRequest
	{
		[JsonConstructor]
		public AddReportRequest(string resource, long id, long type, long reason, string platforms, string name, string contact, string summary)
		{
			this.Resource = resource;
			this.Id = id;
			this.Type = type;
			this.Reason = reason;
			this.Platforms = platforms;
			this.Name = name;
			this.Contact = contact;
			this.Summary = summary;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddReportRequest._bodyParameters.Clear();
			AddReportRequest._bodyParameters.Add("resource", this.Resource);
			AddReportRequest._bodyParameters.Add("id", this.Id);
			AddReportRequest._bodyParameters.Add("type", this.Type);
			AddReportRequest._bodyParameters.Add("reason", this.Reason);
			AddReportRequest._bodyParameters.Add("platforms", this.Platforms);
			AddReportRequest._bodyParameters.Add("name", this.Name);
			AddReportRequest._bodyParameters.Add("contact", this.Contact);
			AddReportRequest._bodyParameters.Add("summary", this.Summary);
			return AddReportRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Resource;

		internal readonly long Id;

		internal readonly long Type;

		internal readonly long Reason;

		internal readonly string Platforms;

		internal readonly string Name;

		internal readonly string Contact;

		internal readonly string Summary;
	}
}
