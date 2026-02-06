using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Photon.Voice.Unity
{
	[AddComponentMenu("Photon Voice/Voice Connection")]
	[DisallowMultipleComponent]
	[HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/voice-intro")]
	public class VoiceConnection : ConnectionHandler, ILoggable
	{
		public event Action<Speaker> SpeakerLinked;

		public event Action<RemoteVoiceLink> RemoteVoiceAdded;

		public VoiceLogger Logger
		{
			get
			{
				if (this.logger == null)
				{
					this.logger = new VoiceLogger(this, string.Format("{0}.{1}", base.name, base.GetType().Name), this.logLevel);
				}
				return this.logger;
			}
			protected set
			{
				this.logger = value;
			}
		}

		public DebugLevel LogLevel
		{
			get
			{
				if (this.Logger != null)
				{
					this.logLevel = this.Logger.LogLevel;
				}
				return this.logLevel;
			}
			set
			{
				this.logLevel = value;
				if (this.Logger == null)
				{
					return;
				}
				this.Logger.LogLevel = this.logLevel;
			}
		}

		public new LoadBalancingTransport Client
		{
			get
			{
				if (this.client == null)
				{
					this.client = new LoadBalancingTransport2(this.Logger, ConnectionProtocol.Udp);
					this.client.ClientType = ClientAppType.Voice;
					VoiceClient voiceClient = this.client.VoiceClient;
					voiceClient.OnRemoteVoiceInfoAction = (VoiceClient.RemoteVoiceInfoDelegate)Delegate.Combine(voiceClient.OnRemoteVoiceInfoAction, new VoiceClient.RemoteVoiceInfoDelegate(this.OnRemoteVoiceInfo));
					this.client.StateChanged += this.OnVoiceStateChanged;
					this.client.OpResponseReceived += this.OnOperationResponseReceived;
					base.Client = this.client;
					base.StartFallbackSendAckThread();
				}
				return this.client;
			}
		}

		public VoiceClient VoiceClient
		{
			get
			{
				return this.Client.VoiceClient;
			}
		}

		public ClientState ClientState
		{
			get
			{
				return this.Client.State;
			}
		}

		public float FramesReceivedPerSecond { get; private set; }

		public float FramesLostPerSecond { get; private set; }

		public float FramesLostPercent { get; private set; }

		public GameObject SpeakerPrefab
		{
			get
			{
				return this.speakerPrefab;
			}
			set
			{
				if (value != this.speakerPrefab)
				{
					if (value != null && value)
					{
						Speaker componentInChildren = value.GetComponentInChildren<Speaker>(true);
						if (componentInChildren == null || !componentInChildren)
						{
							if (this.Logger.IsErrorEnabled)
							{
								this.Logger.LogError("SpeakerPrefab must have a component of type Speaker in its hierarchy.", Array.Empty<object>());
							}
							return;
						}
					}
					this.speakerPrefab = value;
				}
			}
		}

		public Recorder PrimaryRecorder
		{
			get
			{
				if (!this.primaryRecorderInitialized)
				{
					this.TryInitializePrimaryRecorder();
				}
				return this.primaryRecorder;
			}
			set
			{
				this.primaryRecorder = value;
				this.primaryRecorderInitialized = false;
				this.TryInitializePrimaryRecorder();
			}
		}

		public DebugLevel GlobalRecordersLogLevel
		{
			get
			{
				return this.globalRecordersLogLevel;
			}
			set
			{
				this.globalRecordersLogLevel = value;
				for (int i = 0; i < this.initializedRecorders.Count; i++)
				{
					Recorder recorder = this.initializedRecorders[i];
					if (!recorder.IgnoreGlobalLogLevel)
					{
						recorder.LogLevel = this.globalRecordersLogLevel;
					}
				}
			}
		}

		public DebugLevel GlobalSpeakersLogLevel
		{
			get
			{
				return this.globalSpeakersLogLevel;
			}
			set
			{
				this.globalSpeakersLogLevel = value;
				for (int i = 0; i < this.linkedSpeakers.Count; i++)
				{
					Speaker speaker = this.linkedSpeakers[i];
					if (!speaker.IgnoreGlobalLogLevel)
					{
						speaker.LogLevel = this.globalSpeakersLogLevel;
					}
				}
			}
		}

		[Obsolete("Use SetGlobalPlaybackDelayConfiguration methods instead")]
		public int GlobalPlaybackDelay
		{
			get
			{
				return this.globalPlaybackDelaySettings.MinDelaySoft;
			}
			set
			{
				if (value >= 0 && value <= this.globalPlaybackDelaySettings.MaxDelaySoft)
				{
					this.globalPlaybackDelaySettings.MinDelaySoft = value;
				}
			}
		}

		public string BestRegionSummaryInPreferences
		{
			get
			{
				return PlayerPrefs.GetString("VoiceCloudBestRegion", null);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					PlayerPrefs.DeleteKey("VoiceCloudBestRegion");
					return;
				}
				PlayerPrefs.SetString("VoiceCloudBestRegion", value);
			}
		}

		public int GlobalPlaybackDelayMinSoft
		{
			get
			{
				return this.globalPlaybackDelaySettings.MinDelaySoft;
			}
		}

		public int GlobalPlaybackDelayMaxSoft
		{
			get
			{
				return this.globalPlaybackDelaySettings.MaxDelaySoft;
			}
		}

		public int GlobalPlaybackDelayMaxHard
		{
			get
			{
				return this.globalPlaybackDelaySettings.MaxDelayHard;
			}
		}

		public bool ConnectUsingSettings(AppSettings overwriteSettings = null)
		{
			if (this.Client.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: {0}", new object[]
					{
						this.Client.LoadBalancingPeer.PeerState
					});
				}
				return false;
			}
			if (ConnectionHandler.AppQuits)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Can't connect: Application is closing. Unity called OnApplicationQuit().", Array.Empty<object>());
				}
				return false;
			}
			if (overwriteSettings != null)
			{
				this.Settings = overwriteSettings;
			}
			if (this.Settings == null)
			{
				if (this.Logger.IsErrorEnabled)
				{
					this.Logger.LogError("Settings are null", Array.Empty<object>());
				}
				return false;
			}
			if (string.IsNullOrEmpty(this.Settings.AppIdVoice) && string.IsNullOrEmpty(this.Settings.Server))
			{
				if (this.Logger.IsErrorEnabled)
				{
					this.Logger.LogError("Provide an AppId or a Server address in Settings to be able to connect", Array.Empty<object>());
				}
				return false;
			}
			if (this.Settings.IsMasterServerAddress && string.IsNullOrEmpty(this.Client.UserId))
			{
				this.Client.UserId = Guid.NewGuid().ToString();
			}
			if (string.IsNullOrEmpty(this.Settings.BestRegionSummaryFromStorage))
			{
				this.Settings.BestRegionSummaryFromStorage = this.BestRegionSummaryInPreferences;
			}
			return this.client.ConnectUsingSettings(this.Settings);
		}

		public void InitRecorder(Recorder rec)
		{
			if (rec == null)
			{
				if (this.Logger.IsErrorEnabled)
				{
					this.Logger.LogError("rec is null.", Array.Empty<object>());
				}
				return;
			}
			if (!rec)
			{
				if (this.Logger.IsErrorEnabled)
				{
					this.Logger.LogError("rec is destroyed.", Array.Empty<object>());
				}
				return;
			}
			rec.Init(this);
		}

		public void SetPlaybackDelaySettings(PlaybackDelaySettings gpds)
		{
			this.SetGlobalPlaybackDelaySettings(gpds.MinDelaySoft, gpds.MaxDelaySoft, gpds.MaxDelayHard);
		}

		public void SetGlobalPlaybackDelaySettings(int low, int high, int max)
		{
			if (low >= 0 && low < high)
			{
				if (max < high)
				{
					max = high;
				}
				this.globalPlaybackDelaySettings.MinDelaySoft = low;
				this.globalPlaybackDelaySettings.MaxDelaySoft = high;
				this.globalPlaybackDelaySettings.MaxDelayHard = max;
				for (int i = 0; i < this.linkedSpeakers.Count; i++)
				{
					this.linkedSpeakers[i].SetPlaybackDelaySettings(this.globalPlaybackDelaySettings);
				}
				return;
			}
			if (this.Logger.IsErrorEnabled)
			{
				this.Logger.LogError("Wrong playback delay config values, make sure 0 <= Low < High, low={0}, high={1}, max={2}", new object[]
				{
					low,
					high,
					max
				});
			}
		}

		public virtual bool TryLateLinkingUsingUserData(Speaker speaker, object userData)
		{
			if (speaker == null || !speaker)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Speaker is null or destroyed.", Array.Empty<object>());
				}
				return false;
			}
			if (speaker.IsLinked)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Speaker already linked.", Array.Empty<object>());
				}
				return false;
			}
			if (!this.Client.InRoom)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Client not joined to a voice room, client state: {0}.", new object[]
					{
						Enum.GetName(typeof(ClientState), this.ClientState)
					});
				}
				return false;
			}
			RemoteVoiceLink remoteVoiceLink;
			if (this.TryGetFirstVoiceStreamByUserData(userData, out remoteVoiceLink))
			{
				if (this.Logger.IsInfoEnabled)
				{
					this.Logger.LogInfo("Speaker 'late-linking' for remoteVoice {0}.", new object[]
					{
						remoteVoiceLink
					});
				}
				this.LinkSpeaker(speaker, remoteVoiceLink);
				return speaker.IsLinked;
			}
			return false;
		}

		protected override void Awake()
		{
			base.Awake();
			if (this.enableSupportLogger)
			{
				this.supportLoggerComponent = base.gameObject.AddComponent<SupportLogger>();
				this.supportLoggerComponent.Client = this.Client;
				this.supportLoggerComponent.LogTrafficStats = true;
			}
			if (this.runInBackground)
			{
				Application.runInBackground = this.runInBackground;
			}
			if (!this.primaryRecorderInitialized)
			{
				this.TryInitializePrimaryRecorder();
			}
		}

		protected virtual void Update()
		{
			this.VoiceClient.Service();
			for (int i = 0; i < this.linkedSpeakers.Count; i++)
			{
				this.linkedSpeakers[i].Service();
			}
			for (int j = 0; j < this.initializedRecorders.Count; j++)
			{
				Recorder recorder = this.initializedRecorders[j];
				if (recorder.MicrophoneDeviceChangeDetected)
				{
					recorder.HandleDeviceChange();
				}
			}
		}

		protected virtual void FixedUpdate()
		{
			if (Time.timeScale > this.MinimalTimeScaleToDispatchInFixedUpdate)
			{
				this.Dispatch();
			}
		}

		protected void Dispatch()
		{
			bool flag = true;
			while (flag)
			{
				flag = this.Client.LoadBalancingPeer.DispatchIncomingCommands();
			}
		}

		private void LateUpdate()
		{
			if (Time.timeScale <= this.MinimalTimeScaleToDispatchInFixedUpdate)
			{
				this.Dispatch();
			}
			int num = (int)(Time.realtimeSinceStartup * 1000f);
			if (this.SendAsap || num > this.nextSendTickCount)
			{
				this.SendAsap = false;
				bool flag = true;
				int num2 = 0;
				while (flag && num2 < this.MaxDatagrams)
				{
					flag = this.Client.LoadBalancingPeer.SendOutgoingCommands();
					num2++;
				}
				this.nextSendTickCount = num + this.updateInterval;
			}
			if (num > this.nextStatsTickCount && this.statsResetInterval > 0)
			{
				this.CalcStatistics();
				this.nextStatsTickCount = num + this.statsResetInterval;
			}
		}

		protected override void OnDisable()
		{
			if (ConnectionHandler.AppQuits)
			{
				this.CleanUp();
				SupportClass.StopAllBackgroundCalls();
			}
		}

		protected virtual void OnDestroy()
		{
			this.CleanUp();
		}

		protected virtual Speaker SimpleSpeakerFactory(int playerId, byte voiceId, object userData)
		{
			Speaker speaker = null;
			bool flag = false;
			if (this.SpeakerPrefab != null && this.SpeakerPrefab)
			{
				Speaker[] componentsInChildren = Object.Instantiate<GameObject>(this.SpeakerPrefab).GetComponentsInChildren<Speaker>(true);
				if (componentsInChildren.Length != 0)
				{
					speaker = componentsInChildren[0];
					if (componentsInChildren.Length > 1 && this.Logger.IsWarningEnabled)
					{
						this.Logger.LogWarning("Multiple Speaker components found attached to the GameObject (VoiceConnection.SpeakerPrefab) or its children. Using the first one we found.", Array.Empty<object>());
					}
				}
				if (speaker == null)
				{
					if (this.Logger.IsErrorEnabled)
					{
						this.Logger.LogError("Unexpected: SpeakerPrefab does not have a component of type Speaker in its hierarchy.", Array.Empty<object>());
					}
				}
				else
				{
					flag = true;
				}
			}
			if (!flag)
			{
				if (!this.AutoCreateSpeakerIfNotFound)
				{
					return null;
				}
				if (this.Logger.IsInfoEnabled)
				{
					this.Logger.LogInfo("Auto creating a new Speaker as none found", Array.Empty<object>());
				}
				speaker = new GameObject().AddComponent<Speaker>();
			}
			speaker.Actor = ((this.Client.CurrentRoom != null) ? this.Client.CurrentRoom.GetPlayer(playerId, false) : null);
			speaker.name = ((speaker.Actor != null && !string.IsNullOrEmpty(speaker.Actor.NickName)) ? speaker.Actor.NickName : string.Format("Speaker for Player {0} Voice #{1}", playerId, voiceId));
			Speaker speaker2 = speaker;
			speaker2.OnRemoteVoiceRemoveAction = (Action<Speaker>)Delegate.Combine(speaker2.OnRemoteVoiceRemoveAction, new Action<Speaker>(this.DeleteVoiceOnRemoteVoiceRemove));
			return speaker;
		}

		internal void DeleteVoiceOnRemoteVoiceRemove(Speaker speaker)
		{
			if (speaker != null)
			{
				if (this.Logger.IsInfoEnabled)
				{
					this.Logger.LogInfo("Remote voice removed, delete speaker", Array.Empty<object>());
				}
				Object.Destroy(speaker.gameObject);
			}
		}

		private void OnRemoteVoiceInfo(int channelId, int playerId, byte voiceId, VoiceInfo voiceInfo, ref RemoteVoiceOptions options)
		{
			RemoteVoiceLink remoteVoice = new RemoteVoiceLink(voiceInfo, playerId, (int)voiceId, channelId);
			if (this.RemoteLinkValidator != null && !this.RemoteLinkValidator(remoteVoice))
			{
				return;
			}
			if (voiceInfo.Codec != Codec.AudioOpus)
			{
				if (this.Logger.IsDebugEnabled)
				{
					this.Logger.LogInfo("OnRemoteVoiceInfo skipped as codec is not Opus, {0}", new object[]
					{
						remoteVoice
					});
				}
				return;
			}
			remoteVoice.Init(ref options);
			if (this.Logger.IsInfoEnabled)
			{
				this.Logger.LogInfo("OnRemoteVoiceInfo {0}", new object[]
				{
					remoteVoice
				});
			}
			for (int i = 0; i < this.cachedRemoteVoices.Count; i++)
			{
				RemoteVoiceLink remoteVoiceLink = this.cachedRemoteVoices[i];
				if (remoteVoiceLink.Equals(remoteVoice) && this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Possible duplicate remoteVoiceInfo cached:{0} vs. received:{1}", new object[]
					{
						remoteVoiceLink,
						remoteVoice
					});
				}
			}
			this.cachedRemoteVoices.Add(remoteVoice);
			if (this.RemoteVoiceAdded != null)
			{
				this.RemoteVoiceAdded(remoteVoice);
			}
			remoteVoice.RemoteVoiceRemoved += delegate()
			{
				if (this.Logger.IsInfoEnabled)
				{
					this.Logger.LogInfo("RemoteVoiceRemoved {0}", new object[]
					{
						remoteVoice
					});
				}
				if (!this.cachedRemoteVoices.Remove(remoteVoice) && this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Cached remote voice not removed {0}", new object[]
					{
						remoteVoice
					});
				}
			};
			Speaker speaker = null;
			if (this.SpeakerFactory != null)
			{
				speaker = this.SpeakerFactory(playerId, voiceId, voiceInfo.UserData);
			}
			if (speaker == null)
			{
				speaker = this.SimpleSpeakerFactory(playerId, voiceId, voiceInfo.UserData);
			}
			else if (speaker.IsLinked)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Overriding speaker link, old:{0} new:{1}", new object[]
					{
						speaker.RemoteVoiceLink,
						remoteVoice
					});
				}
				speaker.OnRemoteVoiceRemove();
			}
			this.LinkSpeaker(speaker, remoteVoice);
		}

		protected virtual void OnVoiceStateChanged(ClientState fromState, ClientState toState)
		{
			if (this.Logger.IsDebugEnabled)
			{
				this.Logger.LogDebug("OnVoiceStateChanged from {0} to {1}", new object[]
				{
					fromState,
					toState
				});
			}
			if (fromState == ClientState.Joined)
			{
				this.StopInitializedRecorders();
				this.ClearRemoteVoicesCache();
			}
			if (toState != ClientState.Joined)
			{
				if (toState == ClientState.ConnectedToMasterServer && this.Client.RegionHandler != null)
				{
					if (this.Settings != null)
					{
						this.Settings.BestRegionSummaryFromStorage = this.Client.RegionHandler.SummaryToCache;
					}
					this.BestRegionSummaryInPreferences = this.Client.RegionHandler.SummaryToCache;
					return;
				}
			}
			else
			{
				this.StartInitializedRecorders();
			}
		}

		protected void CalcStatistics()
		{
			float time = Time.time;
			int num = this.VoiceClient.FramesReceived - this.referenceFramesReceived;
			int num2 = this.VoiceClient.FramesLost - this.referenceFramesLost;
			float num3 = time - this.statsReferenceTime;
			if (num3 > 0f)
			{
				if (num + num2 > 0)
				{
					this.FramesReceivedPerSecond = (float)num / num3;
					this.FramesLostPerSecond = (float)num2 / num3;
					this.FramesLostPercent = 100f * (float)num2 / (float)(num + num2);
				}
				else
				{
					this.FramesReceivedPerSecond = 0f;
					this.FramesLostPerSecond = 0f;
					this.FramesLostPercent = 0f;
				}
			}
			this.referenceFramesReceived = this.VoiceClient.FramesReceived;
			this.referenceFramesLost = this.VoiceClient.FramesLost;
			this.statsReferenceTime = time;
		}

		private void CleanUp()
		{
			bool flag = this.client != null;
			if (this.Logger.IsDebugEnabled)
			{
				this.Logger.LogDebug("Client exists? {0}, already cleaned up? {1}", new object[]
				{
					flag,
					this.cleanedUp
				});
			}
			if (this.cleanedUp)
			{
				return;
			}
			base.StopFallbackSendAckThread();
			if (flag)
			{
				this.client.StateChanged -= this.OnVoiceStateChanged;
				this.client.OpResponseReceived -= this.OnOperationResponseReceived;
				this.client.Disconnect(DisconnectCause.DisconnectByClientLogic);
				if (this.client.LoadBalancingPeer != null)
				{
					this.client.LoadBalancingPeer.Disconnect();
					this.client.LoadBalancingPeer.StopThread();
				}
				this.client.Dispose();
			}
			this.cleanedUp = true;
		}

		protected void LinkSpeaker(Speaker speaker, RemoteVoiceLink remoteVoice)
		{
			if (speaker != null)
			{
				if (!speaker.IgnoreGlobalLogLevel)
				{
					speaker.LogLevel = this.GlobalSpeakersLogLevel;
				}
				speaker.SetPlaybackDelaySettings(this.globalPlaybackDelaySettings);
				if (speaker.OnRemoteVoiceInfo(remoteVoice))
				{
					if (speaker.Actor == null)
					{
						if (this.Client.CurrentRoom == null)
						{
							if (this.Logger.IsErrorEnabled)
							{
								this.Logger.LogError("RemoteVoiceInfo event received while CurrentRoom is null", Array.Empty<object>());
							}
						}
						else
						{
							Player player = this.Client.CurrentRoom.GetPlayer(remoteVoice.PlayerId, false);
							if (player == null)
							{
								if (this.Logger.IsErrorEnabled)
								{
									this.Logger.LogError("RemoteVoiceInfo event received while respective actor not found in the room, {0}", new object[]
									{
										remoteVoice
									});
								}
							}
							else
							{
								speaker.Actor = player;
							}
						}
					}
					if (this.Logger.IsInfoEnabled)
					{
						this.Logger.LogInfo("Speaker linked with remote voice {0}", new object[]
						{
							remoteVoice
						});
					}
					this.linkedSpeakers.Add(speaker);
					remoteVoice.RemoteVoiceRemoved += delegate()
					{
						this.linkedSpeakers.Remove(speaker);
					};
					if (this.SpeakerLinked != null)
					{
						this.SpeakerLinked(speaker);
						return;
					}
				}
			}
			else if (this.Logger.IsWarningEnabled)
			{
				this.Logger.LogWarning("Speaker is null. Remote voice {0} not linked.", new object[]
				{
					remoteVoice
				});
			}
		}

		private void ClearRemoteVoicesCache()
		{
			if (this.cachedRemoteVoices.Count > 0)
			{
				if (this.Logger.IsInfoEnabled)
				{
					this.Logger.LogInfo("{0} cached remote voices info cleared", new object[]
					{
						this.cachedRemoteVoices.Count
					});
				}
				this.cachedRemoteVoices.Clear();
			}
		}

		private void TryInitializePrimaryRecorder()
		{
			if (this.primaryRecorder != null)
			{
				if (!this.primaryRecorder.IsInitialized)
				{
					this.primaryRecorder.Init(this);
				}
				this.primaryRecorderInitialized = this.primaryRecorder.IsInitialized;
			}
		}

		internal void AddInitializedRecorder(Recorder rec)
		{
			this.initializedRecorders.Add(rec);
		}

		internal void RemoveInitializedRecorder(Recorder rec)
		{
			this.initializedRecorders.Remove(rec);
		}

		private void StartInitializedRecorders()
		{
			for (int i = 0; i < this.initializedRecorders.Count; i++)
			{
				this.initializedRecorders[i].CheckAndAutoStart();
			}
		}

		private void StopInitializedRecorders()
		{
			for (int i = 0; i < this.initializedRecorders.Count; i++)
			{
				Recorder recorder = this.initializedRecorders[i];
				if (recorder.IsRecording && recorder.RecordOnlyWhenJoined)
				{
					recorder.StopRecordingInternal();
				}
			}
		}

		private bool TryGetFirstVoiceStreamByUserData(object userData, out RemoteVoiceLink remoteVoiceLink)
		{
			remoteVoiceLink = null;
			if (userData == null)
			{
				return false;
			}
			if (this.Logger.IsWarningEnabled)
			{
				int num = 0;
				for (int i = 0; i < this.cachedRemoteVoices.Count; i++)
				{
					RemoteVoiceLink remoteVoiceLink2 = this.cachedRemoteVoices[i];
					if (userData.Equals(remoteVoiceLink2.Info.UserData))
					{
						num++;
						if (num == 1)
						{
							remoteVoiceLink = remoteVoiceLink2;
							if (this.Logger.IsDebugEnabled)
							{
								this.Logger.LogWarning("(first) remote voice stream found by UserData:{0}", new object[]
								{
									userData,
									remoteVoiceLink2
								});
							}
						}
						else
						{
							this.Logger.LogWarning("{0} remote voice stream found (so far) using same UserData:{0}", new object[]
							{
								num,
								remoteVoiceLink2
							});
						}
					}
				}
				return num > 0;
			}
			for (int j = 0; j < this.cachedRemoteVoices.Count; j++)
			{
				RemoteVoiceLink remoteVoiceLink3 = this.cachedRemoteVoices[j];
				if (userData.Equals(remoteVoiceLink3.Info.UserData))
				{
					remoteVoiceLink = remoteVoiceLink3;
					if (this.Logger.IsDebugEnabled)
					{
						this.Logger.LogWarning("(first) remote voice stream found by UserData:{0}", new object[]
						{
							userData,
							remoteVoiceLink3
						});
					}
					return true;
				}
			}
			return false;
		}

		protected virtual void OnOperationResponseReceived(OperationResponse operationResponse)
		{
			if (this.Logger.IsErrorEnabled && operationResponse.ReturnCode != 0 && (operationResponse.OperationCode != 225 || operationResponse.ReturnCode == 32760))
			{
				this.Logger.LogError("Operation {0} response error code {1} message {2}", new object[]
				{
					operationResponse.OperationCode,
					operationResponse.ReturnCode,
					operationResponse.DebugMessage
				});
			}
		}

		private VoiceLogger logger;

		[SerializeField]
		private DebugLevel logLevel = DebugLevel.INFO;

		private const string PlayerPrefsKey = "VoiceCloudBestRegion";

		private LoadBalancingTransport client;

		[SerializeField]
		private bool enableSupportLogger;

		private SupportLogger supportLoggerComponent;

		[SerializeField]
		private int updateInterval = 50;

		private int nextSendTickCount;

		[SerializeField]
		private bool runInBackground = true;

		[SerializeField]
		private int statsResetInterval = 1000;

		private int nextStatsTickCount;

		private float statsReferenceTime;

		private int referenceFramesLost;

		private int referenceFramesReceived;

		[SerializeField]
		private GameObject speakerPrefab;

		private bool cleanedUp;

		protected List<RemoteVoiceLink> cachedRemoteVoices = new List<RemoteVoiceLink>();

		[SerializeField]
		[FormerlySerializedAs("PrimaryRecorder")]
		private Recorder primaryRecorder;

		private bool primaryRecorderInitialized;

		[SerializeField]
		private DebugLevel globalRecordersLogLevel = DebugLevel.INFO;

		[SerializeField]
		private DebugLevel globalSpeakersLogLevel = DebugLevel.INFO;

		[SerializeField]
		[HideInInspector]
		private int globalPlaybackDelay = 200;

		[SerializeField]
		private PlaybackDelaySettings globalPlaybackDelaySettings = new PlaybackDelaySettings
		{
			MinDelaySoft = 200,
			MaxDelaySoft = 400,
			MaxDelayHard = 1000
		};

		private List<Speaker> linkedSpeakers = new List<Speaker>();

		private List<Recorder> initializedRecorders = new List<Recorder>();

		public AppSettings Settings;

		public Func<int, byte, object, Speaker> SpeakerFactory;

		public VoiceConnection.ValidateRemoteLinkDelegate RemoteLinkValidator;

		public float MinimalTimeScaleToDispatchInFixedUpdate = -1f;

		public bool AutoCreateSpeakerIfNotFound = true;

		public int MaxDatagrams = 3;

		public bool SendAsap;

		public delegate bool ValidateRemoteLinkDelegate(RemoteVoiceLink link);
	}
}
