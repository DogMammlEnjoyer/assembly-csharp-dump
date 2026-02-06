using System;
using System.Globalization;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Voice.Unity
{
	public class VoiceLogger : ILogger
	{
		public VoiceLogger(Object context, string tag, DebugLevel level = DebugLevel.ERROR)
		{
			this.context = context;
			this.Tag = tag;
			this.LogLevel = level;
		}

		public VoiceLogger(string tag, DebugLevel level = DebugLevel.ERROR)
		{
			this.Tag = tag;
			this.LogLevel = level;
		}

		public string Tag { get; set; }

		public DebugLevel LogLevel { get; set; }

		public bool IsErrorEnabled
		{
			get
			{
				return this.LogLevel >= DebugLevel.ERROR;
			}
		}

		public bool IsWarningEnabled
		{
			get
			{
				return this.LogLevel >= DebugLevel.WARNING;
			}
		}

		public bool IsInfoEnabled
		{
			get
			{
				return this.LogLevel >= DebugLevel.INFO;
			}
		}

		public bool IsDebugEnabled
		{
			get
			{
				return this.LogLevel == DebugLevel.ALL;
			}
		}

		public void LogError(string fmt, params object[] args)
		{
			if (!this.IsErrorEnabled)
			{
				return;
			}
			fmt = this.GetFormatString(fmt);
			if (this.context == null)
			{
				Debug.LogErrorFormat(fmt, args);
				return;
			}
			Debug.LogErrorFormat(this.context, fmt, args);
		}

		public void LogWarning(string fmt, params object[] args)
		{
			if (!this.IsWarningEnabled)
			{
				return;
			}
			fmt = this.GetFormatString(fmt);
			if (this.context == null)
			{
				Debug.LogWarningFormat(fmt, args);
				return;
			}
			Debug.LogWarningFormat(this.context, fmt, args);
		}

		public void LogInfo(string fmt, params object[] args)
		{
			if (!this.IsInfoEnabled)
			{
				return;
			}
			fmt = this.GetFormatString(fmt);
			if (this.context == null)
			{
				Debug.LogFormat(fmt, args);
				return;
			}
			Debug.LogFormat(this.context, fmt, args);
		}

		public void LogDebug(string fmt, params object[] args)
		{
			if (!this.IsDebugEnabled)
			{
				return;
			}
			this.LogInfo(fmt, args);
		}

		private string GetFormatString(string fmt)
		{
			return string.Format("[{0}] {1}:{2}", this.Tag, this.GetTimestamp(), fmt);
		}

		private string GetTimestamp()
		{
			return DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", new CultureInfo("en-US"));
		}

		private readonly Object context;
	}
}
