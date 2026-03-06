using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ErrorObject
	{
		[JsonConstructor]
		public ErrorObject(ErrorObject.EmbeddedError error)
		{
			this.Error = error;
		}

		internal readonly ErrorObject.EmbeddedError Error;

		[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		internal readonly struct EmbeddedError
		{
			[JsonConstructor]
			public EmbeddedError(long code, long errorRef, string message, JObject errors)
			{
				this.Code = code;
				this.ErrorRef = errorRef;
				this.Message = message;
				this.Errors = errors;
			}

			internal readonly long Code;

			internal readonly long ErrorRef;

			internal readonly string Message;

			internal readonly JObject Errors;
		}
	}
}
