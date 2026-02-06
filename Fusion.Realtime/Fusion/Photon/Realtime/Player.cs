using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class Player
	{
		protected internal Room RoomReference { get; set; }

		public int ActorNumber
		{
			get
			{
				return this.actorNumber;
			}
		}

		public bool HasRejoined { get; internal set; }

		public string NickName
		{
			get
			{
				return this.nickName;
			}
			set
			{
				bool flag = !string.IsNullOrEmpty(this.nickName) && this.nickName.Equals(value);
				if (!flag)
				{
					this.nickName = value;
					bool isLocal = this.IsLocal;
					if (isLocal)
					{
						this.SetPlayerNameProperty();
					}
				}
			}
		}

		public string UserId { get; internal set; }

		public bool IsMasterClient
		{
			get
			{
				bool flag = this.RoomReference == null;
				return !flag && this.ActorNumber == this.RoomReference.MasterClientId;
			}
		}

		public bool IsInactive { get; protected internal set; }

		public Hashtable CustomProperties { get; set; }

		protected internal Player(string nickName, int actorNumber, bool isLocal) : this(nickName, actorNumber, isLocal, null)
		{
		}

		protected internal Player(string nickName, int actorNumber, bool isLocal, Hashtable playerProperties)
		{
			this.IsLocal = isLocal;
			this.actorNumber = actorNumber;
			this.NickName = nickName;
			this.CustomProperties = new Hashtable();
			this.InternalCacheProperties(playerProperties);
		}

		public Player Get(int id)
		{
			bool flag = this.RoomReference == null;
			Player result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this.RoomReference.GetPlayer(id, false);
			}
			return result;
		}

		public Player GetNext()
		{
			return this.GetNextFor(this.ActorNumber);
		}

		public Player GetNextFor(Player currentPlayer)
		{
			bool flag = currentPlayer == null;
			Player result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this.GetNextFor(currentPlayer.ActorNumber);
			}
			return result;
		}

		public Player GetNextFor(int currentPlayerId)
		{
			bool flag = this.RoomReference == null || this.RoomReference.Players == null || this.RoomReference.Players.Count < 2;
			Player result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Dictionary<int, Player> players = this.RoomReference.Players;
				int num = int.MaxValue;
				int num2 = currentPlayerId;
				foreach (int num3 in players.Keys)
				{
					bool flag2 = num3 < num2;
					if (flag2)
					{
						num2 = num3;
					}
					else
					{
						bool flag3 = num3 > currentPlayerId && num3 < num;
						if (flag3)
						{
							num = num3;
						}
					}
				}
				result = ((num != int.MaxValue) ? players[num] : players[num2]);
			}
			return result;
		}

		protected internal virtual void InternalCacheProperties(Hashtable properties)
		{
			bool flag = properties == null || properties.Count == 0 || this.CustomProperties.Equals(properties);
			if (!flag)
			{
				bool flag2 = !this.IsLocal && properties.ContainsKey(byte.MaxValue);
				if (flag2)
				{
					string text = (string)properties[byte.MaxValue];
					this.NickName = text;
				}
				bool flag3 = properties.ContainsKey(253);
				if (flag3)
				{
					this.UserId = (string)properties[253];
				}
				bool flag4 = properties.ContainsKey(254);
				if (flag4)
				{
					this.IsInactive = (bool)properties[254];
				}
				this.CustomProperties.MergeStringKeys(properties);
				this.CustomProperties.StripKeysWithNullValues();
			}
		}

		public override string ToString()
		{
			return string.Format("#{0:00} '{1}'", this.ActorNumber, this.NickName);
		}

		public string ToStringFull()
		{
			return string.Format("#{0:00} '{1}'{2} {3}", new object[]
			{
				this.ActorNumber,
				this.NickName,
				this.IsInactive ? " (inactive)" : "",
				this.CustomProperties.ToStringFull()
			});
		}

		public override bool Equals(object p)
		{
			Player player = p as Player;
			return player != null && this.GetHashCode() == player.GetHashCode();
		}

		public override int GetHashCode()
		{
			return this.ActorNumber;
		}

		protected internal void ChangeLocalID(int newID)
		{
			bool flag = !this.IsLocal;
			if (!flag)
			{
				this.actorNumber = newID;
			}
		}

		public bool SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, WebFlags webFlags = null)
		{
			bool flag = propertiesToSet == null || propertiesToSet.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Hashtable hashtable = propertiesToSet.StripToStringKeys();
				bool flag2 = this.RoomReference != null;
				if (flag2)
				{
					bool isOffline = this.RoomReference.IsOffline;
					if (isOffline)
					{
						bool flag3 = hashtable.Count == 0;
						if (flag3)
						{
							result = false;
						}
						else
						{
							this.CustomProperties.Merge(hashtable);
							this.CustomProperties.StripKeysWithNullValues();
							this.RoomReference.LoadBalancingClient.InRoomCallbackTargets.OnPlayerPropertiesUpdate(this, hashtable);
							result = true;
						}
					}
					else
					{
						Hashtable expectedProperties = expectedValues.StripToStringKeys();
						result = this.RoomReference.LoadBalancingClient.OpSetPropertiesOfActor(this.actorNumber, hashtable, expectedProperties, webFlags);
					}
				}
				else
				{
					bool isLocal = this.IsLocal;
					if (isLocal)
					{
						bool flag4 = hashtable.Count == 0;
						if (flag4)
						{
							return false;
						}
						bool flag5 = expectedValues == null && webFlags == null;
						if (flag5)
						{
							this.CustomProperties.Merge(hashtable);
							this.CustomProperties.StripKeysWithNullValues();
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		internal bool UpdateNickNameOnJoined()
		{
			bool flag = this.RoomReference == null || this.RoomReference.CustomProperties == null || !this.IsLocal;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string b = this.RoomReference.CustomProperties.ContainsKey(byte.MaxValue) ? (this.RoomReference.CustomProperties[byte.MaxValue] as string) : string.Empty;
				bool flag2 = !string.Equals(this.NickName, b);
				result = (!flag2 || this.SetPlayerNameProperty());
			}
			return result;
		}

		private bool SetPlayerNameProperty()
		{
			bool flag = this.RoomReference != null && !this.RoomReference.IsOffline;
			bool result;
			if (flag)
			{
				Hashtable hashtable = new Hashtable();
				hashtable[byte.MaxValue] = this.nickName;
				result = this.RoomReference.LoadBalancingClient.OpSetPropertiesOfActor(this.ActorNumber, hashtable, null, null);
			}
			else
			{
				result = false;
			}
			return result;
		}

		private int actorNumber = -1;

		public readonly bool IsLocal;

		private string nickName = string.Empty;

		public object TagObject;
	}
}
