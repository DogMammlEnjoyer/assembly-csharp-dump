using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddCommentRequest : IApiRequest
	{
		[JsonConstructor]
		public AddCommentRequest(long replyid, string content)
		{
			this.Replyid = replyid;
			this.Content = content;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddCommentRequest._bodyParameters.Clear();
			AddCommentRequest._bodyParameters.Add("replyid", this.Replyid);
			AddCommentRequest._bodyParameters.Add("content", this.Content);
			return AddCommentRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long Replyid;

		internal readonly string Content;
	}
}
