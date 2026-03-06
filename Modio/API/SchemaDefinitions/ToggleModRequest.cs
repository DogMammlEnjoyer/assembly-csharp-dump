using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ToggleModRequest : IApiRequest
	{
		[JsonConstructor]
		public ToggleModRequest(string marketplace_effect, string limited_effect, string code)
		{
			this.MarketplaceEffect = marketplace_effect;
			this.LimitedEffect = limited_effect;
			this.Code = code;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			ToggleModRequest._bodyParameters.Clear();
			ToggleModRequest._bodyParameters.Add("marketplace_effect", this.MarketplaceEffect);
			ToggleModRequest._bodyParameters.Add("limited_effect", this.LimitedEffect);
			ToggleModRequest._bodyParameters.Add("code", this.Code);
			return ToggleModRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string MarketplaceEffect;

		internal readonly string LimitedEffect;

		internal readonly string Code;
	}
}
