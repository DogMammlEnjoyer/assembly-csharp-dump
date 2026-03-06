using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct UpdateTeamMemberRequest : IApiRequest
	{
		[JsonConstructor]
		public UpdateTeamMemberRequest(string email, string position, long level)
		{
			this.Email = email;
			this.Position = position;
			this.Level = level;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			UpdateTeamMemberRequest._bodyParameters.Clear();
			UpdateTeamMemberRequest._bodyParameters.Add("email", this.Email);
			UpdateTeamMemberRequest._bodyParameters.Add("position", this.Position);
			UpdateTeamMemberRequest._bodyParameters.Add("level", this.Level);
			return UpdateTeamMemberRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Email;

		internal readonly string Position;

		internal readonly long Level;
	}
}
