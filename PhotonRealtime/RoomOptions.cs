using System;
using ExitGames.Client.Photon;

namespace Photon.Realtime
{
	public class RoomOptions
	{
		public bool IsVisible
		{
			get
			{
				return this.isVisible;
			}
			set
			{
				this.isVisible = value;
			}
		}

		public bool IsOpen
		{
			get
			{
				return this.isOpen;
			}
			set
			{
				this.isOpen = value;
			}
		}

		public bool CleanupCacheOnLeave
		{
			get
			{
				return this.cleanupCacheOnLeave;
			}
			set
			{
				this.cleanupCacheOnLeave = value;
			}
		}

		public bool SuppressRoomEvents { get; set; }

		public bool SuppressPlayerInfo { get; set; }

		public bool PublishUserId { get; set; }

		public bool DeleteNullProperties { get; set; }

		public bool BroadcastPropsChangeToAll
		{
			get
			{
				return this.broadcastPropsChangeToAll;
			}
			set
			{
				this.broadcastPropsChangeToAll = value;
			}
		}

		private bool isVisible = true;

		private bool isOpen = true;

		public byte MaxPlayers;

		public int PlayerTtl;

		public int EmptyRoomTtl;

		private bool cleanupCacheOnLeave = true;

		public Hashtable CustomRoomProperties;

		public string[] CustomRoomPropertiesForLobby = new string[0];

		public string[] Plugins;

		private bool broadcastPropsChangeToAll = true;
	}
}
