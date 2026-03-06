using System;
using System.Collections.Generic;

namespace Meta.XR.MultiplayerBlocks.Colocation
{
	internal static class NetworkDataUtils
	{
		public static ulong? GetOculusIdOfColocatedGroupOwnerFromColocationGroupId(uint colocationGroupId)
		{
			foreach (Anchor anchor in NetworkAdapter.NetworkData.GetAllAnchors())
			{
				if (colocationGroupId == anchor.colocationGroupId)
				{
					return new ulong?(anchor.ownerOculusId);
				}
			}
			return null;
		}

		public static List<Player> GetAllPlayersFromColocationGroupId(uint colocationGroupId)
		{
			List<Player> allPlayers = NetworkAdapter.NetworkData.GetAllPlayers();
			List<Player> list = new List<Player>();
			foreach (Player player in allPlayers)
			{
				if (colocationGroupId == player.colocationGroupId)
				{
					list.Add(player);
				}
			}
			return list;
		}

		public static List<Player> GetAllPlayersColocatedWith(ulong oculusId, bool includeMyself)
		{
			INetworkData networkData = NetworkAdapter.NetworkData;
			List<Player> allPlayers = networkData.GetAllPlayers();
			uint colocationGroupId = networkData.GetPlayerWithOculusId(oculusId).Value.colocationGroupId;
			List<Player> list = new List<Player>();
			foreach (Player player in allPlayers)
			{
				if (player.colocationGroupId == colocationGroupId)
				{
					list.Add(player);
					if (!includeMyself && player.oculusId == oculusId)
					{
						list.RemoveAt(list.Count - 1);
					}
				}
			}
			return list;
		}

		public static Player? GetPlayerFromOculusId(ulong oculusId)
		{
			return NetworkAdapter.NetworkData.GetPlayerWithOculusId(oculusId);
		}
	}
}
