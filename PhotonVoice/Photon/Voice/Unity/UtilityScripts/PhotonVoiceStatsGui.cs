using System;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Voice.Unity.UtilityScripts
{
	public class PhotonVoiceStatsGui : MonoBehaviour
	{
		private void OnEnable()
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
			this.voiceClient = this.voiceConnection.VoiceClient;
			this.peer = this.voiceConnection.Client.LoadBalancingPeer;
			if (this.statsRect.x <= 0f)
			{
				this.statsRect.x = (float)Screen.width - this.statsRect.width;
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
			{
				this.statsWindowOn = !this.statsWindowOn;
				this.statsOn = true;
			}
		}

		private void OnGUI()
		{
			if (this.peer.TrafficStatsEnabled != this.statsOn)
			{
				this.peer.TrafficStatsEnabled = this.statsOn;
			}
			if (!this.statsWindowOn)
			{
				return;
			}
			this.statsRect = GUILayout.Window(this.windowId, this.statsRect, new GUI.WindowFunction(this.TrafficStatsWindow), "Voice Client Messages (shift+tab)", Array.Empty<GUILayoutOption>());
		}

		private void TrafficStatsWindow(int windowId)
		{
			bool flag = false;
			TrafficStatsGameLevel trafficStatsGameLevel = this.peer.TrafficStatsGameLevel;
			long num = this.peer.TrafficStatsElapsedMs / 1000L;
			if (num == 0L)
			{
				num = 1L;
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.buttonsOn = GUILayout.Toggle(this.buttonsOn, "buttons", Array.Empty<GUILayoutOption>());
			this.healthStatsVisible = GUILayout.Toggle(this.healthStatsVisible, "health", Array.Empty<GUILayoutOption>());
			this.trafficStatsOn = GUILayout.Toggle(this.trafficStatsOn, "traffic", Array.Empty<GUILayoutOption>());
			this.voiceStatsOn = GUILayout.Toggle(this.voiceStatsOn, "voice stats", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			string text = string.Format("Out {0,4} | In {1,4} | Sum {2,4}", trafficStatsGameLevel.TotalOutgoingMessageCount, trafficStatsGameLevel.TotalIncomingMessageCount, trafficStatsGameLevel.TotalMessageCount);
			string text2 = string.Format("{0}sec average:", num);
			string text3 = string.Format("Out {0,4} | In {1,4} | Sum {2,4}", (long)trafficStatsGameLevel.TotalOutgoingMessageCount / num, (long)trafficStatsGameLevel.TotalIncomingMessageCount / num, (long)trafficStatsGameLevel.TotalMessageCount / num);
			GUILayout.Label(text, Array.Empty<GUILayoutOption>());
			GUILayout.Label(text2, Array.Empty<GUILayoutOption>());
			GUILayout.Label(text3, Array.Empty<GUILayoutOption>());
			if (this.buttonsOn)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				this.statsOn = GUILayout.Toggle(this.statsOn, "stats on", Array.Empty<GUILayoutOption>());
				if (GUILayout.Button("Reset", Array.Empty<GUILayoutOption>()))
				{
					this.peer.TrafficStatsReset();
					this.peer.TrafficStatsEnabled = true;
				}
				flag = GUILayout.Button("To Log", Array.Empty<GUILayoutOption>());
				GUILayout.EndHorizontal();
			}
			string text4 = string.Empty;
			string text5 = string.Empty;
			if (this.trafficStatsOn)
			{
				GUILayout.Box("Voice Client Traffic Stats", Array.Empty<GUILayoutOption>());
				text4 = "Incoming: \n" + this.peer.TrafficStatsIncoming;
				text5 = "Outgoing: \n" + this.peer.TrafficStatsOutgoing;
				GUILayout.Label(text4, Array.Empty<GUILayoutOption>());
				GUILayout.Label(text5, Array.Empty<GUILayoutOption>());
			}
			string text6 = string.Empty;
			if (this.healthStatsVisible)
			{
				GUILayout.Box("Voice Client Health Stats", Array.Empty<GUILayoutOption>());
				text6 = string.Format("ping: {6}|{9}[+/-{7}|{10}]ms resent:{8} \n\nmax ms between\nsend: {0,4} \ndispatch: {1,4} \n\nlongest dispatch for: \nev({3}):{2,3}ms \nop({5}):{4,3}ms", new object[]
				{
					trafficStatsGameLevel.LongestDeltaBetweenSending,
					trafficStatsGameLevel.LongestDeltaBetweenDispatching,
					trafficStatsGameLevel.LongestEventCallback,
					trafficStatsGameLevel.LongestEventCallbackCode,
					trafficStatsGameLevel.LongestOpResponseCallback,
					trafficStatsGameLevel.LongestOpResponseCallbackOpCode,
					this.peer.RoundTripTime,
					this.peer.RoundTripTimeVariance,
					this.peer.ResentReliableCommands,
					this.voiceClient.RoundTripTime,
					this.voiceClient.RoundTripTimeVariance
				});
				GUILayout.Label(text6, Array.Empty<GUILayoutOption>());
			}
			string empty = string.Empty;
			if (this.voiceStatsOn)
			{
				GUILayout.Box("Voice Frames Stats", Array.Empty<GUILayoutOption>());
				GUILayout.Label(string.Format("received: {0}, {1:F2}/s \n\nlost: {2}, {3:F2}/s ({4:F2}%) \n\nsent: {5} ({6} bytes)", new object[]
				{
					this.voiceClient.FramesReceived,
					this.voiceConnection.FramesReceivedPerSecond,
					this.voiceClient.FramesLost,
					this.voiceConnection.FramesLostPerSecond,
					this.voiceConnection.FramesLostPercent,
					this.voiceClient.FramesSent,
					this.voiceClient.FramesSentBytes
				}), Array.Empty<GUILayoutOption>());
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

		private bool statsWindowOn = true;

		private bool statsOn;

		private bool healthStatsVisible;

		private bool trafficStatsOn;

		private bool buttonsOn;

		private bool voiceStatsOn = true;

		private Rect statsRect = new Rect(0f, 100f, 300f, 50f);

		private int windowId = 200;

		private PhotonPeer peer;

		private VoiceConnection voiceConnection;

		private VoiceClient voiceClient;
	}
}
