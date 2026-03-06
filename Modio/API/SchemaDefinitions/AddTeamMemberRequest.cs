using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddTeamMemberRequest : IApiRequest
	{
		[JsonConstructor]
		public AddTeamMemberRequest(string email, long to_user_id, string position, long level)
		{
			this.Email = email;
			this.ToUserId = to_user_id;
			this.Position = position;
			this.Level = level;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddTeamMemberRequest._bodyParameters.Clear();
			AddTeamMemberRequest._bodyParameters.Add("email", this.Email);
			AddTeamMemberRequest._bodyParameters.Add("to_user_id", this.ToUserId);
			AddTeamMemberRequest._bodyParameters.Add("position", this.Position);
			AddTeamMemberRequest._bodyParameters.Add("level", this.Level);
			return AddTeamMemberRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Email;

		internal readonly long ToUserId;

		internal readonly string Position;

		internal readonly long Level;
	}
}
