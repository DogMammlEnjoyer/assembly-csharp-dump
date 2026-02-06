using System;
using Fusion.Protocol;

namespace Fusion
{
	public sealed class HostMigrationToken
	{
		public GameMode GameMode { get; private set; }

		internal CloudCommunicator CloudCommunicator { get; private set; }

		internal byte[] ResumeState
		{
			get
			{
				Snapshot hostSnapshot = this.HostSnapshot;
				return (hostSnapshot != null) ? hostSnapshot.Data : null;
			}
		}

		internal Tick? ResumeTick
		{
			get
			{
				Snapshot hostSnapshot = this.HostSnapshot;
				int? num = (hostSnapshot != null) ? new int?(hostSnapshot.Tick) : null;
				return (num != null) ? new Tick?(num.GetValueOrDefault()) : null;
			}
		}

		internal NetworkId? ResumeId
		{
			get
			{
				return (this.HostSnapshot != null) ? new NetworkId?(new NetworkId(this.HostSnapshot.NetworkID)) : null;
			}
		}

		private Snapshot HostSnapshot { get; }

		internal HostMigrationToken(Snapshot hostSnapshot, CloudCommunicator cloudCommunicator, GameMode gameMode)
		{
			this.HostSnapshot = hostSnapshot;
			this.CloudCommunicator = cloudCommunicator;
			this.GameMode = gameMode;
		}
	}
}
