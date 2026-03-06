using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct UpdateCommentRequest : IApiRequest
	{
		[JsonConstructor]
		public UpdateCommentRequest(string content)
		{
			this.Content = content;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			UpdateCommentRequest._bodyParameters.Clear();
			UpdateCommentRequest._bodyParameters.Add("content", this.Content);
			return UpdateCommentRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Content;
	}
}
