using System;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine;

namespace Photon.Voice.PUN
{
	[AddComponentMenu("Photon Voice/Photon Voice View")]
	[RequireComponent(typeof(PhotonView))]
	[HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/voice-for-pun")]
	public class PhotonVoiceView : VoiceComponent
	{
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
				if (this.IsPhotonViewReady && base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("No need to set Recorder as the PhotonView does not belong to local player", Array.Empty<object>());
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
				if (this.IsPhotonViewReady && base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Speaker not set because the PhotonView does not belong to a remote player or SetupDebugSpeaker is disabled", Array.Empty<object>());
				}
			}
		}

		public bool IsSetup
		{
			get
			{
				return this.IsPhotonViewReady && (!this.RequiresRecorder || this.IsRecorder) && (!this.RequiresSpeaker || this.IsSpeaker);
			}
		}

		public bool IsSpeaker { get; private set; }

		public bool IsSpeaking
		{
			get
			{
				return this.IsSpeaker && this.SpeakerInUse.IsPlaying;
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

		public bool IsPhotonViewReady
		{
			get
			{
				return this.photonView != null && this.photonView && this.photonView.ViewID > 0;
			}
		}

		internal bool RequiresSpeaker
		{
			get
			{
				return this.SetupDebugSpeaker || (this.IsPhotonViewReady && !this.photonView.IsMine);
			}
		}

		internal bool RequiresRecorder
		{
			get
			{
				return this.IsPhotonViewReady && this.photonView.IsMine;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.photonView = base.GetComponent<PhotonView>();
			this.Init();
		}

		private void OnEnable()
		{
			if (this.onEnableCalledOnce)
			{
				this.Init();
				return;
			}
			this.onEnableCalledOnce = true;
		}

		private void Start()
		{
			this.Init();
		}

		private void CheckLateLinking()
		{
			if (PhotonVoiceNetwork.Instance.Client.InRoom)
			{
				if (this.IsSpeaker)
				{
					if (!this.IsSpeakerLinked)
					{
						PhotonVoiceNetwork.Instance.CheckLateLinking(this.SpeakerInUse, this.photonView.ViewID);
						return;
					}
					if (base.Logger.IsDebugEnabled)
					{
						base.Logger.LogDebug("Speaker already linked", Array.Empty<object>());
						return;
					}
				}
				else if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("PhotonVoiceView does not have a Speaker and may not need late linking check", Array.Empty<object>());
					return;
				}
			}
			else if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Voice client is still not in a room, skipping late linking check", Array.Empty<object>());
			}
		}

		internal void Setup()
		{
			if (this.IsSetup)
			{
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("PhotonVoiceView already setup", Array.Empty<object>());
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
					if (PhotonVoiceNetwork.Instance.PrimaryRecorder != null && PhotonVoiceNetwork.Instance.PrimaryRecorder)
					{
						this.recorderInUse = PhotonVoiceNetwork.Instance.PrimaryRecorder;
						return this.SetupRecorder(this.recorderInUse);
					}
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("PrimaryRecorder is not set.", Array.Empty<object>());
					}
				}
				Recorder[] componentsInChildren = base.GetComponentsInChildren<Recorder>();
				if (componentsInChildren.Length != 0)
				{
					Recorder recorder = componentsInChildren[0];
					if (componentsInChildren.Length > 1 && base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Multiple Recorder components found attached to the GameObject or its children.", Array.Empty<object>());
					}
					if (recorder != null && recorder)
					{
						this.recorderInUse = recorder;
						return this.SetupRecorder(this.recorderInUse);
					}
				}
				if (!this.AutoCreateRecorderIfNotFound)
				{
					if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("No Recorder found to be setup.", Array.Empty<object>());
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
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot setup a null Recorder.", Array.Empty<object>());
				}
				return false;
			}
			if (!recorder)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot setup a destroyed Recorder.", Array.Empty<object>());
				}
				return false;
			}
			if (!this.IsPhotonViewReady)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Recorder setup cannot be done before assigning a valid ViewID to the PhotonView attached to the same GameObject as the PhotonVoiceView.", Array.Empty<object>());
				}
				return false;
			}
			recorder.UserData = this.photonView.ViewID;
			if (!recorder.IsInitialized)
			{
				this.RecorderInUse.Init(PhotonVoiceNetwork.Instance);
			}
			if (recorder.RequiresRestart)
			{
				recorder.RestartRecording(false);
			}
			return recorder.IsInitialized && recorder.UserData is int && this.photonView.ViewID == (int)recorder.UserData;
		}

		private bool SetupSpeaker()
		{
			if (this.speakerInUse == null)
			{
				Speaker[] componentsInChildren = base.GetComponentsInChildren<Speaker>(true);
				if (componentsInChildren.Length != 0)
				{
					this.speakerInUse = componentsInChildren[0];
					if (componentsInChildren.Length > 1 && base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Multiple Speaker components found attached to the GameObject or its children. Using the first one we found.", Array.Empty<object>());
					}
				}
				if (this.speakerInUse == null)
				{
					bool flag = false;
					if (PhotonVoiceNetwork.Instance.SpeakerPrefab != null)
					{
						GameObject gameObject = Object.Instantiate<GameObject>(PhotonVoiceNetwork.Instance.SpeakerPrefab, base.transform, false);
						componentsInChildren = gameObject.GetComponentsInChildren<Speaker>(true);
						if (componentsInChildren.Length != 0)
						{
							this.speakerInUse = componentsInChildren[0];
							if (componentsInChildren.Length > 1 && base.Logger.IsWarningEnabled)
							{
								base.Logger.LogWarning("Multiple Speaker components found attached to the GameObject (PhotonVoiceNetwork.SpeakerPrefab) or its children. Using the first one we found.", Array.Empty<object>());
							}
						}
						if (this.speakerInUse == null)
						{
							if (base.Logger.IsErrorEnabled)
							{
								base.Logger.LogError("SpeakerPrefab does not have a component of type Speaker in its hierarchy.", Array.Empty<object>());
							}
							Object.Destroy(gameObject);
						}
						else
						{
							flag = true;
						}
					}
					if (!flag)
					{
						if (!PhotonVoiceNetwork.Instance.AutoCreateSpeakerIfNotFound)
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
			if (speaker == null)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot setup a null Speaker", Array.Empty<object>());
				}
				return false;
			}
			if (!speaker)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot setup a destroyed Speaker", Array.Empty<object>());
				}
				return false;
			}
			AudioSource component = speaker.GetComponent<AudioSource>();
			if (component == null)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Unexpected (null?): no AudioSource found attached to the same GameObject as the Speaker component", Array.Empty<object>());
				}
				return false;
			}
			if (!component)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Unexpected (destroyed?): no AudioSource found attached to the same GameObject as the Speaker component", Array.Empty<object>());
				}
				return false;
			}
			if (component.mute && base.Logger.IsWarningEnabled)
			{
				base.Logger.LogWarning("audioSource.mute is true, playback may not work properly", Array.Empty<object>());
			}
			if (component.volume <= 0f && base.Logger.IsWarningEnabled)
			{
				base.Logger.LogWarning("audioSource.volume is zero, playback may not work properly", Array.Empty<object>());
			}
			if (!component.enabled && base.Logger.IsWarningEnabled)
			{
				base.Logger.LogWarning("audioSource.enabled is false, playback may not work properly", Array.Empty<object>());
			}
			return true;
		}

		internal void SetupRecorderInUse()
		{
			if (this.IsRecorder)
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Recorder already setup", Array.Empty<object>());
				}
				return;
			}
			if (!this.RequiresRecorder)
			{
				if (this.IsPhotonViewReady && base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Recorder not needed", Array.Empty<object>());
				}
				return;
			}
			this.IsRecorder = this.SetupRecorder();
			if (!this.IsRecorder)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Recorder not setup for PhotonVoiceView: playback may not work properly.", Array.Empty<object>());
					return;
				}
			}
			else
			{
				if (!this.RecorderInUse.IsRecording && !this.RecorderInUse.AutoStart && base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("PhotonVoiceView.RecorderInUse.AutoStart is false, don't forget to start recording manually using recorder.StartRecording() or recorder.IsRecording = true.", Array.Empty<object>());
				}
				if (!this.RecorderInUse.TransmitEnabled && base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("PhotonVoiceView.RecorderInUse.TransmitEnabled is false, don't forget to set it to true to enable transmission.", Array.Empty<object>());
				}
				if (!this.RecorderInUse.isActiveAndEnabled && this.RecorderInUse.RecordOnlyWhenEnabled && base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("PhotonVoiceView.RecorderInUse may not work properly as RecordOnlyWhenEnabled is set to true and recorder is disabled or attached to an inactive GameObject.", Array.Empty<object>());
				}
			}
		}

		internal void SetupSpeakerInUse()
		{
			if (this.IsSpeaker)
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Speaker already setup", Array.Empty<object>());
				}
				return;
			}
			if (!this.RequiresSpeaker)
			{
				if (this.IsPhotonViewReady && base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Speaker not needed", Array.Empty<object>());
				}
				return;
			}
			this.IsSpeaker = this.SetupSpeaker();
			if (!this.IsSpeaker)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Speaker not setup for PhotonVoiceView: voice chat will not work.", Array.Empty<object>());
					return;
				}
			}
			else
			{
				this.CheckLateLinking();
			}
		}

		public void Init()
		{
			if (this.IsPhotonViewReady)
			{
				this.Setup();
				this.CheckLateLinking();
				return;
			}
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Tried to initialize PhotonVoiceView but PhotonView does not have a valid allocated ViewID yet.", Array.Empty<object>());
			}
		}

		private PhotonView photonView;

		[SerializeField]
		private Recorder recorderInUse;

		[SerializeField]
		private Speaker speakerInUse;

		private bool onEnableCalledOnce;

		public bool AutoCreateRecorderIfNotFound;

		public bool UsePrimaryRecorder;

		public bool SetupDebugSpeaker;
	}
}
