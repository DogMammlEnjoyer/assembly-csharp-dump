using System;
using System.Collections.Generic;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Events;

namespace Fusion
{
	[AddComponentMenu("Fusion/Network Events")]
	public class NetworkEvents : Behaviour, INetworkRunnerCallbacks, IPublicFacingInterface
	{
		void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
			this.OnObjectExitAOI.Invoke(runner, obj, player);
		}

		void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
			this.OnObjectEnterAOI.Invoke(runner, obj, player);
		}

		void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			NetworkEvents.PlayerEvent playerJoined = this.PlayerJoined;
			if (playerJoined != null)
			{
				playerJoined.Invoke(runner, player);
			}
		}

		void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			NetworkEvents.PlayerEvent playerLeft = this.PlayerLeft;
			if (playerLeft != null)
			{
				playerLeft.Invoke(runner, player);
			}
		}

		void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
		{
			NetworkEvents.InputEvent onInput = this.OnInput;
			if (onInput != null)
			{
				onInput.Invoke(runner, input);
			}
		}

		void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
		{
			NetworkEvents.InputPlayerEvent onInputMissing = this.OnInputMissing;
			if (onInputMissing != null)
			{
				onInputMissing.Invoke(runner, player, input);
			}
		}

		void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
		{
			NetworkEvents.ShutdownEvent onShutdown = this.OnShutdown;
			if (onShutdown != null)
			{
				onShutdown.Invoke(runner, shutdownReason);
			}
		}

		void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
		{
			NetworkEvents.RunnerEvent onConnectedToServer = this.OnConnectedToServer;
			if (onConnectedToServer != null)
			{
				onConnectedToServer.Invoke(runner);
			}
		}

		void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
		{
			NetworkEvents.DisconnectFromServerEvent onDisconnectedFromServer = this.OnDisconnectedFromServer;
			if (onDisconnectedFromServer != null)
			{
				onDisconnectedFromServer.Invoke(runner, reason);
			}
		}

		void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
		{
			NetworkEvents.ConnectRequestEvent onConnectRequest = this.OnConnectRequest;
			if (onConnectRequest != null)
			{
				onConnectRequest.Invoke(runner, request, token);
			}
		}

		void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
		{
			NetworkEvents.ConnectFailedEvent onConnectFailed = this.OnConnectFailed;
			if (onConnectFailed != null)
			{
				onConnectFailed.Invoke(runner, remoteAddress, reason);
			}
		}

		void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
		{
			NetworkEvents.SimulationMessageEvent onSimulationMessage = this.OnSimulationMessage;
			if (onSimulationMessage != null)
			{
				onSimulationMessage.Invoke(runner, message);
			}
		}

		void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
		{
			NetworkEvents.SessionListUpdateEvent onSessionListUpdate = this.OnSessionListUpdate;
			if (onSessionListUpdate != null)
			{
				onSessionListUpdate.Invoke(runner, sessionList);
			}
		}

		void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
		{
			NetworkEvents.ReliableDataEvent onReliableData = this.OnReliableData;
			if (onReliableData != null)
			{
				onReliableData.Invoke(runner, player, key, data);
			}
		}

		void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
		{
			NetworkEvents.ReliableProgressEvent onReliableProgress = this.OnReliableProgress;
			if (onReliableProgress != null)
			{
				onReliableProgress.Invoke(runner, player, key, progress);
			}
		}

		void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
		{
			NetworkEvents.RunnerEvent onSceneLoadDone = this.OnSceneLoadDone;
			if (onSceneLoadDone != null)
			{
				onSceneLoadDone.Invoke(runner);
			}
		}

		void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
		{
			NetworkEvents.RunnerEvent onSceneLoadStart = this.OnSceneLoadStart;
			if (onSceneLoadStart != null)
			{
				onSceneLoadStart.Invoke(runner);
			}
		}

		void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
		{
			NetworkEvents.CustomAuthenticationResponse onCustomAuthenticationResponse = this.OnCustomAuthenticationResponse;
			if (onCustomAuthenticationResponse != null)
			{
				onCustomAuthenticationResponse.Invoke(runner, data);
			}
		}

		void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
		{
			NetworkEvents.HostMigrationEvent onHostMigration = this.OnHostMigration;
			if (onHostMigration != null)
			{
				onHostMigration.Invoke(runner, hostMigrationToken);
			}
		}

		public NetworkEvents.InputEvent OnInput;

		public NetworkEvents.InputPlayerEvent OnInputMissing;

		public NetworkEvents.RunnerEvent OnConnectedToServer;

		public NetworkEvents.DisconnectFromServerEvent OnDisconnectedFromServer;

		public NetworkEvents.ConnectRequestEvent OnConnectRequest;

		public NetworkEvents.ConnectFailedEvent OnConnectFailed;

		public NetworkEvents.PlayerEvent PlayerJoined;

		public NetworkEvents.PlayerEvent PlayerLeft;

		public NetworkEvents.SimulationMessageEvent OnSimulationMessage;

		public NetworkEvents.ShutdownEvent OnShutdown;

		public NetworkEvents.SessionListUpdateEvent OnSessionListUpdate;

		public NetworkEvents.CustomAuthenticationResponse OnCustomAuthenticationResponse;

		public NetworkEvents.HostMigrationEvent OnHostMigration;

		public NetworkEvents.RunnerEvent OnSceneLoadDone;

		public NetworkEvents.RunnerEvent OnSceneLoadStart;

		public NetworkEvents.ReliableDataEvent OnReliableData;

		public NetworkEvents.ReliableProgressEvent OnReliableProgress;

		public NetworkEvents.ObjectPlayerEvent OnObjectEnterAOI;

		public NetworkEvents.ObjectPlayerEvent OnObjectExitAOI;

		[Serializable]
		public class InputEvent : UnityEvent<NetworkRunner, NetworkInput>
		{
		}

		[Serializable]
		public class InputPlayerEvent : UnityEvent<NetworkRunner, PlayerRef, NetworkInput>
		{
		}

		[Serializable]
		public class ConnectRequestEvent : UnityEvent<NetworkRunner, NetworkRunnerCallbackArgs.ConnectRequest, byte[]>
		{
		}

		[Serializable]
		public class ConnectFailedEvent : UnityEvent<NetworkRunner, NetAddress, NetConnectFailedReason>
		{
		}

		[Serializable]
		public class DisconnectFromServerEvent : UnityEvent<NetworkRunner, NetDisconnectReason>
		{
		}

		[Serializable]
		public class ShutdownEvent : UnityEvent<NetworkRunner, ShutdownReason>
		{
		}

		[Serializable]
		public class PlayerEvent : UnityEvent<NetworkRunner, PlayerRef>
		{
		}

		[Serializable]
		public class RunnerEvent : UnityEvent<NetworkRunner>
		{
		}

		[Serializable]
		public class SimulationMessageEvent : UnityEvent<NetworkRunner, SimulationMessagePtr>
		{
		}

		[Serializable]
		public class SessionListUpdateEvent : UnityEvent<NetworkRunner, List<SessionInfo>>
		{
		}

		[Serializable]
		public class CustomAuthenticationResponse : UnityEvent<NetworkRunner, Dictionary<string, object>>
		{
		}

		[Serializable]
		public class HostMigrationEvent : UnityEvent<NetworkRunner, HostMigrationToken>
		{
		}

		[Serializable]
		public class ReliableDataEvent : UnityEvent<NetworkRunner, PlayerRef, ReliableKey, ArraySegment<byte>>
		{
		}

		[Serializable]
		public class ReliableProgressEvent : UnityEvent<NetworkRunner, PlayerRef, ReliableKey, float>
		{
		}

		[Serializable]
		public class ObjectEvent : UnityEvent<NetworkRunner, NetworkObject>
		{
		}

		[Serializable]
		public class ObjectPlayerEvent : UnityEvent<NetworkRunner, NetworkObject, PlayerRef>
		{
		}
	}
}
