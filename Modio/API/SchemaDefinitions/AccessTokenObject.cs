using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct AccessTokenObject
	{
		[JsonConstructor]
		public AccessTokenObject(long code, string access_token, long date_expires)
		{
			this.Code = code;
			this.AccessToken = access_token;
			this.DateExpires = date_expires;
		}

		public readonly long Code;

		public readonly string AccessToken;

		public readonly long DateExpires;
	}
}
