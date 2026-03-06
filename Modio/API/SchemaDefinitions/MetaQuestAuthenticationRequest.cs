using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct MetaQuestAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public MetaQuestAuthenticationRequest(string device, string nonce, string userId, string accessToken, bool termsAgreed, [Nullable(2)] string email, long dateExpires)
		{
			this.Device = device;
			this.Nonce = nonce;
			this.UserId = userId;
			this.AccessToken = accessToken;
			this.TermsAgreed = termsAgreed;
			this.Email = email;
			this.DateExpires = dateExpires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			MetaQuestAuthenticationRequest._bodyParameters.Clear();
			MetaQuestAuthenticationRequest._bodyParameters.Add("device", this.Device);
			MetaQuestAuthenticationRequest._bodyParameters.Add("nonce", this.Nonce);
			MetaQuestAuthenticationRequest._bodyParameters.Add("user_id", this.UserId);
			MetaQuestAuthenticationRequest._bodyParameters.Add("access_token", this.AccessToken);
			MetaQuestAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			if (!string.IsNullOrEmpty(this.Email))
			{
				MetaQuestAuthenticationRequest._bodyParameters.Add("email", this.Email);
			}
			MetaQuestAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return MetaQuestAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Device;

		internal readonly string Nonce;

		internal readonly string UserId;

		internal readonly string AccessToken;

		internal readonly bool TermsAgreed;

		[Nullable(2)]
		internal readonly string Email;

		internal readonly long DateExpires;
	}
}
