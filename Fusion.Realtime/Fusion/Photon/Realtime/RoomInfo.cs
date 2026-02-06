using System;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class RoomInfo
	{
		public Hashtable CustomProperties
		{
			get
			{
				return this.customProperties;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int PlayerCount { get; private set; }

		public int MaxPlayers
		{
			get
			{
				return this.maxPlayers;
			}
		}

		public bool IsOpen
		{
			get
			{
				return this.isOpen;
			}
		}

		public bool IsVisible
		{
			get
			{
				return this.isVisible;
			}
		}

		protected internal RoomInfo(string roomName, Hashtable roomProperties)
		{
			this.InternalCacheProperties(roomProperties);
			this.name = roomName;
		}

		public override bool Equals(object other)
		{
			RoomInfo roomInfo = other as RoomInfo;
			return roomInfo != null && this.Name.Equals(roomInfo.name);
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode();
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

		public string ToStringFull()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
			{
				this.name,
				this.isVisible ? "visible" : "hidden",
				this.isOpen ? "open" : "closed",
				this.maxPlayers,
				this.PlayerCount,
				this.customProperties.ToStringFull()
			});
		}

		protected internal virtual void InternalCacheProperties(Hashtable propertiesToCache)
		{
			bool flag = propertiesToCache == null || propertiesToCache.Count == 0 || this.customProperties.Equals(propertiesToCache);
			if (!flag)
			{
				bool flag2 = propertiesToCache.ContainsKey(251);
				if (flag2)
				{
					this.RemovedFromList = (bool)propertiesToCache[251];
					bool removedFromList = this.RemovedFromList;
					if (removedFromList)
					{
						return;
					}
				}
				bool flag3 = propertiesToCache.ContainsKey(243);
				if (flag3)
				{
					this.maxPlayers = Convert.ToInt32(propertiesToCache[243]);
				}
				else
				{
					bool flag4 = propertiesToCache.ContainsKey(byte.MaxValue);
					if (flag4)
					{
						this.maxPlayers = Convert.ToInt32(propertiesToCache[byte.MaxValue]);
					}
				}
				bool flag5 = propertiesToCache.ContainsKey(253);
				if (flag5)
				{
					this.isOpen = (bool)propertiesToCache[253];
				}
				bool flag6 = propertiesToCache.ContainsKey(254);
				if (flag6)
				{
					this.isVisible = (bool)propertiesToCache[254];
				}
				bool flag7 = propertiesToCache.ContainsKey(252);
				if (flag7)
				{
					this.PlayerCount = Convert.ToInt32(propertiesToCache[252]);
				}
				bool flag8 = propertiesToCache.ContainsKey(249);
				if (flag8)
				{
					this.autoCleanUp = (bool)propertiesToCache[249];
				}
				bool flag9 = propertiesToCache.ContainsKey(248);
				if (flag9)
				{
					this.masterClientId = (int)propertiesToCache[248];
				}
				bool flag10 = propertiesToCache.ContainsKey(250);
				if (flag10)
				{
					this.propertiesListedInLobby = (propertiesToCache[250] as string[]);
				}
				bool flag11 = propertiesToCache.ContainsKey(247);
				if (flag11)
				{
					this.expectedUsers = (string[])propertiesToCache[247];
				}
				bool flag12 = propertiesToCache.ContainsKey(245);
				if (flag12)
				{
					this.emptyRoomTtl = (int)propertiesToCache[245];
				}
				bool flag13 = propertiesToCache.ContainsKey(246);
				if (flag13)
				{
					this.playerTtl = (int)propertiesToCache[246];
				}
				this.customProperties.MergeStringKeys(propertiesToCache);
				this.customProperties.StripKeysWithNullValues();
			}
		}

		public bool RemovedFromList;

		private Hashtable customProperties = new Hashtable();

		protected int maxPlayers = 0;

		protected int emptyRoomTtl = 0;

		protected int playerTtl = 0;

		protected string[] expectedUsers;

		protected bool isOpen = true;

		protected bool isVisible = true;

		protected bool autoCleanUp = true;

		protected string name;

		public int masterClientId;

		protected string[] propertiesListedInLobby;
	}
}
