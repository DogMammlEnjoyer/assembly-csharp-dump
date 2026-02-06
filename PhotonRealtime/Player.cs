using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Realtime
{
	public class Player
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
				if (!string.IsNullOrEmpty(this.nickName) && this.nickName.Equals(value))
				{
					return;
				}
				this.nickName = value;
				if (this.IsLocal)
				{
					this.SetPlayerNameProperty();
				}
			}
		}

		public string DefaultName
		{
			get
			{
				if (Application.isPlaying && !this.isDefaultGorillaNameSet)
				{
					this.isDefaultGorillaNameSet = true;
					this.defaultName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
				}
				return this.defaultName;
			}
		}

		public string UserId { get; internal set; }

		public bool IsMasterClient
		{
			get
			{
				return this.RoomReference != null && this.ActorNumber == this.RoomReference.MasterClientId;
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
			if (this.RoomReference == null)
			{
				return null;
			}
			return this.RoomReference.GetPlayer(id, false);
		}

		public Player GetNext()
		{
			return this.GetNextFor(this.ActorNumber);
		}

		public Player GetNextFor(Player currentPlayer)
		{
			if (currentPlayer == null)
			{
				return null;
			}
			return this.GetNextFor(currentPlayer.ActorNumber);
		}

		public Player GetNextFor(int currentPlayerId)
		{
			if (this.RoomReference == null || this.RoomReference.Players == null || this.RoomReference.Players.Count < 2)
			{
				return null;
			}
			Dictionary<int, Player> players = this.RoomReference.Players;
			int num = int.MaxValue;
			int num2 = currentPlayerId;
			foreach (int num3 in players.Keys)
			{
				if (num3 < num2)
				{
					num2 = num3;
				}
				else if (num3 > currentPlayerId && num3 < num)
				{
					num = num3;
				}
			}
			if (num == 2147483647)
			{
				return players[num2];
			}
			return players[num];
		}

		protected internal virtual void InternalCacheProperties(Hashtable properties)
		{
			if (properties == null || properties.Count == 0 || this.CustomProperties.Equals(properties))
			{
				return;
			}
			if (properties.ContainsKey(255))
			{
				string text = properties[byte.MaxValue] as string;
				if (text != null)
				{
					if (this.IsLocal)
					{
						if (!text.Equals(this.nickName))
						{
							this.SetPlayerNameProperty();
						}
					}
					else
					{
						this.NickName = text;
					}
				}
			}
			if (properties.ContainsKey(253))
			{
				this.UserId = (string)properties[253];
			}
			if (properties.ContainsKey(254))
			{
				this.IsInactive = (bool)properties[254];
			}
			this.CustomProperties.MergeStringKeys(properties);
			this.CustomProperties.StripKeysWithNullValues();
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
			if (!this.IsLocal)
			{
				return;
			}
			this.actorNumber = newID;
		}

		public bool SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, WebFlags webFlags = null)
		{
			if (propertiesToSet == null || propertiesToSet.Count == 0)
			{
				return false;
			}
			Hashtable hashtable = propertiesToSet.StripToStringKeys();
			if (this.RoomReference == null)
			{
				if (this.IsLocal)
				{
					if (hashtable.Count == 0)
					{
						return false;
					}
					if (expectedValues == null && webFlags == null)
					{
						this.CustomProperties.Merge(hashtable);
						this.CustomProperties.StripKeysWithNullValues();
						return true;
					}
				}
				return false;
			}
			if (!this.RoomReference.IsOffline)
			{
				Hashtable expectedProperties = expectedValues.StripToStringKeys();
				return this.RoomReference.LoadBalancingClient.OpSetPropertiesOfActor(this.actorNumber, hashtable, expectedProperties, webFlags);
			}
			if (hashtable.Count == 0)
			{
				return false;
			}
			this.CustomProperties.Merge(hashtable);
			this.CustomProperties.StripKeysWithNullValues();
			this.RoomReference.LoadBalancingClient.InRoomCallbackTargets.OnPlayerPropertiesUpdate(this, hashtable);
			return true;
		}

		private bool SetPlayerNameProperty()
		{
			if (this.RoomReference != null && !this.RoomReference.IsOffline)
			{
				Hashtable hashtable = new Hashtable();
				hashtable[byte.MaxValue] = this.nickName;
				return this.RoomReference.LoadBalancingClient.OpSetPropertiesOfActor(this.ActorNumber, hashtable, null, null);
			}
			return false;
		}

		private int actorNumber = -1;

		public readonly bool IsLocal;

		private string nickName = string.Empty;

		private bool isDefaultGorillaNameSet;

		private string defaultName = "gorilla????";

		public object TagObject;
	}
}
