using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ModerationRulesHistoryRequestObject
	{
		[JsonConstructor]
		public ModerationRulesHistoryRequestObject(string timeframe)
		{
			this.Timeframe = timeframe;
		}

		internal readonly string Timeframe;
	}
}
