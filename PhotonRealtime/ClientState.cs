using System;

namespace Photon.Realtime
{
	public enum ClientState
	{
		PeerCreated,
		Authenticating,
		Authenticated,
		JoiningLobby,
		JoinedLobby,
		DisconnectingFromMasterServer,
		[Obsolete("Renamed to DisconnectingFromMasterServer")]
		DisconnectingFromMasterserver = 5,
		ConnectingToGameServer,
		[Obsolete("Renamed to ConnectingToGameServer")]
		ConnectingToGameserver = 6,
		ConnectedToGameServer,
		[Obsolete("Renamed to ConnectedToGameServer")]
		ConnectedToGameserver = 7,
		Joining,
		Joined,
		Leaving,
		DisconnectingFromGameServer,
		[Obsolete("Renamed to DisconnectingFromGameServer")]
		DisconnectingFromGameserver = 11,
		ConnectingToMasterServer,
		[Obsolete("Renamed to ConnectingToMasterServer.")]
		ConnectingToMasterserver = 12,
		Disconnecting,
		Disconnected,
		ConnectedToMasterServer,
		[Obsolete("Renamed to ConnectedToMasterServer.")]
		ConnectedToMasterserver = 15,
		[Obsolete("Renamed to ConnectedToMasterServer.")]
		ConnectedToMaster = 15,
		ConnectingToNameServer,
		ConnectedToNameServer,
		DisconnectingFromNameServer,
		ConnectWithFallbackProtocol
	}
}
