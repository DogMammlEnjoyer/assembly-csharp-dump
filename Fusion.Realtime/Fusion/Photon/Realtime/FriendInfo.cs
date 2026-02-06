using System;

namespace Fusion.Photon.Realtime
{
	internal class FriendInfo
	{
		[Obsolete("Use UserId.")]
		public string Name
		{
			get
			{
				return this.UserId;
			}
		}

		public string UserId { get; protected internal set; }

		public bool IsOnline { get; protected internal set; }

		public string Room { get; protected internal set; }

		public bool IsInRoom
		{
			get
			{
				return this.IsOnline && !string.IsNullOrEmpty(this.Room);
			}
		}

		public override string ToString()
		{
			return string.Format("{0}\t is: {1}", this.UserId, (!this.IsOnline) ? "offline" : (this.IsInRoom ? "playing" : "on master"));
		}
	}
}
