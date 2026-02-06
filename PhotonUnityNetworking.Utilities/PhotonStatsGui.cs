using System;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	public class PhotonStatsGui : MonoBehaviour
	{
		public void Start()
		{
			if (this.statsRect.x <= 0f)
			{
				this.statsRect.x = (float)Screen.width - this.statsRect.width;
			}
		}

		public void Update()
		{
			if (this.turnOn)
			{
				this.turnOn = false;
				this.statsWindowOn = !this.statsWindowOn;
				this.statsOn = true;
			}
		}

		public void OnGUI()
		{
			if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled != this.statsOn)
			{
				PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = this.statsOn;
			}
			if (!this.statsWindowOn)
			{
				return;
			}
			this.statsRect = GUILayout.Window(this.WindowId, this.statsRect, new GUI.WindowFunction(this.TrafficStatsWindow), "Messages (shift+tab)", Array.Empty<GUILayoutOption>());
		}

		public void TrafficStatsWindow(int windowID)
		{
			bool flag = false;
			TrafficStatsGameLevel trafficStatsGameLevel = PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsGameLevel;
			long num = PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsElapsedMs / 1000L;
			if (num == 0L)
			{
				num = 1L;
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.buttonsOn = GUILayout.Toggle(this.buttonsOn, "buttons", Array.Empty<GUILayoutOption>());
			this.healthStatsVisible = GUILayout.Toggle(this.healthStatsVisible, "health", Array.Empty<GUILayoutOption>());
			this.trafficStatsOn = GUILayout.Toggle(this.trafficStatsOn, "traffic", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			string text = string.Format("Out {0,4} | In {1,4} | Sum {2,4}", trafficStatsGameLevel.TotalOutgoingByteCount, trafficStatsGameLevel.TotalIncomingByteCount, trafficStatsGameLevel.TotalByteCount);
			string text2 = string.Format("{0}sec average:", num);
			string text3 = string.Format("Out {0,4} | In {1,4} | Sum {2,4}", (long)trafficStatsGameLevel.TotalOutgoingByteCount / num, (long)trafficStatsGameLevel.TotalIncomingByteCount / num, (long)trafficStatsGameLevel.TotalByteCount / num);
			GUILayout.Label(text, Array.Empty<GUILayoutOption>());
			GUILayout.Label(text2, Array.Empty<GUILayoutOption>());
			GUILayout.Label(text3, Array.Empty<GUILayoutOption>());
			if (this.buttonsOn)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				this.statsOn = GUILayout.Toggle(this.statsOn, "stats on", Array.Empty<GUILayoutOption>());
				if (GUILayout.Button("Reset", Array.Empty<GUILayoutOption>()))
				{
					PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsReset();
					PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = true;
				}
				flag = GUILayout.Button("To Log", Array.Empty<GUILayoutOption>());
				GUILayout.EndHorizontal();
			}
			string text4 = string.Empty;
			string text5 = string.Empty;
			if (this.trafficStatsOn)
			{
				GUILayout.Box("Traffic Stats", Array.Empty<GUILayoutOption>());
				text4 = "Incoming: \n" + PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsIncoming.ToString();
				text5 = "Outgoing: \n" + PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsOutgoing.ToString();
				GUILayout.Label(text4, Array.Empty<GUILayoutOption>());
				GUILayout.Label(text5, Array.Empty<GUILayoutOption>());
			}
			string text6 = string.Empty;
			if (this.healthStatsVisible)
			{
				GUILayout.Box("Health Stats", Array.Empty<GUILayoutOption>());
				text6 = string.Format("ping: {6}[+/-{7}]ms resent:{8} \n\nmax ms between\nsend: {0,4} \ndispatch: {1,4} \n\nlongest dispatch for: \nev({3}):{2,3}ms \nop({5}):{4,3}ms", new object[]
				{
					trafficStatsGameLevel.LongestDeltaBetweenSending,
					trafficStatsGameLevel.LongestDeltaBetweenDispatching,
					trafficStatsGameLevel.LongestEventCallback,
					trafficStatsGameLevel.LongestEventCallbackCode,
					trafficStatsGameLevel.LongestOpResponseCallback,
					trafficStatsGameLevel.LongestOpResponseCallbackOpCode,
					PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTime,
					PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTimeVariance,
					PhotonNetwork.NetworkingClient.LoadBalancingPeer.ResentReliableCommands
				});
				GUILayout.Label(text6, Array.Empty<GUILayoutOption>());
			}
			if (flag)
			{
				Debug.Log(string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", new object[]
				{
					text,
					text2,
					text3,
					text4,
					text5,
					text6
				}));
			}
			if (GUI.changed)
			{
				this.statsRect.height = 100f;
			}
			GUI.DragWindow();
		}

		public bool statsWindowOn = true;

		public bool statsOn = true;

		public bool healthStatsVisible;

		public bool trafficStatsOn;

		public bool buttonsOn;

		public Rect statsRect = new Rect(0f, 100f, 200f, 50f);

		public int WindowId = 100;

		public bool turnOn;
	}
}
