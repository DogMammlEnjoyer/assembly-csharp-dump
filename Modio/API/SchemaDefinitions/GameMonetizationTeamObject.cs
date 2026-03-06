using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GameMonetizationTeamObject
	{
		[JsonConstructor]
		public GameMonetizationTeamObject(long team_id)
		{
			this.TeamId = team_id;
		}

		internal readonly long TeamId;
	}
}
