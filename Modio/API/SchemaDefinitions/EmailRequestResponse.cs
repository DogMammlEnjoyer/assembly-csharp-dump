using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct EmailRequestResponse
	{
		[JsonConstructor]
		public EmailRequestResponse(long code, string message)
		{
			this.Code = code;
			this.Message = message;
		}

		internal readonly long Code;

		internal readonly string Message;
	}
}
