using System;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Fusion.Photon.Realtime
{
	[Serializable]
	public class AppSettings
	{
		public DebugLevel NetworkLogging
		{
			get
			{
				bool flag = InternalLogStreams.LogTraceRealtime != null;
				DebugLevel result;
				if (flag)
				{
					result = DebugLevel.ALL;
				}
				else
				{
					LogLevel level = Log.Settings.Level;
					LogLevel logLevel = level;
					if (logLevel > LogLevel.Warn)
					{
						if (logLevel != LogLevel.Error)
						{
							result = DebugLevel.OFF;
						}
						else
						{
							result = DebugLevel.ERROR;
						}
					}
					else
					{
						result = DebugLevel.WARNING;
					}
				}
				return result;
			}
			set
			{
			}
		}

		public bool IsMasterServerAddress
		{
			get
			{
				return !this.UseNameServer;
			}
		}

		public bool IsBestRegion
		{
			get
			{
				return this.UseNameServer && string.IsNullOrEmpty(this.FixedRegion);
			}
		}

		public bool IsDefaultNameServer
		{
			get
			{
				return this.UseNameServer && string.IsNullOrEmpty(this.Server);
			}
		}

		public bool IsDefaultPort
		{
			get
			{
				return this.Port <= 0;
			}
		}

		public string ToStringFull()
		{
			return string.Format("appId {0}{1}{2}{3}use ns: {4}, reg: {5}, {9}, {6}{7}{8}auth: {10}", new object[]
			{
				string.IsNullOrEmpty(this.AppIdRealtime) ? string.Empty : ("Realtime/PUN: " + this.HideAppId(this.AppIdRealtime) + ", "),
				string.IsNullOrEmpty(this.AppIdFusion) ? string.Empty : ("Fusion: " + this.HideAppId(this.AppIdFusion) + ", "),
				string.IsNullOrEmpty(this.AppIdChat) ? string.Empty : ("Chat: " + this.HideAppId(this.AppIdChat) + ", "),
				string.IsNullOrEmpty(this.AppIdVoice) ? string.Empty : ("Voice: " + this.HideAppId(this.AppIdVoice) + ", "),
				string.IsNullOrEmpty(this.AppVersion) ? string.Empty : ("AppVersion: " + this.AppVersion + ", "),
				"UseNameServer: " + this.UseNameServer.ToString() + ", ",
				"Fixed Region: " + this.FixedRegion + ", ",
				string.IsNullOrEmpty(this.Server) ? string.Empty : ("Server: " + this.Server + ", "),
				this.IsDefaultPort ? string.Empty : ("Port: " + this.Port.ToString() + ", "),
				string.IsNullOrEmpty(this.ProxyServer) ? string.Empty : ("Proxy: " + this.ProxyServer + ", "),
				this.Protocol,
				this.AuthMode
			});
		}

		public static bool IsAppId(string val)
		{
			try
			{
				new Guid(val);
			}
			catch
			{
				return false;
			}
			return true;
		}

		private string HideAppId(string appId)
		{
			return (string.IsNullOrEmpty(appId) || appId.Length < 8) ? appId : (appId.Substring(0, 8) + "***");
		}

		public AppSettings CopyTo(AppSettings d)
		{
			d.AppIdRealtime = this.AppIdRealtime;
			d.AppIdFusion = this.AppIdFusion;
			d.AppIdChat = this.AppIdChat;
			d.AppIdVoice = this.AppIdVoice;
			d.AppVersion = this.AppVersion;
			d.UseNameServer = this.UseNameServer;
			d.FixedRegion = this.FixedRegion;
			d.BestRegionSummaryFromStorage = this.BestRegionSummaryFromStorage;
			d.Server = this.Server;
			d.Port = this.Port;
			d.ProxyServer = this.ProxyServer;
			d.Protocol = this.Protocol;
			d.AuthMode = this.AuthMode;
			d.EnableLobbyStatistics = this.EnableLobbyStatistics;
			d.NetworkLogging = this.NetworkLogging;
			d.EnableProtocolFallback = this.EnableProtocolFallback;
			return d;
		}

		public AppSettings GetCopy()
		{
			return this.CopyTo(new AppSettings());
		}

		[InlineHelp]
		[NonSerialized]
		public string AppIdRealtime;

		[InlineHelp]
		public string AppIdFusion;

		[InlineHelp]
		public string AppIdChat;

		[InlineHelp]
		public string AppIdVoice;

		[InlineHelp]
		public string AppVersion;

		[InlineHelp]
		public bool UseNameServer = true;

		[InlineHelp]
		public string FixedRegion;

		[InlineHelp]
		[NonSerialized]
		public string BestRegionSummaryFromStorage;

		[InlineHelp]
		public string Server;

		[InlineHelp]
		public int Port;

		[InlineHelp]
		public string ProxyServer;

		[Header("Miscellaneous")]
		[InlineHelp]
		public ConnectionProtocol Protocol = ConnectionProtocol.Udp;

		[InlineHelp]
		public bool EnableProtocolFallback = true;

		[InlineHelp]
		public AuthModeOption AuthMode = AuthModeOption.Auth;

		[InlineHelp]
		public bool EnableLobbyStatistics;
	}
}
