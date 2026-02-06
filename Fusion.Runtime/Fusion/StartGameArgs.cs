using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Fusion.Photon.Realtime;
using Fusion.Sockets;

namespace Fusion
{
	public struct StartGameArgs
	{
		public override string ToString()
		{
			string text = (this.CustomCallbackInterfaces != null) ? string.Join<Type>(", ", this.CustomCallbackInterfaces.AsEnumerable<Type>()) : null;
			string text2 = (this.ConnectionToken != null) ? string.Format("Length: {0}", this.ConnectionToken.Length) : null;
			string text3 = null;
			bool flag = this.SessionProperties != null;
			if (flag)
			{
				foreach (KeyValuePair<string, SessionProperty> keyValuePair in this.SessionProperties)
				{
					text3 += string.Format("{0}={1}, ", keyValuePair.Key, keyValuePair.Value);
				}
			}
			return string.Concat(new string[]
			{
				"[StartGameArgs: ",
				string.Format("{0}={1}, ", "GameMode", this.GameMode),
				"SessionName=",
				this.SessionName,
				", ",
				string.Format("{0}={1}, ", "IsVisible", this.IsVisible),
				string.Format("{0}={1}, ", "IsOpen", this.IsOpen),
				string.Format("{0}={1}, ", "Address", this.Address),
				string.Format("{0}={1}, ", "CustomPublicAddress", this.CustomPublicAddress),
				string.Format("{0}={1}, ", "ObjectProvider", this.ObjectProvider),
				string.Format("{0}={1}, ", "SceneManager", this.SceneManager),
				string.Format("{0}={1}, ", "PlayerCount", this.PlayerCount),
				string.Format("{0}={1}, ", "Scene", this.Scene),
				string.Format("{0}={1}, ", "DisableNATPunchthrough", this.DisableNATPunchthrough),
				"CustomCallbackInterfaces=",
				text,
				", ConnectionToken=",
				text2,
				", SessionProperties=",
				text3,
				", CustomLobbyName=",
				this.CustomLobbyName,
				", CustomSTUNServer=",
				this.CustomSTUNServer,
				", ",
				string.Format("{0}={1}, ", "AuthValues", this.AuthValues),
				string.Format("{0}={1}, ", "CustomPhotonAppSettings", this.CustomPhotonAppSettings),
				string.Format("{0}={1}, ", "EnableClientSessionCreation", this.EnableClientSessionCreation),
				string.Format("{0}={1}]", "HostMigrationToken", this.HostMigrationToken)
			});
		}

		public GameMode GameMode;

		public string SessionName;

		public Func<string> SessionNameGenerator;

		public NetAddress? Address;

		public NetAddress? CustomPublicAddress;

		public INetworkObjectProvider ObjectProvider;

		public INetworkSceneManager SceneManager;

		public INetworkRunnerUpdater Updater;

		public INetworkObjectInitializer ObjectInitializer;

		public NetworkProjectConfig Config;

		public int? PlayerCount;

		public NetworkSceneInfo? Scene;

		public Action<NetworkRunner> OnGameStarted;

		public bool DisableNATPunchthrough;

		public Type[] CustomCallbackInterfaces;

		public byte[] ConnectionToken;

		public Dictionary<string, SessionProperty> SessionProperties;

		public bool? IsOpen;

		public bool? IsVisible;

		public MatchmakingMode? MatchmakingMode;

		public bool? UseDefaultPhotonCloudPorts;

		public string CustomLobbyName;

		public string CustomSTUNServer;

		public AuthenticationValues AuthValues;

		public FusionAppSettings CustomPhotonAppSettings;

		public bool? EnableClientSessionCreation;

		public HostMigrationToken HostMigrationToken;

		public Action<NetworkRunner> HostMigrationResume;

		public CancellationToken StartGameCancellationToken;

		public bool? UseCachedRegions;
	}
}
