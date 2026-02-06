using System;
using System.Diagnostics;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Realtime
{
	public class ConnectionHandler : MonoBehaviour
	{
		public LoadBalancingClient Client { get; set; }

		public int CountSendAcksOnly { get; private set; }

		public bool FallbackThreadRunning
		{
			get
			{
				return this.fallbackThreadId < byte.MaxValue;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void StaticReset()
		{
			ConnectionHandler.AppQuits = false;
		}

		protected void OnApplicationQuit()
		{
			ConnectionHandler.AppQuits = true;
		}

		protected virtual void Awake()
		{
			if (this.ApplyDontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		protected virtual void OnDisable()
		{
			this.StopFallbackSendAckThread();
			if (ConnectionHandler.AppQuits)
			{
				if (this.Client != null && this.Client.IsConnected)
				{
					this.Client.Disconnect(DisconnectCause.DisconnectByClientLogic);
					this.Client.LoadBalancingPeer.StopThread();
				}
				SupportClass.StopAllBackgroundCalls();
			}
		}

		public void StartFallbackSendAckThread()
		{
			if (this.FallbackThreadRunning)
			{
				return;
			}
			this.fallbackThreadId = SupportClass.StartBackgroundCalls(new Func<bool>(this.RealtimeFallbackThread), 50, "RealtimeFallbackThread");
		}

		public void StopFallbackSendAckThread()
		{
			if (!this.FallbackThreadRunning)
			{
				return;
			}
			SupportClass.StopBackgroundCalls(this.fallbackThreadId);
			this.fallbackThreadId = byte.MaxValue;
		}

		public bool RealtimeFallbackThread()
		{
			if (this.Client != null)
			{
				if (!this.Client.IsConnected)
				{
					this.didSendAcks = false;
					return true;
				}
				if (this.Client.LoadBalancingPeer.ConnectionTime - this.Client.LoadBalancingPeer.LastSendOutgoingTime > 100)
				{
					if (!this.didSendAcks)
					{
						this.backgroundStopwatch.Reset();
						this.backgroundStopwatch.Start();
					}
					if (this.backgroundStopwatch.ElapsedMilliseconds > (long)this.KeepAliveInBackground)
					{
						if (this.DisconnectAfterKeepAlive)
						{
							this.Client.Disconnect(DisconnectCause.DisconnectByClientLogic);
						}
						return true;
					}
					this.didSendAcks = true;
					int countSendAcksOnly = this.CountSendAcksOnly;
					this.CountSendAcksOnly = countSendAcksOnly + 1;
					this.Client.LoadBalancingPeer.SendAcksOnly();
				}
				else
				{
					this.didSendAcks = false;
				}
			}
			return true;
		}

		public bool DisconnectAfterKeepAlive;

		public int KeepAliveInBackground = 60000;

		public bool ApplyDontDestroyOnLoad = true;

		[NonSerialized]
		public static bool AppQuits;

		private byte fallbackThreadId = byte.MaxValue;

		private bool didSendAcks;

		private readonly Stopwatch backgroundStopwatch = new Stopwatch();
	}
}
