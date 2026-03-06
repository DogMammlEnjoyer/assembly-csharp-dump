using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ReorderModMediaRequest : IApiRequest
	{
		[JsonConstructor]
		public ReorderModMediaRequest(string[] images, string[] youtube, string[] sketchfab)
		{
			this.Images = images;
			this.Youtube = youtube;
			this.Sketchfab = sketchfab;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			ReorderModMediaRequest._bodyParameters.Clear();
			ReorderModMediaRequest._bodyParameters.Add("images", this.Images);
			ReorderModMediaRequest._bodyParameters.Add("youtube", this.Youtube);
			ReorderModMediaRequest._bodyParameters.Add("sketchfab", this.Sketchfab);
			return ReorderModMediaRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string[] Images;

		internal readonly string[] Youtube;

		internal readonly string[] Sketchfab;
	}
}
