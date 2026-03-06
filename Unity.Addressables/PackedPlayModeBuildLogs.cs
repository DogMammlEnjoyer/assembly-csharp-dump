using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PackedPlayModeBuildLogs
{
	public List<PackedPlayModeBuildLogs.RuntimeBuildLog> RuntimeBuildLogs
	{
		get
		{
			return this.m_RuntimeBuildLogs;
		}
		set
		{
			this.m_RuntimeBuildLogs = value;
		}
	}

	[SerializeField]
	private List<PackedPlayModeBuildLogs.RuntimeBuildLog> m_RuntimeBuildLogs = new List<PackedPlayModeBuildLogs.RuntimeBuildLog>();

	[Serializable]
	public struct RuntimeBuildLog
	{
		public RuntimeBuildLog(LogType type, string message)
		{
			this.Type = type;
			this.Message = message;
		}

		public LogType Type;

		public string Message;
	}
}
