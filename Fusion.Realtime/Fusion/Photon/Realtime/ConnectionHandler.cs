using System;
using System.Diagnostics;
using System.Threading;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Fusion.Photon.Realtime
{
	internal class ConnectionHandler : MonoBehaviour
	{
		public LoadBalancingClient Client { get; set; }

		public int CountSendAcksOnly { get; private set; }

		public bool FallbackThreadRunning { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void StaticReset()
		{
			ConnectionHandler.AppQuits = false;
			ConnectionHandler.AppPause = false;
			ConnectionHandler.AppPauseRecent = false;
			ConnectionHandler.AppOutOfFocus = false;
			ConnectionHandler.AppOutOfFocusRecent = false;
		}

		protected virtual void Awake()
		{
			bool applyDontDestroyOnLoad = this.ApplyDontDestroyOnLoad;
			if (applyDontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		protected virtual void OnDisable()
		{
			this.StopFallbackSendAckThread();
			bool appQuits = ConnectionHandler.AppQuits;
			if (appQuits)
			{
				bool flag = this.Client != null && this.Client.IsConnected;
				if (flag)
				{
					this.Client.Disconnect(DisconnectCause.ApplicationQuit);
					this.Client.LoadBalancingPeer.StopThread();
					this.Client.LoadBalancingPeer.IsSimulationEnabled = false;
				}
				SupportClass.StopAllBackgroundCalls();
			}
		}

		public void OnApplicationQuit()
		{
			ConnectionHandler.AppQuits = true;
		}

		public void OnApplicationPause(bool pause)
		{
			ConnectionHandler.AppPause = pause;
			if (pause)
			{
				ConnectionHandler.AppPauseRecent = true;
				base.CancelInvoke("ResetAppPauseRecent");
			}
			else
			{
				base.Invoke("ResetAppPauseRecent", 5f);
			}
		}

		private void ResetAppPauseRecent()
		{
			ConnectionHandler.AppPauseRecent = false;
		}

		public void OnApplicationFocus(bool focus)
		{
			ConnectionHandler.AppOutOfFocus = !focus;
			bool flag = !focus;
			if (flag)
			{
				ConnectionHandler.AppOutOfFocusRecent = true;
				base.CancelInvoke("ResetAppOutOfFocusRecent");
			}
			else
			{
				base.Invoke("ResetAppOutOfFocusRecent", 5f);
			}
		}

		private void ResetAppOutOfFocusRecent()
		{
			ConnectionHandler.AppOutOfFocusRecent = false;
		}

		public static bool IsNetworkReachableUnity()
		{
			return Application.internetReachability > NetworkReachability.NotReachable;
		}

		public void StartFallbackSendAckThread()
		{
			bool isUNITY_WEBGL = RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
			if (isUNITY_WEBGL)
			{
				bool flag = !this.FallbackThreadRunning;
				if (flag)
				{
					base.InvokeRepeating("RealtimeFallbackInvoke", 0.05f, 0.05f);
				}
			}
			else
			{
				bool flag2 = this.stateTimer != null;
				if (flag2)
				{
					return;
				}
				this.stateTimer = new Timer(new TimerCallback(this.RealtimeFallback), null, 50, 50);
			}
			this.FallbackThreadRunning = true;
		}

		public void StopFallbackSendAckThread()
		{
			bool isUNITY_WEBGL = RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
			if (isUNITY_WEBGL)
			{
				bool fallbackThreadRunning = this.FallbackThreadRunning;
				if (fallbackThreadRunning)
				{
					base.CancelInvoke("RealtimeFallbackInvoke");
				}
			}
			else
			{
				bool flag = this.stateTimer != null;
				if (flag)
				{
					this.stateTimer.Dispose();
					this.stateTimer = null;
				}
			}
			this.FallbackThreadRunning = false;
		}

		public void RealtimeFallbackInvoke()
		{
			this.RealtimeFallback(null);
		}

		public void RealtimeFallback(object state = null)
		{
			bool flag = this.Client == null;
			if (!flag)
			{
				bool flag2 = this.Client.IsConnected && this.Client.LoadBalancingPeer.ConnectionTime - this.Client.LoadBalancingPeer.LastSendOutgoingTime > 100;
				if (flag2)
				{
					bool flag3 = !this.didSendAcks;
					if (flag3)
					{
						this.backgroundStopwatch.Reset();
						this.backgroundStopwatch.Start();
					}
					bool flag4 = this.backgroundStopwatch.ElapsedMilliseconds > (long)this.KeepAliveInBackground;
					if (flag4)
					{
						bool flag5 = this.DisconnectAfterKeepAlive && this.Client.State != ClientState.Disconnecting;
						if (flag5)
						{
							this.Client.Disconnect();
						}
					}
					else
					{
						this.didSendAcks = true;
						int countSendAcksOnly = this.CountSendAcksOnly;
						this.CountSendAcksOnly = countSendAcksOnly + 1;
						this.Client.LoadBalancingPeer.SendAcksOnly();
					}
				}
				else
				{
					bool isRunning = this.backgroundStopwatch.IsRunning;
					if (isRunning)
					{
						this.backgroundStopwatch.Reset();
					}
					this.didSendAcks = false;
				}
			}
		}

		public bool DisconnectAfterKeepAlive = false;

		public int KeepAliveInBackground = 60000;

		public bool ApplyDontDestroyOnLoad = true;

		[NonSerialized]
		public static bool AppQuits;

		[NonSerialized]
		public static bool AppPause;

		[NonSerialized]
		public static bool AppPauseRecent;

		[NonSerialized]
		public static bool AppOutOfFocus;

		[NonSerialized]
		public static bool AppOutOfFocusRecent;

		private bool didSendAcks;

		private readonly Stopwatch backgroundStopwatch = new Stopwatch();

		private Timer stateTimer;
	}
}
