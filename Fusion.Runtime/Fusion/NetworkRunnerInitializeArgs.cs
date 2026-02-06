using System;
using Fusion.Sockets;

namespace Fusion
{
	internal struct NetworkRunnerInitializeArgs
	{
		public bool IsSinglePlayer
		{
			get
			{
				return this.SimulationMode.GetValueOrDefault() == SimulationModes.Host && this.PlayerCount.GetValueOrDefault() == 1;
			}
		}

		public NetworkSceneInfo? Scene;

		public NetAddress? Address;

		public NetAddress? PublicAddress;

		public int? PlayerCount;

		public SimulationModes? SimulationMode;

		public int? InputWordCount;

		public int? SceneInfoWordCount;

		public NetworkProjectConfig Config;

		public Action<NetworkRunner> OnGameStarted;

		public INetworkObjectProvider ObjectProvider;

		public INetworkSceneManager SceneManager;

		public INetworkRunnerUpdater Updater;

		public INetworkObjectInitializer ObjectInitializer;

		public Type[] CustomCallbackInterfaces;

		public byte[] ConnectionToken;

		public NetworkId? ResumeId;

		public Tick? ResumeTick;

		public byte[] ResumeState;

		public Action<NetworkRunner> HostMigrationResume;
	}
}
