using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	public static class PlayerNumberingExtensions
	{
		public static int GetPlayerNumber(this Player player)
		{
			if (player == null)
			{
				return -1;
			}
			if (PhotonNetwork.OfflineMode)
			{
				return 0;
			}
			if (!PhotonNetwork.IsConnectedAndReady)
			{
				return -1;
			}
			object obj;
			if (player.CustomProperties.TryGetValue("pNr", out obj))
			{
				return (int)((byte)obj);
			}
			return -1;
		}

		public static void SetPlayerNumber(this Player player, int playerNumber)
		{
			if (player == null)
			{
				return;
			}
			if (PhotonNetwork.OfflineMode)
			{
				return;
			}
			if (playerNumber < 0)
			{
				Debug.LogWarning("Setting invalid playerNumber: " + playerNumber.ToString() + " for: " + player.ToStringFull());
			}
			if (!PhotonNetwork.IsConnectedAndReady)
			{
				Debug.LogWarning("SetPlayerNumber was called in state: " + PhotonNetwork.NetworkClientState.ToString() + ". Not IsConnectedAndReady.");
				return;
			}
			if (player.GetPlayerNumber() != playerNumber)
			{
				Debug.Log("PlayerNumbering: Set number " + playerNumber.ToString());
				player.SetCustomProperties(new Hashtable
				{
					{
						"pNr",
						(byte)playerNumber
					}
				}, null, null);
			}
		}
	}
}
