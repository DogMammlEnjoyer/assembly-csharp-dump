using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	public class PlayerNumbering : MonoBehaviourPunCallbacks
	{
		public static event PlayerNumbering.PlayerNumberingChanged OnPlayerNumberingChanged;

		public void Awake()
		{
			if (PlayerNumbering.instance != null && PlayerNumbering.instance != this && PlayerNumbering.instance.gameObject != null)
			{
				Object.DestroyImmediate(PlayerNumbering.instance.gameObject);
			}
			PlayerNumbering.instance = this;
			if (this.dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			this.RefreshData();
		}

		public override void OnJoinedRoom()
		{
			this.RefreshData();
		}

		public override void OnLeftRoom()
		{
			PhotonNetwork.LocalPlayer.CustomProperties.Remove("pNr");
		}

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			this.RefreshData();
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			this.RefreshData();
		}

		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			if (changedProps != null && changedProps.ContainsKey("pNr"))
			{
				this.RefreshData();
			}
		}

		public void RefreshData()
		{
			if (PhotonNetwork.CurrentRoom == null)
			{
				return;
			}
			if (PhotonNetwork.LocalPlayer.GetPlayerNumber() >= 0)
			{
				PlayerNumbering.SortedPlayers = (from p in PhotonNetwork.CurrentRoom.Players.Values
				orderby p.GetPlayerNumber()
				select p).ToArray<Player>();
				if (PlayerNumbering.OnPlayerNumberingChanged != null)
				{
					PlayerNumbering.OnPlayerNumberingChanged();
				}
				return;
			}
			HashSet<int> hashSet = new HashSet<int>();
			Player[] array = (from p in PhotonNetwork.PlayerList
			orderby p.ActorNumber
			select p).ToArray<Player>();
			string text = "all players: ";
			foreach (Player player in array)
			{
				text = string.Concat(new string[]
				{
					text,
					player.ActorNumber.ToString(),
					"=pNr:",
					player.GetPlayerNumber().ToString(),
					", "
				});
				int playerNumber = player.GetPlayerNumber();
				if (player.IsLocal)
				{
					Debug.Log("PhotonNetwork.CurrentRoom.PlayerCount = " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
					for (int j = 0; j < (int)PhotonNetwork.CurrentRoom.PlayerCount; j++)
					{
						if (!hashSet.Contains(j))
						{
							player.SetPlayerNumber(j);
							break;
						}
					}
					break;
				}
				if (playerNumber < 0)
				{
					break;
				}
				hashSet.Add(playerNumber);
			}
			PlayerNumbering.SortedPlayers = (from p in PhotonNetwork.CurrentRoom.Players.Values
			orderby p.GetPlayerNumber()
			select p).ToArray<Player>();
			if (PlayerNumbering.OnPlayerNumberingChanged != null)
			{
				PlayerNumbering.OnPlayerNumberingChanged();
			}
		}

		public static PlayerNumbering instance;

		public static Player[] SortedPlayers;

		public const string RoomPlayerIndexedProp = "pNr";

		public bool dontDestroyOnLoad;

		public delegate void PlayerNumberingChanged();
	}
}
