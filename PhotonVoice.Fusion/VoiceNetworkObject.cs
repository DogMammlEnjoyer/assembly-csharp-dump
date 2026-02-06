using System;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Voice.Unity;
using UnityEngine;

namespace Photon.Voice.Fusion
{
	[NetworkBehaviourWeaved(0)]
	public class VoiceNetworkObject : NetworkBehaviour, ILoggableDependent, ILoggable
	{
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

		public bool IgnoreGlobalLogLevel
		{
			get
			{
				return this.ignoreGlobalLogLevel;
			}
			set
			{
				this.ignoreGlobalLogLevel = value;
			}
		}

		public Recorder RecorderInUse
		{
			get
			{
				return this.recorderInUse;
			}
			set
			{
				if (value != this.recorderInUse)
				{
					this.recorderInUse = value;
					this.IsRecorder = false;
				}
				if (this.RequiresRecorder)
				{
					this.SetupRecorderInUse();
					return;
				}
				if (this.IsNetworkObjectReady && this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("No need to set Recorder as this is a remote NetworkObject.", Array.Empty<object>());
				}
			}
		}

		public Speaker SpeakerInUse
		{
			get
			{
				return this.speakerInUse;
			}
			set
			{
				if (this.speakerInUse != value)
				{
					this.speakerInUse = value;
					this.IsSpeaker = false;
				}
				if (this.RequiresSpeaker)
				{
					this.SetupSpeakerInUse();
					return;
				}
				if (this.IsNetworkObjectReady && this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Speaker not set because this is a local NetworkObject and SetupDebugSpeaker is disabled.", Array.Empty<object>());
				}
			}
		}

		public bool IsSetup
		{
			get
			{
				return this.IsNetworkObjectReady && (!this.RequiresRecorder || this.IsRecorder) && (!this.RequiresSpeaker || this.IsSpeaker);
			}
		}

		public bool IsSpeaker { get; private set; }

		public bool IsSpeaking
		{
			get
			{
				return this.SpeakerInUse.IsPlaying;
			}
		}

		public bool IsRecorder { get; private set; }

		public bool IsRecording
		{
			get
			{
				return this.IsRecorder && this.RecorderInUse.IsCurrentlyTransmitting;
			}
		}

		public bool IsSpeakerLinked
		{
			get
			{
				return this.IsSpeaker && this.SpeakerInUse.IsLinked;
			}
		}

		internal bool IsNetworkObjectReady
		{
			get
			{
				return base.Object && base.Object != null && base.Object && base.Object.IsValid;
			}
		}

		internal bool RequiresSpeaker
		{
			get
			{
				return this.IsNetworkObjectReady && this.IsPlayer && (this.SetupDebugSpeaker || !this.IsLocal);
			}
		}

		internal bool RequiresRecorder
		{
			get
			{
				return this.IsNetworkObjectReady && this.IsPlayer && this.IsLocal;
			}
		}

		internal bool IsPlayer
		{
			get
			{
				return base.Runner.IsPlayer;
			}
		}

		internal bool IsLocal
		{
			get
			{
				return base.Object.HasInputAuthority || base.Object.HasStateAuthority;
			}
		}

		internal void Setup()
		{
			if (this.IsSetup)
			{
				if (this.Logger.IsDebugEnabled)
				{
					this.Logger.LogDebug("VoiceNetworkObject already setup", Array.Empty<object>());
				}
				return;
			}
			this.SetupRecorderInUse();
			this.SetupSpeakerInUse();
		}

		private bool SetupRecorder()
		{
			if (this.recorderInUse == null)
			{
				if (this.UsePrimaryRecorder)
				{
					if (this.voiceConnection.PrimaryRecorder != null && this.voiceConnection.PrimaryRecorder)
					{
						this.recorderInUse = this.voiceConnection.PrimaryRecorder;
						return this.SetupRecorder(this.recorderInUse);
					}
					if (this.Logger.IsErrorEnabled)
					{
						this.Logger.LogError("PrimaryRecorder is not set.", Array.Empty<object>());
					}
				}
				Recorder[] componentsInChildren = base.GetComponentsInChildren<Recorder>();
				if (componentsInChildren.Length != 0)
				{
					Recorder recorder = componentsInChildren[0];
					if (componentsInChildren.Length > 1 && this.Logger.IsWarningEnabled)
					{
						this.Logger.LogWarning("Multiple Recorder components found attached to the GameObject or its children.", Array.Empty<object>());
					}
					if (recorder != null && recorder)
					{
						this.recorderInUse = recorder;
						return this.SetupRecorder(this.recorderInUse);
					}
				}
				if (!this.AutoCreateRecorderIfNotFound)
				{
					if (this.Logger.IsWarningEnabled)
					{
						this.Logger.LogWarning("No Recorder found to be setup.", Array.Empty<object>());
					}
					return false;
				}
				this.recorderInUse = base.gameObject.AddComponent<Recorder>();
			}
			return this.SetupRecorder(this.recorderInUse);
		}

		private bool SetupRecorder(Recorder recorder)
		{
			if (recorder == null)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Cannot setup a null Recorder.", Array.Empty<object>());
				}
				return false;
			}
			if (!recorder)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Cannot setup a destroyed Recorder.", Array.Empty<object>());
				}
				return false;
			}
			if (!this.IsNetworkObjectReady)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Recorder setup cannot be done as the NetworkObject is not valid or not ready yet.", Array.Empty<object>());
				}
				return false;
			}
			recorder.UserData = this.GetUserData();
			if (!recorder.IsInitialized)
			{
				this.RecorderInUse.Init(this.voiceConnection);
			}
			if (recorder.RequiresRestart)
			{
				recorder.RestartRecording(false);
			}
			return recorder.IsInitialized;
		}

		private bool SetupSpeaker()
		{
			if (this.speakerInUse == null)
			{
				Speaker[] componentsInChildren = base.GetComponentsInChildren<Speaker>(true);
				if (componentsInChildren.Length != 0)
				{
					this.speakerInUse = componentsInChildren[0];
					if (componentsInChildren.Length > 1 && this.Logger.IsWarningEnabled)
					{
						this.Logger.LogWarning("Multiple Speaker components found attached to the GameObject or its children. Using the first one we found.", Array.Empty<object>());
					}
				}
				if (this.speakerInUse == null)
				{
					bool flag = false;
					if (this.voiceConnection.SpeakerPrefab != null)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.voiceConnection.SpeakerPrefab, base.transform, false);
						componentsInChildren = gameObject.GetComponentsInChildren<Speaker>(true);
						if (componentsInChildren.Length != 0)
						{
							this.speakerInUse = componentsInChildren[0];
							if (componentsInChildren.Length > 1 && this.Logger.IsWarningEnabled)
							{
								this.Logger.LogWarning("Multiple Speaker components found attached to the GameObject (VoiceConnection.SpeakerPrefab) or its children. Using the first one we found.", Array.Empty<object>());
							}
						}
						if (this.speakerInUse == null)
						{
							if (this.Logger.IsErrorEnabled)
							{
								this.Logger.LogError("SpeakerPrefab does not have a component of type Speaker in its hierarchy.", Array.Empty<object>());
							}
							UnityEngine.Object.Destroy(gameObject);
						}
						else
						{
							flag = true;
						}
					}
					if (!flag)
					{
						if (!this.voiceConnection.AutoCreateSpeakerIfNotFound)
						{
							return false;
						}
						this.speakerInUse = base.gameObject.AddComponent<Speaker>();
					}
				}
			}
			return this.SetupSpeaker(this.speakerInUse);
		}

		private bool SetupSpeaker(Speaker speaker)
		{
			if (this.speakerInUse == null)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Cannot setup a null Speaker", Array.Empty<object>());
				}
				return false;
			}
			if (!this.speakerInUse)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Cannot setup a destroyed Speaker", Array.Empty<object>());
				}
				return false;
			}
			AudioSource component = speaker.GetComponent<AudioSource>();
			if (component == null)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Unexpected (null?): no AudioSource found attached to the same GameObject as the Speaker component", Array.Empty<object>());
				}
				return false;
			}
			if (!component)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Unexpected (destroyed?): no AudioSource found attached to the same GameObject as the Speaker component", Array.Empty<object>());
				}
				return false;
			}
			if (component.mute && this.Logger.IsWarningEnabled)
			{
				this.Logger.LogWarning("audioSource.mute is true, playback may not work properly", Array.Empty<object>());
			}
			if (component.volume <= 0f && this.Logger.IsWarningEnabled)
			{
				this.Logger.LogWarning("audioSource.volume is zero, playback may not work properly", Array.Empty<object>());
			}
			if (!component.enabled && this.Logger.IsWarningEnabled)
			{
				this.Logger.LogWarning("audioSource.enabled is false, playback may not work properly", Array.Empty<object>());
			}
			return true;
		}

		internal void SetupRecorderInUse()
		{
			if (this.IsRecorder)
			{
				if (this.Logger.IsInfoEnabled)
				{
					this.Logger.LogInfo("Recorder already setup", Array.Empty<object>());
				}
				return;
			}
			if (!this.RequiresRecorder)
			{
				if (this.IsNetworkObjectReady && this.Logger.IsInfoEnabled)
				{
					this.Logger.LogInfo("Recorder not needed", Array.Empty<object>());
				}
				return;
			}
			this.IsRecorder = this.SetupRecorder();
			if (!this.IsRecorder)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Recorder not setup for VoiceNetworkObject: playback may not work properly.", Array.Empty<object>());
					return;
				}
			}
			else
			{
				if (!this.RecorderInUse.IsRecording && !this.RecorderInUse.AutoStart && this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("VoiceNetworkObject.RecorderInUse.AutoStart is false, don't forget to start recording manually using recorder.StartRecording() or recorder.IsRecording = true.", Array.Empty<object>());
				}
				if (!this.RecorderInUse.TransmitEnabled && this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("VoiceNetworkObject.RecorderInUse.TransmitEnabled is false, don't forget to set it to true to enable transmission.", Array.Empty<object>());
				}
				if (!this.RecorderInUse.isActiveAndEnabled && this.RecorderInUse.RecordOnlyWhenEnabled && this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("VoiceNetworkObject.RecorderInUse may not work properly as RecordOnlyWhenEnabled is set to true and recorder is disabled or attached to an inactive GameObject.", Array.Empty<object>());
				}
			}
		}

		internal void SetupSpeakerInUse()
		{
			if (this.IsSpeaker)
			{
				if (this.Logger.IsInfoEnabled)
				{
					this.Logger.LogInfo("Speaker already setup", Array.Empty<object>());
				}
				return;
			}
			if (!this.RequiresSpeaker)
			{
				if (this.IsNetworkObjectReady && this.Logger.IsInfoEnabled)
				{
					this.Logger.LogInfo("Speaker not needed", Array.Empty<object>());
				}
				return;
			}
			this.IsSpeaker = this.SetupSpeaker();
			if (!this.IsSpeaker)
			{
				if (this.Logger.IsWarningEnabled)
				{
					this.Logger.LogWarning("Speaker not setup for VoiceNetworkObject: voice chat will not work.", Array.Empty<object>());
					return;
				}
			}
			else
			{
				this.CheckLateLinking();
			}
		}

		private object GetUserData()
		{
			return base.Object.Id;
		}

		private void CheckLateLinking()
		{
			if (this.voiceConnection.Client.InRoom)
			{
				if (this.IsSpeaker)
				{
					if (!this.IsSpeakerLinked)
					{
						if (this.voiceConnection.TryLateLinkingUsingUserData(this.SpeakerInUse, this.GetUserData()))
						{
							if (this.Logger.IsDebugEnabled)
							{
								this.Logger.LogDebug("Late linking attempt succeeded.", Array.Empty<object>());
								return;
							}
						}
						else if (this.Logger.IsDebugEnabled)
						{
							this.Logger.LogDebug("Late linking attempt failed.", Array.Empty<object>());
							return;
						}
					}
					else if (this.Logger.IsDebugEnabled)
					{
						this.Logger.LogDebug("Speaker already linked", Array.Empty<object>());
						return;
					}
				}
				else if (this.Logger.IsDebugEnabled)
				{
					this.Logger.LogDebug("VoiceNetworkObject does not have a Speaker and may not need late linking check", Array.Empty<object>());
					return;
				}
			}
			else if (this.Logger.IsDebugEnabled)
			{
				this.Logger.LogDebug("Voice client is still not in a room, skipping late linking check", Array.Empty<object>());
			}
		}

		public override void Spawned()
		{
			this.voiceConnection = base.Runner.GetComponent<VoiceConnection>();
			this.Setup();
		}

		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
		}

		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
		}

		private VoiceConnection voiceConnection;

		[SerializeField]
		private Speaker speakerInUse;

		[SerializeField]
		private Recorder recorderInUse;

		[SerializeField]
		protected DebugLevel logLevel = DebugLevel.ERROR;

		private VoiceLogger logger;

		[SerializeField]
		[HideInInspector]
		private bool ignoreGlobalLogLevel;

		public bool AutoCreateRecorderIfNotFound;

		public bool UsePrimaryRecorder;

		public bool SetupDebugSpeaker;
	}
}
