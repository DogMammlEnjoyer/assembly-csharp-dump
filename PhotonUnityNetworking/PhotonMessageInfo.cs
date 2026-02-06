using System;
using Photon.Realtime;

namespace Photon.Pun
{
	public struct PhotonMessageInfo
	{
		public PhotonMessageInfo(Player player, int timestamp, PhotonView view)
		{
			this.Sender = player;
			this.timeInt = timestamp;
			this.photonView = view;
		}

		[Obsolete("Use SentServerTime instead.")]
		public double timestamp
		{
			get
			{
				return this.timeInt / 1000.0;
			}
		}

		public double SentServerTime
		{
			get
			{
				return this.timeInt / 1000.0;
			}
		}

		public int SentServerTimestamp
		{
			get
			{
				return this.timeInt;
			}
		}

		public override string ToString()
		{
			return string.Format("[PhotonMessageInfo: Sender='{1}' Senttime={0}]", this.SentServerTime, this.Sender);
		}

		private readonly int timeInt;

		public readonly Player Sender;

		public readonly PhotonView photonView;
	}
}
