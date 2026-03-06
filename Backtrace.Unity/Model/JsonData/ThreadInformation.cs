using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Backtrace.Unity.Extensions;
using Backtrace.Unity.Json;

namespace Backtrace.Unity.Model.JsonData
{
	public class ThreadInformation
	{
		public string Name { get; private set; }

		public bool Fault { get; private set; }

		public BacktraceJObject ToJson()
		{
			List<BacktraceJObject> list = new List<BacktraceJObject>();
			for (int i = 0; i < this.Stack.Count<BacktraceStackFrame>(); i++)
			{
				list.Add(this.Stack.ElementAt(i).ToJson());
			}
			BacktraceJObject backtraceJObject = new BacktraceJObject(new Dictionary<string, string>
			{
				{
					"name",
					this.Name
				}
			});
			backtraceJObject.Add("fault", this.Fault);
			backtraceJObject.ComplexObjects.Add("stack", list);
			return backtraceJObject;
		}

		public ThreadInformation(string threadName, bool fault, IEnumerable<BacktraceStackFrame> stack)
		{
			this.Stack = (stack ?? new List<BacktraceStackFrame>());
			this.Name = threadName;
			this.Fault = fault;
		}

		public ThreadInformation(Thread thread, IEnumerable<BacktraceStackFrame> stack, bool faultingThread = false) : this(thread.GenerateValidThreadName().ToLower(), faultingThread, stack)
		{
		}

		private ThreadInformation()
		{
		}

		internal IEnumerable<BacktraceStackFrame> Stack = new List<BacktraceStackFrame>();
	}
}
