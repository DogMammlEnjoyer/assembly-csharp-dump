using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Backtrace.Unity.Interfaces;
using Backtrace.Unity.Model;
using Backtrace.Unity.Model.Attributes;
using Backtrace.Unity.Model.Breadcrumbs;
using Backtrace.Unity.Runtime.Native.Base;
using Backtrace.Unity.Types;
using UnityEngine;

namespace Backtrace.Unity.Runtime.Native.Windows
{
	internal sealed class NativeClient : NativeClientBase, INativeClient, IDynamicAttributeProvider, IStartupMinidumpSender
	{
		[DllImport("BacktraceCrashpadWindows")]
		private static extern bool Initialize(string submissionUrl, [MarshalAs(UnmanagedType.LPWStr)] string databasePath, [MarshalAs(UnmanagedType.LPWStr)] string handlerPath, string[] attachments, int attachmentSize);

		[DllImport("BacktraceCrashpadWindows", EntryPoint = "AddAttribute")]
		private static extern bool AddNativeAttribute(string key, string value);

		[DllImport("BacktraceCrashpadWindows", EntryPoint = "DumpWithoutCrash")]
		private static extern void NativeReport(string message, bool setMainThreadAsFaultingThread);

		public NativeClient(BacktraceConfiguration configuration, BacktraceBreadcrumbs breadcrumbs, IDictionary<string, string> clientAttributes, IEnumerable<string> attachments) : base(configuration, breadcrumbs)
		{
			NativeClient.CleanScopedAttributes();
			this.HandleNativeCrashes(clientAttributes, attachments);
			this.AddScopedAttributes(clientAttributes);
			if (!configuration.ReportFilterType.HasFlag(ReportFilterType.Hang))
			{
				this.HandleAnr();
			}
		}

		private void HandleNativeCrashes(IDictionary<string, string> clientAttributes, IEnumerable<string> attachments)
		{
			if (!this._configuration.CaptureNativeCrashes || !this._configuration.Enabled)
			{
				return;
			}
			string pluginDirectoryPath = this.GetPluginDirectoryPath();
			if (!Directory.Exists(pluginDirectoryPath))
			{
				Debug.LogWarning("Backtrace native lib directory doesn't exist");
				return;
			}
			if (this.Isx86Build(pluginDirectoryPath) || IntPtr.Size == 4)
			{
				return;
			}
			string defaultPathToCrashpadHandler = this.GetDefaultPathToCrashpadHandler(pluginDirectoryPath);
			if (string.IsNullOrEmpty(defaultPathToCrashpadHandler) || !File.Exists(defaultPathToCrashpadHandler))
			{
				Debug.LogWarning("Backtrace native integration status: Cannot find path to Crashpad handler.");
				return;
			}
			string crashpadDatabasePath = this._configuration.CrashpadDatabasePath;
			if (string.IsNullOrEmpty(crashpadDatabasePath) || !Directory.Exists(this._configuration.GetFullDatabasePath()))
			{
				Debug.LogWarning("Backtrace native integration status: database path doesn't exist");
				return;
			}
			string submissionUrl = new BacktraceCredentials(this._configuration.GetValidServerUrl()).GetMinidumpSubmissionUrl().ToString();
			if (!Directory.Exists(crashpadDatabasePath))
			{
				Directory.CreateDirectory(crashpadDatabasePath);
			}
			this.CaptureNativeCrashes = NativeClient.Initialize(submissionUrl, crashpadDatabasePath, defaultPathToCrashpadHandler, attachments.ToArray<string>(), attachments.Count<string>());
			if (!this.CaptureNativeCrashes)
			{
				Debug.LogWarning("Backtrace native integration status: Cannot initialize Crashpad client");
				return;
			}
			foreach (KeyValuePair<string, string> keyValuePair in clientAttributes)
			{
				NativeClient.AddNativeAttribute(keyValuePair.Key, (keyValuePair.Value == null) ? string.Empty : keyValuePair.Value);
			}
			NativeClient.AddNativeAttribute("error.type", "Crash");
		}

		private bool Isx86Build(string pluginDirectoryPath)
		{
			return File.Exists(Path.Combine(Path.Combine(pluginDirectoryPath, "x86"), "BacktraceCrashpadWindows.dll"));
		}

		public void GetAttributes(IDictionary<string, string> attributes)
		{
		}

		public void HandleAnr()
		{
			if (!this.CaptureNativeCrashes || !this._configuration.HandleANR)
			{
				return;
			}
			bool reported = false;
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			this.AnrThread = new Thread(delegate()
			{
				float num = 0f;
				while (this.AnrThread.IsAlive && !this.StopAnr)
				{
					if (!this.PreventAnr)
					{
						if (num == 0f)
						{
							num = this.LastUpdateTime;
						}
						else if (num == this.LastUpdateTime)
						{
							if (!reported)
							{
								this.OnAnrDetection();
								reported = true;
								NativeClient.AddNativeAttribute("error.type", "Hang");
								NativeClient.NativeReport("ANRException: Blocked thread detected.", true);
								NativeClient.AddNativeAttribute("error.type", "Hang");
							}
						}
						else
						{
							reported = false;
						}
						num = this.LastUpdateTime;
					}
					else if (num != 0f)
					{
						num = 0f;
					}
					Thread.Sleep(this.AnrWatchdogTimeout);
				}
			});
			this.AnrThread.IsBackground = true;
			this.AnrThread.Start();
		}

		public bool OnOOM()
		{
			return false;
		}

		public void SetAttribute(string key, string value)
		{
			if (string.IsNullOrEmpty(key))
			{
				return;
			}
			if (value == null)
			{
				value = string.Empty;
			}
			this.AddAttributes(key, value);
		}

		public IEnumerator SendMinidumpOnStartup(ICollection<string> clientAttachments, IBacktraceApi backtraceApi)
		{
			string path = string.Format("Temp/{0}/{1}/crashes", Application.companyName, Application.productName);
			string[] array = new string[]
			{
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), path),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), path)
			};
			List<string> list = new List<string>();
			foreach (string text in array)
			{
				if (Directory.Exists(text))
				{
					list.Add(text);
				}
			}
			if (list.Count == 0)
			{
				yield break;
			}
			IDictionary<string, string> attributes = NativeClient.GetScopedAttributes();
			attributes["error.type"] = "Crash";
			foreach (string nativeCrashesDir in list)
			{
				List<string> attachments = (clientAttachments == null) ? new List<string>() : new List<string>(clientAttachments);
				string[] directories = Directory.GetDirectories(nativeCrashesDir);
				string[] array3 = directories;
				for (int j = 0; j < array3.Length; j++)
				{
					string path2 = array3[j];
					string crashDirFullPath = Path.Combine(nativeCrashesDir, path2);
					string[] files = Directory.GetFiles(crashDirFullPath);
					if (!files.Any((string n) => n.EndsWith("backtrace.json")))
					{
						string minidumpPath = files.FirstOrDefault((string n) => n.EndsWith("crash.dmp"));
						if (!string.IsNullOrEmpty(minidumpPath))
						{
							List<string> attachments2 = (from n in files.Concat(attachments)
							where n != minidumpPath
							select n).ToList<string>();
							yield return backtraceApi.SendMinidump(minidumpPath, attachments2, attributes, delegate(BacktraceResult result)
							{
								if (result != null && result.Status == BacktraceResultStatus.Ok)
								{
									File.Create(Path.Combine(crashDirFullPath, "backtrace.json"));
								}
							});
						}
					}
				}
				array3 = null;
				attachments = null;
				nativeCrashesDir = null;
			}
			List<string>.Enumerator enumerator = default(List<string>.Enumerator);
			yield break;
			yield break;
		}

		private string GetPluginDirectoryPath()
		{
			return Path.Combine(Application.dataPath, "Plugins");
		}

		private string GetDefaultPathToCrashpadHandler(string pluginDirectoryPath)
		{
			return Path.Combine(Path.Combine(pluginDirectoryPath, "x86_64"), "crashpad_handler.dll");
		}

		internal static void CleanScopedAttributes()
		{
			string @string = PlayerPrefs.GetString("backtrace-scoped-attributes");
			if (!NativeClient.HasScopedAttributesEmpty(@string))
			{
				return;
			}
			foreach (string arg in JsonUtility.FromJson<NativeClient.ScopedAttributesContainer>(@string).Keys)
			{
				PlayerPrefs.DeleteKey(string.Format("bt-{0}", arg));
			}
			PlayerPrefs.DeleteKey("backtrace-scoped-attributes");
		}

		internal static IDictionary<string, string> GetScopedAttributes()
		{
			string @string = PlayerPrefs.GetString("backtrace-scoped-attributes");
			if (!NativeClient.HasScopedAttributesEmpty(@string))
			{
				return new Dictionary<string, string>();
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in JsonUtility.FromJson<NativeClient.ScopedAttributesContainer>(@string).Keys)
			{
				string string2 = PlayerPrefs.GetString(string.Format("bt-{0}", text), string.Empty);
				dictionary[text] = string2;
			}
			foreach (KeyValuePair<string, string> keyValuePair in ((IEnumerable<KeyValuePair<string, string>>)new Dictionary<string, string>
			{
				{
					"backtrace-uuid",
					"guid"
				},
				{
					"backtrace-app-version",
					"application.version"
				},
				{
					"backtrace-session-id",
					"application.session"
				}
			}))
			{
				string string3 = PlayerPrefs.GetString(keyValuePair.Key, string.Empty);
				if (!string.IsNullOrEmpty(string3))
				{
					PlayerPrefs.DeleteKey(keyValuePair.Key);
					dictionary[keyValuePair.Value] = string3;
				}
			}
			return dictionary;
		}

		private void AddAttributes(string key, string value)
		{
			if (this.CaptureNativeCrashes)
			{
				NativeClient.AddNativeAttribute(key, value);
			}
			this.AddScopedAttribute(key, value);
		}

		internal void AddScopedAttributes(IDictionary<string, string> attributes)
		{
			if (!this._configuration.SendUnhandledGameCrashesOnGameStartup)
			{
				return;
			}
			NativeClient.ScopedAttributesContainer scopedAttributesContainer = new NativeClient.ScopedAttributesContainer();
			foreach (KeyValuePair<string, string> keyValuePair in attributes)
			{
				scopedAttributesContainer.Keys.Add(keyValuePair.Key);
				PlayerPrefs.SetString(string.Format("bt-{0}", keyValuePair.Key), keyValuePair.Value);
			}
			PlayerPrefs.SetString("backtrace-scoped-attributes", JsonUtility.ToJson(scopedAttributesContainer));
		}

		private void AddScopedAttribute(string key, string value)
		{
			if (!this._configuration.SendUnhandledGameCrashesOnGameStartup)
			{
				return;
			}
			string @string = PlayerPrefs.GetString("backtrace-scoped-attributes");
			NativeClient.ScopedAttributesContainer scopedAttributesContainer = NativeClient.HasScopedAttributesEmpty(@string) ? JsonUtility.FromJson<NativeClient.ScopedAttributesContainer>(@string) : new NativeClient.ScopedAttributesContainer();
			scopedAttributesContainer.Keys.Add(key);
			PlayerPrefs.SetString("backtrace-scoped-attributes", JsonUtility.ToJson(scopedAttributesContainer));
			PlayerPrefs.SetString(string.Format("bt-{0}", key), value);
		}

		private static bool HasScopedAttributesEmpty(string attributesJson)
		{
			return !string.IsNullOrEmpty(attributesJson) && !(attributesJson == "{}");
		}

		private const string ScopedAttributeListKey = "backtrace-scoped-attributes";

		private const string ScopedAttributesPattern = "bt-{0}";

		internal const string VersionKey = "backtrace-app-version";

		internal const string MachineUuidKey = "backtrace-uuid";

		internal const string SessionKey = "backtrace-session-id";

		[Serializable]
		private class ScopedAttributesContainer
		{
			public List<string> Keys = new List<string>();
		}
	}
}
