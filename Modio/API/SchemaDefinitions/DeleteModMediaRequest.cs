using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct DeleteModMediaRequest : IApiRequest
	{
		[JsonConstructor]
		public DeleteModMediaRequest(string[] images, string[] youtube, string[] sketchfab)
		{
			this.Images = images;
			this.Youtube = youtube;
			this.Sketchfab = sketchfab;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			DeleteModMediaRequest._bodyParameters.Clear();
			DeleteModMediaRequest._bodyParameters.Add("images", this.Images);
			DeleteModMediaRequest._bodyParameters.Add("youtube", this.Youtube);
			DeleteModMediaRequest._bodyParameters.Add("sketchfab", this.Sketchfab);
			return DeleteModMediaRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string[] Images;

		internal readonly string[] Youtube;

		internal readonly string[] Sketchfab;
	}
}
