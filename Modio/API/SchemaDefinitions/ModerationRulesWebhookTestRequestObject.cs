using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ModerationRulesWebhookTestRequestObject
	{
		[JsonConstructor]
		public ModerationRulesWebhookTestRequestObject(string url)
		{
			this.Url = url;
		}

		internal readonly string Url;
	}
}
