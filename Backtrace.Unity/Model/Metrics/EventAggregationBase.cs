using System;
using System.Collections.Generic;
using Backtrace.Unity.Json;

namespace Backtrace.Unity.Model.Metrics
{
	public abstract class EventAggregationBase
	{
		public long Timestamp { get; set; }

		public string Name { get; private set; }

		public EventAggregationBase(string name, long timestamp)
		{
			this.Name = name;
			this.Timestamp = timestamp;
		}

		internal BacktraceJObject ToBaseObject(IDictionary<string, string> attributes)
		{
			BacktraceJObject backtraceJObject = new BacktraceJObject();
			backtraceJObject.Add("timestamp", this.Timestamp);
			backtraceJObject.Add("attributes", new BacktraceJObject(attributes));
			return backtraceJObject;
		}

		private const string TimestampName = "timestamp";

		private const string AttributesName = "attributes";
	}
}
