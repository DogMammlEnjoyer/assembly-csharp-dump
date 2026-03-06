using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(MemberSerialization.Fields)]
	internal readonly struct ModMediaObject
	{
		[JsonConstructor]
		public ModMediaObject(string[] youtube, string[] sketchfab, ImageObject[] images)
		{
			this.Youtube = youtube;
			this.Sketchfab = sketchfab;
			this.Images = images;
		}

		internal readonly string[] Youtube;

		internal readonly string[] Sketchfab;

		internal readonly ImageObject[] Images;
	}
}
