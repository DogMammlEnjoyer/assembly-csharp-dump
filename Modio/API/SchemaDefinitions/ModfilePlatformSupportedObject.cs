using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ModfilePlatformSupportedObject
	{
		[JsonConstructor]
		public ModfilePlatformSupportedObject(string[] targetted, string[] approved, string[] denied, string[] live, string[] pending)
		{
			this.Targetted = targetted;
			this.Approved = approved;
			this.Denied = denied;
			this.Live = live;
			this.Pending = pending;
		}

		internal readonly string[] Targetted;

		internal readonly string[] Approved;

		internal readonly string[] Denied;

		internal readonly string[] Live;

		internal readonly string[] Pending;
	}
}
