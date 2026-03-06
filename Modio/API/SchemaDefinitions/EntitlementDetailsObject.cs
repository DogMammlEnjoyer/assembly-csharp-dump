using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct EntitlementDetailsObject
	{
		[JsonConstructor]
		public EntitlementDetailsObject(long tokens_allocated)
		{
			this.TokensAllocated = tokens_allocated;
		}

		internal readonly long TokensAllocated;
	}
}
