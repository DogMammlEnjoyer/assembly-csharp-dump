using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	public static class NetworkedPlayerColourNotifier
	{
		static NetworkedPlayerColourNotifier()
		{
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(NetworkedPlayerColourNotifier.OnPlayerJoinedRoom);
			RoomSystem.JoinedRoomEvent += new Action(NetworkedPlayerColourNotifier.OnJoinedRoom);
		}

		public static void SetLocalRigReference(RigContainer rig)
		{
			NetworkedPlayerColourNotifier.m_localRigContainer = rig;
			NetworkedPlayerColourNotifier.m_localRig = rig.Rig;
			NetworkedPlayerColourNotifier.m_localRig.OnColorChanged += NetworkedPlayerColourNotifier.OnLocalColourChanged;
			NetworkedPlayerColourNotifier.m_netColourDirty = false;
		}

		public static void NotifyOthers()
		{
			if (!RoomSystem.JoinedRoom || NetworkedPlayerColourNotifier.m_localRigContainer.netView.IsNull())
			{
				return;
			}
			Color playerColor = NetworkedPlayerColourNotifier.m_localRig.playerColor;
			float r = playerColor.r;
			float g = playerColor.g;
			float b = playerColor.b;
			NetworkedPlayerColourNotifier.m_localRigContainer.netView.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.Others, new object[]
			{
				r,
				g,
				b
			});
		}

		private static void OnLocalColourChanged(Color color)
		{
			if (!RoomSystem.JoinedRoom)
			{
				return;
			}
			NetworkedPlayerColourNotifier.m_netColourDirty = (NetworkedPlayerColourNotifier.m_initialNetColour != color);
		}

		private static void OnPlayerJoinedRoom(NetPlayer player)
		{
			if (NetworkedPlayerColourNotifier.m_netColourDirty && NetworkedPlayerColourNotifier.m_localRigContainer.netView.IsNotNull())
			{
				Color playerColor = NetworkedPlayerColourNotifier.m_localRig.playerColor;
				float r = playerColor.r;
				float g = playerColor.g;
				float b = playerColor.b;
				NetworkedPlayerColourNotifier.m_localRigContainer.netView.SendRPC("RPC_InitializeNoobMaterial", player, new object[]
				{
					r,
					g,
					b
				});
			}
		}

		private static void OnJoinedRoom()
		{
			NetworkedPlayerColourNotifier.m_initialNetColour = NetworkedPlayerColourNotifier.m_localRig.playerColor;
			NetworkedPlayerColourNotifier.m_netColourDirty = false;
		}

		private static RigContainer m_localRigContainer;

		private static VRRig m_localRig;

		private static Color m_initialNetColour;

		private static bool m_netColourDirty;
	}
}
