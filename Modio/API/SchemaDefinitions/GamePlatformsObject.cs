using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GamePlatformsObject
	{
		[JsonConstructor]
		public GamePlatformsObject(string platform, string label, bool moderated, bool locked)
		{
			this.Platform = platform;
			this.Label = label;
			this.Moderated = moderated;
			this.Locked = locked;
		}

		internal readonly string Platform;

		internal readonly string Label;

		internal readonly bool Moderated;

		internal readonly bool Locked;
	}
}
