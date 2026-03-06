using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(MemberSerialization.Fields)]
	internal readonly struct ModfilePlatformObject
	{
		[JsonConstructor]
		public ModfilePlatformObject(string platform, long status)
		{
			this.Platform = platform;
			this.Status = status;
		}

		internal readonly string Platform;

		internal readonly long Status;
	}
}
