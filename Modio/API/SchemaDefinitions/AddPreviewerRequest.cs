using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddPreviewerRequest : IApiRequest
	{
		[JsonConstructor]
		public AddPreviewerRequest(long member)
		{
			this.Member = member;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddPreviewerRequest._bodyParameters.Clear();
			AddPreviewerRequest._bodyParameters.Add("member", this.Member);
			return AddPreviewerRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long Member;
	}
}
