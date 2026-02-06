using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Photon.Realtime
{
	public class Room : RoomInfo
	{
		public LoadBalancingClient LoadBalancingClient { get; set; }

		public new string Name
		{
			get
			{
				return this.name;
			}
			internal set
			{
				this.name = value;
			}
		}

		public bool IsOffline
		{
			get
			{
				return this.isOffline;
			}
			private set
			{
				this.isOffline = value;
			}
		}

		public new bool IsOpen
		{
			get
			{
				return this.isOpen;
			}
			set
			{
				if (value != this.isOpen && !this.isOffline)
				{
					this.LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable
					{
						{
							253,
							value
						}
					}, null, null);
				}
				this.isOpen = value;
			}
		}

		public new bool IsVisible
		{
			get
			{
				return this.isVisible;
			}
			set
			{
				if (value != this.isVisible && !this.isOffline)
				{
					this.LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable
					{
						{
							254,
							value
						}
					}, null, null);
				}
				this.isVisible = value;
			}
		}

		public new byte MaxPlayers
		{
			get
			{
				return this.maxPlayers;
			}
			set
			{
				if (value != this.maxPlayers && !this.isOffline)
				{
					this.LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable
					{
						{
							byte.MaxValue,
							value
						}
					}, null, null);
				}
				this.maxPlayers = value;
			}
		}

		public new byte PlayerCount
		{
			get
			{
				if (this.Players == null)
				{
					return 0;
				}
				return (byte)this.Players.Count;
			}
		}

		public Dictionary<int, Player> Players
		{
			get
			{
				return this.players;
			}
			private set
			{
				this.players = value;
			}
		}

		public string[] ExpectedUsers
		{
			get
			{
				return this.expectedUsers;
			}
		}

		public int PlayerTtl
		{
			get
			{
				return this.playerTtl;
			}
			set
			{
				if (value != this.playerTtl && !this.isOffline)
				{
					this.LoadBalancingClient.OpSetPropertyOfRoom(246, value);
				}
				this.playerTtl = value;
			}
		}

		public int EmptyRoomTtl
		{
			get
			{
				return this.emptyRoomTtl;
			}
			set
			{
				if (value != this.emptyRoomTtl && !this.isOffline)
				{
					this.LoadBalancingClient.OpSetPropertyOfRoom(245, value);
				}
				this.emptyRoomTtl = value;
			}
		}

		public int MasterClientId
		{
			get
			{
				return this.masterClientId;
			}
		}

		public string[] PropertiesListedInLobby
		{
			get
			{
				return this.propertiesListedInLobby;
			}
			private set
			{
				this.propertiesListedInLobby = value;
			}
		}

		public bool AutoCleanUp
		{
			get
			{
				return this.autoCleanUp;
			}
		}

		public bool BroadcastPropertiesChangeToAll { get; private set; }

		public bool SuppressRoomEvents { get; private set; }

		public bool SuppressPlayerInfo { get; private set; }

		public bool PublishUserId { get; private set; }

		public bool DeleteNullProperties { get; private set; }

		public Room(string roomName, RoomOptions options, bool isOffline = false) : base(roomName, (options != null) ? options.CustomRoomProperties : null)
		{
			if (options != null)
			{
				this.isVisible = options.IsVisible;
				this.isOpen = options.IsOpen;
				this.maxPlayers = options.MaxPlayers;
				this.propertiesListedInLobby = options.CustomRoomPropertiesForLobby;
			}
			this.isOffline = isOffline;
		}

		internal void InternalCacheRoomFlags(int roomFlags)
		{
			this.BroadcastPropertiesChangeToAll = ((roomFlags & 32) != 0);
			this.SuppressRoomEvents = ((roomFlags & 4) != 0);
			this.SuppressPlayerInfo = ((roomFlags & 64) != 0);
			this.PublishUserId = ((roomFlags & 8) != 0);
			this.DeleteNullProperties = ((roomFlags & 16) != 0);
			this.autoCleanUp = ((roomFlags & 2) != 0);
		}

		protected internal override void InternalCacheProperties(Hashtable propertiesToCache)
		{
			int masterClientId = this.masterClientId;
			base.InternalCacheProperties(propertiesToCache);
			if (masterClientId != 0 && this.masterClientId != masterClientId)
			{
				this.LoadBalancingClient.InRoomCallbackTargets.OnMasterClientSwitched(this.GetPlayer(this.masterClientId, false));
			}
		}

		public virtual bool SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			if (propertiesToSet == null || propertiesToSet.Count == 0)
			{
				return false;
			}
			Hashtable hashtable = propertiesToSet.StripToStringKeys();
			if (!this.isOffline)
			{
				return this.LoadBalancingClient.OpSetPropertiesOfRoom(hashtable, expectedProperties, webFlags);
			}
			if (hashtable.Count == 0)
			{
				return false;
			}
			base.CustomProperties.Merge(hashtable);
			base.CustomProperties.StripKeysWithNullValues();
			this.LoadBalancingClient.InRoomCallbackTargets.OnRoomPropertiesUpdate(propertiesToSet);
			return true;
		}

		public bool SetPropertiesListedInLobby(string[] lobbyProps)
		{
			if (this.isOffline)
			{
				return false;
			}
			Hashtable hashtable = new Hashtable();
			hashtable[250] = lobbyProps;
			return this.LoadBalancingClient.OpSetPropertiesOfRoom(hashtable, null, null);
		}

		protected internal virtual void RemovePlayer(Player player)
		{
			this.Players.Remove(player.ActorNumber);
			player.RoomReference = null;
		}

		protected internal virtual void RemovePlayer(int id)
		{
			this.RemovePlayer(this.GetPlayer(id, false));
		}

		public bool SetMasterClient(Player masterClientPlayer)
		{
			if (this.isOffline)
			{
				return false;
			}
			Hashtable gameProperties = new Hashtable
			{
				{
					248,
					masterClientPlayer.ActorNumber
				}
			};
			Hashtable expectedProperties = new Hashtable
			{
				{
					248,
					this.MasterClientId
				}
			};
			return this.LoadBalancingClient.OpSetPropertiesOfRoom(gameProperties, expectedProperties, null);
		}

		public virtual bool AddPlayer(Player player)
		{
			if (!this.Players.ContainsKey(player.ActorNumber))
			{
				this.StorePlayer(player);
				return true;
			}
			return false;
		}

		public virtual Player StorePlayer(Player player)
		{
			this.Players[player.ActorNumber] = player;
			player.RoomReference = this;
			return player;
		}

		public virtual Player GetPlayer(int id, bool findMaster = false)
		{
			int key = (findMaster && id == 0) ? this.MasterClientId : id;
			Player result = null;
			this.Players.TryGetValue(key, out result);
			return result;
		}

		public bool ClearExpectedUsers()
		{
			return this.ExpectedUsers != null && this.ExpectedUsers.Length != 0 && this.SetExpectedUsers(new string[0], this.ExpectedUsers);
		}

		public bool SetExpectedUsers(string[] newExpectedUsers)
		{
			if (newExpectedUsers == null || newExpectedUsers.Length == 0)
			{
				this.LoadBalancingClient.DebugReturn(DebugLevel.ERROR, "newExpectedUsers array is null or empty, call Room.ClearExpectedUsers() instead if this is what you want.");
				return false;
			}
			return this.SetExpectedUsers(newExpectedUsers, this.ExpectedUsers);
		}

		private bool SetExpectedUsers(string[] newExpectedUsers, string[] oldExpectedUsers)
		{
			if (this.isOffline)
			{
				return false;
			}
			Hashtable hashtable = new Hashtable(1);
			hashtable.Add(247, newExpectedUsers);
			Hashtable hashtable2 = null;
			if (oldExpectedUsers != null)
			{
				hashtable2 = new Hashtable(1);
				hashtable2.Add(247, oldExpectedUsers);
			}
			return this.LoadBalancingClient.OpSetPropertiesOfRoom(hashtable, hashtable2, null);
		}

		public override string ToString()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", new object[]
			{
				this.name,
				this.isVisible ? "visible" : "hidden",
				this.isOpen ? "open" : "closed",
				this.maxPlayers,
				this.PlayerCount
			});
		}

		public new string ToStringFull()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
			{
				this.name,
				this.isVisible ? "visible" : "hidden",
				this.isOpen ? "open" : "closed",
				this.maxPlayers,
				this.PlayerCount,
				base.CustomProperties.ToStringFull()
			});
		}

		private bool isOffline;

		private Dictionary<int, Player> players = new Dictionary<int, Player>();
	}
}
