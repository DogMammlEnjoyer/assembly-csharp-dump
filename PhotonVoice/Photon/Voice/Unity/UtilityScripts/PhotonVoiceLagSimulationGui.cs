using System;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Voice.Unity.UtilityScripts
{
	[RequireComponent(typeof(VoiceConnection))]
	public class PhotonVoiceLagSimulationGui : MonoBehaviour
	{
		public void OnEnable()
		{
			VoiceConnection[] components = base.GetComponents<VoiceConnection>();
			if (components == null || components.Length == 0)
			{
				Debug.LogError("No VoiceConnection component found, PhotonVoiceStatsGui disabled", this);
				base.enabled = false;
				return;
			}
			if (components.Length > 1)
			{
				Debug.LogWarningFormat(this, "Multiple VoiceConnection components found, using first occurrence attached to GameObject {0}", new object[]
				{
					components[0].name
				});
			}
			this.voiceConnection = components[0];
			this.peer = this.voiceConnection.Client.LoadBalancingPeer;
			this.debugLostPercent = (float)this.voiceConnection.VoiceClient.DebugLostPercent;
		}

		private void OnGUI()
		{
			if (!this.visible)
			{
				return;
			}
			if (this.peer == null)
			{
				this.windowRect = GUILayout.Window(this.windowId, this.windowRect, new GUI.WindowFunction(this.NetSimHasNoPeerWindow), "Voice Network Simulation", Array.Empty<GUILayoutOption>());
				return;
			}
			this.windowRect = GUILayout.Window(this.windowId, this.windowRect, new GUI.WindowFunction(this.NetSimWindow), "Voice Network Simulation", Array.Empty<GUILayoutOption>());
		}

		private void NetSimHasNoPeerWindow(int windowId)
		{
			GUILayout.Label("No voice peer to communicate with. ", Array.Empty<GUILayoutOption>());
		}

		private void NetSimWindow(int windowId)
		{
			GUILayout.Label(string.Format("Rtt:{0,4} +/-{1,3}", this.peer.RoundTripTime, this.peer.RoundTripTimeVariance), Array.Empty<GUILayoutOption>());
			bool isSimulationEnabled = this.peer.IsSimulationEnabled;
			bool flag = GUILayout.Toggle(isSimulationEnabled, "Simulate", Array.Empty<GUILayoutOption>());
			if (flag != isSimulationEnabled)
			{
				this.peer.IsSimulationEnabled = flag;
			}
			float num = (float)this.peer.NetworkSimulationSettings.IncomingLag;
			GUILayout.Label(string.Format("Lag {0}", num), Array.Empty<GUILayoutOption>());
			num = GUILayout.HorizontalSlider(num, 0f, 500f, Array.Empty<GUILayoutOption>());
			this.peer.NetworkSimulationSettings.IncomingLag = (int)num;
			this.peer.NetworkSimulationSettings.OutgoingLag = (int)num;
			float num2 = (float)this.peer.NetworkSimulationSettings.IncomingJitter;
			GUILayout.Label(string.Format("Jit {0}", num2), Array.Empty<GUILayoutOption>());
			num2 = GUILayout.HorizontalSlider(num2, 0f, 100f, Array.Empty<GUILayoutOption>());
			this.peer.NetworkSimulationSettings.IncomingJitter = (int)num2;
			this.peer.NetworkSimulationSettings.OutgoingJitter = (int)num2;
			float num3 = (float)this.peer.NetworkSimulationSettings.IncomingLossPercentage;
			GUILayout.Label(string.Format("Loss {0}", num3), Array.Empty<GUILayoutOption>());
			num3 = GUILayout.HorizontalSlider(num3, 0f, 10f, Array.Empty<GUILayoutOption>());
			this.peer.NetworkSimulationSettings.IncomingLossPercentage = (int)num3;
			this.peer.NetworkSimulationSettings.OutgoingLossPercentage = (int)num3;
			GUILayout.Label(string.Format("Lost Audio Frames {0}%", (int)this.debugLostPercent), Array.Empty<GUILayoutOption>());
			this.debugLostPercent = GUILayout.HorizontalSlider(this.debugLostPercent, 0f, 100f, Array.Empty<GUILayoutOption>());
			if (flag)
			{
				this.voiceConnection.VoiceClient.DebugLostPercent = (int)this.debugLostPercent;
			}
			else
			{
				this.voiceConnection.VoiceClient.DebugLostPercent = 0;
			}
			if (GUI.changed)
			{
				this.windowRect.height = 100f;
			}
			GUI.DragWindow();
		}

		private VoiceConnection voiceConnection;

		private Rect windowRect = new Rect(0f, 100f, 200f, 100f);

		private int windowId = 201;

		private bool visible = true;

		private PhotonPeer peer;

		private float debugLostPercent;
	}
}
