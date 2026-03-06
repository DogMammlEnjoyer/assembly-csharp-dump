using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct WebMessageObject
	{
		[JsonConstructor]
		public WebMessageObject(long code, bool success, string message)
		{
			this.Code = code;
			this.Success = success;
			this.Message = message;
		}

		internal readonly long Code;

		internal readonly bool Success;

		internal readonly string Message;
	}
}
