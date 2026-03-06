using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct UserDelegationTokenObject
	{
		[JsonConstructor]
		public UserDelegationTokenObject(string entity, string token)
		{
			this.Entity = entity;
			this.Token = token;
		}

		internal readonly string Entity;

		internal readonly string Token;
	}
}
