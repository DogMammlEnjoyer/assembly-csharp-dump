using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddReportResponse
	{
		[JsonConstructor]
		public AddReportResponse(long code, string message)
		{
			this.Code = code;
			this.Message = message;
		}

		internal readonly long Code;

		internal readonly string Message;
	}
}
