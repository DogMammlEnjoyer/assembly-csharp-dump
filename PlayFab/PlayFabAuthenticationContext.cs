using System;

namespace PlayFab
{
	public sealed class PlayFabAuthenticationContext
	{
		public PlayFabAuthenticationContext()
		{
		}

		public PlayFabAuthenticationContext(string clientSessionTicket, string entityToken, string playFabId, string entityId, string entityType) : this()
		{
			this.ClientSessionTicket = clientSessionTicket;
			this.PlayFabId = playFabId;
			this.EntityToken = entityToken;
			this.EntityId = entityId;
			this.EntityType = entityType;
		}

		public void CopyFrom(PlayFabAuthenticationContext other)
		{
			this.ClientSessionTicket = other.ClientSessionTicket;
			this.PlayFabId = other.PlayFabId;
			this.EntityToken = other.EntityToken;
			this.EntityId = other.EntityId;
			this.EntityType = other.EntityType;
		}

		public bool IsClientLoggedIn()
		{
			return !string.IsNullOrEmpty(this.ClientSessionTicket);
		}

		public bool IsEntityLoggedIn()
		{
			return !string.IsNullOrEmpty(this.EntityToken);
		}

		public void ForgetAllCredentials()
		{
			this.PlayFabId = null;
			this.ClientSessionTicket = null;
			this.EntityToken = null;
			this.EntityId = null;
			this.EntityType = null;
		}

		public string ClientSessionTicket;

		public string PlayFabId;

		public string EntityToken;

		public string EntityId;

		public string EntityType;
	}
}
