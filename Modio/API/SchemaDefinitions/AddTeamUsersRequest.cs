using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddTeamUsersRequest : IApiRequest
	{
		[JsonConstructor]
		public AddTeamUsersRequest(long[] users)
		{
			this.Users = users;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddTeamUsersRequest._bodyParameters.Clear();
			AddTeamUsersRequest._bodyParameters.Add("users", this.Users);
			return AddTeamUsersRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long[] Users;
	}
}
