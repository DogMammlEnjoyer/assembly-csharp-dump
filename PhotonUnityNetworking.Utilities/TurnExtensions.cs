using System;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
	public static class TurnExtensions
	{
		public static void SetTurn(this Room room, int turn, bool setStartTime = false)
		{
			if (room == null || room.CustomProperties == null)
			{
				return;
			}
			Hashtable hashtable = new Hashtable();
			hashtable[TurnExtensions.TurnPropKey] = turn;
			if (setStartTime)
			{
				hashtable[TurnExtensions.TurnStartPropKey] = PhotonNetwork.ServerTimestamp;
			}
			room.SetCustomProperties(hashtable, null, null);
		}

		public static int GetTurn(this RoomInfo room)
		{
			if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnExtensions.TurnPropKey))
			{
				return 0;
			}
			return (int)room.CustomProperties[TurnExtensions.TurnPropKey];
		}

		public static int GetTurnStart(this RoomInfo room)
		{
			if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnExtensions.TurnStartPropKey))
			{
				return 0;
			}
			return (int)room.CustomProperties[TurnExtensions.TurnStartPropKey];
		}

		public static int GetFinishedTurn(this Player player)
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			if (currentRoom == null || currentRoom.CustomProperties == null || !currentRoom.CustomProperties.ContainsKey(TurnExtensions.TurnPropKey))
			{
				return 0;
			}
			string key = TurnExtensions.FinishedTurnPropKey + player.ActorNumber.ToString();
			return (int)currentRoom.CustomProperties[key];
		}

		public static void SetFinishedTurn(this Player player, int turn)
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			if (currentRoom == null || currentRoom.CustomProperties == null)
			{
				return;
			}
			string key = TurnExtensions.FinishedTurnPropKey + player.ActorNumber.ToString();
			Hashtable hashtable = new Hashtable();
			hashtable[key] = turn;
			currentRoom.SetCustomProperties(hashtable, null, null);
		}

		public static readonly string TurnPropKey = "Turn";

		public static readonly string TurnStartPropKey = "TStart";

		public static readonly string FinishedTurnPropKey = "FToA";
	}
}
