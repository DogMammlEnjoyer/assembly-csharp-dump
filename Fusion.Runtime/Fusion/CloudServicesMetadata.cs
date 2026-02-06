using System;
using System.Collections.Generic;
using Fusion.Encryption;
using Fusion.Photon.Realtime;
using Fusion.Protocol;
using Fusion.Sockets.Stun;

namespace Fusion
{
	internal class CloudServicesMetadata
	{
		public NetworkRunnerInitializeArgs RunnerInitializeArgs { get; set; } = default(NetworkRunnerInitializeArgs);

		public NATPunchStage CurrentPunchStage { get; set; } = NATPunchStage.None;

		public JoinProcessStage CurrentJoinStage { get; set; } = JoinProcessStage.Idle;

		public ProtocolMessageVersion CurrentProtocolMessageVersion { get; set; } = ProtocolMessageVersion.V1_6_0;

		public ReflexiveInfo RemoteReflexiveInfo { get; set; } = null;

		public StunResult LocalReflexiveInfo
		{
			get
			{
				return this._localStunResult;
			}
			set
			{
				this._localStunResult = value;
				bool flag = this._localStunResult != null;
				if (flag)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Log(string.Format("Local ReflexiveInfo: {0}", this._localStunResult));
					}
				}
			}
		}

		public byte[] UniqueId { get; set; } = null;

		public int PlayerRef { get; set; } = -1;

		public EncryptionToken EncryptionToken { get; set; }

		public static readonly TypedLobby LobbyClientServer = new TypedLobby("ClientServer", LobbyType.Default);

		public static readonly TypedLobby LobbyShared = new TypedLobby("Shared", LobbyType.Default);

		public ScheduledRequests ScheduledRequests = ScheduledRequests.None;

		public Disconnect LastDisconnectMsg = null;

		public Dictionary<long, ReflexiveInfo> UniqueIdToReflexiveInfoTable = new Dictionary<long, ReflexiveInfo>();

		private StunResult _localStunResult = null;
	}
}
