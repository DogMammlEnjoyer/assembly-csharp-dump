using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class Room : RoomInfo
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
				bool flag = value != this.isOpen;
				if (flag)
				{
					bool flag2 = !this.isOffline;
					if (flag2)
					{
						this.LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable
						{
							{
								253,
								value
							}
						}, null, null);
					}
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
				bool flag = value != this.isVisible;
				if (flag)
				{
					bool flag2 = !this.isOffline;
					if (flag2)
					{
						this.LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable
						{
							{
								254,
								value
							}
						}, null, null);
					}
				}
				this.isVisible = value;
			}
		}

		public new int MaxPlayers
		{
			get
			{
				return this.maxPlayers;
			}
			set
			{
				bool flag = value >= 0 && value != this.maxPlayers;
				if (flag)
				{
					this.maxPlayers = value;
					byte b = (value <= 255) ? ((byte)value) : 0;
					bool flag2 = !this.isOffline;
					if (flag2)
					{
						this.LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable
						{
							{
								byte.MaxValue,
								b
							},
							{
								243,
								this.maxPlayers
							}
						}, null, null);
					}
				}
			}
		}

		public new int PlayerCount
		{
			get
			{
				bool flag = this.Players == null;
				int result;
				if (flag)
				{
					result = 0;
				}
				else
				{
					result = (int)((byte)this.Players.Count);
				}
				return result;
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
				bool flag = value != this.playerTtl;
				if (flag)
				{
					bool flag2 = !this.isOffline;
					if (flag2)
					{
						this.LoadBalancingClient.OpSetPropertyOfRoom(246, value);
					}
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
				bool flag = value != this.emptyRoomTtl;
				if (flag)
				{
					bool flag2 = !this.isOffline;
					if (flag2)
					{
						this.LoadBalancingClient.OpSetPropertyOfRoom(245, value);
					}
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
			bool flag = options != null;
			if (flag)
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
			bool flag = masterClientId != 0 && this.masterClientId != masterClientId;
			if (flag)
			{
				this.LoadBalancingClient.InRoomCallbackTargets.OnMasterClientSwitched(this.GetPlayer(this.masterClientId, false));
			}
		}

		public virtual bool SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
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
				bool flag2 = this.isOffline;
				if (flag2)
				{
					bool flag3 = hashtable.Count == 0;
					if (flag3)
					{
						result = false;
					}
					else
					{
						base.CustomProperties.Merge(hashtable);
						base.CustomProperties.StripKeysWithNullValues();
						this.LoadBalancingClient.InRoomCallbackTargets.OnRoomPropertiesUpdate(propertiesToSet);
						result = true;
					}
				}
				else
				{
					result = this.LoadBalancingClient.OpSetPropertiesOfRoom(hashtable, expectedProperties, webFlags);
				}
			}
			return result;
		}

		public bool SetPropertiesListedInLobby(string[] lobbyProps)
		{
			bool flag = this.isOffline;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Hashtable hashtable = new Hashtable();
				hashtable[250] = lobbyProps;
				result = this.LoadBalancingClient.OpSetPropertiesOfRoom(hashtable, null, null);
			}
			return result;
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
			bool flag = this.isOffline;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
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
				result = this.LoadBalancingClient.OpSetPropertiesOfRoom(gameProperties, expectedProperties, null);
			}
			return result;
		}

		public virtual bool AddPlayer(Player player)
		{
			bool flag = !this.Players.ContainsKey(player.ActorNumber);
			bool result;
			if (flag)
			{
				this.StorePlayer(player);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
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
			bool flag = this.ExpectedUsers == null || this.ExpectedUsers.Length == 0;
			return !flag && this.SetExpectedUsers(new string[0], this.ExpectedUsers);
		}

		public bool SetExpectedUsers(string[] newExpectedUsers)
		{
			bool flag = newExpectedUsers == null || newExpectedUsers.Length == 0;
			bool result;
			if (flag)
			{
				this.LoadBalancingClient.DebugReturn(DebugLevel.ERROR, "newExpectedUsers array is null or empty, call Room.ClearExpectedUsers() instead if this is what you want.");
				result = false;
			}
			else
			{
				result = this.SetExpectedUsers(newExpectedUsers, this.ExpectedUsers);
			}
			return result;
		}

		private bool SetExpectedUsers(string[] newExpectedUsers, string[] oldExpectedUsers)
		{
			bool flag = this.isOffline;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Hashtable hashtable = new Hashtable(1);
				hashtable.Add(247, newExpectedUsers);
				Hashtable hashtable2 = null;
				bool flag2 = oldExpectedUsers != null;
				if (flag2)
				{
					hashtable2 = new Hashtable(1);
					hashtable2.Add(247, oldExpectedUsers);
				}
				result = this.LoadBalancingClient.OpSetPropertiesOfRoom(hashtable, hashtable2, null);
			}
			return result;
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
