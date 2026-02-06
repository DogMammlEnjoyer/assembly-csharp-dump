using System;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	public class PhotonLagSimulationGui : MonoBehaviour
	{
		public PhotonPeer Peer { get; set; }

		public void Start()
		{
			this.Peer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
		}

		public void OnGUI()
		{
			if (!this.Visible)
			{
				return;
			}
			if (this.Peer == null)
			{
				this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, new GUI.WindowFunction(this.NetSimHasNoPeerWindow), "Netw. Sim.", Array.Empty<GUILayoutOption>());
				return;
			}
			this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, new GUI.WindowFunction(this.NetSimWindow), "Netw. Sim.", Array.Empty<GUILayoutOption>());
		}

		private void NetSimHasNoPeerWindow(int windowId)
		{
			GUILayout.Label("No peer to communicate with. ", Array.Empty<GUILayoutOption>());
		}

		private void NetSimWindow(int windowId)
		{
			GUILayout.Label(string.Format("Rtt:{0,4} +/-{1,3}", this.Peer.RoundTripTime, this.Peer.RoundTripTimeVariance), Array.Empty<GUILayoutOption>());
			bool isSimulationEnabled = this.Peer.IsSimulationEnabled;
			bool flag = GUILayout.Toggle(isSimulationEnabled, "Simulate", Array.Empty<GUILayoutOption>());
			if (flag != isSimulationEnabled)
			{
				this.Peer.IsSimulationEnabled = flag;
			}
			float num = (float)this.Peer.NetworkSimulationSettings.IncomingLag;
			GUILayout.Label("Lag " + num.ToString(), Array.Empty<GUILayoutOption>());
			num = GUILayout.HorizontalSlider(num, 0f, 500f, Array.Empty<GUILayoutOption>());
			this.Peer.NetworkSimulationSettings.IncomingLag = (int)num;
			this.Peer.NetworkSimulationSettings.OutgoingLag = (int)num;
			float num2 = (float)this.Peer.NetworkSimulationSettings.IncomingJitter;
			GUILayout.Label("Jit " + num2.ToString(), Array.Empty<GUILayoutOption>());
			num2 = GUILayout.HorizontalSlider(num2, 0f, 100f, Array.Empty<GUILayoutOption>());
			this.Peer.NetworkSimulationSettings.IncomingJitter = (int)num2;
			this.Peer.NetworkSimulationSettings.OutgoingJitter = (int)num2;
			float num3 = (float)this.Peer.NetworkSimulationSettings.IncomingLossPercentage;
			GUILayout.Label("Loss " + num3.ToString(), Array.Empty<GUILayoutOption>());
			num3 = GUILayout.HorizontalSlider(num3, 0f, 10f, Array.Empty<GUILayoutOption>());
			this.Peer.NetworkSimulationSettings.IncomingLossPercentage = (int)num3;
			this.Peer.NetworkSimulationSettings.OutgoingLossPercentage = (int)num3;
			if (GUI.changed)
			{
				this.WindowRect.height = 100f;
			}
			GUI.DragWindow();
		}

		public Rect WindowRect = new Rect(0f, 100f, 120f, 100f);

		public int WindowId = 101;

		public bool Visible = true;
	}
}
