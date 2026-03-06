using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddModMediaRequest : IApiRequest
	{
		public bool GallerySync { get; }

		[JsonConstructor]
		public AddModMediaRequest(ModioAPIFileParameter media, bool gallerySync)
		{
			this._media = media;
			this.GallerySync = gallerySync;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddModMediaRequest._bodyParameters.Clear();
			AddModMediaRequest._bodyParameters.Add(this._media.MediaType, this._media);
			return AddModMediaRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly ModioAPIFileParameter _media;
	}
}
