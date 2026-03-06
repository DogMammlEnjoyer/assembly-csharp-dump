using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AdminEditModPriceRequest : IApiRequest
	{
		[JsonConstructor]
		public AdminEditModPriceRequest(long price)
		{
			this.Price = price;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AdminEditModPriceRequest._bodyParameters.Clear();
			AdminEditModPriceRequest._bodyParameters.Add("price", this.Price);
			return AdminEditModPriceRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long Price;
	}
}
