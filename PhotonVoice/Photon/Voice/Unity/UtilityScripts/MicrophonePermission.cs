using System;
using UnityEngine;

namespace Photon.Voice.Unity.UtilityScripts
{
	[RequireComponent(typeof(Recorder))]
	public class MicrophonePermission : VoiceComponent
	{
		public static event Action<bool> MicrophonePermissionCallback;

		public bool HasPermission
		{
			get
			{
				return this.hasPermission;
			}
			private set
			{
				base.Logger.LogInfo("Microphone Permission Granted: {0}", new object[]
				{
					value
				});
				Action<bool> microphonePermissionCallback = MicrophonePermission.MicrophonePermissionCallback;
				if (microphonePermissionCallback != null)
				{
					microphonePermissionCallback(value);
				}
				if (this.hasPermission != value)
				{
					this.hasPermission = value;
					if (this.hasPermission)
					{
						this.recorder.AutoStart = this.autoStart;
					}
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.recorder = base.GetComponent<Recorder>();
			this.recorder.AutoStart = false;
			this.InitVoice();
		}

		public void InitVoice()
		{
			this.HasPermission = true;
		}

		private Recorder recorder;

		private bool hasPermission;

		[SerializeField]
		private bool autoStart = true;
	}
}
