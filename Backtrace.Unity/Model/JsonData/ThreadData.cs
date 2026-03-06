using System;
using System.Collections.Generic;
using System.Threading;
using Backtrace.Unity.Extensions;
using Backtrace.Unity.Json;

namespace Backtrace.Unity.Model.JsonData
{
	public class ThreadData
	{
		internal ThreadData(IEnumerable<BacktraceStackFrame> exceptionStack, bool faultingThread)
		{
			Thread currentThread = Thread.CurrentThread;
			string text = currentThread.GenerateValidThreadName().ToLower();
			this.ThreadInformations[text] = new ThreadInformation(currentThread, exceptionStack, faultingThread);
			this.MainThread = text;
		}

		public BacktraceJObject ToJson()
		{
			BacktraceJObject backtraceJObject = new BacktraceJObject();
			foreach (KeyValuePair<string, ThreadInformation> keyValuePair in this.ThreadInformations)
			{
				backtraceJObject.Add(keyValuePair.Key, keyValuePair.Value.ToJson());
			}
			return backtraceJObject;
		}

		public Dictionary<string, ThreadInformation> ThreadInformations = new Dictionary<string, ThreadInformation>();

		internal string MainThread = string.Empty;
	}
}
