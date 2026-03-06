using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AvatarObject
	{
		[JsonConstructor]
		public AvatarObject(string filename, string original, string thumb_50x50, string thumb_100x100)
		{
			this.Filename = filename;
			this.Original = original;
			this.Thumb50X50 = thumb_50x50;
			this.Thumb100X100 = thumb_100x100;
		}

		internal readonly string Filename;

		internal readonly string Original;

		internal readonly string Thumb50X50;

		internal readonly string Thumb100X100;
	}
}
