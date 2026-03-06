using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Backtrace.Unity.Model
{
	internal class BacktraceLogManager
	{
		public BacktraceLogManager(uint numberOfLogs)
		{
			this._limit = numberOfLogs;
			this.LogQueue = new Queue<string>();
		}

		public int Size
		{
			get
			{
				return this.LogQueue.Count;
			}
		}

		public bool Disabled
		{
			get
			{
				return this._limit == 0U;
			}
		}

		public bool Enqueue(BacktraceReport report)
		{
			return this.Enqueue(new BacktraceUnityMessage(report));
		}

		public bool Enqueue(string message, string stackTrace, LogType type)
		{
			return this.Enqueue(new BacktraceUnityMessage(message, stackTrace, type));
		}

		public bool Enqueue(BacktraceUnityMessage unityMessage)
		{
			if (this.Disabled)
			{
				return false;
			}
			object obj = this.lockObject;
			lock (obj)
			{
				this.LogQueue.Enqueue(unityMessage.ToString());
				while ((long)this.LogQueue.Count > (long)((ulong)this._limit))
				{
					this.LogQueue.Dequeue();
				}
			}
			return true;
		}

		public string ToSourceCode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in this.LogQueue.ToArray())
			{
				stringBuilder.AppendLine(value);
			}
			return stringBuilder.ToString();
		}

		internal readonly Queue<string> LogQueue;

		private readonly object lockObject = new object();

		private readonly uint _limit;
	}
}
