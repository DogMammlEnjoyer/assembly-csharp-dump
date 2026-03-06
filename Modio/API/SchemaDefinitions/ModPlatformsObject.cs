using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(MemberSerialization.Fields)]
	internal readonly struct ModPlatformsObject
	{
		[JsonConstructor]
		public ModPlatformsObject(string platform, long modfile_live)
		{
			this.Platform = platform;
			this.ModfileLive = modfile_live;
		}

		internal readonly string Platform;

		internal readonly long ModfileLive;
	}
}
