using System;
using System.Collections.Generic;
using Backtrace.Unity.Json;

namespace Backtrace.Unity.Model.Metrics
{
	public sealed class UniqueEvent : EventAggregationBase
	{
		internal UniqueEvent(string name, long timestamp, IDictionary<string, string> attributes) : base(name, timestamp)
		{
			this.Attributes = attributes;
		}

		internal void UpdateTimestamp(long timestamp, IDictionary<string, string> attributes)
		{
			base.Timestamp = timestamp;
			string value;
			if (attributes != null && attributes.TryGetValue(base.Name, out value) && !string.IsNullOrEmpty(value))
			{
				this.Attributes = attributes;
			}
		}

		internal BacktraceJObject ToJson()
		{
			BacktraceJObject backtraceJObject = base.ToBaseObject(this.Attributes);
			backtraceJObject.Add("unique", new string[]
			{
				base.Name
			});
			return backtraceJObject;
		}

		internal const string UniqueEventName = "unique";

		internal IDictionary<string, string> Attributes;
	}
}
