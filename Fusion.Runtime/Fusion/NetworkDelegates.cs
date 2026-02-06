using System;
using System.Collections.Generic;
using Fusion.Sockets;

namespace Fusion
{
	public class NetworkDelegates : INetworkRunnerCallbacks, IPublicFacingInterface
	{
		void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
			Action<NetworkRunner, NetworkObject, PlayerRef> onObjectExitAOI = this.OnObjectExitAOI;
			if (onObjectExitAOI != null)
			{
				onObjectExitAOI(runner, obj, player);
			}
		}

		void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
			Action<NetworkRunner, NetworkObject, PlayerRef> onObjectEnterAOI = this.OnObjectEnterAOI;
			if (onObjectEnterAOI != null)
			{
				onObjectEnterAOI(runner, obj, player);
			}
		}

		void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			Action<NetworkRunner, PlayerRef> onPlayerJoined = this.OnPlayerJoined;
			if (onPlayerJoined != null)
			{
				onPlayerJoined(runner, player);
			}
		}

		void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			Action<NetworkRunner, PlayerRef> onPlayerLeft = this.OnPlayerLeft;
			if (onPlayerLeft != null)
			{
				onPlayerLeft(runner, player);
			}
		}

		void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
		{
			Action<NetworkRunner, NetDisconnectReason> onDisconnectedFromServer = this.OnDisconnectedFromServer;
			if (onDisconnectedFromServer != null)
			{
				onDisconnectedFromServer(runner, reason);
			}
		}

		void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
		{
			Action<NetworkRunner, ShutdownReason> onShutdown = this.OnShutdown;
			if (onShutdown != null)
			{
				onShutdown(runner, shutdownReason);
			}
		}

		void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
		{
			Action<NetworkRunner, NetworkRunnerCallbackArgs.ConnectRequest, byte[]> onConnectRequest = this.OnConnectRequest;
			if (onConnectRequest != null)
			{
				onConnectRequest(runner, request, token);
			}
		}

		void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
		{
			Action<NetworkRunner, NetAddress, NetConnectFailedReason> onConnectFailed = this.OnConnectFailed;
			if (onConnectFailed != null)
			{
				onConnectFailed(runner, remoteAddress, reason);
			}
		}

		void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
		{
			Action<NetworkRunner, SimulationMessagePtr> onUserSimulationMessage = this.OnUserSimulationMessage;
			if (onUserSimulationMessage != null)
			{
				onUserSimulationMessage(runner, message);
			}
		}

		void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
		{
			Action<NetworkRunner, PlayerRef, ReliableKey, ArraySegment<byte>> onReliableDataReceived = this.OnReliableDataReceived;
			if (onReliableDataReceived != null)
			{
				onReliableDataReceived(runner, player, key, data);
			}
		}

		void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
		{
			Action<NetworkRunner, PlayerRef, ReliableKey, float> onReliableDataProgress = this.OnReliableDataProgress;
			if (onReliableDataProgress != null)
			{
				onReliableDataProgress(runner, player, key, progress);
			}
		}

		void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
		{
			Action<NetworkRunner, NetworkInput> onInput = this.OnInput;
			if (onInput != null)
			{
				onInput(runner, input);
			}
		}

		void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
		{
			Action<NetworkRunner, PlayerRef, NetworkInput> onInputMissing = this.OnInputMissing;
			if (onInputMissing != null)
			{
				onInputMissing(runner, player, input);
			}
		}

		void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
		{
			Action<NetworkRunner> onConnectedToServer = this.OnConnectedToServer;
			if (onConnectedToServer != null)
			{
				onConnectedToServer(runner);
			}
		}

		void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
		{
			Action<NetworkRunner, List<SessionInfo>> onSessionListUpdated = this.OnSessionListUpdated;
			if (onSessionListUpdated != null)
			{
				onSessionListUpdated(runner, sessionList);
			}
		}

		void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
		{
			Action<NetworkRunner> onSceneLoadDone = this.OnSceneLoadDone;
			if (onSceneLoadDone != null)
			{
				onSceneLoadDone(runner);
			}
		}

		void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
		{
			Action<NetworkRunner> onSceneLoadStart = this.OnSceneLoadStart;
			if (onSceneLoadStart != null)
			{
				onSceneLoadStart(runner);
			}
		}

		void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
		{
			Action<NetworkRunner, Dictionary<string, object>> onCustomAuthenticationResponse = this.OnCustomAuthenticationResponse;
			if (onCustomAuthenticationResponse != null)
			{
				onCustomAuthenticationResponse(runner, data);
			}
		}

		void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
		{
			Action<NetworkRunner, HostMigrationToken> onHostMigration = this.OnHostMigration;
			if (onHostMigration != null)
			{
				onHostMigration(runner, hostMigrationToken);
			}
		}

		public Action<NetworkRunner, PlayerRef> OnPlayerJoined;

		public Action<NetworkRunner, PlayerRef> OnPlayerLeft;

		public Action<NetworkRunner, NetworkInput> OnInput;

		public Action<NetworkRunner, PlayerRef, NetworkInput> OnInputMissing;

		public Action<NetworkRunner, ShutdownReason> OnShutdown;

		public Action<NetworkRunner, NetDisconnectReason> OnDisconnectedFromServer;

		public Action<NetworkRunner, NetworkRunnerCallbackArgs.ConnectRequest, byte[]> OnConnectRequest;

		public Action<NetworkRunner, NetAddress, NetConnectFailedReason> OnConnectFailed;

		public Action<NetworkRunner, SimulationMessagePtr> OnUserSimulationMessage;

		public Action<NetworkRunner, PlayerRef, ReliableKey, ArraySegment<byte>> OnReliableDataReceived;

		public Action<NetworkRunner, PlayerRef, ReliableKey, float> OnReliableDataProgress;

		public Action<NetworkRunner, NetworkObject, PlayerRef> OnObjectExitAOI;

		public Action<NetworkRunner, NetworkObject, PlayerRef> OnObjectEnterAOI;

		public Action<NetworkRunner> OnConnectedToServer;

		public Action<NetworkRunner> OnSceneLoadDone;

		public Action<NetworkRunner> OnSceneLoadStart;

		public Action<NetworkRunner, List<SessionInfo>> OnSessionListUpdated;

		public Action<NetworkRunner, Dictionary<string, object>> OnCustomAuthenticationResponse;

		public Action<NetworkRunner, HostMigrationToken> OnHostMigration;
	}
}
