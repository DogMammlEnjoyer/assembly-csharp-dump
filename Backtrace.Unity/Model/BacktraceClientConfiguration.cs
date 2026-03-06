using System;
using Backtrace.Unity.Types;
using UnityEngine;

namespace Backtrace.Unity.Model
{
	[Serializable]
	public class BacktraceClientConfiguration : ScriptableObject
	{
		public void UpdateServerUrl()
		{
			if (string.IsNullOrEmpty(this.ServerUrl))
			{
				return;
			}
			Uri uri;
			if (Uri.TryCreate(this.ServerUrl, UriKind.RelativeOrAbsolute, out uri))
			{
				try
				{
					this.ServerUrl = new UriBuilder(this.ServerUrl)
					{
						Scheme = Uri.UriSchemeHttps,
						Port = 6098
					}.Uri.ToString();
				}
				catch (Exception)
				{
					Debug.LogWarning("Invalid Backtrace URL");
				}
			}
		}

		public bool ValidateServerUrl()
		{
			if (!this.ServerUrl.Contains("backtrace.io") && !this.ServerUrl.Contains("submit.backtrace.io"))
			{
				return false;
			}
			Uri uri;
			bool result = Uri.TryCreate(this.ServerUrl, UriKind.RelativeOrAbsolute, out uri);
			try
			{
				new UriBuilder(this.ServerUrl)
				{
					Scheme = Uri.UriSchemeHttps,
					Port = 6098
				}.Uri.ToString();
			}
			catch (Exception)
			{
				return false;
			}
			return result;
		}

		public string ServerUrl;

		public int ReportPerMin;

		public bool HandleUnhandledExceptions = true;

		public bool IgnoreSslValidation;

		public bool DestroyOnLoad = true;

		public bool HandleANR = true;

		public bool OomReports;

		public int GameObjectDepth;

		public MiniDumpType MinidumpType = MiniDumpType.None;
	}
}
