using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal class RenderGraphLogger
	{
		public void Initialize(string logName)
		{
			StringBuilder stringBuilder;
			if (!this.m_LogMap.TryGetValue(logName, out stringBuilder))
			{
				stringBuilder = new StringBuilder();
				this.m_LogMap.Add(logName, stringBuilder);
			}
			this.m_CurrentBuilder = stringBuilder;
			this.m_CurrentBuilder.Clear();
			this.m_CurrentIndentation = 0;
		}

		public void IncrementIndentation(int value)
		{
			this.m_CurrentIndentation += Math.Abs(value);
		}

		public void DecrementIndentation(int value)
		{
			this.m_CurrentIndentation = Math.Max(0, this.m_CurrentIndentation - Math.Abs(value));
		}

		public void LogLine(string format, params object[] args)
		{
			for (int i = 0; i < this.m_CurrentIndentation; i++)
			{
				this.m_CurrentBuilder.Append('\t');
			}
			this.m_CurrentBuilder.AppendFormat(format, args);
			this.m_CurrentBuilder.AppendLine();
		}

		public void FlushLogs()
		{
			string text = "";
			foreach (KeyValuePair<string, StringBuilder> keyValuePair in this.m_LogMap)
			{
				StringBuilder value = keyValuePair.Value;
				value.AppendLine();
				text += value.ToString();
			}
			this.m_LogMap.Clear();
			Debug.Log(text);
		}

		private Dictionary<string, StringBuilder> m_LogMap = new Dictionary<string, StringBuilder>();

		private StringBuilder m_CurrentBuilder;

		private int m_CurrentIndentation;
	}
}
