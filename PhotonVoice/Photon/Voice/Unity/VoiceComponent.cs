using System;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Voice.Unity
{
	[HelpURL("https://doc.photonengine.com/en-us/voice/v2")]
	public abstract class VoiceComponent : MonoBehaviour, ILoggableDependent, ILoggable
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

		public static string CurrentPlatform
		{
			get
			{
				if (string.IsNullOrEmpty(VoiceComponent.currentPlatform))
				{
					VoiceComponent.currentPlatform = Enum.GetName(typeof(RuntimePlatform), Application.platform);
				}
				return VoiceComponent.currentPlatform;
			}
		}

		protected virtual void Awake()
		{
			if (this.logger == null)
			{
				this.logger = new VoiceLogger(this, string.Format("{0}.{1}", base.name, base.GetType().Name), this.logLevel);
			}
		}

		private VoiceLogger logger;

		[SerializeField]
		protected DebugLevel logLevel = DebugLevel.WARNING;

		[SerializeField]
		[HideInInspector]
		private bool ignoreGlobalLogLevel;

		private static string currentPlatform;
	}
}
