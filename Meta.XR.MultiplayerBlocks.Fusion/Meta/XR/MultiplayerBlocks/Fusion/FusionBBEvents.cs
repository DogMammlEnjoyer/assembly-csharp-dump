using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Fusion
{
	public class FusionBBEvents : MonoBehaviour, INetworkRunnerCallbacks, IPublicFacingInterface
	{
		public static event Action<NetworkRunner> OnConnectedToServer;

		public static event Action<NetworkRunner, PlayerRef> OnPlayerJoined;

		public static event Action<NetworkRunner, NetworkInput> OnInput;

		public static event Action<NetworkRunner, NetAddress, NetConnectFailedReason> OnConnectFailed;

		public static event Action<NetworkRunner, NetworkRunnerCallbackArgs.ConnectRequest, byte[]> OnConnectRequest;

		public static event Action<NetworkRunner, Dictionary<string, object>> OnCustomAuthenticationResponse;

		public static event Action<NetworkRunner, HostMigrationToken> OnHostMigration;

		public static event Action<NetworkRunner, PlayerRef, NetworkInput> OnInputMissing;

		public static event Action<NetworkRunner, PlayerRef> OnPlayerLeft;

		public static event Action<NetworkRunner> OnSceneLoadDone;

		public static event Action<NetworkRunner> OnSceneLoadStart;

		public static event Action<NetworkRunner, List<SessionInfo>> OnSessionListUpdated;

		public static event Action<NetworkRunner, ShutdownReason> OnShutdown;

		public static event Action<NetworkRunner, SimulationMessagePtr> OnUserSimulationMessage;

		public static event Action<NetworkRunner, NetworkObject, PlayerRef> OnObjectExitAOI;

		public static event Action<NetworkRunner, NetworkObject, PlayerRef> OnObjectEnterAOI;

		public static event Action<NetworkRunner, NetDisconnectReason> OnDisconnectedFromServer;

		public static event Action<NetworkRunner, PlayerRef, ReliableKey, ArraySegment<byte>> OnReliableDataReceived;

		public static event Action<NetworkRunner, PlayerRef, ReliableKey, float> OnReliableDataProgress;

		void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
		{
			Action<NetworkRunner> onConnectedToServer = FusionBBEvents.OnConnectedToServer;
			if (onConnectedToServer == null)
			{
				return;
			}
			onConnectedToServer(runner);
		}

		void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			Action<NetworkRunner, PlayerRef> onPlayerJoined = FusionBBEvents.OnPlayerJoined;
			if (onPlayerJoined == null)
			{
				return;
			}
			onPlayerJoined(runner, player);
		}

		void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
		{
			Action<NetworkRunner, NetworkInput> onInput = FusionBBEvents.OnInput;
			if (onInput == null)
			{
				return;
			}
			onInput(runner, input);
		}

		void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
		{
			Debug.LogWarning("OnConnectFailed");
			Action<NetworkRunner, NetAddress, NetConnectFailedReason> onConnectFailed = FusionBBEvents.OnConnectFailed;
			if (onConnectFailed == null)
			{
				return;
			}
			onConnectFailed(runner, remoteAddress, reason);
		}

		void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
		{
			Debug.LogWarning("OnConnectRequest");
			Action<NetworkRunner, NetworkRunnerCallbackArgs.ConnectRequest, byte[]> onConnectRequest = FusionBBEvents.OnConnectRequest;
			if (onConnectRequest == null)
			{
				return;
			}
			onConnectRequest(runner, request, token);
		}

		void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
		{
			Action<NetworkRunner, Dictionary<string, object>> onCustomAuthenticationResponse = FusionBBEvents.OnCustomAuthenticationResponse;
			if (onCustomAuthenticationResponse == null)
			{
				return;
			}
			onCustomAuthenticationResponse(runner, data);
		}

		void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
		{
			Action<NetworkRunner, HostMigrationToken> onHostMigration = FusionBBEvents.OnHostMigration;
			if (onHostMigration == null)
			{
				return;
			}
			onHostMigration(runner, hostMigrationToken);
		}

		void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
		{
			Action<NetworkRunner, PlayerRef, NetworkInput> onInputMissing = FusionBBEvents.OnInputMissing;
			if (onInputMissing == null)
			{
				return;
			}
			onInputMissing(runner, player, input);
		}

		void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			Action<NetworkRunner, PlayerRef> onPlayerLeft = FusionBBEvents.OnPlayerLeft;
			if (onPlayerLeft == null)
			{
				return;
			}
			onPlayerLeft(runner, player);
		}

		void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
		{
			Action<NetworkRunner> onSceneLoadDone = FusionBBEvents.OnSceneLoadDone;
			if (onSceneLoadDone == null)
			{
				return;
			}
			onSceneLoadDone(runner);
		}

		void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
		{
			Action<NetworkRunner> onSceneLoadStart = FusionBBEvents.OnSceneLoadStart;
			if (onSceneLoadStart == null)
			{
				return;
			}
			onSceneLoadStart(runner);
		}

		void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
		{
			Action<NetworkRunner, List<SessionInfo>> onSessionListUpdated = FusionBBEvents.OnSessionListUpdated;
			if (onSessionListUpdated == null)
			{
				return;
			}
			onSessionListUpdated(runner, sessionList);
		}

		void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
		{
			Action<NetworkRunner, ShutdownReason> onShutdown = FusionBBEvents.OnShutdown;
			if (onShutdown == null)
			{
				return;
			}
			onShutdown(runner, shutdownReason);
		}

		void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
		{
			Action<NetworkRunner, SimulationMessagePtr> onUserSimulationMessage = FusionBBEvents.OnUserSimulationMessage;
			if (onUserSimulationMessage == null)
			{
				return;
			}
			onUserSimulationMessage(runner, message);
		}

		void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
			Action<NetworkRunner, NetworkObject, PlayerRef> onObjectExitAOI = FusionBBEvents.OnObjectExitAOI;
			if (onObjectExitAOI == null)
			{
				return;
			}
			onObjectExitAOI(runner, obj, player);
		}

		void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
			Action<NetworkRunner, NetworkObject, PlayerRef> onObjectEnterAOI = FusionBBEvents.OnObjectEnterAOI;
			if (onObjectEnterAOI == null)
			{
				return;
			}
			onObjectEnterAOI(runner, obj, player);
		}

		void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
		{
			Action<NetworkRunner, NetDisconnectReason> onDisconnectedFromServer = FusionBBEvents.OnDisconnectedFromServer;
			if (onDisconnectedFromServer == null)
			{
				return;
			}
			onDisconnectedFromServer(runner, reason);
		}

		void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
		{
			Action<NetworkRunner, PlayerRef, ReliableKey, ArraySegment<byte>> onReliableDataReceived = FusionBBEvents.OnReliableDataReceived;
			if (onReliableDataReceived == null)
			{
				return;
			}
			onReliableDataReceived(runner, player, key, data);
		}

		void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
		{
			Action<NetworkRunner, PlayerRef, ReliableKey, float> onReliableDataProgress = FusionBBEvents.OnReliableDataProgress;
			if (onReliableDataProgress == null)
			{
				return;
			}
			onReliableDataProgress(runner, player, key, progress);
		}
	}
}
