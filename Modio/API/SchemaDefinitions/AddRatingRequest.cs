using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddRatingRequest : IApiRequest
	{
		[JsonConstructor]
		public AddRatingRequest(long rating)
		{
			this.Rating = rating;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddRatingRequest._bodyParameters.Clear();
			AddRatingRequest._bodyParameters.Add("rating", this.Rating);
			return AddRatingRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long Rating;
	}
}
