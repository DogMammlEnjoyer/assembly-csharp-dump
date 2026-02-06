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
				this.RemovedFromList = (bool)propertiesToCache[251];
				if (this.RemovedFromList)
				{
					return;
				}
			}
			if (propertiesToCache.ContainsKey(255))
			{
				this.maxPlayers = (byte)propertiesToCache[byte.MaxValue];
			}
			if (propertiesToCache.ContainsKey(253))
			{
				this.isOpen = (bool)propertiesToCache[253];
			}
			if (propertiesToCache.ContainsKey(254))
			{
				this.isVisible = (bool)propertiesToCache[254];
			}
			if (propertiesToCache.ContainsKey(252))
			{
				this.PlayerCount = (int)((byte)propertiesToCache[252]);
			}
			if (propertiesToCache.ContainsKey(249))
			{
				this.autoCleanUp = (bool)propertiesToCache[249];
			}
			if (propertiesToCache.ContainsKey(248))
			{
				this.masterClientId = (int)propertiesToCache[248];
			}
			if (propertiesToCache.ContainsKey(250))
			{
				this.propertiesListedInLobby = (propertiesToCache[250] as string[]);
			}
			if (propertiesToCache.ContainsKey(247))
			{
				this.expectedUsers = (string[])propertiesToCache[247];
			}
			if (propertiesToCache.ContainsKey(245))
			{
				this.emptyRoomTtl = (int)propertiesToCache[245];
			}
			if (propertiesToCache.ContainsKey(246))
			{
				this.playerTtl = (int)propertiesToCache[246];
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
