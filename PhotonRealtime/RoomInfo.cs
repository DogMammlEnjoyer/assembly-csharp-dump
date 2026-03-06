using System;
using ExitGames.Client.Photon;

namespace Photon.Realtime
{
	public class RoomInfo
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

		public byte MaxPlayers
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
			if (propertiesToCache == null || propertiesToCache.Count == 0 || this.customProperties.Equals(propertiesToCache))
			{
				return;
			}
			if (propertiesToCache.ContainsKey(251))
			{
				object obj = propertiesToCache[251];
				if (obj is bool)
				{
					bool removedFromList = (bool)obj;
					this.RemovedFromList = removedFromList;
					if (this.RemovedFromList)
					{
						return;
					}
				}
			}
			if (propertiesToCache.ContainsKey(255))
			{
				object obj = propertiesToCache[byte.MaxValue];
				if (obj is byte)
				{
					byte b = (byte)obj;
					this.maxPlayers = b;
				}
			}
			if (propertiesToCache.ContainsKey(253))
			{
				object obj = propertiesToCache[253];
				if (obj is bool)
				{
					bool flag = (bool)obj;
					this.isOpen = flag;
				}
			}
			if (propertiesToCache.ContainsKey(254))
			{
				object obj = propertiesToCache[254];
				if (obj is bool)
				{
					bool flag2 = (bool)obj;
					this.isVisible = flag2;
				}
			}
			if (propertiesToCache.ContainsKey(252) && (propertiesToCache[252] is int || propertiesToCache[252] is byte))
			{
				this.PlayerCount = (int)((byte)propertiesToCache[252]);
			}
			if (propertiesToCache.ContainsKey(249))
			{
				object obj = propertiesToCache[249];
				if (obj is bool)
				{
					bool flag3 = (bool)obj;
					this.autoCleanUp = flag3;
				}
			}
			if (propertiesToCache.ContainsKey(248))
			{
				object obj = propertiesToCache[248];
				if (obj is int)
				{
					int num = (int)obj;
					this.masterClientId = num;
				}
			}
			if (propertiesToCache.ContainsKey(250))
			{
				this.propertiesListedInLobby = (propertiesToCache[250] as string[]);
			}
			if (propertiesToCache.ContainsKey(247))
			{
				this.expectedUsers = (propertiesToCache[247] as string[]);
			}
			if (propertiesToCache.ContainsKey(245))
			{
				object obj = propertiesToCache[245];
				if (obj is int)
				{
					int num2 = (int)obj;
					this.emptyRoomTtl = num2;
				}
			}
			if (propertiesToCache.ContainsKey(246))
			{
				object obj = propertiesToCache[246];
				if (obj is int)
				{
					int num3 = (int)obj;
					this.playerTtl = num3;
				}
			}
			this.customProperties.MergeStringKeys(propertiesToCache);
			this.customProperties.StripKeysWithNullValues();
		}

		public bool RemovedFromList;

		private Hashtable customProperties = new Hashtable();

		protected byte maxPlayers;

		protected int emptyRoomTtl;

		protected int playerTtl;

		protected string[] expectedUsers;

		protected bool isOpen = true;

		protected bool isVisible = true;

		protected bool autoCleanUp = true;

		protected string name;

		public int masterClientId;

		protected string[] propertiesListedInLobby;
	}
}
