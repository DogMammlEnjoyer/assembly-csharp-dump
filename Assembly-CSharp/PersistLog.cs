using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PersistLog : MonoBehaviour
{
	private void OnEnable()
	{
		PersistLog.<OnEnable>d__4 <OnEnable>d__;
		<OnEnable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnEnable>d__.<>4__this = this;
		<OnEnable>d__.<>1__state = -1;
		<OnEnable>d__.<>t__builder.Start<PersistLog.<OnEnable>d__4>(ref <OnEnable>d__);
	}

	private void OnDisable()
	{
		this.OnDestroy();
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= this.LogMessageEnqueue;
		Application.logMessageReceived -= this.LogMessageReceived;
		if (PersistLog.sr == null)
		{
			return;
		}
		PersistLog.sr.Close();
		PersistLog.sr = null;
	}

	private void LogMessageEnqueue(string msg, string strace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
		{
			this.earlyQ.Add(Tuple.Create<string, string>(msg, strace));
		}
	}

	private void LogMessageReceived(string msg, string strace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
		{
			if (this.plog == msg + strace)
			{
				if (!this.dup)
				{
					PersistLog.sr.Write(string.Format("T+{0} >> Duplicate log entry... Supressing further\n\n", Time.time));
					this.dup = true;
				}
			}
			else
			{
				PersistLog.sr.Write(string.Format("T+{0} >> {1}\n==========================\n{2}\n\n", Time.time, msg, strace));
				this.dup = false;
			}
			PersistLog.sr.Flush();
		}
		this.plog = msg + strace;
	}

	public static void Log(string msg)
	{
		msg = string.Format("T+{0} >[DEV MSG]> {1}\n\n", Time.time, msg);
		Debug.Log(msg);
		if (PersistLog.sr == null)
		{
			return;
		}
		PersistLog.sr.Write(msg);
		PersistLog.sr.Flush();
	}

	private static StreamWriter sr;

	private string plog;

	private bool dup;

	private List<Tuple<string, string>> earlyQ;
}
