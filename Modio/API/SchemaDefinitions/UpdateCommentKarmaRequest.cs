using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct UpdateCommentKarmaRequest : IApiRequest
	{
		[JsonConstructor]
		public UpdateCommentKarmaRequest(long karma)
		{
			this.Karma = karma;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			UpdateCommentKarmaRequest._bodyParameters.Clear();
			UpdateCommentKarmaRequest._bodyParameters.Add("karma", this.Karma);
			return UpdateCommentKarmaRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long Karma;
	}
}
