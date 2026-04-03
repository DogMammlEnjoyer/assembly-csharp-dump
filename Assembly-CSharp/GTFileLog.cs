using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class GTFileLog
{
	private static GTFileLog.FLogInstance Default
	{
		get
		{
			if (GTFileLog._default != null)
			{
				return GTFileLog._default;
			}
			object registryLock = GTFileLog._registryLock;
			GTFileLog.FLogInstance @default;
			lock (registryLock)
			{
				if (GTFileLog._default == null)
				{
					GTFileLog._default = new GTFileLog.FLogInstance("main");
				}
				@default = GTFileLog._default;
			}
			return @default;
		}
	}

	public static GTFileLog.FLogInstance GetLog(string name)
	{
		object registryLock = GTFileLog._registryLock;
		GTFileLog.FLogInstance result;
		lock (registryLock)
		{
			GTFileLog.FLogInstance flogInstance;
			if (GTFileLog._instances.TryGetValue(name, out flogInstance))
			{
				result = flogInstance;
			}
			else
			{
				GTFileLog.FLogInstance flogInstance2 = new GTFileLog.FLogInstance(name);
				GTFileLog._instances[name] = flogInstance2;
				result = flogInstance2;
			}
		}
		return result;
	}

	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void Log(string msg)
	{
		GTFileLog.Default.WriteEntry("LOG", msg, StackTraceUtility.ExtractStackTrace());
	}

	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarning(string msg)
	{
		GTFileLog.Default.WriteEntry("WARN", msg, StackTraceUtility.ExtractStackTrace());
	}

	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogError(string msg)
	{
		GTFileLog.Default.WriteEntry("ERR", msg, StackTraceUtility.ExtractStackTrace());
	}

	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogNoTrace(string msg)
	{
		GTFileLog.Default.WriteEntryNoTrace("LOG", msg);
	}

	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarningNoTrace(string msg)
	{
		GTFileLog.Default.WriteEntryNoTrace("WARN", msg);
	}

	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorNoTrace(string msg)
	{
		GTFileLog.Default.WriteEntryNoTrace("ERR", msg);
	}

	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void CLog(string msg)
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null && GTFileLog._default.IsActive)
			{
				GTFileLog._default.WriteEntryNoTrace("LOG", msg);
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				if (flogInstance.IsActive)
				{
					flogInstance.WriteEntryNoTrace("LOG", msg);
				}
			}
		}
		Debug.Log("[GT/FLog] " + msg);
	}

	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void CLogWarning(string msg)
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null && GTFileLog._default.IsActive)
			{
				GTFileLog._default.WriteEntryNoTrace("WARN", msg);
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				if (flogInstance.IsActive)
				{
					flogInstance.WriteEntryNoTrace("WARN", msg);
				}
			}
		}
		Debug.LogWarning("[GT/FLog] " + msg);
	}

	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void CLogError(string msg)
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null && GTFileLog._default.IsActive)
			{
				GTFileLog._default.WriteEntryNoTrace("ERR", msg);
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				if (flogInstance.IsActive)
				{
					flogInstance.WriteEntryNoTrace("ERR", msg);
				}
			}
		}
		Debug.LogError("[GT/FLog] " + msg);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void Reset()
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null)
			{
				GTFileLog._default.Close();
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				flogInstance.Close();
			}
		}
	}

	private static void OnUnityLogMessage(string condition, string stackTrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		if (GTFileLog._inCallback)
		{
			return;
		}
		GTFileLog._inCallback = true;
		try
		{
			string level = (type == LogType.Exception) ? "EXCEPTION" : ((type == LogType.Assert) ? "ASSERT" : "UNITY_ERR");
			GTFileLog.Default.WriteEntry(level, condition, stackTrace);
		}
		finally
		{
			GTFileLog._inCallback = false;
		}
	}

	internal static string GetTimestamp()
	{
		if (!(NetworkSystem.Instance != null))
		{
			return Mathf.FloorToInt(Time.realtimeSinceStartup * 1000f).ToString() + "u";
		}
		return NetworkSystem.Instance.ServerTimestamp.ToString();
	}

	internal static string ExtractFirstExternalCaller(string stackTrace)
	{
		if (string.IsNullOrEmpty(stackTrace))
		{
			return "(unknown)";
		}
		int num;
		for (int i = 0; i < stackTrace.Length; i = num + 1)
		{
			num = stackTrace.IndexOf('\n', i);
			if (num < 0)
			{
				num = stackTrace.Length;
			}
			int num2 = num - i;
			if (num2 > 0 && stackTrace.IndexOf("GTFileLog", i, Math.Min(num2, 60), StringComparison.Ordinal) < 0)
			{
				return stackTrace.Substring(i, num2).Trim();
			}
		}
		return "(unknown)";
	}

	private static readonly object _registryLock = new object();

	private static Dictionary<string, GTFileLog.FLogInstance> _instances = new Dictionary<string, GTFileLog.FLogInstance>();

	private static GTFileLog.FLogInstance _default;

	[ThreadStatic]
	private static bool _inCallback;

	public sealed class FLogInstance
	{
		internal FLogInstance(string prefix)
		{
			this._prefix = prefix;
		}

		internal bool IsActive
		{
			get
			{
				object @lock = this._lock;
				bool result;
				lock (@lock)
				{
					result = (this._writer != null);
				}
				return result;
			}
		}

		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void Log(string msg)
		{
			this.WriteEntry("LOG", msg, StackTraceUtility.ExtractStackTrace());
		}

		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogWarning(string msg)
		{
			this.WriteEntry("WARN", msg, StackTraceUtility.ExtractStackTrace());
		}

		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogError(string msg)
		{
			this.WriteEntry("ERR", msg, StackTraceUtility.ExtractStackTrace());
		}

		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogNoTrace(string msg)
		{
			this.WriteEntryNoTrace("LOG", msg);
		}

		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogWarningNoTrace(string msg)
		{
			this.WriteEntryNoTrace("WARN", msg);
		}

		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogErrorNoTrace(string msg)
		{
			this.WriteEntryNoTrace("ERR", msg);
		}

		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void CLog(string msg)
		{
			this.WriteEntryNoTrace("LOG", msg);
			Debug.Log("[GT/FLog:" + this._prefix + "] " + msg);
		}

		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void CLogWarning(string msg)
		{
			this.WriteEntryNoTrace("WARN", msg);
			Debug.LogWarning("[GT/FLog:" + this._prefix + "] " + msg);
		}

		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void CLogError(string msg)
		{
			this.WriteEntryNoTrace("ERR", msg);
			Debug.LogError("[GT/FLog:" + this._prefix + "] " + msg);
		}

		internal void WriteEntryNoTrace(string level, string msg)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this.EnsureWriter(null);
				if (this._writer != null)
				{
					try
					{
						string timestamp = GTFileLog.GetTimestamp();
						this._writer.WriteLine(string.Concat(new string[]
						{
							"[",
							timestamp,
							"] [",
							level,
							"] ",
							msg
						}));
					}
					catch (Exception ex)
					{
						Debug.LogError("[GT/GTFileLog:" + this._prefix + "] Write failed: " + ex.Message);
						this.CloseWriter();
					}
				}
			}
		}

		internal void WriteEntry(string level, string msg, string trace)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this.EnsureWriter(trace);
				if (this._writer != null)
				{
					try
					{
						string timestamp = GTFileLog.GetTimestamp();
						this._writer.WriteLine(string.Concat(new string[]
						{
							"[",
							timestamp,
							"] [",
							level,
							"] ",
							msg,
							"\n- - - -"
						}));
						this._writer.WriteLine(trace);
						this._writer.WriteLine("");
					}
					catch (Exception ex)
					{
						Debug.LogError("[GT/GTFileLog:" + this._prefix + "] Write failed: " + ex.Message);
						this.CloseWriter();
					}
				}
			}
		}

		private void EnsureWriter(string callerTrace)
		{
			if (this._writer != null || this._failed)
			{
				return;
			}
			if (ApplicationQuittingState.IsQuitting)
			{
				this._failed = true;
				return;
			}
			try
			{
				string persistentDataPath = Application.persistentDataPath;
				Directory.CreateDirectory(persistentDataPath);
				string str = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
				string str2 = "flog_" + this._prefix + "_" + str;
				string text = Path.Combine(persistentDataPath, str2 + ".log");
				for (int i = 1; i <= 10; i++)
				{
					try
					{
						this._writer = new StreamWriter(text, true)
						{
							AutoFlush = true
						};
						break;
					}
					catch (IOException obj) when (i < 10)
					{
						text = Path.Combine(persistentDataPath, str2 + "_" + (i + 1).ToString() + ".log");
					}
				}
				if (this._writer == null)
				{
					throw new IOException("All 10 log file attempts failed due to sharing violations.");
				}
				this._writer.WriteLine(string.Format("--- {0} log started {1:u} ---", this._prefix, DateTime.UtcNow));
				this._writer.WriteLine("--- playerName: " + PlayerPrefs.GetString("playerName", "(unset)") + " ---");
				string text2 = (callerTrace != null) ? GTFileLog.ExtractFirstExternalCaller(callerTrace) : "(no-trace)";
				Debug.Log(string.Concat(new string[]
				{
					"<color=orange><b>[GT/GTFileLog:",
					this._prefix,
					"]</b> Writing to \"",
					text,
					"\". First caller: ",
					text2,
					"</color>"
				}));
				GTFileLog.FLogInstance.PruneOldFlogFiles(persistentDataPath);
			}
			catch (Exception ex)
			{
				this._failed = true;
				Debug.LogError("[GT/GTFileLog:" + this._prefix + "] Failed to create log file: " + ex.Message);
			}
		}

		private static void PruneOldFlogFiles(string dir)
		{
			try
			{
				string[] files = Directory.GetFiles(dir, "flog_*.log");
				if (files.Length > 10)
				{
					Array.Sort<string>(files, (string a, string b) => File.GetLastWriteTimeUtc(a).CompareTo(File.GetLastWriteTimeUtc(b)));
					int num = files.Length - 10;
					for (int i = 0; i < num; i++)
					{
						try
						{
							File.Delete(files[i]);
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
		}

		private void CloseWriter()
		{
			try
			{
				StreamWriter writer = this._writer;
				if (writer != null)
				{
					writer.Flush();
				}
				StreamWriter writer2 = this._writer;
				if (writer2 != null)
				{
					writer2.Dispose();
				}
			}
			catch
			{
			}
			this._writer = null;
		}

		internal void Close()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this.CloseWriter();
				this._failed = false;
			}
		}

		private StreamWriter _writer;

		private bool _failed;

		private readonly object _lock = new object();

		private readonly string _prefix;

		private const string FilePrefix = "flog_";

		private const int MaxFlogFiles = 10;
	}
}
